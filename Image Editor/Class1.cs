using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

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
                        temp = CalculatePixelEnergies(j, i, image);
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
                        temp = CalculatePixelEnergies(j, i, image);
                        Edge e = new Edge();
                        e.p = (i + 1) * width + j;
                        e.w = 1 / temp.Y;
                        ImageGraph[i * width + j].Add(e);
                        e.p = i * width + j;
                        e.w = 1 / temp.Y;
                        ImageGraph[(i + 1) * width + j].Add(e);
                    }
                    else if (i == height - 1 && j == width - 1)
                    {
                        temp = CalculatePixelEnergies(j, i - 1, image);
                        Edge e = new Edge();
                        e.p = (i - 1) * width + j;
                        e.w = 1 / temp.Y;
                        ImageGraph[i * width + j].Add(e);
                        temp = CalculatePixelEnergies(j - 1, i, image);
                        e.p = i * width + j - 1;
                        e.w = 1 / temp.X;
                        ImageGraph[i * width + j].Add(e);
                    }
                }
            }
            output();
        }

        private static void output()
        {
            using (StreamWriter writetext = new StreamWriter("output.txt"))
            {
                string g = "The constructed graph" + Environment.NewLine;
                writetext.WriteLine(g);
                for (int i = 0; i < ImageGraph.Length; i++)
                {
                    string s = " The  index node" + i + Environment.NewLine + "Edges" + Environment.NewLine;
                    for (int j = 0; j < ImageGraph[i].Count; j++)
                    {
                        if (ImageGraph[i][j].w == double.PositiveInfinity)
                            s += "edge from   " + i + "  To  " + ImageGraph[i][j].p + "  With Weights  " + 1E+16 + Environment.NewLine;
                        else
                            s += "edge from   " + i + "  To  " + ImageGraph[i][j].p + "  With Weights  " + ImageGraph[i][j].w + Environment.NewLine;
                    }
                    s += Environment.NewLine + Environment.NewLine;
                    writetext.WriteLine(s);                    
                }
            }
        }

        public static void outputShortestPath(Point[] arr, int source, Point sourcePoint, int destination, Point destintaionPoint)
        {
            using (StreamWriter sw = new StreamWriter("shortestPath.txt"))
            {
                sw.WriteLine(" The Shortest path from Node  " + source + "at position   " + sourcePoint.X + "  " + sourcePoint.Y);
                sw.WriteLine(" The Shortest path to Node  " + destination + "at position   " + destintaionPoint.X + "  " + destintaionPoint.Y);
                for (int i = arr.Length - 1; i >= 0; i--)
                {
                    sw.WriteLine("Node  " + arr[i] + " at position x " + arr[i].X + " at position y   " + arr[i].Y);
                }
            }
        }
        public static double[] shortestReach(int n, List<Edge>[] edges, int s)
        {
            //
            //  E is the number of edges
            //  N is the number of nodes
            //
            double[] arr = new double[n + 1];//array holds the path value from source to each node
            for (int i = 0; i <= n; i++)//O(N)
            {
                arr[i] = 1000000000;//set path value from source to each node as high value
            }
            heap h = new heap(n);
            h.add(0, s);//add the source node path value equals 0    O(log(N))
            while (!h.empty())//O(E log(N))
            {
                pair x = h.getmin();//get the minimum value and remove it from the heap
                if (arr[x.second] > x.first)//check if the new path value is better than the one we already have
                {
                    arr[x.second] = (double)x.first;//update the path value
                    for (int i = 0; i < edges[x.second].Count; i++)//loop over edges connected to the node we have now O(E)
                    {
                        if (arr[edges[x.second][i].p] > x.first + edges[x.second][i].w)//check if the new path value is better than the one we already have
                        {
                            h.add(x.first + edges[x.second][i].w, edges[x.second][i].p);//O(log(N))
                        }
                    }
                }
            }
            return arr;
        }
        public static int[] line(int s, int d, double[] arr, List<Edge>[] edges)
        {
            List<int> l = new List<int>();// create list to hold the nodes in the shortest path from source to destination
            while (d != s)//start first time from destination and loop till it equals the source node   O(N)
            {
                l.Add(d);
                for (int i = 0; i < edges[d].Count; i++)//loop over the connected edges to the node we have now O(E)
                {
                    if (arr[d] == arr[edges[d][i].p] + edges[d][i].w)//check if that node is part of the path from source to destination
                    {
                        d = edges[d][i].p;//update d with new node and break
                        break;
                    }
                }
            }
            l.Add(s);//add the source to the list to finish the path 
            int[] a = new int[l.Count];
            for (int i = 0; i < a.Length; i++)//O(N)
            {
                a[i] = l[i];//copy the nodes from the list to an array
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
            if (last == size)//check if the array size can have anymore elements or not
            {
                Array.Resize(ref arr, arr.Length * 2);//double the size of the array 
                size = arr.Length;//set the size varialbe to the new size of the array
            }
            arr[last] = new pair(a, b);//put the new element after the last element in the array
            int i = last;
            while (i != 1 && arr[i].first < arr[i / 2].first)//compare the new element with it's parent and swap them to keep it minimum tree O(log(N))
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
            pair x = arr[1],y;
            last--;
            if (last != 0)//update the tree if it still have any elements
            {
                arr[1] = arr[last];//put the last element in the first place
                int i = 1;
                while (i < last)//update the i-th element with it's childs O(log(N))
                {
                    //finding minimum between i and it's childs to update the tree

                    if ((i * 2) + 1 < last)//check if valid right and left childs
                    {

                        if (arr[i * 2].first < arr[(i * 2) + 1].first && arr[i * 2].first < arr[i].first)
                        {
                            y = arr[i * 2];
                            arr[i * 2] = arr[i];
                            arr[i] = y;
                            i *= 2;
                        }
                        else if (arr[i * 2].first >= arr[(i * 2) + 1].first && arr[(i * 2) + 1].first < arr[i].first)
                        {
                            y = arr[(i * 2) + 1];
                            arr[(i * 2) + 1] = arr[i];
                            arr[i] = y;
                            i *= 2;
                            i++;
                        }
                        else
                            break;
                    }
                    else if (i*2<last)//check if valid left child
                    {
                        if (arr[i * 2].first < arr[i].first)
                        {
                            y = arr[i * 2];
                            arr[i * 2] = arr[i];
                            arr[i] = y;
                            i *= 2;
                        }
                        else
                            break;
                    }
                    else//this node has no childs
                    {
                        break;
                    }
                }
            }
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
