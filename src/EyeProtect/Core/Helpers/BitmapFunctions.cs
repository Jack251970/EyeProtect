using System;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace EyeProtect.Core.Helpers
{
    public static class BitmapFunctions
    {
        public static Bitmap ResizeBitmapWithPadding(Bitmap originalBitmap, int targetWidth, int targetHeight)
        {
            // Calculate scaling factor to fit the image in the target size while maintaining aspect ratio
            float scaleX = (float)targetWidth / originalBitmap.Width;
            float scaleY = (float)targetHeight / originalBitmap.Height;
            float scale = Math.Min(scaleX, scaleY);

            int newWidth = (int)(originalBitmap.Width * scale);
            int newHeight = (int)(originalBitmap.Height * scale);

            // Create a new bitmap with the target dimensions
            Bitmap resizedBitmap = new Bitmap(targetWidth, targetHeight);

            using (Graphics graphics = Graphics.FromImage(resizedBitmap))
            {
                // Fill with black background
                graphics.Clear(Color.Black);

                // Calculate position to center the resized image
                int x = (targetWidth - newWidth) / 2;
                int y = (targetHeight - newHeight) / 2;

                // Set high quality rendering
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                // Draw the resized image centered
                graphics.DrawImage(originalBitmap, x, y, newWidth, newHeight);
            }

            return resizedBitmap;
        }

        public static Tensor<float> PreprocessBitmapForFaceDetection(Bitmap bitmap, Tensor<float> input)
        {
            // Lock the bitmap's bits
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)bmpData.Scan0.ToPointer();
                    int stride = bmpData.Stride;

                    // Normalize and convert to tensor format (CHW - Channels, Height, Width)
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        byte* row = scan0 + (y * stride);
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            // BGR format in bitmap
                            byte b = row[x * 3];
                            byte g = row[x * 3 + 1];
                            byte r = row[x * 3 + 2];

                            // Normalize to [0, 1] and convert to RGB format for model
                            input[0, 0, y, x] = r / 255.0f;
                            input[0, 1, y, x] = g / 255.0f;
                            input[0, 2, y, x] = b / 255.0f;
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }

            return input;
        }
    }
}
