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
    public class CompressionTests
    {
        [Test]
        public void CompressBytesTest()
        {
            string inputString = "".PadRight(1000, 'z');
            Compression compression = new Compression();
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
            byte[] compressedBytes = compression.CompressBytes(CompressionType.MiniLZO, inputBytes);
            byte[] decompressedBytes = compression.DecompressBytes(CompressionType.MiniLZO, compressedBytes);
            
            Assert.LessOrEqual(compressedBytes.Length,inputBytes.Length);
            Assert.AreEqual(inputBytes, decompressedBytes);
        }

#if !NETSTANDARD
        [Test]
        public void CompressIsolatedStorageFileTest()
        {
            string inputString = "".PadRight(1000, 'z');
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);

            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();

            //Create the input file
            string inputFilePath = "InputFile.txt";
            using (IsolatedStorageFileStream outputStream = new IsolatedStorageFileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, store))
            {
                outputStream.Write(inputBytes,0,inputBytes.Length);
            }

            //Compress it
            string compressedPath = "Compressed.txt";
            Compression compression = new Compression(store);
            compression.CompressIsolatedStorageFile(CompressionType.MiniLZO, inputFilePath,compressedPath);

            //Decompress
            string uncompressedPath = "Uncompressed.txt";
            compression.DecompressIsolatedStorageFile(CompressionType.MiniLZO, compressedPath, uncompressedPath);

            byte[] decompressedBytes = new byte[inputBytes.Length];

            using (IsolatedStorageFileStream inputStream = new IsolatedStorageFileStream(uncompressedPath, FileMode.Open, FileAccess.Read, FileShare.Read, store))
            {
                inputStream.Read(decompressedBytes, 0, inputBytes.Length);
            }

            Assert.AreEqual(inputBytes,decompressedBytes);


        }
#endif
    }
}
