using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;

namespace Algorithms.Sections
{
    public class MorphologicalOperations
    {
        public static Image<Bgr, byte> ConnectedComponents(Image<Gray, byte> inputImage)
        {
            int height = inputImage.Height;
            int width = inputImage.Width;

            Image<Gray, byte> binary = new Image<Gray, byte>(width, height);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    binary.Data[y, x, 0] = inputImage.Data[y, x, 0] >= 128 ? (byte)255 : (byte)0;

            Image<Bgr, byte> result = new Image<Bgr, byte>(width, height);
            bool[,] visited = new bool[height, width];

            Random rand = new Random();

            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (binary.Data[y, x, 0] == 255 && !visited[y, x])
                    {
                        byte r = (byte)rand.Next(50, 256);
                        byte g = (byte)rand.Next(50, 256);
                        byte b = (byte)rand.Next(50, 256);

                        Queue<Tuple<int, int>> coada = new Queue<Tuple<int, int>>();
                        coada.Enqueue(new Tuple<int, int>(y, x));
                        visited[y, x] = true;

                        while (coada.Count != 0)
                        {
                            Tuple<int, int> current = coada.Dequeue();
                            int cy = current.Item1;
                            int cx = current.Item2;

                            result.Data[cy, cx, 0] = b;
                            result.Data[cy, cx, 1] = g;
                            result.Data[cy, cx, 2] = r;

                            for (int i = 0; i < 8; i++)
                            {
                                int ny = cy + dy[i];
                                int nx = cx + dx[i];

                                if (ny >= 0 && ny < height && nx >= 0 && nx < width
                                    && !visited[ny, nx]
                                    && binary.Data[ny, nx, 0] == 255)
                                {
                                    visited[ny, nx] = true;
                                    coada.Enqueue(new Tuple<int, int>(ny, nx));
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}