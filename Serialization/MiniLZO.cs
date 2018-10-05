// Modified by Owen Emlen to avoid using unsafe
// Probably slower than the original, but this mod lets us use the class with Silverlight/etc
#define USE_SAFE
#define BOUNDARY_CHECKS

using System;
using System.Diagnostics;
using System.IO;

namespace KellermanSoftware.Serialization
{
    internal static class MiniLZO
    {
        private const byte BITS = 14;
        private const uint D_MASK = (1 << BITS) - 1;
        private const uint M2_MAX_LEN = 8;
        private const uint M2_MAX_OFFSET = 0x0800;
        private const byte M3_MARKER = 32;
        private const uint M3_MAX_OFFSET = 0x4000;
        private const byte M4_MARKER = 16;
        private const uint M4_MAX_LEN = 9;
        private const uint M4_MAX_OFFSET = 0xbfff;

        private static readonly uint DICT_SIZE = 65536 + 3;

        static MiniLZO()
        {
            if (IntPtr.Size == 8)
            {
                DICT_SIZE = (65536 + 3) * 2;
            }
            else
            {
                DICT_SIZE = 65536 + 3;
            }
        }

        public static byte[] Compress(byte[] src)
        {
            if (src == null)
                return null;

            return Compress(src, 0, src.Length);
        }

        public static byte[] Compress(byte[] src, int srcCount)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (srcCount > src.Length)
                throw new ArgumentOutOfRangeException("src[] has length " + src.Length + ", but srcCount was " + srcCount);

            return Compress(src, 0, srcCount);
        }

        public static byte[] Compress(byte[] src, int srcStart, int srcLength)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (srcStart < 0)
                throw new ArgumentOutOfRangeException("srcStart was " + srcStart);

            if (srcStart + srcLength > src.Length)
                throw new ArgumentOutOfRangeException("src[] has length " + src.Length + ", but srcStart + srcLength was " + (srcStart + srcLength));

            uint dstlen = (uint)(srcLength + (srcLength / 16) + 64 + 3 + 4);
            byte[] dst = new byte[dstlen];

            uint compressedSize = Compress(src, (uint)srcStart, (uint)srcLength, dst, 0, dstlen, null);

            if (dst.Length != compressedSize)
            {
                byte[] final = new byte[compressedSize];
                Buffer.BlockCopy(dst, 0, final, 0, (int)compressedSize);
                dst = final;
            }

            return dst;
        }

        public static byte[] Compress(MemoryStream source)
        {
            if (source == null)
                return null;

            byte[] destinationBuffer;
            uint sourceOffset;
            byte[] sourceBuffer;

#if NETCOREAPP1_1
            ArraySegment<byte> tempBuf;
            source.TryGetBuffer(out tempBuf);
            sourceBuffer = tempBuf.Array;
#else
            sourceBuffer = source.GetBuffer();
#endif
            uint sourceCapacity = (uint)source.Capacity;
            uint sourceLength = (uint)source.Length;
            uint destinationLength = sourceLength + (sourceLength / 16) + 64 + 3 + 4;

            uint unusedSpace = sourceCapacity - sourceLength;
            uint inplaceOverhead = Math.Min(sourceLength, M4_MAX_OFFSET) + sourceLength / 64 + 16 + 3 + 4;

            if (unusedSpace < inplaceOverhead)
            {
                sourceOffset = 0;
                destinationBuffer = new byte[destinationLength];
            }
            else
            {
                sourceOffset = inplaceOverhead;
                source.SetLength(sourceLength + inplaceOverhead);
                destinationBuffer = sourceBuffer;
                Buffer.BlockCopy(destinationBuffer, 0, destinationBuffer, (int)inplaceOverhead, (int)sourceLength);
            }

            uint compressedSize = Compress(sourceBuffer, sourceOffset, sourceLength, destinationBuffer, 0, destinationLength, null);

            if (destinationBuffer == sourceBuffer)
            {
                source.SetLength(compressedSize);
                source.Capacity = (int)compressedSize;
#if NETCOREAPP1_1
                ArraySegment<byte> buf;
                source.TryGetBuffer(out buf);
                return buf.Array;
#else
                return source.GetBuffer();
#endif               
            }

            byte[] final = new byte[compressedSize];
            Buffer.BlockCopy(destinationBuffer, 0, final, 0, (int)compressedSize);
            return final;
        }

