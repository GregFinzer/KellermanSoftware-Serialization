#if !NETSTANDARD

using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KellermanSoftware.Serialization
{
    internal static class WriteableBitmapLogic
    {


        public static void SerializeWriteableBitmap(BinaryWriter writer,
                                                    WriteableBitmap bitmap)
        {
            writer.Write(bitmap.PixelWidth);
            writer.Write(bitmap.PixelHeight);
            writer.Write(bitmap.DpiX);
            writer.Write(bitmap.DpiY);
            WritePixelFormat(writer, bitmap.Format);
            WriteBitmapPalette(writer, bitmap.Palette);

            int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[bitmap.PixelHeight * stride];
            bitmap.CopyPixels(pixels, stride, 0);
            writer.Write(pixels, 0, pixels.Length);
        }

        public static WriteableBitmap ReadWriteableBitmap(BinaryReader reader)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            double dpiX = reader.ReadDouble();
            double dpiY = reader.ReadDouble();
            PixelFormat pixelFormat = ReadPixelFormat(reader);
            BitmapPalette palette = ReadBitmapPalette(reader);

            int stride = (width * pixelFormat.BitsPerPixel + 7) / 8;
            byte[] pixels = reader.ReadBytes(height * stride);

            WriteableBitmap bitmap = new WriteableBitmap(width,
                                                         height,
                                                         dpiX,
                                                         dpiY,
                                                         pixelFormat,
                                                         palette);
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return bitmap;
        }

        private static void WritePixelFormat(BinaryWriter writer,
                                             PixelFormat pixelFormat)
        {
            if (pixelFormat == PixelFormats.Bgr101010)
            {
                writer.Write((byte)PixelFormatEnum.Bgr101010);
            }
            else if (pixelFormat == PixelFormats.Bgr24)
            {
                writer.Write((byte)PixelFormatEnum.Bgr24);
            }
            else if (pixelFormat == PixelFormats.Bgr32)
            {
                writer.Write((byte)PixelFormatEnum.Bgr32);
            }
            else if (pixelFormat == PixelFormats.Bgr555)
            {
                writer.Write((byte)PixelFormatEnum.Bgr555);
            }
            else if (pixelFormat == PixelFormats.Bgr565)
            {
                writer.Write((byte)PixelFormatEnum.Bgr565);
            }
            else if (pixelFormat == PixelFormats.Bgra32)
            {
                writer.Write((byte)PixelFormatEnum.Bgra32);
            }
            else if (pixelFormat == PixelFormats.BlackWhite)
            {
                writer.Write((byte)PixelFormatEnum.BlackWhite);
            }
            else if (pixelFormat == PixelFormats.Cmyk32)
            {
                writer.Write((byte)PixelFormatEnum.Cmyk32);
            }
            else if (pixelFormat == PixelFormats.Default)
            {
                writer.Write((byte)PixelFormatEnum.Default);
            }
            else if (pixelFormat == PixelFormats.Gray16)
            {
                writer.Write((byte)PixelFormatEnum.Gray16);
            }
            else if (pixelFormat == PixelFormats.Gray2)
            {
                writer.Write((byte)PixelFormatEnum.Gray2);
            }
            else if (pixelFormat == PixelFormats.Gray32Float)
            {
                writer.Write((byte)PixelFormatEnum.Gray32Float);
            }
            else if (pixelFormat == PixelFormats.Gray4)
            {
                writer.Write((byte)PixelFormatEnum.Gray4);
            }
            else if (pixelFormat == PixelFormats.Gray8)
            {
                writer.Write((byte)PixelFormatEnum.Gray8);
            }
            else if (pixelFormat == PixelFormats.Indexed1)
            {
                writer.Write((byte)PixelFormatEnum.Indexed1);
            }
            else if (pixelFormat == PixelFormats.Indexed2)
            {
                writer.Write((byte)PixelFormatEnum.Indexed2);
            }
            else if (pixelFormat == PixelFormats.Indexed4)
            {
                writer.Write((byte)PixelFormatEnum.Indexed4);
            }
            else if (pixelFormat == PixelFormats.Indexed8)
            {
                writer.Write((byte)PixelFormatEnum.Indexed8);
            }
            else if (pixelFormat == PixelFormats.Pbgra32)
            {
                writer.Write((byte)PixelFormatEnum.Pbgra32);
            }
            else if (pixelFormat == PixelFormats.Prgba128Float)
            {
                writer.Write((byte)PixelFormatEnum.Prgba128Float);
            }
            else if (pixelFormat == PixelFormats.Prgba64)
            {
                writer.Write((byte)PixelFormatEnum.Prgba64);
            }
            else if (pixelFormat == PixelFormats.Rgb128Float)
            {
                writer.Write((byte)PixelFormatEnum.Rgb128Float);
            }
            else if (pixelFormat == PixelFormats.Rgb24)
            {
                writer.Write((byte)PixelFormatEnum.Rgb24);
            }
            else if (pixelFormat == PixelFormats.Rgb48)
            {
                writer.Write((byte)PixelFormatEnum.Rgb48);
            }
            else if (pixelFormat == PixelFormats.Rgba128Float)
            {
                writer.Write((byte)PixelFormatEnum.Rgba128Float);
            }
            else if (pixelFormat == PixelFormats.Rgba64)
            {
                writer.Write((byte)PixelFormatEnum.Rgba64);
            }
            else
            {
                throw new SerializerException(string.Format("Unsupported PixelFormat: {0}",
                                                            pixelFormat.ToString()));
            }
        }

        private static PixelFormat ReadPixelFormat(BinaryReader reader)
        {
            PixelFormatEnum format = (PixelFormatEnum)reader.ReadByte();
            switch (format)
            {
                case PixelFormatEnum.Bgr101010:
                    return PixelFormats.Bgr101010;
                case PixelFormatEnum.Bgr24:
                    return PixelFormats.Bgr24;
                case PixelFormatEnum.Bgr32:
                    return PixelFormats.Bgr32;
                case PixelFormatEnum.Bgr555:
                    return PixelFormats.Bgr555;
                case PixelFormatEnum.Bgr565:
                    return PixelFormats.Bgr565;
                case PixelFormatEnum.Bgra32:
                    return PixelFormats.Bgra32;
                case PixelFormatEnum.BlackWhite:
                    return PixelFormats.BlackWhite;
                case PixelFormatEnum.Cmyk32:
                    return PixelFormats.Cmyk32;
                case PixelFormatEnum.Default:
                    return PixelFormats.Default;
                case PixelFormatEnum.Gray16:
                    return PixelFormats.Gray16;
                case PixelFormatEnum.Gray2:
                    return PixelFormats.Gray2;
                case PixelFormatEnum.Gray32Float:
                    return PixelFormats.Gray32Float;
                case PixelFormatEnum.Gray4:
                    return PixelFormats.Gray4;
                case PixelFormatEnum.Gray8:
                    return PixelFormats.Gray8;
                case PixelFormatEnum.Indexed1:
                    return PixelFormats.Indexed1;
                case PixelFormatEnum.Indexed2:
                    return PixelFormats.Indexed2;
                case PixelFormatEnum.Indexed4:
                    return PixelFormats.Indexed4;
                case PixelFormatEnum.Indexed8:
                    return PixelFormats.Indexed8;
                case PixelFormatEnum.Pbgra32:
                    return PixelFormats.Pbgra32;
                case PixelFormatEnum.Prgba128Float:
                    return PixelFormats.Prgba128Float;
                case PixelFormatEnum.Prgba64:
                    return PixelFormats.Prgba64;
                case PixelFormatEnum.Rgb128Float:
                    return PixelFormats.Rgb128Float;
                case PixelFormatEnum.Rgb24:
                    return PixelFormats.Rgb24;
                case PixelFormatEnum.Rgb48:
                    return PixelFormats.Rgb48;
                case PixelFormatEnum.Rgba128Float:
                    return PixelFormats.Rgba128Float;
                case PixelFormatEnum.Rgba64:
                    return PixelFormats.Rgba64;
                default:
                    throw new SerializerException(string.Format("Unsupported PixelFormat: {0}",
                                                                format.ToString()));
            }
        }

        private static void WriteBitmapPalette(BinaryWriter writer,
                                               BitmapPalette palette)
        {
            if (palette == null)
            {
                writer.Write((byte)BitmapPaletteEnum.None);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.BlackAndWhite))
            {
                writer.Write((byte)BitmapPaletteEnum.BlackAndWhite);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.BlackAndWhiteTransparent))
            {
                writer.Write((byte)BitmapPaletteEnum.BlackAndWhiteTransparent);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Gray16))
            {
                writer.Write((byte)BitmapPaletteEnum.Gray16);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Gray16Transparent))
            {
                writer.Write((byte)BitmapPaletteEnum.Gray16Transparent);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Gray256))
            {
                writer.Write((byte)BitmapPaletteEnum.Gray256);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Gray256Transparent))
            {
                writer.Write((byte)BitmapPaletteEnum.Gray256Transparent);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Gray4))
            {
                writer.Write((byte)BitmapPaletteEnum.Gray4);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Gray4Transparent))
            {
                writer.Write((byte)BitmapPaletteEnum.Gray4Transparent);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone125))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone125);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone125Transparent))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone125Transparent);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone216))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone216);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone216Transparent))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone216Transparent);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone252))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone252);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone252Transparent))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone252Transparent);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone256))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone256);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone256Transparent))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone256Transparent);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone27))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone27);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone27Transparent))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone27Transparent);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone64))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone64);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone64Transparent))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone64Transparent);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone8))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone8);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.Halftone8Transparent))
            {
                writer.Write((byte)BitmapPaletteEnum.Halftone8Transparent);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.WebPalette))
            {
                writer.Write((byte)BitmapPaletteEnum.WebPalette);
            }
            else if (CheckAreEqual(palette, BitmapPalettes.WebPaletteTransparent))
            {
                writer.Write((byte)BitmapPaletteEnum.WebPaletteTransparent);
            }
            else
            {
                throw new SerializerException(string.Format("Unsupported BitmapPalette: {0}",
                                                            palette.ToString()));
            }
        }

        private static BitmapPalette ReadBitmapPalette(BinaryReader reader)
        {
            BitmapPaletteEnum palette = (BitmapPaletteEnum)reader.ReadByte();
            switch (palette)
            {
                case BitmapPaletteEnum.None:
                    return null;
                case BitmapPaletteEnum.BlackAndWhite:
                    return BitmapPalettes.BlackAndWhite;
                case BitmapPaletteEnum.BlackAndWhiteTransparent:
                    return BitmapPalettes.BlackAndWhiteTransparent;
                case BitmapPaletteEnum.Gray16:
                    return BitmapPalettes.Gray16;
                case BitmapPaletteEnum.Gray16Transparent:
                    return BitmapPalettes.Gray16Transparent;
                case BitmapPaletteEnum.Gray256:
                    return BitmapPalettes.Gray256;
                case BitmapPaletteEnum.Gray256Transparent:
                    return BitmapPalettes.Gray256Transparent;
                case BitmapPaletteEnum.Gray4:
                    return BitmapPalettes.Gray4;
                case BitmapPaletteEnum.Gray4Transparent:
                    return BitmapPalettes.Gray4Transparent;
                case BitmapPaletteEnum.Halftone125:
                    return BitmapPalettes.Halftone125;
                case BitmapPaletteEnum.Halftone125Transparent:
                    return BitmapPalettes.Halftone125Transparent;
                case BitmapPaletteEnum.Halftone216:
                    return BitmapPalettes.Halftone216;
                case BitmapPaletteEnum.Halftone216Transparent:
                    return BitmapPalettes.Halftone216Transparent;
                case BitmapPaletteEnum.Halftone252:
                    return BitmapPalettes.Halftone252;
                case BitmapPaletteEnum.Halftone252Transparent:
                    return BitmapPalettes.Halftone252Transparent;
                case BitmapPaletteEnum.Halftone256:
                    return BitmapPalettes.Halftone256;
                case BitmapPaletteEnum.Halftone256Transparent:
                    return BitmapPalettes.Halftone256Transparent;
                case BitmapPaletteEnum.Halftone27:
                    return BitmapPalettes.Halftone27;
                case BitmapPaletteEnum.Halftone27Transparent:
                    return BitmapPalettes.Halftone27Transparent;
                case BitmapPaletteEnum.Halftone64:
                    return BitmapPalettes.Halftone64;
                case BitmapPaletteEnum.Halftone64Transparent:
                    return BitmapPalettes.Halftone64Transparent;
                case BitmapPaletteEnum.Halftone8:
                    return BitmapPalettes.Halftone8;
                case BitmapPaletteEnum.Halftone8Transparent:
                    return BitmapPalettes.Halftone8Transparent;
                case BitmapPaletteEnum.WebPalette:
                    return BitmapPalettes.WebPalette;
                case BitmapPaletteEnum.WebPaletteTransparent:
                    return BitmapPalettes.WebPaletteTransparent;
                default:
                    throw new SerializerException(string.Format("Unsupported BitmapPalette: {0}",
                                                                palette.ToString()));
            }
        }

        private static bool CheckAreEqual(BitmapPalette first, BitmapPalette second)
        {
            if (first == null && second == null)
            {
                return true;
            }

            if ((first == null && second != null) ||
                (first != null && second == null))
            {
                return false;
            }

            if (first.Colors.Count != second.Colors.Count)
            {
                return false;
            }

            for (int i = 0; i < first.Colors.Count; ++i)
            {
                if (first.Colors[i] != second.Colors[i])
                {
                    return false;
                }
            }

            return true;
        }

        private enum PixelFormatEnum : byte
        {
            Bgr101010,
            Bgr24,
            Bgr32,
            Bgr555,
            Bgr565,
            Bgra32,
            BlackWhite,
            Cmyk32,
            Default,
            Gray16,
            Gray2,
            Gray32Float,
            Gray4,
            Gray8,
            Indexed1,
            Indexed2,
            Indexed4,
            Indexed8,
            Pbgra32,
            Prgba128Float,
            Prgba64,
            Rgb128Float,
            Rgb24,
            Rgb48,
            Rgba128Float,
            Rgba64
        }

        private enum BitmapPaletteEnum : byte
        { 
            None,
            BlackAndWhite,
            BlackAndWhiteTransparent,
            Gray16,
            Gray16Transparent,
            Gray256,
            Gray256Transparent,
            Gray4,
            Gray4Transparent,
            Halftone125,
            Halftone125Transparent,
            Halftone216,
            Halftone216Transparent,
            Halftone252,
            Halftone252Transparent,
            Halftone256,
            Halftone256Transparent,
            Halftone27,
            Halftone27Transparent,
            Halftone64,
            Halftone64Transparent,
            Halftone8,
            Halftone8Transparent,
            WebPalette,
            WebPaletteTransparent
        }
    }
}
#endif