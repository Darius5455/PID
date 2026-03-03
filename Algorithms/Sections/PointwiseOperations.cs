using Emgu.CV;
using Emgu.CV.Structure;
using Algorithms.Utilities;
using System;


namespace Algorithms.Sections
{
    public class PointwiseOperations
    {
        public static byte[] CreateLinearOperatorLUT(double alpha, double beta)
        {
            byte[] lut = new byte[256];
            for (int r = 0; r < 256; r++)
            {
                lut[r] = Utils.Clamp(alpha * r + beta);
            }
            return lut;
        }

        public static byte[] CreateGammaCorrectionLUT(double gamma)
        {
            byte[] lut = new byte[256];
            for (int r = 0; r < 256; r++)
            {
                double normalized = r / 255.0;
                double corrected = System.Math.Pow(normalized, gamma);
                lut[r] = Utils.Clamp(corrected * 255.0);
            }
            return lut;
        }
        public static byte[] CreateGammaOperatorLUT(double gamma)
        {
            byte[] lut = new byte[256];

            double c = System.Math.Pow(255.0, 1.0 - gamma);

            for (int r = 0; r < 256; r++)
            {
                lut[r] = Utils.Clamp(c * System.Math.Pow(r, gamma));
            }

            return lut;
        }




        public static Image<Gray, byte> ApplyLut(Image<Gray, byte> Img, byte[] lut)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(Img.Size);

            for (int y = 0; y < Img.Height; ++y)
            {
                for (int x = 0; x < Img.Width; ++x)
                {
                    byte r = Img.Data[y, x, 0];
                    byte s = lut[r];
                    result.Data[y, x, 0] = s;
                }
            }

            return result;
        }

        public static Image<Bgr, byte> ApplyLut(Image<Bgr, byte> Img, byte[] lut)
        {

            Image<Bgr, byte> result = new Image<Bgr, byte>(Img.Size);

            for (int y = 0; y < Img.Height; ++y)
            {
                for (int x = 0; x < Img.Width; ++x)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        byte r = Img.Data[y, x, c];
                        byte s = lut[r];
                        result.Data[y, x, c] = s;
                    }
                }
            }

            return result;
        }

        public static Image<Bgr, byte> ContrastStretchingHSV(Image<Bgr, byte> inputImage)
        {
            Image<Hsv, byte> hsvImage = inputImage.Convert<Hsv, byte>();
            
            byte minV = 255;
            byte maxV = 0;
            
            for (int y = 0; y < hsvImage.Height; y++)
            {
                for (int x = 0; x < hsvImage.Width; x++)
                {
                    byte v = hsvImage.Data[y, x, 2];
                    if (v < minV) minV = v;
                    if (v > maxV) maxV = v;
                }
            }
            
            if (maxV == minV)
            {
                return inputImage.Copy();
            }
            
            byte[] lut = new byte[256];
            double v1 = minV;
            double v2 = maxV;
            double y1 = 0;
            double y2 = 255;
            
            for (int v = 0; v < 256; v++)
            {
                double newV = (y2 - y1) / (v2 - v1) * (v - v1) + y1;
                lut[v] = Utils.Clamp(newV);
            }
            
            Image<Hsv, byte> hsvRezult = hsvImage.Copy();
            
            for (int y = 0; y < hsvRezult.Height; y++)
            {
                for (int x = 0; x < hsvRezult.Width; x++)
                {
                    byte oldV = hsvImage.Data[y, x, 2];
                    hsvRezult.Data[y, x, 2] = lut[oldV];
                }
            }
            
            Image<Bgr, byte> rezult = hsvRezult.Convert<Bgr, byte>();
            return rezult;
        }

        public static Image<Bgr, byte> ConvertRGBtoHSV(Image<Bgr, byte> inputImage)
        {
            Image<Hsv, byte> hsvByte = inputImage.Convert<Hsv, byte>();
            
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            
            for (int y = 0; y < hsvByte.Height; y++)
            {
                for (int x = 0; x < hsvByte.Width; x++)
                {
                    result.Data[y, x, 0] = hsvByte.Data[y, x, 0];
                    result.Data[y, x, 1] = hsvByte.Data[y, x, 1];
                    result.Data[y, x, 2] = hsvByte.Data[y, x, 2];
                }
            }
            
            return result;
        }
    }
}