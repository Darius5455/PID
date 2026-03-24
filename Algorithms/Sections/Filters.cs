using Emgu.CV;
using Emgu.CV.Structure;
using Algorithms.Utilities;
using System;

namespace Algorithms.Sections
{
    public class Filters
    {
        public static double[,] GaussMask(double sigma = 1, int size = 5)
        {
            double[,] mask = new double[size, size];
            int k = size / 2;
            double sum = 0;

            for (int y = -k; y <= k; y++)
            {
                for (int x = -k; x <= k; x++)
                {
                    double value = Math.Exp(-(x * x + y * y) / (2 * sigma * sigma)) / (2 * Math.PI * sigma * sigma);
                    mask[y + k, x + k] = value;
                    sum += value;
                }
            }

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    mask[i, j] /= sum;

            return mask;
        }

        public static Image<Gray, byte> ApplyFilter(Image<Gray, byte> inputImage, double[,] mask)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            int size = mask.GetLength(0);
            int k = size / 2;
            int width = inputImage.Width;
            int height = inputImage.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double sum = 0;
                    for (int i = -k; i <= k; i++)
                    {
                        for (int j = -k; j <= k; j++)
                        {
                            int ny = y + i;
                            int nx = x + j;

                            if (ny < 0) ny = 0;
                            else if (ny >= height) ny = height - 1;

                            if (nx < 0) nx = 0;
                            else if (nx >= width) nx = width - 1;

                            sum += mask[i + k, j + k] * inputImage.Data[ny, nx, 0];
                        }
                    }

                    result.Data[y, x, 0] = Utils.Clamp(sum);
                }
            }

            return result;
        }

        public static Image<Gray, byte> UnsharpMask(Image<Gray, byte> inputImage)
        {
            double[,] mask = GaussMask();
            Image<Gray, byte> lowPass = ApplyFilter(inputImage, mask);
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    double val = inputImage.Data[y, x, 0] + (inputImage.Data[y, x, 0] - lowPass.Data[y, x, 0]);
                    result.Data[y, x, 0] = Utils.Clamp(val);
                }
            }

            return result;
        }

        public static Image<Bgr, byte> UnsharpMask(Image<Bgr, byte> inputImage)
        {
            double[,] mask = GaussMask();
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int c = 0; c < 3; c++)
            {
                Image<Gray, byte> channel = new Image<Gray, byte>(inputImage.Size);
                for (int y = 0; y < inputImage.Height; y++)
                    for (int x = 0; x < inputImage.Width; x++)
                        channel.Data[y, x, 0] = inputImage.Data[y, x, c];

                Image<Gray, byte> lowPass = ApplyFilter(channel, mask);

                for (int y = 0; y < inputImage.Height; y++)
                {
                    for (int x = 0; x < inputImage.Width; x++)
                    {
                        double val = channel.Data[y, x, 0] + (channel.Data[y, x, 0] - lowPass.Data[y, x, 0]);
                        result.Data[y, x, c] = Utils.Clamp(val);
                    }
                }
            }

            return result;
        }
    }
}
