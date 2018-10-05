#region Includes

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;
using NUnit.Framework;

#endregion

namespace SerializationTests
{
    [TestFixture]
    public class EncryptionTests
    {
        [Test]
        public void EncryptStringTest()
        {
            Encryption encryption = new Encryption();

            string creditCardNumber = "4444333322221111";
            string password = "&%)(&JUI";

            string encrypted = encryption.EncryptString(creditCardNumber, password);
            Assert.AreNotEqual(creditCardNumber, encrypted);

            string decrypted = encryption.DecryptString(encrypted, password);

            Assert.AreEqual(creditCardNumber, decrypted);
        }

        [Test]
        public void BadPasswordTest()
        {
            Encryption encryption = new Encryption();

            string creditCardNumber = "4444333322221111";
            string password = "&%)(&JUI";

            string encrypted = encryption.EncryptString(creditCardNumber, password);
            Assert.AreNotEqual(creditCardNumber, encrypted);

            Assert.That(() => encryption.DecryptString(encrypted, "badpassword"), Throws.TypeOf< System.Security.Cryptography.CryptographicException>());
        }

        [Test]
        public void EncryptBytesTest()
        {
            Encryption encryption = new Encryption();

            string creditCardNumber = "4444333322221111";
            byte[] inputBytes = Encoding.UTF8.GetBytes(creditCardNumber);
            string password = "&%)(&JUI";

            byte[] encrypted = encryption.EncryptBytes(inputBytes, password);
            byte[] decrypted = encryption.DecryptBytes(encrypted, password);

            Assert.AreEqual(decrypted, inputBytes);

        }

#if !NETSTANDARD
        [Test]
        public void EncryptIsolatedStorageFileTest()
        {
            string password = "&%)(&JUI";
            string creditCardNumber = "4444333322221111";
            byte[] inputBytes = Encoding.UTF8.GetBytes(creditCardNumber);

            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();

            //Create the input file
            string inputFilePath = "InputFile.txt";
            using (IsolatedStorageFileStream outputStream = new IsolatedStorageFileStream(inputFilePath,
                FileMode.Create, FileAccess.Write, FileShare.None, store))
            {
                outputStream.Write(inputBytes, 0, inputBytes.Length);
            }

            //Encrypt it
            string encryptedPath = "Encrypted.txt";
            Encryption encryption = new Encryption(store);
            encryption.EncryptIsolatedStorageFile(inputFilePath, encryptedPath, password);

            //Decrypt
            string decryptedPath = "Decrypted.txt";
            encryption.DecryptIsolatedStorageFile(encryptedPath, decryptedPath, password);

            byte[] decryptedBytes = new byte[inputBytes.Length];

            using (IsolatedStorageFileStream inputStream =
                new IsolatedStorageFileStream(decryptedPath, FileMode.Open, FileAccess.Read, FileShare.Read, store))
            {
                inputStream.Read(decryptedBytes, 0, inputBytes.Length);
            }

            Assert.AreEqual(inputBytes, decryptedBytes);


        }
#endif
    }
}
