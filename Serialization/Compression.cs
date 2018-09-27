#region Includes
// Modified by Owen Emlen to avoid using unsafe
// Probably slower than the original, but this mod lets us use the class with Silverlight/etc
#define BOUNDARY_CHECKS

using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;



#endregion

namespace KellermanSoftware.Serialization
{
    /// <summary>
    /// LZO Compression Compatible with .NET, Silverlight and Windows Phone 7
    /// </summary>
    public class Compression
    {
        #region Class Variables

        private IsolatedStorageFile _store;
        private const byte BITS = 14;
        private const uint D_MASK = (1 << BITS) - 1;
        private const uint M2_MAX_LEN = 8;
        private const uint M2_MAX_OFFSET = 0x0800;
        private const byte M3_MARKER = 32;
        private const uint M3_MAX_OFFSET = 0x4000;
        private const byte M4_MARKER = 16;
        private const uint M4_MAX_LEN = 9;
        private const uint M4_MAX_OFFSET = 0xbfff;

        private uint _dictSize = 65536 + 3;
#endregion

#region Properties
        private IsolatedStorageFile Store
        {
            get { return _store ?? (_store = GetIsolatedStorage()); }
        }
#endregion

#region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Compression()
        {
            SetupDict();
        }


        private IsolatedStorageFile GetIsolatedStorage()
        {
            return IsolatedStorageFile.GetUserStoreForDomain();
        }

        private void SetupDict()
        {
            if (IntPtr.Size == 8)
            {
                _dictSize = (65536 + 3) * 2;
            }
            else
            {
                _dictSize = 65536 + 3;
            }
        }

        /// <summary>
        /// Constructor that takes in an IsolatedStorageFile
        /// </summary>
        /// <param name="store"></param>
        public Compression(IsolatedStorageFile store)
        {
            _store = store;
            SetupDict();
        }

        #endregion

