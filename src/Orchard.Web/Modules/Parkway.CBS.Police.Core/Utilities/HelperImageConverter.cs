using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Parkway.CBS.Police.Core.Utilities
{ 
    public class HelperImageConverter
    {
        /// <summary>
        /// Convert to bitmap
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="Bitmap"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static Bitmap ConvertToPicture(byte[] imageBytes, ref Bitmap Bitmap)
        {
            if (imageBytes == null)
                throw new Exception();
            if (imageBytes.Length == 0)
                throw new Exception();
            int num1 = 0;
            FT_IMAGE_SIZE ftImageSize = new FT_IMAGE_SIZE();
            IntPtr num2 = Marshal.AllocHGlobal(imageBytes.Length);
            Marshal.Copy(imageBytes, 0, num2, imageBytes.Length);
            IntPtr num3 = IntPtr.Zero;
            int length;
            try
            {
                FD_IMAGE structure = (FD_IMAGE)Marshal.PtrToStructure(num2, typeof(FD_IMAGE));
                int uOffsetToImage = (int)structure.uOffsetToImage;
                length = (int)structure.uSize - uOffsetToImage;
                ftImageSize.width = structure.Header.ImageFormat.iWidth;
                ftImageSize.height = structure.Header.ImageFormat.iHeight;
                num3 = Marshal.AllocHGlobal(length);
                Marshal.Copy(imageBytes, uOffsetToImage, num3, length);
            }
            catch (Exception ex)
            {
                Marshal.FreeHGlobal(num2);
                if (num3 != IntPtr.Zero)
                    Marshal.FreeHGlobal(num3);
                throw new Exception();

            }
            if (num1 == 0)
            {
                Bitmap original = new Bitmap(ftImageSize.width, ftImageSize.height, PixelFormat.Format32bppArgb);
                byte[] destination = new byte[length];
                Marshal.Copy(num3, destination, 0, length);
                byte[] source = new byte[length * 4];
                for (int index = 0; index < length * 4; index += 4)
                {
                    source[index + 3] = byte.MaxValue;
                    source[index] = source[index + 1] = source[index + 2] = destination[index >> 2];
                }
                Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
                BitmapData bitmapdata = original.LockBits(rect, ImageLockMode.ReadWrite, original.PixelFormat);
                IntPtr scan0 = bitmapdata.Scan0;
                Marshal.Copy(source, 0, scan0, length * 4);
                original.UnlockBits(bitmapdata);
                Size size;
                if (Bitmap != null)
                {
                    size = Bitmap.Size;
                    Bitmap.Dispose();
                }
                else
                    size = original.Size;
                Bitmap = new Bitmap(original, size);
                Bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                original.Dispose();
            }
            Marshal.FreeHGlobal(num2);
            Marshal.FreeHGlobal(num3);
            return Bitmap;
        }

        /// <summary>
        /// Converts to Bitmap
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static Bitmap ToPicture(byte[] bytes) 
        {
            Bitmap bitmap = null;
            return ConvertToPicture(bytes, ref bitmap);
        }

        /// <summary>
        /// Convert base64 to biometric finger print image
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToBase64BiometricsImage(string value) 
        {
            var bytes = Convert.FromBase64String(value);
            var bitmap = ToPicture(bytes);
            var bitmapArray = BitmapToBytes(bitmap);

            return Convert.ToBase64String(bitmapArray);
        }

        /// <summary>
        /// Convert bitmap image to byte []
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private static byte[] BitmapToBytes(Bitmap img) 
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        internal struct FT_IMAGE_SIZE
        {
            public int width;
            public int height;
        }

        internal struct FD_IMAGE_FORMAT_VERSION
        {
            public uint uMajor;
            public uint uMinor;
            public uint uBuild;
        }

        internal struct FD_DATA_HEADER
        {
            public uint uDataType;
            public ulong DeviceId;
            public ulong DeviceType;
            public int iDataAcqProgress;
        }

        internal struct FD_IMAGE_FORMAT
        {
            public uint uDataType;
            public uint uImageType;
            public int iWidth;
            public int iHeight;
            public int iXdpi;
            public int iYdpi;
            public uint uBPP;
            public uint uPadding;
            public uint uSignificantBpp;
            public uint uPolarity;
            public uint uRGBcolorRepresentation;
            public uint uPlanes;
        }

        internal struct FD_IMAGE_HEADER
        {
            public HelperImageConverter.FD_IMAGE_FORMAT_VERSION version;
            public FD_IMAGE_FORMAT ImageFormat;
        }

        internal struct FD_IMAGE
        {
            public uint uSize;
            public HelperImageConverter.FD_DATA_HEADER DataHeader;
            public uint uOffsetToImage;
            public HelperImageConverter.FD_IMAGE_HEADER Header;
            public uint uExtensionSize;
        }
    }
}