#if USE_SAFE
        public static byte[] Decompress(byte[] src)
        {
            if (src == null)
                return null;

            byte[] dst = new byte[(src[src.Length - 4] | (src[src.Length - 3] << 8) | (src[src.Length - 2] << 16 | src[src.Length - 1] << 24))];

            uint t = 0;
            uint lpos;
            uint lip_end = (uint)src.Length - 4;
            uint lop_end = (uint)dst.Length;

            uint lip = 0;
            uint lop = 0;
            bool match = false;
            bool match_next = false;
            bool match_done = false;
            bool copy_match = false;
            bool first_literal_run = false;
            bool eof_found = false;

            if (src[lip] > 17)
            {
                t = (uint)(src[lip] - 17); lip++;
                if (t < 4)
                    match_next = true;
                else
                {
#if BOUNDARY_CHECKS
                    Debug.Assert(t > 0);
                    if ((lop_end - lop) < t)
                        throw new OverflowException("Output Overrun");
                    if ((lip_end - lip) < t + 1)
                        throw new OverflowException("Input Overrun");
#endif
                    do
                    {
                        dst[lop] = src[lip]; lop++; lip++;
                    } while (--t > 0);
                    first_literal_run = true;
                }
            }
            while (!eof_found && lip < lip_end)
            {
                if (!match_next && !first_literal_run)
                {
                    t = src[lip]; lip++;
                    if (t >= 16)
                        match = true;
                    else
                    {
                        if (t == 0)
                        {
#if BOUNDARY_CHECKS
                            if ((lip_end - lip) < 1)
                                throw new OverflowException("Input Overrun");
#endif
                            while (src[lip] == 0)
                            {
                                t += 255;
                                lip++;
#if BOUNDARY_CHECKS
                                if ((lip_end - lip) < 1)
                                    throw new OverflowException("Input Overrun");
#endif
                            }
                            t += (uint)(15 + src[lip]);
                            lip++;
                        }
#if BOUNDARY_CHECKS
                        Debug.Assert(t > 0);
                        if ((lop_end - lop) < t + 3)
                            throw new OverflowException("Output Overrun");
                        if ((lip_end - lip) < t + 4)
                            throw new OverflowException("Input Overrun");
#endif
                        for (int x = 0; x < 4; ++x, ++lop, ++lip)
                            dst[lop] = src[lip];
                        if (--t > 0)
                        {
                            if (t >= 4)
                            {
                                do
                                {
                                    for (int x = 0; x < 4; ++x, ++lop, ++lip)
                                        dst[lop] = src[lip];
                                    t -= 4;
                                } while (t >= 4);
                                if (t > 0)
                                {
                                    do
                                    {
                                        dst[lop] = src[lip]; lop++; lip++;
                                    } while (--t > 0);
                                }
                            }
                            else
                            {
                                do
                                {
                                    dst[lop] = src[lip]; lop++; lip++;
                                } while (--t > 0);
                            }
                        }
                    }
                }
                if (!match && !match_next)
                {
                    first_literal_run = false;

                    t = src[lip]; lip++;
                    if (t >= 16)
                        match = true;
                    else
                    {
                        lpos = lop - (1 + M2_MAX_OFFSET);
                        lpos -= t >> 2;
                        lpos -= ((uint)src[lip]) << 2;
                        lip++;
#if BOUNDARY_CHECKS
                        if (lpos < 0 || lpos >= lop)
                            throw new OverflowException("Lookbehind Overrun");
                        if ((lop_end - lop) < 3)
                            throw new OverflowException("Output Overrun");
#endif

                        dst[lop] = dst[lpos]; lop++; lpos++;
                        dst[lop] = dst[lpos]; lop++; lpos++;
                        dst[lop] = dst[lpos]; lop++; lpos++;
                        match_done = true;
                    }
                }
                match = false;
                do
                {
                    if (t >= 64)
                    {
                        lpos = lop - 1;
                        lpos -= (t >> 2) & 7;
                        lpos -= ((uint)src[lip]) << 3;
                        lip++;
                        t = (t >> 5) - 1;
#if BOUNDARY_CHECKS
                        if (lpos < 0 || lpos >= lop)
                            throw new OverflowException("Lookbehind Overrun");
                        if ((lop_end - lop) < t + 2)
                            throw new OverflowException("Output Overrun");
#endif
                        copy_match = true;
                    }
                    else if (t >= 32)
                    {
                        t &= 31;
                        if (t == 0)
                        {
#if BOUNDARY_CHECKS
                            if ((lip_end - lip) < 1)
                                throw new OverflowException("Input Overrun");
#endif
                            while (src[lip] == 0)
                            {
                                t += 255;
                                lip++;
#if BOUNDARY_CHECKS
                                if ((lip_end - lip) < 1)
                                    throw new OverflowException("Input Overrun");
#endif
                            }
                            t += (uint)(31 + src[lip]);
                            lip++;
                        }
                        lpos = lop - 1;
                        lpos -= (uint)GetUShortFrom2Bytes(src, lip) >> 2;
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
                            if ((lip_end - lip) < 1)
                                throw new OverflowException("Input Overrun");
#endif
                            while (src[lip] == 0)
                            {
                                t += 255;
                                lip++;
#if BOUNDARY_CHECKS
                                if ((lip_end - lip) < 1)
                                    throw new OverflowException("Input Overrun");
#endif
                            }
                            t += (uint)(7 + src[lip]);
                            lip++;
                        }
                        lpos -= (uint)GetUShortFrom2Bytes(src, lip) >> 2;
                        lip += 2;
                        if (lpos == lop)
                            eof_found = true;
                        else
                            lpos -= 0x4000;
                    }
                    else
                    {
                        lpos = lop - 1;
                        lpos -= t >> 2;
                        lpos -= ((uint)src[lip]) << 2;
                        lip++;
#if BOUNDARY_CHECKS
                        if (lpos < 0 || lpos >= lop)
                            throw new OverflowException("Lookbehind Overrun");
                        if ((lop_end - lop) < 2)
                            throw new OverflowException("Output Overrun");
#endif
                        dst[lop] = dst[lpos]; lop++; lpos++;
                        dst[lop] = dst[lpos]; lop++; lpos++;
                        match_done = true;
                    }
                    if (!eof_found && !match_done && !copy_match)
                    {
#if BOUNDARY_CHECKS
                        if (lpos < 0 || lpos >= lop)
                            throw new OverflowException("Lookbehind Overrun");
                        Debug.Assert(t > 0);
                        if ((lop_end - lop) < t + 2)
                            throw new OverflowException("Output Overrun");
#endif
                    }
                    if (!eof_found && t >= 2 * 4 - 2 && (lop - lpos) >= 4 && !match_done && !copy_match)
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
                    else if (!eof_found && !match_done)
                    {
                        copy_match = false;

                        dst[lop] = dst[lpos]; lop++; lpos++;
                        dst[lop] = dst[lpos]; lop++; lpos++;
                        do
                        {
                            dst[lop] = dst[lpos]; lop++; lpos++;
                        } while (--t > 0);
                    }

                    if (!eof_found && !match_next)
                    {
                        match_done = false;

                        t = (uint)(src[lip - 2] & 3);
                        if (t == 0)
                            break;
                    }
                    if (!eof_found)
                    {
                        match_next = false;
#if BOUNDARY_CHECKS
                        Debug.Assert(t > 0);
                        Debug.Assert(t < 4);
                        if ((lop_end - lop) < t)
                            throw new OverflowException("Output Overrun");
                        if ((lip_end - lip) < t + 1)
                            throw new OverflowException("Input Overrun");
#endif
                        dst[lop] = src[lip]; lop++; lip++;
                        if (t > 1)
                        {
                            dst[lop] = src[lip]; lop++; lip++;
                            if (t > 2)
                            {
                                dst[lop] = src[lip]; lop++; lip++;
                            }
                        }
                        t = src[lip]; lip++;
                    }
                } while (!eof_found && lip < lip_end);
            }
            if (!eof_found)
                throw new OverflowException("EOF Marker Not Found");

