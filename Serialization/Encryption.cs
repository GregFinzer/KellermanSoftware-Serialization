using System;
using System.IO;

#if !NETSTANDARD
using System.IO.IsolatedStorage;
#endif

using System.Security.Cryptography;
using System.Text;

namespace KellermanSoftware.Serialization
{
    /// <summary>
    /// Industry standard AES Encryption suitable for credit cards and other private information
    /// </summary>
    public class Encryption : IEncryption
    {
#if !NETSTANDARD
        private IsolatedStorageFile _store;
#endif
        private readonly char[] _ivChars = new[] { '~', '#', '!', '?', '@', '%', '.', '^', 'f', 'A', '7', '9', '&', '|', ';', 'Z' };

        /// <summary>
        /// Specify the block size for the encryption algorithm.  Default is 128, can only be set for non-silverlight projects. AesManaged can only be 128
        /// </summary>
        public int BlockSize
        {
            get;
            set;
        }

        /// <summary>
        /// Specify the key size for the encryption algorithm.  Default is 128
        /// </summary>
        public int KeySize { get; set; }

#if !NETSTANDARD
        private IsolatedStorageFile Store
        {
            get { return _store ?? (_store = GetIsolatedStorage()); }
        }
#endif

        /// <summary>
        /// See http://en.wikipedia.org/wiki/Salt_%28cryptography%29
        /// </summary>
        public string Salt { get; set;}


        /// <summary>
        /// Constructor that will automatically get the current isolated storage
        /// </summary>
        public Encryption()
        {
            KeySize = 128;
            BlockSize = 128;
            Salt = "PasswordSalt";
        }

#if !NETSTANDARD
        private IsolatedStorageFile GetIsolatedStorage()
        {
            return IsolatedStorageFile.GetUserStoreForDomain();
        }

        /// <summary>
        /// Constructor that takes in an IsolatedStorageFile
        /// </summary>
        /// <param name="store"></param>
        public Encryption(IsolatedStorageFile store) :this()
        {
            _store = store;
        }
#endif

        /// <summary>Encrypt the passed string and base64 encode it</summary>
        /// <param name="inputString"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <example>
        /// 	<code title="Example" description="" lang="C#">Encryption encryption = new Encryption();
        ///  
        /// string creditCardNumber = "4444333322221111";
        /// string password = "&amp;%)(&amp;JUI";
        ///  
        /// string encrypted = encryption.EncryptString(creditCardNumber, password);
        ///  
        /// string decrypted = encryption.DecryptString(encrypted, password);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">Dim encryption As New Encryption()
        ///  
        /// Dim creditCardNumber As String = "4444333322221111"
        /// Dim password As String = "&amp;%)(&amp;JUI"
        ///  
        /// Dim encrypted As String = encryption.EncryptString(creditCardNumber, password)
        ///  
        /// Dim decrypted As String = encryption.DecryptString(encrypted, password)</code>
        /// </example>
        public string EncryptString(string inputString, string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
            byte[] encryptedBytes = EncryptBytes(inputBytes, password);
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>Decrypt a Base64 encoded and encrypted string</summary>
        /// <param name="encryptedString"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <example>
        /// 	<code title="Example" description="" lang="C#">Encryption encryption = new Encryption();
        ///  
        /// string creditCardNumber = "4444333322221111";
        /// string password = "&amp;%)(&amp;JUI";
        ///  
        /// string encrypted = encryption.EncryptString(creditCardNumber, password);
        ///  
        /// string decrypted = encryption.DecryptString(encrypted, password);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">Dim encryption As New Encryption()
        ///  
        /// Dim creditCardNumber As String = "4444333322221111"
        /// Dim password As String = "&amp;%)(&amp;JUI"
        ///  
        /// Dim encrypted As String = encryption.EncryptString(creditCardNumber, password)
        ///  
        /// Dim decrypted As String = encryption.DecryptString(encrypted, password)</code>
        /// </example>
        public string DecryptString(string encryptedString, string password)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedString);
            byte[] decryptedBytes = DecryptBytes(encryptedBytes, password);
            return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length);
        }