        #region Public Methods


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
        public void CompressFile(string inputFilePath, string outputFilePath)
        {
            using (IsolatedStorageFileStream inputStream = new IsolatedStorageFileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Store))
            {
                byte[] inputBytes = ReadAllBytes(inputStream);
                byte[] compressedBytes = CompressBytes(inputBytes);

                using (IsolatedStorageFileStream outputStream = new IsolatedStorageFileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, Store))
                {
                    outputStream.Write(compressedBytes,0,compressedBytes.Length);
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
        public void DecompressFile(string inputFilePath, string outputFilePath)
        {
            using (IsolatedStorageFileStream inputStream = new IsolatedStorageFileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Store))
            {

                byte[] inputBytes = ReadAllBytes(inputStream);
                byte[] decompressedBytes = DecompressBytes(inputBytes);

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


        /// <summary>Compress the passed in bytes using LZO compression</summary>
        /// <param name="inputBytes"></param>
        /// <returns></returns>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">string inputString = "".PadRight(1000, 'z');
        /// Compression compression = new Compression();
        /// byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
        /// byte[] compressedBytes = compression.CompressBytes(inputBytes);
        /// byte[] decompressedBytes = compression.DecompressBytes(compressedBytes);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">Dim inputString As String = "".PadRight(1000, "z"c)
        /// Dim compression As New Compression()
        /// Dim inputBytes() As Byte = Encoding.UTF8.GetBytes(inputString)
        /// Dim compressedBytes() As Byte = compression.CompressBytes(inputBytes)
        /// Dim decompressedBytes() As Byte = compression.DecompressBytes(compressedBytes)</code>
        /// </example>
        public byte[] CompressBytes(byte[] inputBytes)
        {
            if (inputBytes == null)
                return null;

            return CompressBytes(inputBytes, 0, inputBytes.Length);
        }



        /// <summary>
        /// Compress a range of bytes using LZO compression
        /// </summary>
        /// <param name="inputBytes"></param>
        /// <param name="inputByteStart"></param>
        /// <param name="inputByteLength"></param>
        /// <returns></returns>
        public byte[] CompressBytes(byte[] inputBytes, int inputByteStart, int inputByteLength)
        {
            if (inputBytes == null)
                throw new ArgumentNullException("inputBytes");

            if (inputByteStart < 0)
                throw new ArgumentOutOfRangeException("inputByteStart was " + inputByteStart);

            if (inputByteStart + inputByteLength > inputBytes.Length)
                throw new ArgumentOutOfRangeException("inputBytes[] has length " + inputBytes.Length + ", but inputByteStart + inputByteLength was " + (inputByteStart + inputByteLength));

            uint dstlen = (uint)(inputByteLength + (inputByteLength / 16) + 64 + 3 + 4);
            byte[] dst = new byte[dstlen];

            uint compressedSize = CompressBytes(inputBytes, (uint)inputByteStart, (uint)inputByteLength, dst, 0, dstlen, null);

            if (dst.Length != compressedSize)
            {
                byte[] final = new byte[compressedSize];
                Buffer.BlockCopy(dst, 0, final, 0, (int)compressedSize);
                dst = final;
            }

            return dst;
        }



        /// <summary>Decompress the passed bytes using LZO compression</summary>
        /// <param name="inputBytes"></param>
        /// <returns></returns>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">string inputString = "".PadRight(1000, 'z');
        /// Compression compression = new Compression();
        /// byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
        /// byte[] compressedBytes = compression.CompressBytes(inputBytes);
        /// byte[] decompressedBytes = compression.DecompressBytes(compressedBytes);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">Dim inputString As String = "".PadRight(1000, "z"c)
        /// Dim compression As New Compression()
        /// Dim inputBytes() As Byte = Encoding.UTF8.GetBytes(inputString)
        /// Dim compressedBytes() As Byte = compression.CompressBytes(inputBytes)
        /// Dim decompressedBytes() As Byte = compression.DecompressBytes(compressedBytes)</code>
        /// </example>
        public byte[] DecompressBytes(byte[] inputBytes)
        {
            if (inputBytes == null)
                return null;

            byte[] dst = new byte[(inputBytes[inputBytes.Length - 4] | (inputBytes[inputBytes.Length - 3] << 8) | (inputBytes[inputBytes.Length - 2] << 16 | inputBytes[inputBytes.Length - 1] << 24))];

            uint t = 0;
            uint lipEnd = (uint)inputBytes.Length - 4;
            uint lopEnd = (uint)dst.Length;

            uint lip = 0;
            uint lop = 0;
            bool match = false;
            bool matchNext = false;
            bool matchDone = false;
            bool copyMatch = false;
            bool firstLiteralRun = false;
            bool eofFound = false;

            if (inputBytes[lip] > 17)
            {
                t = (uint)(inputBytes[lip] - 17); lip++;
                if (t < 4)
                    matchNext = true;
                else
                {
#if BOUNDARY_CHECKS
                    Debug.Assert(t > 0);
                    if ((lopEnd - lop) < t)
                        throw new OverflowException("Output Overrun");
                    if ((lipEnd - lip) < t + 1)
                        throw new OverflowException("Input Overrun");
#endif
                    do
                    {
                        dst[lop] = inputBytes[lip]; lop++; lip++;
                    } while (--t > 0);
                    firstLiteralRun = true;
                }
            }
            while (!eofFound && lip < lipEnd)
            {
                if (!matchNext && !firstLiteralRun)
                {
                    t = inputBytes[lip]; lip++;
                    if (t >= 16)
                        match = true;
                    else
                    {
                        if (t == 0)
                        {
#if BOUNDARY_CHECKS
                            if ((lipEnd - lip) < 1)
                                throw new OverflowException("Input Overrun");
#endif
                            while (inputBytes[lip] == 0)
                            {
                                t += 255;
                                lip++;
#if BOUNDARY_CHECKS
                                if ((lipEnd - lip) < 1)
                                    throw new OverflowException("Input Overrun");
#endif
                            }
                            t += (uint)(15 + inputBytes[lip]);
                            lip++;
                        }
#if BOUNDARY_CHECKS
                        Debug.Assert(t > 0);
                        if ((lopEnd - lop) < t + 3)
                            throw new OverflowException("Output Overrun");
                        if ((lipEnd - lip) < t + 4)
                            throw new OverflowException("Input Overrun");
#endif
                        for (int x = 0; x < 4; ++x, ++lop, ++lip)
                            dst[lop] = inputBytes[lip];
                        if (--t > 0)
                        {
                            if (t >= 4)
                            {
                                do
                                {
                                    for (int x = 0; x < 4; ++x, ++lop, ++lip)
                                        dst[lop] = inputBytes[lip];
                                    t -= 4;
                                } while (t >= 4);
                                if (t > 0)
                                {
                                    do
                                    {
                                        dst[lop] = inputBytes[lip]; lop++; lip++;
                                    } while (--t > 0);
                                }
                            }
                            else
                            {
                                do
                                {
                                    dst[lop] = inputBytes[lip]; lop++; lip++;
                                } while (--t > 0);
                            }
                        }
                    }
                }
                uint lpos;
                if (!match && !matchNext)
                {
                    firstLiteralRun = false;

                    t = inputBytes[lip]; lip++;
                    if (t >= 16)
                        match = true;
                    else
                    {
                        lpos = lop - (1 + M2_MAX_OFFSET);
                        lpos -= t >> 2;
                        lpos -= ((uint)inputBytes[lip]) << 2;
                        lip++;
#if BOUNDARY_CHECKS
                        if (lpos < 0 || lpos >= lop)
                            throw new OverflowException("Lookbehind Overrun");
                        if ((lopEnd - lop) < 3)
                            throw new OverflowException("Output Overrun");
#endif

                        dst[lop] = dst[lpos]; lop++; lpos++;
                        dst[lop] = dst[lpos]; lop++; lpos++;
                        dst[lop] = dst[lpos]; lop++; lpos++;
                        matchDone = true;
                    }
                }
                match = false;
                do
                {
                    if (t >= 64)
                    {
                        lpos = lop - 1;
                        lpos -= (t >> 2) & 7;
                        lpos -= ((uint)inputBytes[lip]) << 3;
                        lip++;
                        t = (t >> 5) - 1;
#if BOUNDARY_CHECKS
                        if (lpos < 0 || lpos >= lop)
                            throw new OverflowException("Lookbehind Overrun");
                        if ((lopEnd - lop) < t + 2)
                            throw new OverflowException("Output Overrun");
#endif
                        copyMatch = true;
                    }
                    else if (t >= 32)
                    {
                        t &= 31;
                        if (t == 0)
                        {
#if BOUNDARY_CHECKS
                            if ((lipEnd - lip) < 1)
                                throw new OverflowException("Input Overrun");
#endif
                            while (inputBytes[lip] == 0)
                            {
                                t += 255;
                                lip++;
#if BOUNDARY_CHECKS
                                if ((lipEnd - lip) < 1)
                                    throw new OverflowException("Input Overrun");
#endif
                            }
                            t += (uint)(31 + inputBytes[lip]);
                            lip++;
                        }
                        lpos = lop - 1;
                        lpos -= (uint)GetUShortFrom2Bytes(inputBytes, lip) >> 2;
                        lip += 2;
                    }
                    else if (t >= 16)
                    {
                        lpos = lop;
                        lpos -= (t & 8) << 11;

                        t &= 7;
                        if (t == 0)
                        {
#if BOUNDARY_CHECKS
                            if ((lipEnd - lip) < 1)
                                throw new OverflowException("Input Overrun");
#endif
                            while (inputBytes[lip] == 0)
                            {
                                t += 255;
                                lip++;
#if BOUNDARY_CHECKS
                                if ((lipEnd - lip) < 1)
                                    throw new OverflowException("Input Overrun");
#endif
                            }
                            t += (uint)(7 + inputBytes[lip]);
                            lip++;
                        }
                        lpos -= (uint)GetUShortFrom2Bytes(inputBytes, lip) >> 2;
                        lip += 2;
                        if (lpos == lop)
                            eofFound = true;
                        else
                            lpos -= 0x4000;
                    }
                    else
                    {
                        lpos = lop - 1;
                        lpos -= t >> 2;
                        lpos -= ((uint)inputBytes[lip]) << 2;
                        lip++;
#if BOUNDARY_CHECKS
                        if (lpos < 0 || lpos >= lop)
                            throw new OverflowException("Lookbehind Overrun");
                        if ((lopEnd - lop) < 2)
                            throw new OverflowException("Output Overrun");
#endif
                        dst[lop] = dst[lpos]; lop++; lpos++;
                        dst[lop] = dst[lpos]; lop++; lpos++;
                        matchDone = true;
                    }
                    if (!eofFound && !matchDone && !copyMatch)
                    {
#if BOUNDARY_CHECKS
                        if (lpos < 0 || lpos >= lop)
                            throw new OverflowException("Lookbehind Overrun");
                        Debug.Assert(t > 0);
                        if ((lopEnd - lop) < t + 2)
                            throw new OverflowException("Output Overrun");
#endif
                    }
                    if (!eofFound && t >= 2 * 4 - 2 && (lop - lpos) >= 4 && !matchDone && !copyMatch)
                    {
                        for (int x = 0; x < 4; ++x, ++lop, ++lpos)
                            dst[lop] = dst[lpos];

                        t -= 2;
                        do
                        {
                            for (int x = 0; x < 4; ++x, ++lop, ++lpos)
                                dst[lop] = dst[lpos];
                            t -= 4;
                        } while (t >= 4);
                        if (t > 0)
                        {
                            do
                            {
                                dst[lop] = dst[lpos]; lop++; lpos++;
                            } while (--t > 0);
                        }
                    }
                    else if (!eofFound && !matchDone)
                    {
                        copyMatch = false;

                        dst[lop] = dst[lpos]; lop++; lpos++;
                        dst[lop] = dst[lpos]; lop++; lpos++;
                        do
                        {
                            dst[lop] = dst[lpos]; lop++; lpos++;
                        } while (--t > 0);
                    }

                    if (!eofFound && !matchNext)
                    {
                        matchDone = false;

                        t = (uint)(inputBytes[lip - 2] & 3);
                        if (t == 0)
                            break;
                    }
                    if (!eofFound)
                    {
                        matchNext = false;
#if BOUNDARY_CHECKS
                        Debug.Assert(t > 0);
                        Debug.Assert(t < 4);
                        if ((lopEnd - lop) < t)
                            throw new OverflowException("Output Overrun");
                        if ((lipEnd - lip) < t + 1)
                            throw new OverflowException("Input Overrun");
#endif
                        dst[lop] = inputBytes[lip]; lop++; lip++;
                        if (t > 1)
                        {
                            dst[lop] = inputBytes[lip]; lop++; lip++;
                            if (t > 2)
                            {
                                dst[lop] = inputBytes[lip]; lop++; lip++;
                            }
                        }
                        t = inputBytes[lip]; lip++;
                    }
                } while (!eofFound && lip < lipEnd);
            }
            if (!eofFound)
                throw new OverflowException("EOF Marker Not Found");
            else
            {
#if BOUNDARY_CHECKS
                Debug.Assert(t == 1);
                if (lip > lipEnd)
                    throw new OverflowException("Input Overrun");

                if (lip < lipEnd)
                    throw new OverflowException("Input Not Consumed");
#endif
            }


            return dst;
        }
#endregion

#region Private Methods


        private uint CompressBytes(byte[] inputBytes, 
            uint inputByteStart, 
            uint inputBytesLength, 
            byte[] outputBytes, 
            uint outputByteStart, 
            uint outputBytesLength, 
            uint[] dictNew)
        {
            // using all of dict_size?
            if (dictNew == null)
                dictNew = new uint[_dictSize];

            uint tmp;
            if (inputBytesLength <= M2_MAX_LEN + 5)
            {
                tmp = inputBytesLength;
                outputBytesLength = 0;
            }
            else
            {
                uint linEnd = inputByteStart + inputBytesLength;
                uint lipEnd = inputByteStart + inputBytesLength - M2_MAX_LEN - 5;

                uint lii = inputByteStart;
                uint lip = inputByteStart + 4;
                uint lop = outputByteStart;

                bool literal = false;
                bool match = false;

                for (; ; )
                {
                    uint offset = 0;
                    uint index = DIndex1(inputBytes, lip);
                    uint lpos = lip - (lip - dictNew[index]);

                    if (lpos < inputByteStart || (offset = (lip - lpos)) <= 0 || offset > M4_MAX_OFFSET)
                        literal = true;
                    else if (offset <= M2_MAX_OFFSET || inputBytes[lpos + 3] == inputBytes[lip + 3]) { }
                    else
                    {
                        index = DIndex2(index);
                        lpos = lip - (lip - dictNew[index]);
                        if (lpos < inputByteStart || (offset = (lip - lpos)) <= 0 || offset > M4_MAX_OFFSET)
                            literal = true;
                        else if (offset <= M2_MAX_OFFSET || inputBytes[lpos + 3] == inputBytes[lip + 3]) { }
                        else
                            literal = true;
                    }

                    if (!literal)
                    {
                        if (GetUShortFrom2Bytes(inputBytes, lpos) == GetUShortFrom2Bytes(inputBytes, lip) && inputBytes[lpos + 2] == inputBytes[lip + 2])
                            match = true;
                    }

                    literal = false;
                    if (!match)
                    {
                        dictNew[index] = lip;
                        lip++;
                        if (lip >= lipEnd)
                            break;
                        continue;
                    }
                    match = false;
                    dictNew[index] = lip;
                    if (lip - lii > 0)
                    {
                        uint t = (lip - lii);
                        if (t <= 3)
                        {

                            outputBytes[lop - 2] |= (byte)(t);
                        }
                        else if (t <= 18)
                        {
                            outputBytes[lop] = (byte)(t - 3); lop++;
                        }
                        else
                        {
                            uint tt = t - 18;
                            outputBytes[lop] = 0; lop++;
                            while (tt > 255)
                            {
                                tt -= 255;
                                outputBytes[lop] = 0; lop++;
                            }
                            Debug.Assert(tt > 0);
                            outputBytes[lop] = (byte)(tt); lop++;
                        }
                        do
                        {
                            outputBytes[lop] = inputBytes[lii]; lop++; lii++;


                        } while (--t > 0);
                    }
                    Debug.Assert(lii == lip);
                    lip += 3;
                    uint length;
                    if (inputBytes[lpos + 3] != inputBytes[lip++] ||
                        inputBytes[lpos + 4] != inputBytes[lip++] ||
                        inputBytes[lpos + 5] != inputBytes[lip++] ||
                        inputBytes[lpos + 6] != inputBytes[lip++] ||
                        inputBytes[lpos + 7] != inputBytes[lip++] ||
                        inputBytes[lpos + 8] != inputBytes[lip++])
                    {
                        lip--;
                        length = (lip - lii);
                        Debug.Assert(length >= 3);
                        Debug.Assert(length <= M2_MAX_LEN);
                        if (offset <= M2_MAX_OFFSET)
                        {
                            --offset;
                            outputBytes[lop] = (byte)(((length - 1) << 5) | ((offset & 7) << 2));
                            lop++;
                            outputBytes[lop] = (byte)(offset >> 3);
                            lop++;
                        }
                        else if (offset <= M3_MAX_OFFSET)
                        {
                            --offset;
                            outputBytes[lop] = (byte)(M3_MARKER | (length - 2));
                            lop++;
                            outputBytes[lop] = (byte)((offset & 63) << 2);
                            lop++;
                            outputBytes[lop] = (byte)(offset >> 6);
                            lop++;
                        }
                        else
                        {
                            offset -= 0x4000;
                            Debug.Assert(offset > 0);
                            Debug.Assert(offset <= 0x7FFF);
                            outputBytes[lop] = (byte)(M4_MARKER | ((offset & 0x4000) >> 11) | (length - 2));
                            lop++;
                            outputBytes[lop] = (byte)((offset & 63) << 2);
                            lop++;
                            outputBytes[lop] = (byte)(offset >> 6);
                            lop++;
                        }
                    }
                    else
                    {
                        uint lm = lpos + M2_MAX_LEN + 1;
                        while (lip < linEnd && inputBytes[lm] == inputBytes[lip])
                        {
                            lm++;
                            lip++;
                        }
                        length = (lip - lii);
                        Debug.Assert(length > M2_MAX_LEN);
                        if (offset <= M3_MAX_OFFSET)
                        {
                            --offset;
                            if (length <= 33)
                            {
                                outputBytes[lop] = (byte)(M3_MARKER | (length - 2)); lop++;
                            }
                            else
                            {
                                length -= 33;
                                outputBytes[lop] = M3_MARKER | 0; lop++;
                                while (length > 255)
                                {
                                    length -= 255;
                                    outputBytes[lop] = 0; lop++;
                                }
                                Debug.Assert(length > 0);
                                outputBytes[lop] = (byte)(length); lop++;

                            }
                        }
                        else
                        {
                            offset -= 0x4000;
                            Debug.Assert(offset > 0);
                            Debug.Assert(offset <= 0x7FFF);
                            if (length <= M4_MAX_LEN)
                            {
                                outputBytes[lop] = (byte)(M4_MARKER | ((offset & 0x4000) >> 11) | (length - 2)); lop++;
                            }
                            else
                            {
                                length -= M4_MAX_LEN;
                                outputBytes[lop] = (byte)(M4_MARKER | ((offset & 0x4000) >> 11)); lop++;
                                while (length > 255)
                                {
                                    length -= 255;
                                    outputBytes[lop] = 0; lop++;
                                }
                                Debug.Assert(length > 0);
                                outputBytes[lop] = (byte)(length); lop++;
                            }
                        }
                        outputBytes[lop] = (byte)((offset & 63) << 2); lop++;
                        outputBytes[lop] = (byte)(offset >> 6); lop++;
                    }
                    lii = lip;
                    if (lip >= lipEnd)
                        break;
                }
                outputBytesLength = (lop - outputByteStart);
                tmp = (linEnd - lii);
            }

            if (tmp > 0)
            {
                uint ii = inputBytesLength - tmp + inputByteStart;
                if (outputBytesLength == 0 && tmp <= 238)
                {
                    outputBytes[outputBytesLength++] = (byte)(17 + tmp);
                }
                else if (tmp <= 3)
                {
                    outputBytes[outputBytesLength - 2] |= (byte)(tmp);
                }
                else if (tmp <= 18)
                {
                    outputBytes[outputBytesLength++] = (byte)(tmp - 3);
                }
                else
                {
                    uint tt = tmp - 18;
                    outputBytes[outputBytesLength++] = 0;
                    while (tt > 255)
                    {
                        tt -= 255;
                        outputBytes[outputBytesLength++] = 0;
                    }
                    Debug.Assert(tt > 0);
                    outputBytes[outputBytesLength++] = (byte)(tt);
                }
                do
                {
                    outputBytes[outputBytesLength++] = inputBytes[ii++];
                } while (--tmp > 0);
            }
            outputBytes[outputBytesLength++] = M4_MARKER | 1;
            outputBytes[outputBytesLength++] = 0;
            outputBytes[outputBytesLength++] = 0;

            // Append the inputStream count
            outputBytes[outputBytesLength++] = (byte)inputBytesLength;
            outputBytes[outputBytesLength++] = (byte)(inputBytesLength >> 8);
            outputBytes[outputBytesLength++] = (byte)(inputBytesLength >> 16);
            outputBytes[outputBytesLength++] = (byte)(inputBytesLength >> 24);

            return outputBytesLength;
        }

        private uint DIndex1(byte[] src, uint input)
        {
            byte b2 = src[input + 2];
            byte b1 = src[input + 1];
            byte b0 = src[input];

            return D_MASK & ((0x21 *
                              (
                                  ((uint)(((b2 << 6) ^ b1) << 5) ^ b0)
                                  << 5) ^ b0
                             ) >> 5);
        }

        private uint DIndex2(uint idx)
        {
            return (idx & (D_MASK & 0x7FF)) ^ (((D_MASK >> 1) + 1) | 0x1F);
        }


        private ushort GetUShortFrom2Bytes(byte[] workmem, uint index)
        {
            return (ushort)((workmem[index]) + (workmem[index + 1] * 256));
        }
#endregion
    }
}