#if BOUNDARY_CHECKS
            Debug.Assert(t == 1);
            if (lip > lip_end)
                throw new OverflowException("Input Overrun");

            if (lip < lip_end)
                throw new OverflowException("Input Not Consumed");
#endif

            return dst;
        }

        private static uint Compress(byte[] src, uint srcstart, uint srcLength, byte[] dst, uint dststart, uint dstlen, uint[] dictNew)
        {
            // using all of dict_size?
            if (dictNew == null)
                dictNew = new uint[DICT_SIZE];

            uint tmp;
            if (srcLength <= M2_MAX_LEN + 5)
            {
                tmp = srcLength;
                dstlen = 0;
            }
            else
            {


                uint lin_end = srcstart + srcLength;
                uint lip_end = srcstart + srcLength - M2_MAX_LEN - 5;




                uint lii = srcstart;
                uint lip = srcstart + 4;
                uint lop = dststart;

                bool literal = false;
                bool match = false;

                uint length;

                for (; ; )
                {
                    uint offset = 0;
                    uint index = D_INDEX1(src, lip);
                    uint lpos = lip - (lip - dictNew[index]);

                    if (lpos < srcstart || (offset = (lip - lpos)) <= 0 || offset > M4_MAX_OFFSET)
                        literal = true;
                    else if (offset <= M2_MAX_OFFSET || src[lpos + 3] == src[lip + 3]) { }
                    else
                    {
                        index = D_INDEX2(index);
                        lpos = lip - (lip - dictNew[index]);
                        if (lpos < srcstart || (offset = (lip - lpos)) <= 0 || offset > M4_MAX_OFFSET)
                            literal = true;
                        else if (offset <= M2_MAX_OFFSET || src[lpos + 3] == src[lip + 3]) { }
                        else
                            literal = true;
                    }

                    if (!literal)
                    {
                        if (GetUShortFrom2Bytes(src, lpos) == GetUShortFrom2Bytes(src, lip) && src[lpos + 2] == src[lip + 2])
                            match = true;
                    }

                    literal = false;
                    if (!match)
                    {
                        dictNew[index] = lip;
                        lip++;
                        if (lip >= lip_end)
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

                            dst[lop - 2] |= (byte)(t);
                        }
                        else if (t <= 18)
                        {
                            dst[lop] = (byte)(t - 3); lop++;
                        }
                        else
                        {
                            uint tt = t - 18;
                            dst[lop] = 0; lop++;
                            while (tt > 255)
                            {
                                tt -= 255;
                                dst[lop] = 0; lop++;
                            }
                            Debug.Assert(tt > 0);
                            dst[lop] = (byte)(tt); lop++;
                        }
                        do
                        {
                            dst[lop] = src[lii]; lop++; lii++;


                        } while (--t > 0);
                    }
                    Debug.Assert(lii == lip);
                    lip += 3;
                    if (src[lpos + 3] != src[lip++] ||
                        src[lpos + 4] != src[lip++] ||
                        src[lpos + 5] != src[lip++] ||
                        src[lpos + 6] != src[lip++] ||
                        src[lpos + 7] != src[lip++] ||
                        src[lpos + 8] != src[lip++])
                    {
                        lip--;
                        length = (lip - lii);
                        Debug.Assert(length >= 3);
                        Debug.Assert(length <= M2_MAX_LEN);
                        if (offset <= M2_MAX_OFFSET)
                        {
                            --offset;
                            dst[lop] = (byte)(((length - 1) << 5) | ((offset & 7) << 2));
                            lop++;
                            dst[lop] = (byte)(offset >> 3);
                            lop++;
                        }
                        else if (offset <= M3_MAX_OFFSET)
                        {
                            --offset;
                            dst[lop] = (byte)(M3_MARKER | (length - 2));
                            lop++;
                            dst[lop] = (byte)((offset & 63) << 2);
                            lop++;
                            dst[lop] = (byte)(offset >> 6);
                            lop++;
                        }
                        else
                        {
                            offset -= 0x4000;
                            Debug.Assert(offset > 0);
                            Debug.Assert(offset <= 0x7FFF);
                            dst[lop] = (byte)(M4_MARKER | ((offset & 0x4000) >> 11) | (length - 2));
                            lop++;
                            dst[lop] = (byte)((offset & 63) << 2);
                            lop++;
                            dst[lop] = (byte)(offset >> 6);
                            lop++;
                        }
                    }
                    else
                    {
                        uint lm = lpos + M2_MAX_LEN + 1;
                        while (lip < lin_end && src[lm] == src[lip])
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
                                dst[lop] = (byte)(M3_MARKER | (length - 2)); lop++;
                            }
                            else
                            {
                                length -= 33;
                                dst[lop] = M3_MARKER | 0; lop++;
                                while (length > 255)
                                {
                                    length -= 255;
                                    dst[lop] = 0; lop++;
                                }
                                Debug.Assert(length > 0);
                                dst[lop] = (byte)(length); lop++;

                            }
                        }
                        else
                        {
                            offset -= 0x4000;
                            Debug.Assert(offset > 0);
                            Debug.Assert(offset <= 0x7FFF);
                            if (length <= M4_MAX_LEN)
                            {
                                dst[lop] = (byte)(M4_MARKER | ((offset & 0x4000) >> 11) | (length - 2)); lop++;
                            }
                            else
                            {
                                length -= M4_MAX_LEN;
                                dst[lop] = (byte)(M4_MARKER | ((offset & 0x4000) >> 11)); lop++;
                                while (length > 255)
                                {
                                    length -= 255;
                                    dst[lop] = 0; lop++;
                                }
                                Debug.Assert(length > 0);
                                dst[lop] = (byte)(length); lop++;
                            }
                        }
                        dst[lop] = (byte)((offset & 63) << 2); lop++;
                        dst[lop] = (byte)(offset >> 6); lop++;
                    }
                    lii = lip;
                    if (lip >= lip_end)
                        break;
                }
                dstlen = (lop - dststart);
                tmp = (lin_end - lii);
            }

            if (tmp > 0)
            {
                uint ii = srcLength - tmp + srcstart;
                if (dstlen == 0 && tmp <= 238)
                {
                    dst[dstlen++] = (byte)(17 + tmp);
                }
                else if (tmp <= 3)
                {
                    dst[dstlen - 2] |= (byte)(tmp);
                }
                else if (tmp <= 18)
                {
                    dst[dstlen++] = (byte)(tmp - 3);
                }
                else
                {
                    uint tt = tmp - 18;
                    dst[dstlen++] = 0;
                    while (tt > 255)
                    {
                        tt -= 255;
                        dst[dstlen++] = 0;
                    }
                    Debug.Assert(tt > 0);
                    dst[dstlen++] = (byte)(tt);
                }
                do
                {
                    dst[dstlen++] = src[ii++];
                } while (--tmp > 0);
            }
            dst[dstlen++] = M4_MARKER | 1;
            dst[dstlen++] = 0;
            dst[dstlen++] = 0;

            // Append the source count
            dst[dstlen++] = (byte)srcLength;
            dst[dstlen++] = (byte)(srcLength >> 8);
            dst[dstlen++] = (byte)(srcLength >> 16);
            dst[dstlen++] = (byte)(srcLength >> 24);

            return dstlen;
        }