#if !NETSTANDARD
        /// <summary>Encrypt a file in isolated storage</summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="password"></param>
        /// <example>
        /// 	<code title="Example" description="" lang="C#">string password = "&amp;%)(&amp;JUI";
        ///  string creditCardNumber = "4444333322221111";
        ///  byte[] inputBytes = Encoding.UTF8.GetBytes(creditCardNumber);
        ///  
        /// #if SILVERLIGHT
        /// IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
        /// #else
        ///  IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();
        /// #endif
        ///  
        ///  //Create the input file
        ///  string inputFilePath = "InputFile.txt";
        ///  using (IsolatedStorageFileStream outputStream = new IsolatedStorageFileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, store))
        ///  {
        ///      outputStream.Write(inputBytes, 0, inputBytes.Length);
        ///  }
        ///  
        ///  //Encrypt it
        ///  string encryptedPath = "Encrypted.txt";
        ///  Encryption encryption = new Encryption(store);
        ///  encryption.EncryptFile(inputFilePath,encryptedPath,password);
        ///  
        ///  //Decrypt
        ///  string decryptedPath = "Decrypted.txt";
        ///  encryption.DecryptFile(encryptedPath, decryptedPath, password);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">Dim password As String = "&amp;%)(&amp;JUI"
        ///  Dim creditCardNumber As String = "4444333322221111"
        ///  Dim inputBytes() As Byte = Encoding.UTF8.GetBytes(creditCardNumber)
        ///  
        /// #If SILVERLIGHT Then
        /// Dim store As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
        /// #Else
        ///  Dim store As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForDomain()
        /// #End If
        ///  
        ///  'Create the input file
        ///  Dim inputFilePath As String = "InputFile.txt"
        ///  Using outputStream As New IsolatedStorageFileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, store)
        /// 	 outputStream.Write(inputBytes, 0, inputBytes.Length)
        ///  End Using
        ///  
        ///  'Encrypt it
        ///  Dim encryptedPath As String = "Encrypted.txt"
        ///  Dim encryption As New Encryption(store)
        ///  encryption.EncryptFile(inputFilePath,encryptedPath,password)
        ///  
        ///  'Decrypt
        ///  Dim decryptedPath As String = "Decrypted.txt"
        ///  encryption.DecryptFile(encryptedPath, decryptedPath, password)</code>
        /// </example>
        public void EncryptIsolatedStorageFile(string inputFilePath, string outputFilePath, string password)
        {
            using (IsolatedStorageFileStream inputStream = new IsolatedStorageFileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Store))
            {
                using (IsolatedStorageFileStream outputStream = new IsolatedStorageFileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, Store))
                {
#if SILVERLIGHT
                    using (AesManaged aes = new AesManaged())
#else
                    using (RijndaelManaged aes = new RijndaelManaged())
#endif
                    {
                        DerrivePasswordAndIv(aes, password);

                        using (CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            const int BUFFER_LENGTH = 4096;
                            byte[] inputBytes = new byte[BUFFER_LENGTH];
                            int bytesRead;

                            while ((bytesRead = inputStream.Read(inputBytes, 0, BUFFER_LENGTH)) > 0)
                            {
                                cryptoStream.Write(inputBytes, 0, bytesRead);
                            }

                            cryptoStream.FlushFinalBlock();
                        }
                    }
                }
            }
        }

        /// <summary>Decrypt a file in isolated storage</summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="password"></param>
        /// <example>
        /// 	<code title="Example" description="" lang="C#">string password = "&amp;%)(&amp;JUI";
        ///  string creditCardNumber = "4444333322221111";
        ///  byte[] inputBytes = Encoding.UTF8.GetBytes(creditCardNumber);
        ///  
        /// #if SILVERLIGHT
        /// IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
        /// #else
        ///  IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();
        /// #endif
        ///  
        ///  //Create the input file
        ///  string inputFilePath = "InputFile.txt";
        ///  using (IsolatedStorageFileStream outputStream = new IsolatedStorageFileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, store))
        ///  {
        ///      outputStream.Write(inputBytes, 0, inputBytes.Length);
        ///  }
        ///  
        ///  //Encrypt it
        ///  string encryptedPath = "Encrypted.txt";
        ///  Encryption encryption = new Encryption(store);
        ///  encryption.EncryptFile(inputFilePath,encryptedPath,password);
        ///  
        ///  //Decrypt
        ///  string decryptedPath = "Decrypted.txt";
        ///  encryption.DecryptFile(encryptedPath, decryptedPath, password);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">Dim password As String = "&amp;%)(&amp;JUI"
        ///  Dim creditCardNumber As String = "4444333322221111"
        ///  Dim inputBytes() As Byte = Encoding.UTF8.GetBytes(creditCardNumber)
        ///  
        /// #If SILVERLIGHT Then
        /// Dim store As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
        /// #Else
        ///  Dim store As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForDomain()
        /// #End If
        ///  
        ///  'Create the input file
        ///  Dim inputFilePath As String = "InputFile.txt"
        ///  Using outputStream As New IsolatedStorageFileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, store)
        /// 	 outputStream.Write(inputBytes, 0, inputBytes.Length)
        ///  End Using
        ///  
        ///  'Encrypt it
        ///  Dim encryptedPath As String = "Encrypted.txt"
        ///  Dim encryption As New Encryption(store)
        ///  encryption.EncryptFile(inputFilePath,encryptedPath,password)
        ///  
        ///  'Decrypt
        ///  Dim decryptedPath As String = "Decrypted.txt"
        ///  encryption.DecryptFile(encryptedPath, decryptedPath, password)</code>
        /// </example>
        public void DecryptIsolatedStorageFile(string inputFilePath, string outputFilePath, string password)
        {
            using (IsolatedStorageFileStream inputStream = new IsolatedStorageFileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Store))
            {
                using (IsolatedStorageFileStream outputStream = new IsolatedStorageFileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, Store))
                {
#if SILVERLIGHT
                    using (AesManaged aes = new AesManaged())
#else
                    using (RijndaelManaged aes = new RijndaelManaged())
#endif
                    {
                        DerrivePasswordAndIv(aes, password);

                        using (CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            const int BUFFER_LENGTH = 4096;
                            byte[] inputBytes = new byte[BUFFER_LENGTH];
                            int bytesRead;

                            while ((bytesRead = inputStream.Read(inputBytes, 0, BUFFER_LENGTH)) > 0)
                            {
                                cryptoStream.Write(inputBytes, 0, bytesRead);
                            }

                            cryptoStream.FlushFinalBlock();
                        }
                    }
                }
            }
        }
