using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Emgu.CV;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace EyeProtect.Core.Helpers
{
    public static class BitmapFunctions
    {
        public static Bitmap ResizeVideoFrameWithPadding(Mat frame, int targetWidth, int targetHeight)
        {
            // Determine the scaling factor
            float scale = Math.Min((float)targetWidth / frame.Width, (float)targetHeight / frame.Height);

            // Calculate new scaled dimensions
            int scaledWidth = (int)(frame.Width * scale);
            int scaledHeight = (int)(frame.Height * scale);

            // Calculate padding offsets (centering the image)
            int offsetX = (targetWidth - scaledWidth) / 2;
            int offsetY = (targetHeight - scaledHeight) / 2;

            // Resize the frame
            Mat resizedFrame = new Mat();
            CvInvoke.Resize(frame, resizedFrame, new Size(scaledWidth, scaledHeight), 0, 0, Emgu.CV.CvEnum.Inter.Cubic);

            // Create padded bitmap with white background
            Bitmap paddedBitmap = new Bitmap(targetWidth, targetHeight);

            using (Graphics graphics = Graphics.FromImage(paddedBitmap))
            {
                graphics.Clear(Color.White); // White padding background
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // Convert resized Mat to Bitmap and draw it centered
                Bitmap resizedBitmap = new Bitmap(scaledWidth, scaledHeight, stride: scaledWidth * 3, PixelFormat.Format24bppRgb, resizedFrame.DataPointer);
                graphics.DrawImage(resizedBitmap, offsetX, offsetY, scaledWidth, scaledHeight);
                resizedBitmap.Dispose();
            }

            resizedFrame.Dispose();
            return paddedBitmap;
        }

        public static Tensor<float> PreprocessBitmapForFaceDetection(Bitmap bitmap, Tensor<float> input)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int stride = bmpData.Stride;
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(stride) * height;
            byte[] rgbValues = new byte[bytes];

            Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * stride) + (x * 3);
                    byte blue = rgbValues[index];
                    byte green = rgbValues[index + 1];
                    byte red = rgbValues[index + 2];

                    // Convert to grayscale and normalize to [0,1]
                    float gray = (0.299f * red + 0.587f * green + 0.114f * blue) / 255f;

                    input[0, 0, y, x] = (gray - .442f) / .28f;
                }
            }

            bitmap.UnlockBits(bmpData);

            return input;
        }
    }
}
