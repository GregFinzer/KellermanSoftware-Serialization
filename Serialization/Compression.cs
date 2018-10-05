#region Includes
using System;
using System.IO;
using System.IO.Compression;

#if !NETSTANDARD
using System.IO.IsolatedStorage;
#endif

#endregion

namespace KellermanSoftware.Serialization
{
    /// <summary>
    /// Compress/Decompress with GZip, Deflate, or MiniLZO
    /// </summary>
    public class Compression
    {
#region Class Variables
#if !NETSTANDARD
        private IsolatedStorageFile _store;
#endif
#endregion

#region Properties
#if !NETSTANDARD
        private IsolatedStorageFile Store
        {
            get { return _store ?? (_store = GetIsolatedStorage()); }
        }
#endif
#endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Compression()
        {
        }

#if !NETSTANDARD
        /// <summary>
        /// Constructor that takes in an IsolatedStorageFile
        /// </summary>
        /// <param name="store"></param>
        public Compression(IsolatedStorageFile store)
        {
            _store = store;
        }

        private IsolatedStorageFile GetIsolatedStorage()
        {
            return IsolatedStorageFile.GetUserStoreForDomain();
        }

        /// <summary>Compress a file in isolated storage using LZO compression</summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <example>
        /// 	<code title="Example" description="" lang="C#">string inputString = "".PadRight(1000, 'z');
        /// byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
        ///  
        /// #if SILVERLIGHT
        /// IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
        /// #else
        /// IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();
        /// #endif
        ///  
        /// //Create the input file
        /// string inputFilePath = "InputFile.txt";
        /// using (IsolatedStorageFileStream outputStream = new IsolatedStorageFileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, store))
        /// {
        ///     outputStream.Write(inputBytes,0,inputBytes.Length);
        /// }
        ///  
        /// //Compress it
        /// string compressedPath = "Compressed.txt";
        /// Compression compression = new Compression(store);
        /// compression.CompressFile(inputFilePath,compressedPath);
        ///  
        /// //Decompress
        /// string uncompressedPath = "Uncompressed.txt";
        /// compression.DecompressFile(compressedPath,uncompressedPath);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">Dim inputString As String = "".PadRight(1000, "z"c)
        /// Dim inputBytes() As Byte = Encoding.UTF8.GetBytes(inputString)
        ///  
        /// #If SILVERLIGHT Then
        /// Dim store As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
        /// #Else
        /// Dim store As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForDomain()
        /// #End If
        ///  
        /// 'Create the input file
        /// Dim inputFilePath As String = "InputFile.txt"
        /// Using outputStream As New IsolatedStorageFileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, store)
        /// 	outputStream.Write(inputBytes,0,inputBytes.Length)
        /// End Using
        ///  
        /// 'Compress it
        /// Dim compressedPath As String = "Compressed.txt"
        /// Dim compression As New Compression(store)
        /// compression.CompressFile(inputFilePath,compressedPath)
        ///  
        /// 'Decompress
        /// Dim uncompressedPath As String = "Uncompressed.txt"
        /// compression.DecompressFile(compressedPath,uncompressedPath)</code>
        /// </example>
        public void CompressIsolatedStorageFile(CompressionType compressionType, string inputFilePath, string outputFilePath)
        {
            using (IsolatedStorageFileStream inputStream = new IsolatedStorageFileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Store))
            {
                byte[] inputBytes = ReadAllBytes(inputStream);
                byte[] compressedBytes = CompressBytes(compressionType, inputBytes);

                using (IsolatedStorageFileStream outputStream = new IsolatedStorageFileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, Store))
                {
                    outputStream.Write(compressedBytes, 0, compressedBytes.Length);
                }
            }
        }

        /// <summary>Decompress a file in isolated storage using LZO compression</summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <example>
        /// 	<code title="Example" description="" lang="C#">string inputString = "".PadRight(1000, 'z');
        /// byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
        ///  
        /// #if SILVERLIGHT
        /// IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
        /// #else
        /// IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();
        /// #endif
        ///  
        /// //Create the input file
        /// string inputFilePath = "InputFile.txt";
        /// using (IsolatedStorageFileStream outputStream = new IsolatedStorageFileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, store))
        /// {
        ///     outputStream.Write(inputBytes,0,inputBytes.Length);
        /// }
        ///  
        /// //Compress it
        /// string compressedPath = "Compressed.txt";
        /// Compression compression = new Compression(store);
        /// compression.CompressFile(inputFilePath,compressedPath);
        ///  
        /// //Decompress
        /// string uncompressedPath = "Uncompressed.txt";
        /// compression.DecompressFile(compressedPath,uncompressedPath);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">Dim inputString As String = "".PadRight(1000, "z"c)
        /// Dim inputBytes() As Byte = Encoding.UTF8.GetBytes(inputString)
        ///  
        /// #If SILVERLIGHT Then
        /// Dim store As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
        /// #Else
        /// Dim store As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForDomain()
        /// #End If
        ///  
        /// 'Create the input file
        /// Dim inputFilePath As String = "InputFile.txt"
        /// Using outputStream As New IsolatedStorageFileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, store)
        /// 	outputStream.Write(inputBytes,0,inputBytes.Length)
        /// End Using
        ///  
        /// 'Compress it
        /// Dim compressedPath As String = "Compressed.txt"
        /// Dim compression As New Compression(store)
        /// compression.CompressFile(inputFilePath,compressedPath)
        ///  
        /// 'Decompress
        /// Dim uncompressedPath As String = "Uncompressed.txt"
        /// compression.DecompressFile(compressedPath,uncompressedPath)</code>
        /// </example>
        public void DecompressIsolatedStorageFile(CompressionType compressionType, string inputFilePath, string outputFilePath)
        {
            using (IsolatedStorageFileStream inputStream = new IsolatedStorageFileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Store))
            {

                byte[] inputBytes = ReadAllBytes(inputStream);
                byte[] decompressedBytes = DecompressBytes(compressionType, inputBytes);

                using (IsolatedStorageFileStream outputStream = new IsolatedStorageFileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, Store))
                {
                    outputStream.Write(decompressedBytes, 0, decompressedBytes.Length);
                }
            }
        }

        private byte[] ReadAllBytes(IsolatedStorageFileStream inputStream)
        {
            byte[] inputBytes;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = inputStream.Read(buffer, 0, 4096)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                inputBytes = memoryStream.ToArray();
            }

            return inputBytes;
        }