#endif

        /// <summary>Encrypt the passed bytes</summary>
        /// <param name="inputBytes"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <example>
        /// 	<code title="Example" description="" lang="C#">Encryption encryption = new Encryption();
        ///  
        /// string creditCardNumber = "4444333322221111";
        /// byte[] inputBytes = Encoding.UTF8.GetBytes(creditCardNumber);
        /// string password = "&amp;%)(&amp;JUI";
        ///  
        /// byte[] encrypted = encryption.EncryptBytes(inputBytes, password);
        /// byte[] decrypted = encryption.DecryptBytes(encrypted, password);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">Dim encryption As New Encryption()
        ///  
        /// Dim creditCardNumber As String = "4444333322221111"
        /// Dim inputBytes() As Byte = Encoding.UTF8.GetBytes(creditCardNumber)
        /// Dim password As String = "&amp;%)(&amp;JUI"
        ///  
        /// Dim encrypted() As Byte = encryption.EncryptBytes(inputBytes, password)
        /// Dim decrypted() As Byte = encryption.DecryptBytes(encrypted, password)</code>
        /// </example>
        public byte[] EncryptBytes(byte[] inputBytes, string password)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
#if SILVERLIGHT
                using (AesManaged aes = new AesManaged())
#else
                using (RijndaelManaged aes = new RijndaelManaged())
#endif
                {
                    DerrivePasswordAndIv(aes, password);

                    using (CryptoStream cs = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputBytes, 0, inputBytes.Length);
                        cs.FlushFinalBlock();

                        return memoryStream.ToArray();
                    }
                }
            }
        }

        /// <summary>Decrypt the passed bytes</summary>
        /// <param name="inputBytes"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <example>
        /// 	<code title="Example" description="" lang="C#">Encryption encryption = new Encryption();
        ///  
        /// string creditCardNumber = "4444333322221111";
        /// byte[] inputBytes = Encoding.UTF8.GetBytes(creditCardNumber);
        /// string password = "&amp;%)(&amp;JUI";
        ///  
        /// byte[] encrypted = encryption.EncryptBytes(inputBytes, password);
        /// byte[] decrypted = encryption.DecryptBytes(encrypted, password);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">Dim encryption As New Encryption()
        ///  
        /// Dim creditCardNumber As String = "4444333322221111"
        /// Dim inputBytes() As Byte = Encoding.UTF8.GetBytes(creditCardNumber)
        /// Dim password As String = "&amp;%)(&amp;JUI"
        ///  
        /// Dim encrypted() As Byte = encryption.EncryptBytes(inputBytes, password)
        /// Dim decrypted() As Byte = encryption.DecryptBytes(encrypted, password)</code>
        /// </example>
        public byte[] DecryptBytes(byte[] inputBytes, string password)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
#if SILVERLIGHT
                using (AesManaged aes = new AesManaged())
#else
                using (RijndaelManaged aes = new RijndaelManaged())
#endif
                {
                    DerrivePasswordAndIv(aes, password);

                    using (CryptoStream cs = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputBytes, 0, inputBytes.Length);
                        cs.FlushFinalBlock();

                        return memoryStream.ToArray();
                    }
                }
            }
        }


#if SILVERLIGHT
        private void DerrivePasswordAndIv(AesManaged aes, string password)
#else
        private void DerrivePasswordAndIv(RijndaelManaged aes, string password)
#endif
        {
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;

            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(Salt));
            aes.Key = deriveBytes.GetBytes(aes.KeySize / 8);
            aes.IV = Encoding.UTF8.GetBytes(_ivChars, 0, aes.BlockSize / 8);
        }
    }
}