#else
        private static unsafe uint Compress(byte[] src, uint srcstart, uint srcLength, byte[] dst, uint dststart, uint dstlen, byte[] workmem, uint workmemstart)
        {
            uint tmp;
            if (srcLength <= M2_MAX_LEN + 5)
            {
                tmp = (uint)srcLength;
                dstlen = 0;
            }
            else
            {
                fixed (byte* work = &workmem[workmemstart], input = &src[srcstart], output = &dst[dststart])
                {
                    byte** dict = (byte**)work;
                    byte* in_end = input + srcLength;
                    byte* ip_end = input + srcLength - M2_MAX_LEN - 5;
                    byte* ii = input;
                    byte* ip = input + 4;
                    byte* op = output;
                    bool literal = false;
                    bool match = false;
                    uint offset;
                    uint length;
                    uint index;
                    byte* pos;

                    for (; ; )
                    {
                        offset = 0;
                        index = D_INDEX1(ip);
                        pos = ip - (ip - dict[index]);
                        if (pos < input || (offset = (uint)(ip - pos)) <= 0 || offset > M4_MAX_OFFSET)
                            literal = true;
                        else if (offset <= M2_MAX_OFFSET || pos[3] == ip[3]) { }
                        else
                        {
                            index = D_INDEX2(index);
                            pos = ip - (ip - dict[index]);
                            if (pos < input || (offset = (uint)(ip - pos)) <= 0 || offset > M4_MAX_OFFSET)
                                literal = true;
                            else if (offset <= M2_MAX_OFFSET || pos[3] == ip[3]) { }
                            else
                                literal = true;
                        }

                        if (!literal)
                        {
                            if (*((ushort*)pos) == *((ushort*)ip) && pos[2] == ip[2])
                                match = true;
                        }

                        literal = false;
                        if (!match)
                        {
                            dict[index] = ip;
                            ++ip;
                            if (ip >= ip_end)
                                break;
                            continue;
                        }
                        match = false;
                        dict[index] = ip;
                        if (ip - ii > 0)
                        {
                            uint t = (uint)(ip - ii);
                            if (t <= 3)
                            {
                                Debug.Assert(op - 2 > output);
                                op[-2] |= (byte)(t);
                            }
                            else if (t <= 18)
                                *op++ = (byte)(t - 3);
                            else
                            {
                                uint tt = t - 18;
                                *op++ = 0;
                                while (tt > 255)
                                {
                                    tt -= 255;
                                    *op++ = 0;
                                }
                                Debug.Assert(tt > 0);
                                *op++ = (byte)(tt);
                            }
                            do
                            {
                                *op++ = *ii++;
                            } while (--t > 0);
                        }
                        Debug.Assert(ii == ip);
                        ip += 3;
                        if (pos[3] != *ip++ || pos[4] != *ip++ || pos[5] != *ip++
                           || pos[6] != *ip++ || pos[7] != *ip++ || pos[8] != *ip++)
                        {
                            --ip;
                            length = (uint)(ip - ii);
                            Debug.Assert(length >= 3);
                            Debug.Assert(length <= M2_MAX_LEN);
                            if (offset <= M2_MAX_OFFSET)
                            {
                                --offset;
                                *op++ = (byte)(((length - 1) << 5) | ((offset & 7) << 2));
                                *op++ = (byte)(offset >> 3);
                            }
                            else if (offset <= M3_MAX_OFFSET)
                            {
                                --offset;
                                *op++ = (byte)(M3_MARKER | (length - 2));
                                *op++ = (byte)((offset & 63) << 2);
                                *op++ = (byte)(offset >> 6);
                            }
                            else
                            {
                                offset -= 0x4000;
                                Debug.Assert(offset > 0);
                                Debug.Assert(offset <= 0x7FFF);
                                *op++ = (byte)(M4_MARKER | ((offset & 0x4000) >> 11) | (length - 2));
                                *op++ = (byte)((offset & 63) << 2);
                                *op++ = (byte)(offset >> 6);
                            }
                        }
                        else
                        {
                            byte* m = pos + M2_MAX_LEN + 1;
                            while (ip < in_end && *m == *ip)
                            {
                                ++m;
                                ++ip;
                            }
                            length = (uint)(ip - ii);
                            Debug.Assert(length > M2_MAX_LEN);
                            if (offset <= M3_MAX_OFFSET)
                            {
                                --offset;
                                if (length <= 33)
                                    *op++ = (byte)(M3_MARKER | (length - 2));
                                else
                                {
                                    length -= 33;
                                    *op++ = M3_MARKER | 0;
                                    while (length > 255)
                                    {
                                        length -= 255;
                                        *op++ = 0;
                                    }
                                    Debug.Assert(length > 0);
                                    *op++ = (byte)(length);
                                }
                            }
                            else
                            {
                                offset -= 0x4000;
                                Debug.Assert(offset > 0);
                                Debug.Assert(offset <= 0x7FFF);
                                if (length <= M4_MAX_LEN)
                                    *op++ = (byte)(M4_MARKER | ((offset & 0x4000) >> 11) | (length - 2));
                                else
                                {
                                    length -= M4_MAX_LEN;
                                    *op++ = (byte)(M4_MARKER | ((offset & 0x4000) >> 11));
                                    while (length > 255)
                                    {
                                        length -= 255;
                                        *op++ = 0;
                                    }
                                    Debug.Assert(length > 0);
                                    *op++ = (byte)(length);
                                }
                            }
                            *op++ = (byte)((offset & 63) << 2);
                            *op++ = (byte)(offset >> 6);
                        }
                        ii = ip;
                        if (ip >= ip_end)
                            break;
                    }
                    dstlen = (uint)(op - output);
                    tmp = (uint)(in_end - ii);
                }
            }
            if (tmp > 0)
            {
                uint ii = (uint)srcLength - tmp + srcstart;
                if (dstlen == 0 && tmp <= 238)
                {
                    dst[dstlen++] = (byte)(17 + tmp);
                }
                else if (tmp <= 3)
                {
                    dst[dstlen - 2] |= (byte)(tmp);
                }
                else if (tmp <= 18)
                {
                    dst[dstlen++] = (byte)(tmp - 3);
                }
                else
                {
                    uint tt = tmp - 18;
                    dst[dstlen++] = 0;
                    while (tt > 255)
                    {
                        tt -= 255;
                        dst[dstlen++] = 0;
                    }
                    Debug.Assert(tt > 0);
                    dst[dstlen++] = (byte)(tt);
                }
                do
                {
                    dst[dstlen++] = src[ii++];
                } while (--tmp > 0);
            }
            dst[dstlen++] = M4_MARKER | 1;
            dst[dstlen++] = 0;
            dst[dstlen++] = 0;

            // Append the source count
            dst[dstlen++] = (byte)srcLength;
            dst[dstlen++] = (byte)(srcLength >> 8);
            dst[dstlen++] = (byte)(srcLength >> 16);
            dst[dstlen++] = (byte)(srcLength >> 24);

            return dstlen;
        }

        public static unsafe byte[] Decompress(this byte[] src)
        {
            byte[] dst = new byte[(src[src.Length - 4] | (src[src.Length - 3] << 8) | (src[src.Length - 2] << 16 | src[src.Length - 1] << 24))];

            uint t = 0;
            fixed (byte* input = src, output = dst)
            {
                byte* pos = null;
                byte* ip_end = input + src.Length - 4;
                byte* op_end = output + dst.Length;
                byte* ip = input;
                byte* op = output;
                bool match = false;
                bool match_next = false;
                bool match_done = false;
                bool copy_match = false;
                bool first_literal_run = false;
                bool eof_found = false;

                if (*ip > 17)
                {
                    t = (uint)(*ip++ - 17);
                    if (t < 4)
                        match_next = true;
                    else
                    {
                        Debug.Assert(t > 0);
                        if ((op_end - op) < t)
                            throw new OverflowException("Output Overrun");
                        if ((ip_end - ip) < t + 1)
                            throw new OverflowException("Input Overrun");
                        do
                        {
                            *op++ = *ip++;
                        } while (--t > 0);
                        first_literal_run = true;
                    }
                }
                while (!eof_found && ip < ip_end)
                {
                    if (!match_next && !first_literal_run)
                    {
                        t = *ip++;
                        if (t >= 16)
                            match = true;
                        else
                        {
                            if (t == 0)
                            {
                                if ((ip_end - ip) < 1)
                                    throw new OverflowException("Input Overrun");
                                while (*ip == 0)
                                {
                                    t += 255;
                                    ++ip;
                                    if ((ip_end - ip) < 1)
                                        throw new OverflowException("Input Overrun");
                                }
                                t += (uint)(15 + *ip++);
                            }
                            Debug.Assert(t > 0);
                            if ((op_end - op) < t + 3)
                                throw new OverflowException("Output Overrun");
                            if ((ip_end - ip) < t + 4)
                                throw new OverflowException("Input Overrun");
                            for (int x = 0; x < 4; ++x, ++op, ++ip)
                                *op = *ip;
                            if (--t > 0)
                            {
                                if (t >= 4)
                                {
                                    do
                                    {
                                        for (int x = 0; x < 4; ++x, ++op, ++ip)
                                            *op = *ip;
                                        t -= 4;
                                    } while (t >= 4);
                                    if (t > 0)
                                    {
                                        do
                                        {
                                            *op++ = *ip++;
                                        } while (--t > 0);
                                    }
                                }
                                else
                                {
                                    do
                                    {
                                        *op++ = *ip++;
                                    } while (--t > 0);
                                }
                            }
                        }
                    }
                    if (!match && !match_next)
                    {
                        first_literal_run = false;

                        t = *ip++;
                        if (t >= 16)
                            match = true;
                        else
                        {
                            pos = op - (1 + M2_MAX_OFFSET);
                            pos -= t >> 2;
                            pos -= *ip++ << 2;
                            if (pos < output || pos >= op)
                                throw new OverflowException("Lookbehind Overrun");
                            if ((op_end - op) < 3)
                                throw new OverflowException("Output Overrun");
                            *op++ = *pos++;
                            *op++ = *pos++;
                            *op++ = *pos++;
                            match_done = true;
                        }
                    }
                    match = false;
                    do
                    {
                        if (t >= 64)
                        {
                            pos = op - 1;
                            pos -= (t >> 2) & 7;
                            pos -= *ip++ << 3;
                            t = (t >> 5) - 1;
                            if (pos < output || pos >= op)
                                throw new OverflowException("Lookbehind Overrun");
                            if ((op_end - op) < t + 2)
                                throw new OverflowException("Output Overrun");
                            copy_match = true;
                        }
                        else if (t >= 32)
                        {
                            t &= 31;
                            if (t == 0)
                            {
                                if ((ip_end - ip) < 1)
                                    throw new OverflowException("Input Overrun");
                                while (*ip == 0)
                                {
                                    t += 255;
                                    ++ip;
                                    if ((ip_end - ip) < 1)
                                        throw new OverflowException("Input Overrun");
                                }
                                t += (uint)(31 + *ip++);
                            }
                            pos = op - 1;
                            pos -= (*(ushort*)ip) >> 2;
                            ip += 2;
                        }
                        else if (t >= 16)
                        {
                            pos = op;
                            pos -= (t & 8) << 11;

                            t &= 7;
                            if (t == 0)
                            {
                                if ((ip_end - ip) < 1)
                                    throw new OverflowException("Input Overrun");
                                while (*ip == 0)
                                {
                                    t += 255;
                                    ++ip;
                                    if ((ip_end - ip) < 1)
                                        throw new OverflowException("Input Overrun");
                                }
                                t += (uint)(7 + *ip++);
                            }
                            pos -= (*(ushort*)ip) >> 2;
                            ip += 2;
                            if (pos == op)
                                eof_found = true;
                            else
                                pos -= 0x4000;
                        }
                        else
                        {
                            pos = op - 1;
                            pos -= t >> 2;
                            pos -= *ip++ << 2;
                            if (pos < output || pos >= op)
                                throw new OverflowException("Lookbehind Overrun");
                            if ((op_end - op) < 2)
                                throw new OverflowException("Output Overrun");
                            *op++ = *pos++;
                            *op++ = *pos++;
                            match_done = true;
                        }
                        if (!eof_found && !match_done && !copy_match)
                        {
                            if (pos < output || pos >= op)
                                throw new OverflowException("Lookbehind Overrun");
                            Debug.Assert(t > 0);
                            if ((op_end - op) < t + 2)
                                throw new OverflowException("Output Overrun");
                        }
                        if (!eof_found && t >= 2 * 4 - 2 && (op - pos) >= 4 && !match_done && !copy_match)
                        {
                            for (int x = 0; x < 4; ++x, ++op, ++pos)
                                *op = *pos;
                            t -= 2;
                            do
                            {
                                for (int x = 0; x < 4; ++x, ++op, ++pos)
                                    *op = *pos;
                                t -= 4;
                            } while (t >= 4);
                            if (t > 0)
                            {
                                do
                                {
                                    *op++ = *pos++;
                                } while (--t > 0);
                            }
                        }
                        else if (!eof_found && !match_done)
                        {
                            copy_match = false;

                            *op++ = *pos++;
                            *op++ = *pos++;
                            do
                            {
                                *op++ = *pos++;
                            } while (--t > 0);
                        }

                        if (!eof_found && !match_next)
                        {
                            match_done = false;

                            t = (uint)(ip[-2] & 3);
                            if (t == 0)
                                break;
                        }
                        if (!eof_found)
                        {
                            match_next = false;
                            Debug.Assert(t > 0);
                            Debug.Assert(t < 4);
                            if ((op_end - op) < t)
                                throw new OverflowException("Output Overrun");
                            if ((ip_end - ip) < t + 1)
                                throw new OverflowException("Input Overrun");
                            *op++ = *ip++;
                            if (t > 1)
                            {
                                *op++ = *ip++;
                                if (t > 2)
                                    *op++ = *ip++;
                            }
                            t = *ip++;
                        }
                    } while (!eof_found && ip < ip_end);
                }
                if (!eof_found)
                    throw new OverflowException("EOF Marker Not Found");
                else
                {
                    Debug.Assert(t == 1);
                    if (ip > ip_end)
                        throw new OverflowException("Input Overrun");
                    else if (ip < ip_end)
                        throw new OverflowException("Input Not Consumed");
                }
            }

            return dst;
        }
