using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Image_Editor
{
    public struct Edge
    {
        public int p;
        public double w;
    }
    public struct Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }
    public class ImageOperations
    {
        public static List<Edge>[] ImageGraph;

        public static void BuildGraph(RGBPixel[,] image)
        {
            int width = GetWidth(image);
            int height = GetHeight(image);
            Vector2D temp = new Vector2D();
            ImageGraph = new List<Edge>[width * height];
            for (int k = 0; k < width * height; k++)
            {
                ImageGraph[k] = new List<Edge>();
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i != height - 1 && j != width - 1)
                    {
                        temp = CalculatePixelEnergies(j, i, image);
                        Edge e = new Edge();
                        e.p = i * width + j + 1;
                        e.w = 1 / temp.X;
                        ImageGraph[i * width + j].Add(e);
                        e.p = (i + 1) * width + j;
                        e.w = 1 / temp.Y;
                        ImageGraph[i * width + j].Add(e);
                        e.p = i * width + j;
                        e.w = 1 / temp.X;
                        ImageGraph[i * width + j + 1].Add(e);
                        e.p = i * width + j;
                        e.w = 1 / temp.Y;
                        ImageGraph[(i + 1) * width + j].Add(e);
                    }
                    else if (i == height - 1 && j != width - 1)
                    {
                        Edge e = new Edge();
                        e.p = i * width + j + 1;
                        e.w = 1 / temp.X;
                        ImageGraph[i * width + j].Add(e);
                        e.p = i * width + j;
                        e.w = 1 / temp.X;
                        ImageGraph[i * width + j + 1].Add(e);
                    }
                    else if (i != height - 1 && j == width - 1)
                    {
                        Edge e = new Edge();
                        e.p = (i + 1) * width + j;
                        e.w = 1 / temp.Y;
                        ImageGraph[i * width + j].Add(e);
                        e.p = i * width + j;
                        e.w = 1 / temp.Y;
                        ImageGraph[(i + 1) * width + j].Add(e);
                    }

                }
            }

        }
       public static int[] shortestReach(int n, List<Edge>[] edges, int s)
        {
            //List<List<pair>> l = new List<List<pair>>();
            //for (int i = 0; i <= n; i++)
            //    l.Add(new List<pair>());
            //for (int f = 0; f < edges.Length; f++)
            //{
            //    l[edges[f][0]].Add(new pair(edges[f][2], edges[f][1]));
            //    l[edges[f][1]].Add(new pair(edges[f][2], edges[f][0]));
            //}
            int[] arr = new int[n + 1];
            for (int i = 0; i <= n; i++)
            {
                arr[i] = 1000000000;
            }
            heap h = new heap(n);
            h.add(0, s);
            while (!h.empty())
            {
                pair x = h.getmin();
                if (arr[x.second] > x.first)
                {
                    arr[x.second] = (int)x.first;
                    for (int i = 0; i < edges[x.second].Count; i++)
                    {
                        if (arr[edges[x.second][i].p] > x.first + edges[x.second][i].w)
                        {
                            h.add(x.first + edges[x.second][i].w, edges[x.second][i].p);
                        }
                    }
                }
            }
            return arr;
        }
        public static int[] line(int s, int d, int[] arr, List<Edge>[] edges)
        {
            List<int> l = new List<int>();
            double p = arr[d];
            while (d != s)
            {
                l.Add(d);
                for (int i = 0; i < edges[d].Count; i++)
                {
                    if (arr[d] == arr[edges[d][i].p] + edges[d][i].w)
                    {
                        p -= edges[d][i].w;
                        d = arr[edges[d][i].p];
                        break;
                    }
                }
            }
            l.Add(s);
            int[] a = new int[l.Count];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = l[i];
            }
            return a;
        }
        public static unsafe RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];


            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[0];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[2];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;

        }

        /// <summary>
        /// Calculate edge energy between
        ///     1. the given pixel and its right one (X)
        ///     2. the given pixel and its bottom one (Y)
        /// </summary>
        /// <param name="x">pixel x-coordinate</param>
        /// <param name="y">pixel y-coordinate</param>
        /// <param name="ImageMatrix">colored image matrix</param>
        /// <returns>edge energy with the right pixel (X) and with the bottom pixel (Y)</returns>
        public static Vector2D CalculatePixelEnergies(int x, int y, RGBPixel[,] ImageMatrix)
        {
            if (ImageMatrix == null) throw new Exception("image is not set!");

            Vector2D gradient = CalculateGradientAtPixel(x, y, ImageMatrix);

            double gradientMagnitude = Math.Sqrt(gradient.X * gradient.X + gradient.Y * gradient.Y);
            double edgeAngle = Math.Atan2(gradient.Y, gradient.X);
            double rotatedEdgeAngle = edgeAngle + Math.PI / 2.0;

            Vector2D energy = new Vector2D();
            energy.X = Math.Abs(gradientMagnitude * Math.Cos(rotatedEdgeAngle));
            energy.Y = Math.Abs(gradientMagnitude * Math.Sin(rotatedEdgeAngle));

            return energy;
        }
        /// <summary>
        /// Calculate Gradient vector between the given pixel and its right and bottom ones
        /// </summary>
        /// <param name="x">pixel x-coordinate</param>
        /// <param name="y">pixel y-coordinate</param>
        /// <param name="ImageMatrix">colored image matrix</param>
        /// <returns></returns>
        private static Vector2D CalculateGradientAtPixel(int x, int y, RGBPixel[,] ImageMatrix)
        {
            Vector2D gradient = new Vector2D();

            RGBPixel mainPixel = ImageMatrix[y, x];
            double pixelGrayVal = 0.21 * mainPixel.red + 0.72 * mainPixel.green + 0.07 * mainPixel.blue;

            if (y == GetHeight(ImageMatrix) - 1)
            {
                //boundary pixel.
                for (int i = 0; i < 3; i++)
                {
                    gradient.Y = 0;
                }
            }
            else
            {
                RGBPixel downPixel = ImageMatrix[y + 1, x];
                double downPixelGrayVal = 0.21 * downPixel.red + 0.72 * downPixel.green + 0.07 * downPixel.blue;

                gradient.Y = pixelGrayVal - downPixelGrayVal;
            }

            if (x == GetWidth(ImageMatrix) - 1)
            {
                //boundary pixel.
                gradient.X = 0;

            }
            else
            {
                RGBPixel rightPixel = ImageMatrix[y, x + 1];
                double rightPixelGrayVal = 0.21 * rightPixel.red + 0.72 * rightPixel.green + 0.07 * rightPixel.blue;

                gradient.X = pixelGrayVal - rightPixelGrayVal;
            }


            return gradient;
        }
        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        public static unsafe void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[0] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[2] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }
    }
    class heap
    {
        public int size, last;
        private pair[] arr;
        public heap(int n)
        {
            arr = new pair[n];
            size = n;
            last = 1;
        }
        public void add(double a, int b)
        {
            if (last == 0)
                last++;
            if (last == size)
            {
                Array.Resize(ref arr, arr.Length * 2);
                size = arr.Length;
            }
            arr[last] = new pair(a, b);
            int i = last;
            while (i != 1 && arr[i].first < arr[i / 2].first)
            {
                pair x = arr[i / 2];
                arr[i / 2] = arr[i];
                arr[i] = x;
                i /= 2;
            }
            last++;
        }
        public pair getmin()
        {
            pair x = arr[1];
            int i = 1;
            while (i < last)
            {
                if ((i * 2) + 1 < last)
                {
                    if (arr[i * 2].first < arr[(i * 2) + 1].first)
                    {
                        arr[i] = arr[i * 2];
                        i *= 2;
                    }
                    else
                    {
                        arr[i] = arr[(i * 2) + 1];
                        i *= 2;
                        i++;
                    }
                }
                else if (i * 2 < last)
                {
                    arr[i] = arr[i * 2];
                    break;
                }
                else
                {
                    for (; i < last - 1; i++)
                    {
                        arr[i] = arr[i + 1];
                        if (i % 2 == 1)
                        {
                            int j = i;
                            while (j != 1 && arr[j].first < arr[j / 2].first)
                            {
                                pair w = arr[j / 2];
                                arr[j / 2] = arr[j];
                                arr[j] = w;
                                j /= 2;
                            }
                        }
                    }
                    break;
                }
            }
            last--;
            return x;
        }
        public bool empty()
        {
            if (last == 0)
                return true;
            return false;
        }
    }
    class pair
    {
        public double first;
        public int second;
        public pair(double a, int b)
        {
            first = a;
            second = b;
        }
    }
}