#endif

        /// <summary>
        /// Compress the passed bytes using the specified compression type
        /// </summary>
        /// <param name="compressionType"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">
        /// Compression compression = new Compression();
        /// 
        ///  
        /// byte[] input = new byte[1024];
        ///  
        /// for (int i = 0; i &lt; 1024; i++)
        ///     input[i] = 5;
        ///  
        /// byte[] results = compression.CompressBytes(CompressionType.GZip, input);
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with GZIP", input.Length, results.Length);
        ///  
        /// results = compression.CompressBytes(CompressionType.Deflate, input);
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with Deflate", input.Length, results.Length);
        ///  
        /// results = compression.CompressBytes(CompressionType.MiniLZO, input);
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with MiniLZO", input.Length, results.Length);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">
        /// Dim compression As New Compression()
        /// 
        ///  
        /// Dim input(1023) As Byte
        ///  
        /// For i As Integer = 0 To 1023
        ///     input(i) = 5
        /// Next i
        ///  
        /// Dim results() As Byte = compression.CompressBytes(CompressionType.GZip, input)
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with GZIP", input.Length, results.Length)
        ///  
        /// results = compression.CompressBytes(CompressionType.Deflate, input)
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with Deflate", input.Length, results.Length)
        ///  
        /// results = compression.CompressBytes(CompressionType.MiniLZO, input)
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with MiniLZO", input.Length, results.Length)</code>
        /// </example>
        public byte[] CompressBytes(CompressionType compressionType, byte[] input)
        {
            try
            {
                MemoryStream msInput = new MemoryStream();
                msInput.Write(input, 0, input.Length);
                return CompressMemoryStream(compressionType, msInput).ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not compress, check input. Error: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Decompress the passed bytes using the specified compression type
        /// </summary>
        /// <param name="compressionType"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">
        /// Compression compression = new Compression();
        /// 
        ///  
        /// byte[] input = new byte[1024];
        ///  
        /// for (int i = 0; i &lt; 1024; i++)
        ///     input[i] = 5;
        ///  
        /// byte[] compressedBytes = compression.CompressBytes(CompressionType.GZip, input);
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with GZIP", input.Length, compressedBytes.Length);
        /// byte[] decompressedBytes = compression.DecompressBytes(CompressionType.GZip, compressedBytes);
        ///  
        /// compressedBytes = compression.CompressBytes(CompressionType.Deflate, input);
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with Deflate", input.Length, compressedBytes.Length);
        /// decompressedBytes = compression.DecompressBytes(CompressionType.Deflate, compressedBytes);
        ///  
        /// compressedBytes = compression.CompressBytes(CompressionType.MiniLZO, input);
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with MiniLZO", input.Length, compressedBytes.Length);
        /// decompressedBytes = compression.DecompressBytes(CompressionType.MiniLZO, compressedBytes);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">
        /// Dim compression As New Compression()
        /// 
        ///  
        /// Dim input(1023) As Byte
        ///  
        /// For i As Integer = 0 To 1023
        ///     input(i) = 5
        /// Next i
        ///  
        /// Dim compressedBytes() As Byte = compression.CompressBytes(CompressionType.GZip, input)
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with GZIP", input.Length, compressedBytes.Length)
        /// Dim decompressedBytes() As Byte = compression.DecompressBytes(CompressionType.GZip, compressedBytes)
        ///  
        /// compressedBytes = compression.CompressBytes(CompressionType.Deflate, input)
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with Deflate", input.Length, compressedBytes.Length)
        /// decompressedBytes = compression.DecompressBytes(CompressionType.Deflate, compressedBytes)
        ///  
        /// compressedBytes = compression.CompressBytes(CompressionType.MiniLZO, input)
        /// Console.WriteLine("{0} bytes compressed to {1} bytes with MiniLZO", input.Length, compressedBytes.Length)
        /// decompressedBytes = compression.DecompressBytes(CompressionType.MiniLZO, compressedBytes)</code>
        /// </example>
        public byte[] DecompressBytes(CompressionType compressionType, byte[] input)
        {
            try
            {
                MemoryStream msInput = new MemoryStream();
                msInput.Write(input, 0, input.Length);
                return DecompressMemoryStream(compressionType, msInput).ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not decompress, check input. Error: " + ex.Message, ex);
            }
        }

        private MemoryStream CompressMemoryStream(CompressionType compressionType, MemoryStream input)
        {
            try
            {
                switch (compressionType)
                {
                    case CompressionType.Deflate:
                        return DeflateCompressMemoryStream(CompressionMode.Compress, input);
                    case CompressionType.GZip:
                        return GZipCompressMemoryStream(CompressionMode.Compress, input);
                    case CompressionType.MiniLZO:
                        return MiniLZOCompressMemoryStream(input);
                    default:
                        throw new NotSupportedException(compressionType.ToString() + " is not supported");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not compress, check input. Error: " + ex.Message, ex);
            }
        }

        private MemoryStream MiniLZOCompressMemoryStream(MemoryStream input)
        {
            return new MemoryStream(MiniLZO.Compress(input));
        }

        private MemoryStream DecompressMemoryStream(CompressionType compressionType, MemoryStream input)
        {
            try
            {
                switch (compressionType)
                {
                    case CompressionType.Deflate:
                        return DeflateDecompressMemoryStream(input);
                    case CompressionType.GZip:
                        return GZipDecompressMemoryStream(input);
                    case CompressionType.MiniLZO:
                        return MiniLZODecompressMemoryStream(input);
                    default:
                        throw new NotSupportedException(compressionType.ToString() + " is not supported");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not decompress, check input. Error: " + ex.Message, ex);
            }
        }

        private MemoryStream MiniLZODecompressMemoryStream(MemoryStream input)
        {
            return new MemoryStream(MiniLZO.Decompress(input.ToArray()));
        }

        /// <summary>
        /// Compress a stream using the specified compression type
        /// </summary>
        /// <param name="compressionType"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">
        /// Compression compression = new Compression();
        /// 
        ///  
        /// StringBuilder sb = new StringBuilder();
        /// for (int i = 0; i &lt; 1024; i++)
        ///     sb.AppendLine("This is a test");
        ///  
        /// File.WriteAllText("input.txt", sb.ToString());
        ///  
        /// using (FileStream inputStream = new FileStream("input.txt", FileMode.Open, FileAccess.Read))
        /// {
        ///     Stream compressedStream = compression.CompressStream(CompressionType.MiniLZO, inputStream);
        ///     Console.WriteLine("{0} bytes compressed to {1} bytes with MiniLZO", inputStream.Length, compressedStream.Length);
        /// }</code>
        /// 	<code title="Example2" description="" lang="VB.NET">
        /// Dim compression As New Compression()
        /// 
        ///  
        /// Dim sb As New StringBuilder()
        /// For i As Integer = 0 To 1023
        ///     sb.AppendLine("This is a test")
        /// Next i
        ///  
        /// File.WriteAllText("input.txt", sb.ToString())
        ///  
        /// Using inputStream As New FileStream("input.txt", FileMode.Open, FileAccess.Read)
        ///     Dim compressedStream As Stream = compression.CompressStream(CompressionType.MiniLZO, inputStream)
        ///     Console.WriteLine("{0} bytes compressed to {1} bytes with MiniLZO", inputStream.Length, compressedStream.Length)
        /// End Using</code>
        /// </example>
        public Stream CompressStream(CompressionType compressionType, Stream input)
        {
            try
            {
                switch (compressionType)
                {
                    case CompressionType.Deflate:
                        return DeflateCompressStream(input);
                    case CompressionType.GZip:
                        return GZipCompressStream(input);
                    case CompressionType.MiniLZO:
                        return MiniLZOCompressStream(input);
                    default:
                        throw new NotSupportedException(compressionType.ToString() + " is not supported");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not compress, check input. Error: " + ex.Message, ex);
            }
        }

        private Stream MiniLZOCompressStream(Stream input)
        {
            MemoryStream memoryStream = new MemoryStream();

            const int BUFFERSIZE = 10000;
            byte[] buffer = new byte[BUFFERSIZE];
            while (true)
            {
                int bytesRead = input.Read(buffer, 0, BUFFERSIZE);
                if (bytesRead == 0)
                {
                    break;
                }
                memoryStream.Write(buffer, 0, bytesRead);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            return new MemoryStream(MiniLZO.Compress(memoryStream));
        }

        /// <summary>
        /// Decompress a stream using the specified compression type
        /// </summary>
        /// <param name="compressionType"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">
        /// Compression compression = new Compression();
        /// 
        ///  
        /// StringBuilder sb = new StringBuilder();
        /// for (int i = 0; i &lt; 1024; i++)
        ///     sb.AppendLine("This is a test");
        ///  
        /// File.WriteAllText("input.txt", sb.ToString());
        ///  
        /// using (FileStream inputStream = new FileStream("input.txt", FileMode.Open, FileAccess.Read))
        /// {
        ///     Stream compressedStream = compression.CompressStream(CompressionType.MiniLZO, inputStream);
        ///     Console.WriteLine("{0} bytes compressed to {1} bytes with MiniLZO", inputStream.Length, compressedStream.Length);
        ///  
        ///     Stream decompressedStream = compression.DecompressStream(CompressionType.MiniLZO, compressedStream);
        /// }</code>
        /// 	<code title="Example2" description="" lang="VB.NET">
        /// Dim compression As New Compression()
        /// 
        ///  
        /// Dim sb As New StringBuilder()
        /// For i As Integer = 0 To 1023
        ///     sb.AppendLine("This is a test")
        /// Next i
        ///  
        /// File.WriteAllText("input.txt", sb.ToString())
        ///  
        /// Using inputStream As New FileStream("input.txt", FileMode.Open, FileAccess.Read)
        ///     Dim compressedStream As Stream = compression.CompressStream(CompressionType.MiniLZO, inputStream)
        ///     Console.WriteLine("{0} bytes compressed to {1} bytes with MiniLZO", inputStream.Length, compressedStream.Length)
        ///  
        ///     Dim decompressedStream As Stream = compression.DecompressStream(CompressionType.MiniLZO, compressedStream)
        /// End Using</code>
        /// </example>
        public Stream DecompressStream(CompressionType compressionType, Stream input)
        {
            try
            {
                switch (compressionType)
                {
                    case CompressionType.Deflate:
                        return DeflateDecompressStream(input);
                    case CompressionType.GZip:
                        return GZipDecompressStream(input);
                    case CompressionType.MiniLZO:
                        return MiniLZODecompressStream(input);
                    default:
                        throw new NotSupportedException(compressionType.ToString() + " is not supported");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not decompress, check input. Error: " + ex.Message, ex);
            }
        }

        private Stream MiniLZODecompressStream(Stream input)
        {
            MemoryStream memoryStream = new MemoryStream();

            const int BUFFERSIZE = 10000;
            byte[] buffer = new byte[BUFFERSIZE];
            while (true)
            {
                int bytesRead = input.Read(buffer, 0, BUFFERSIZE);
                if (bytesRead == 0)
                {
                    break;
                }
                memoryStream.Write(buffer, 0, bytesRead);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            return new MemoryStream(MiniLZO.Decompress(memoryStream.ToArray()));
        }

        /// <summary>
        /// Compress a file using the specified compression type
        /// </summary>
        /// <param name="compressionType"></param>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">
        /// Compression compression = new Compression();
        /// 
        ///  
        /// StringBuilder sb = new StringBuilder();
        /// for (int i = 0; i &lt; 1024; i++)
        ///     sb.AppendLine("This is a test");
        ///  
        /// File.WriteAllText("input.txt", sb.ToString());
        ///  
        /// compression.CompressFile(CompressionType.MiniLZO, "input.txt", "compressed.txt");
        /// compression.DecompressFile(CompressionType.MiniLZO, "compressed.txt", "decompressed.txt");</code>
        /// 	<code title="Example2" description="" lang="VB.NET">
        /// Dim compression As New Compression()
        /// 
        ///  
        /// Dim sb As New StringBuilder()
        /// For i As Integer = 0 To 1023
        ///     sb.AppendLine("This is a test")
        /// Next i
        ///  
        /// File.WriteAllText("input.txt", sb.ToString())
        ///  
        /// compression.CompressFile(CompressionType.MiniLZO, "input.txt", "compressed.txt")
        /// compression.DecompressFile(CompressionType.MiniLZO, "compressed.txt", "decompressed.txt")</code>
        /// </example>
        public void CompressFile(CompressionType compressionType, string inputFilePath, string outputFilePath)
        {
            try
            {
                switch (compressionType)
                {
                    case CompressionType.Deflate:
                        DeflateCompressFile(inputFilePath, outputFilePath);
                        break;
                    case CompressionType.GZip:
                        GZipCompressFile(inputFilePath, outputFilePath);
                        break;
                    case CompressionType.MiniLZO:
                        MiniLZOCompressFile(inputFilePath, outputFilePath);
                        break;
                    default:
                        throw new NotSupportedException(compressionType.ToString() + " is not supported");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not compress, check input. Error: " + ex.Message, ex);
            }
        }

        private void MiniLZOCompressFile(string inputFilePath, string outputFilePath)
        {
            using (FileStream inputFileStream = new FileStream(inputFilePath, FileMode.Open))
            {
                Stream stream = MiniLZOCompressStream(inputFileStream);
                stream.Seek(0, SeekOrigin.Begin);

                using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    const int BUFFERSIZE = 10000;
                    byte[] buffer = new byte[BUFFERSIZE];
                    while (true)
                    {
                        int bytesRead = stream.Read(
                            buffer, 0, BUFFERSIZE);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        outputFileStream.Write(
                            buffer, 0, bytesRead);
                    }
                }
            }
        }

        /// <summary>
        /// Decompress a file using the specified compression type
        /// </summary>
        /// <param name="compressionType"></param>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">
        /// Compression compression = new Compression();
        /// 
        ///  
        /// StringBuilder sb = new StringBuilder();
        /// for (int i = 0; i &lt; 1024; i++)
        ///     sb.AppendLine("This is a test");
        ///  
        /// File.WriteAllText("input.txt", sb.ToString());
        ///  
        /// compression.CompressFile(CompressionType.MiniLZO, "input.txt", "compressed.txt");
        /// compression.DecompressFile(CompressionType.MiniLZO, "compressed.txt", "decompressed.txt");</code>
        /// 	<code title="Example2" description="" lang="VB.NET">
        /// Dim compression As New Compression()
        /// 
        ///  
        /// Dim sb As New StringBuilder()
        /// For i As Integer = 0 To 1023
        ///     sb.AppendLine("This is a test")
        /// Next i
        ///  
        /// File.WriteAllText("input.txt", sb.ToString())
        ///  
        /// compression.CompressFile(CompressionType.MiniLZO, "input.txt", "compressed.txt")
        /// compression.DecompressFile(CompressionType.MiniLZO, "compressed.txt", "decompressed.txt")</code>
        /// </example>
        public void DecompressFile(CompressionType compressionType, string inputFilePath, string outputFilePath)
        {
            try
            {
                switch (compressionType)
                {
                    case CompressionType.Deflate:
                        DeflateDecompressFile(inputFilePath, outputFilePath);
                        break;
                    case CompressionType.GZip:
                        GZipDecompressFile(inputFilePath, outputFilePath);
                        break;
                    case CompressionType.MiniLZO:
                        MiniLZODecompressFile(inputFilePath, outputFilePath);
                        break;
                    default:
                        throw new NotSupportedException(compressionType.ToString() + " is not supported");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not decompress, check input. Error: " + ex.Message, ex);
            }
        }

        private void MiniLZODecompressFile(string inputFilePath, string outputFilePath)
        {
            using (FileStream inputFileStream = new FileStream(inputFilePath, FileMode.Open))
            {
                Stream stream = MiniLZODecompressStream(inputFileStream);
                stream.Seek(0, SeekOrigin.Begin);

                using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    const int BUFFERSIZE = 10000;
                    byte[] buffer = new byte[BUFFERSIZE];
                    while (true)
                    {
                        int bytesRead = stream.Read(
                            buffer, 0, BUFFERSIZE);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        outputFileStream.Write(
                            buffer, 0, bytesRead);
                    }
                }
            }
        }

#region Compress/Decompress Memory Stream

        /// <summary>
        /// Compress a memory stream with Gzip
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private MemoryStream GZipCompressMemoryStream(CompressionMode mode, MemoryStream input)
        {
            MemoryStream outputStream = new MemoryStream();
            input.Position = 0;

            using (GZipStream compressedFileStream = new GZipStream(outputStream, mode))
            {
                // You've got all the streams you need. 
                // Now do the compression.
                const int BUFFERSIZE = 10000;
                int bytesRead = 0;
                byte[] buffer = new byte[BUFFERSIZE];
                while (true)
                {
                    bytesRead = input.Read(
                      buffer, 0, BUFFERSIZE);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    compressedFileStream.Write(
                      buffer, 0, bytesRead);
                }
            }

            return outputStream;
        }

        /// <summary>
        /// Compress a memory stream with deflate
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private MemoryStream DeflateCompressMemoryStream(CompressionMode mode, MemoryStream input)
        {
            MemoryStream outputStream = new MemoryStream();
            input.Position = 0;

            using (DeflateStream compressedStream = new DeflateStream(outputStream, mode))
            {
                // You've got all the streams you need. 
                // Now do the compression.
                const int BUFFERSIZE = 10000;
                int bytesRead = 0;
                byte[] buffer = new byte[BUFFERSIZE];
                while (true)
                {
                    bytesRead = input.Read(
                      buffer, 0, BUFFERSIZE);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    compressedStream.Write(
                      buffer, 0, bytesRead);
                }
            }

            return outputStream;
        }

        /// <summary>
        /// Decompress a Memory stream compress with Deflate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public MemoryStream DeflateDecompressMemoryStream(MemoryStream input)
        {
            MemoryStream outputStream = new MemoryStream();
            input.Position = 0;

            using (DeflateStream compressedStream = new DeflateStream(input, CompressionMode.Decompress))
            {
                // You've got all the streams you need. 
                // Now do the compression.
                const int BUFFERSIZE = 10000;
                int bytesRead = 0;
                byte[] buffer = new byte[BUFFERSIZE];
                while (true)
                {
                    bytesRead = compressedStream.Read(buffer, 0, BUFFERSIZE);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    outputStream.Write(buffer, 0, bytesRead);
                }
            }

            return outputStream;
        }

        /// <summary>
        /// Decompress a Memory stream compress with GZip
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public MemoryStream GZipDecompressMemoryStream(MemoryStream input)
        {
            MemoryStream outputStream = new MemoryStream();
            input.Position = 0;

            using (GZipStream compressedStream = new GZipStream(input, CompressionMode.Decompress))
            {
                // You've got all the streams you need. 
                // Now do the compression.
                const int BUFFERSIZE = 10000;
                int bytesRead = 0;
                byte[] buffer = new byte[BUFFERSIZE];
                while (true)
                {
                    bytesRead = compressedStream.Read(buffer, 0, BUFFERSIZE);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    outputStream.Write(buffer, 0, bytesRead);
                }
            }

            return outputStream;
        }
#endregion

#region Compress/Decompress Streams
        /// <summary>
        /// Compress a stream using Gzip
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Stream GZipCompressStream(Stream input)
        {
            MemoryStream outputStream = new MemoryStream();
            input.Position = 0;

            using (GZipStream compressedFileStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                // You've got all the streams you need. 
                // Now do the compression.
                const int BUFFERSIZE = 10000;
                int bytesRead = 0;
                byte[] buffer = new byte[BUFFERSIZE];
                while (true)
                {
                    bytesRead = input.Read(
                      buffer, 0, BUFFERSIZE);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    compressedFileStream.Write(
                      buffer, 0, bytesRead);
                }
            }

            return outputStream;
        }

        /// <summary>
        /// Compress a stream using Deflate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Stream DeflateCompressStream(Stream input)
        {
            MemoryStream outputStream = new MemoryStream();
            input.Position = 0;

            using (DeflateStream compressedStream = new DeflateStream(outputStream, CompressionMode.Compress))
            {
                // You've got all the streams you need. 
                // Now do the compression.
                const int BUFFERSIZE = 10000;
                int bytesRead = 0;
                byte[] buffer = new byte[BUFFERSIZE];
                while (true)
                {
                    bytesRead = input.Read(
                      buffer, 0, BUFFERSIZE);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    compressedStream.Write(
                      buffer, 0, bytesRead);
                }
            }

            return outputStream;
        }

        /// <summary>
        /// Decompress a stream compress with Deflate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Stream DeflateDecompressStream(Stream input)
        {
            MemoryStream outputStream = new MemoryStream();
            input.Position = 0;

            using (DeflateStream compressedStream = new DeflateStream(input, CompressionMode.Decompress))
            {
                // You've got all the streams you need. 
                // Now do the compression.
                const int BUFFERSIZE = 10000;
                int bytesRead = 0;
                byte[] buffer = new byte[BUFFERSIZE];
                while (true)
                {
                    bytesRead = compressedStream.Read(buffer, 0, BUFFERSIZE);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    outputStream.Write(buffer, 0, bytesRead);
                }
            }

            return outputStream;
        }

        /// <summary>
        /// Decompress a stream compress with GZip
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Stream GZipDecompressStream(Stream input)
        {
            MemoryStream outputStream = new MemoryStream();
            input.Position = 0;

            using (GZipStream compressedStream = new GZipStream(input, CompressionMode.Decompress))
            {
                // You've got all the streams you need. 
                // Now do the compression.
                const int BUFFERSIZE = 10000;
                int bytesRead = 0;
                byte[] buffer = new byte[BUFFERSIZE];
                while (true)
                {
                    bytesRead = compressedStream.Read(buffer, 0, BUFFERSIZE);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    outputStream.Write(buffer, 0, bytesRead);
                }
            }

            return outputStream;
        }






#endregion

#region File Compression/Decompression
        /// <summary>
        /// Compress a file with the deflate algorithim
        /// </summary>        
        /// <param name="inputFileName"></param>
        /// <param name="outputFileName"></param>
        private void DeflateCompressFile(String inputFileName, String outputFileName)
        {
            using (FileStream inputFileStream = new FileStream(inputFileName, FileMode.Open))
            {
                using (FileStream outputFileStream = new FileStream(outputFileName, FileMode.Create))
                {
                    using (DeflateStream compressedFileStream = new DeflateStream(outputFileStream, CompressionMode.Compress))
                    {
                        // You've got all the streams you need. 
                        // Now do the compression.
                        const int BUFFERSIZE = 10000;
                        int bytesRead = 0;
                        byte[] buffer = new byte[BUFFERSIZE];
                        while (true)
                        {
                            bytesRead = inputFileStream.Read(
                              buffer, 0, BUFFERSIZE);
                            if (bytesRead == 0)
                            {
                                break;
                            }
                            compressedFileStream.Write(
                              buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compress a file with the GZip algorithim
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <param name="outputFileName"></param>
        private void GZipCompressFile(String inputFileName, String outputFileName)
        {
            using (FileStream inputFileStream =
              new FileStream(inputFileName, FileMode.Open))
            {
                using (FileStream outputFileStream = new FileStream(outputFileName, FileMode.Create))
                {
                    using (GZipStream compressedFileStream = new GZipStream(outputFileStream, CompressionMode.Compress))
                    {
                        // You've got all the streams you need. 
                        // Now do the compression.
                        const int BUFFERSIZE = 10000;
                        int bytesRead = 0;
                        byte[] buffer = new byte[BUFFERSIZE];
                        while (true)
                        {
                            bytesRead = inputFileStream.Read(
                              buffer, 0, BUFFERSIZE);
                            if (bytesRead == 0)
                            {
                                break;
                            }
                            compressedFileStream.Write(
                              buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decompress a file using Gzip Algorithm
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <param name="outputFileName"></param>
        public void GZipDecompressFile(String inputFileName, String outputFileName)
        {
            using (FileStream inputFileStream = new FileStream(inputFileName, FileMode.Open))
            {
                using (FileStream outputFileStream = new FileStream(outputFileName, FileMode.Create))
                {
                    using (GZipStream compressedFileStream = new GZipStream(inputFileStream, CompressionMode.Decompress))
                    {
                        // You've got all the streams you need. 
                        // Now do the compression.
                        const int BUFFERSIZE = 10000;
                        int bytesRead = 0;
                        byte[] buffer = new byte[BUFFERSIZE];
                        while (true)
                        {
                            bytesRead = compressedFileStream.Read(buffer, 0, BUFFERSIZE);
                            if (bytesRead == 0)
                            {
                                break;
                            }
                            outputFileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decompress a file using Deflate Algorithm
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <param name="outputFileName"></param>
        public void DeflateDecompressFile(String inputFileName, String outputFileName)
        {
            using (FileStream inputFileStream = new FileStream(inputFileName, FileMode.Open))
            {
                using (FileStream outputFileStream = new FileStream(outputFileName, FileMode.Create))
                {
                    using (DeflateStream compressedFileStream = new DeflateStream(inputFileStream, CompressionMode.Decompress))
                    {
                        // You've got all the streams you need. 
                        // Now do the compression.
                        const int BUFFERSIZE = 10000;
                        int bytesRead = 0;
                        byte[] buffer = new byte[BUFFERSIZE];
                        while (true)
                        {
                            bytesRead = compressedFileStream.Read(buffer, 0, BUFFERSIZE);
                            if (bytesRead == 0)
                            {
                                break;
                            }
                            outputFileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }
#endregion

    }
}