#endif

        private static uint D_INDEX1(byte[] src, int input)
        {
            return D_MS(D_MUL(0x21, D_X3(src, input, 5, 5, 6)) >> 5, 0);
        }

        private static uint D_INDEX1(byte[] src, uint input)
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

        private static uint D_INDEX2(uint idx)
        {
            return (idx & (D_MASK & 0x7FF)) ^ (((D_MASK >> 1) + 1) | 0x1F);
        }

        private static uint D_MS(uint v, byte s)
        {
            return (v & (D_MASK >> s)) << s;
        }

        private static uint D_MUL(uint a, uint b)
        {
            return a * b;
        }

        private static uint D_X2(byte[] src, int input, byte s1, byte s2)
        {
            return (uint)((((src[input + 2] << s2) ^ src[input + 1]) << s1) ^ src[input]);
        }

        private static uint D_X3(byte[] src, int input, byte s1, byte s2, byte s3)
        {
            return (D_X2(src, input + 1, s2, s3) << s1) ^ src[input];
        }

        private static uint D_X2(byte[] src, uint input, byte s1, byte s2)
        {
            return (uint)((((src[input + 2] << s2) ^ src[input + 1]) << s1) ^ src[input]);
        }

        private static uint D_X3(byte[] src, uint input, byte s1, byte s2, byte s3)
        {
            return (D_X2(src, input + 1, s2, s3) << s1) ^ src[input];
        }

        private static ushort GetUShortFrom2Bytes(byte[] workmem, uint index)
        {
            return (ushort)((workmem[index]) + (workmem[index + 1] * 256));
        }
    }
}
