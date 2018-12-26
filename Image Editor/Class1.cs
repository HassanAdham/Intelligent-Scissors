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
        /* Receives and array of pixels.
        * Building Graph using Adjacency List.
        * An ImageGraph array is defined of size N^2, each index has the node ID and each node has a list of edges.
        * The list is built for each node to hold its neighboring nodes (Left,Right,Up,Down) and the weights between them and the current node.
        * The code is classified according the node position in the image. (Node with 2 neighbors, node with 3 neighbors and node with 4 neighbors).
        * (For current node)
        * Add to its list the edge weight between current node and right node (If exists) and between bottom node (If exists).
        * For the right node, add to its list the current node as its left node. (If exists).
        * For the bottom node, add to its list the current node as its above node. (If exists).
        * At the end, the ImageGraph array is full all connections between all nodes.
        */
        public static List<Edge>[] ImageGraph;
        public static void BuildGraph(RGBPixel[,] image)//O(N^2)
        {
            int width = GetWidth(image);
            int height = GetHeight(image);
            Vector2D temp = new Vector2D();
            ImageGraph = new List<Edge>[width * height];
            for (int k = 0; k < width * height; k++) //O(N^2)
            {
                //Create a list of edges for each node in the image.
                ImageGraph[k] = new List<Edge>();
            }
            for (int i = 0; i < height; i++)//O(N)
            {
                for (int j = 0; j < width; j++)//O(N)
                {
                    //If node is not in last column nor last row,
                    //i.e Must have both right and bottom nodes.
                    if (i != height - 1 && j != width - 1)
                    {                                      
                        temp = CalculatePixelEnergies(j, i, image);
                        Edge e = new Edge();
                        e.p = i * width + j + 1;
                        if (1 / temp.X == double.PositiveInfinity)
                            e.w = 1E+16;
                        else
                            e.w = 1 / temp.X;
                        ImageGraph[i * width + j].Add(e);
                        e.p = (i + 1) * width + j;
                        if (1 / temp.Y == double.PositiveInfinity)
                            e.w = 1E+16;
                        else
                            e.w = 1 / temp.Y;
                        ImageGraph[i * width + j].Add(e);
                        e.p = i * width + j;
                        if (1 / temp.X == double.PositiveInfinity)
                            e.w = 1E+16;
                        else
                            e.w = 1 / temp.X;
                        ImageGraph[i * width + j + 1].Add(e);
                        e.p = i * width + j;
                        if (1 / temp.Y == double.PositiveInfinity)
                            e.w = 1E+16;
                        else
                            e.w = 1 / temp.Y;
                        ImageGraph[(i + 1) * width + j].Add(e);
                    }
                    //Current node is in last row but not in last column.
                    //i.e Doesn't have bottom node.
                    else if (i == height - 1 && j != width - 1)
                    {                                           
                        temp = CalculatePixelEnergies(j, i, image);
                        Edge e = new Edge();
                        e.p = i * width + j + 1;
                        if (1 / temp.X == double.PositiveInfinity)
                            e.w = 1E+16;
                        else
                            e.w = 1 / temp.X;
                        ImageGraph[i * width + j].Add(e);
                        e.p = i * width + j;
                        if (1 / temp.X == double.PositiveInfinity)
                            e.w = 1E+16;
                        else
                            e.w = 1 / temp.X;
                        ImageGraph[i * width + j + 1].Add(e);
                    }
                    //Current node is in last column but not in last row.
                    //i.e Doesn't have right node.
                    else if (i != height - 1 && j == width - 1)
                    {                                           
                        temp = CalculatePixelEnergies(j, i, image);
                        Edge e = new Edge();
                        e.p = (i + 1) * width + j;
                        if (1 / temp.Y == double.PositiveInfinity)
                            e.w = 1E+16;
                        else
                            e.w = 1 / temp.Y;
                        ImageGraph[i * width + j].Add(e);
                        e.p = i * width + j;
                        if (1 / temp.Y == double.PositiveInfinity)
                            e.w = 1E+16;
                        else
                            e.w = 1 / temp.Y;
                        ImageGraph[(i + 1) * width + j].Add(e);
                    }
                }
            }
        }
        /*
        * It builds an array to save the shortest paths from source to each node.
        * Another array is defined to hold all parents of each node.
        * ------------------------------------------------------------------
        * Setting an initial value for each node with infinity, then add in the priority queue the source node with a path to itself equal zero.
        * Loop keeps iterating until no nodes are yet found.
        * Check the paths value with an already saved value in the array, then it updates the parents.
        * Then it starts to add the connected nodes to it (value of path = current node + edge weight).
        * The function returns the array of parents, to track the paths of each node.
        */
        public static int[] shortestReach(int n, List<Edge>[] edges, int s)//O(ElogN)
        {
            /*
                *  E is the number of edges.
                *  N is the number of nodes.
            */
            //Array holds the path value from source to each node.
            double[] arr = new double[n + 1];
            int[] pa = new int[n + 1];
            for (int i = 0; i <= n; i++)//O(N)
            {
                //Set path value from source to each node as high value.
                arr[i] = 1.7E308;
                pa[i] = -1;
            }
            heap h = new heap(n);
            //Add the source node path value equals 0
            h.add(0,s,s);//O(log(N)).
            while (!h.empty())//O(E log(N)).
            {
                //Get the minimum value and remove it from the heap.
                pair x = h.getmin();
                //Check if the new path value is better than the one we already have.
                if (arr[x.second] > x.first)
                {
                    //Update the path value.
                    arr[x.second] = x.first;
                    pa[x.second] = x.p;
                    //Loop over edges connected to the node we have now .
                    for (int i = 0; i < edges[x.second].Count; i++)//O(E).
                    {
                        //Check if the new path value is better than the one we already have.
                        if (arr[edges[x.second][i].p] > x.first + edges[x.second][i].w)
                        {
                            h.add(x.first + edges[x.second][i].w, edges[x.second][i].p,x.second);//O(log(N)).
                        }
                    }
                }
            }
            return pa;
        }

        /*
        * Looping on the array of parents, until the node is equal to its parent. 
        * Inside the loop, each node is assigned with the value of its parent.
        */
        public static int[] line(int d,int []par)//O(N).
        {
            //Create list to hold the nodes in the shortest path from source to destination.
            List<int> l = new List<int>();
            // Start first time from destination and loop till it equals the source node.
            while (d != par[d])//O(N).
            {
                l.Add(d);
                d = par[d];
            }
            l.Add(d);
            int[] a = new int[l.Count];
            for (int i = 0; i < a.Length; i++)//O(N)
            {
                //Copy the nodes from the list to an array.
                a[i] = l[i];
            }
            return a;
        }
        public static void output()//O(N^2).
        {
            using (StreamWriter writetext = new StreamWriter("output.txt"))
            {
                string g = "The constructed graph" + Environment.NewLine;
                writetext.WriteLine(g);
                for (int i = 0; i < ImageGraph.Length; i++)//O(N^2).
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
        }//
        public static void outputShortestPath(Point[] arr, int source, Point sourcePoint, int destination, Point destintaionPoint)
        {
            int c = 0;
            using (StreamWriter sw = new StreamWriter("shortestPath.txt"))
            {
                sw.WriteLine(" The Shortest path from Node  " + source + "at position   " + sourcePoint.X + "  " + sourcePoint.Y);
                sw.WriteLine(" The Shortest path to Node  " + destination + "at position   " + destintaionPoint.X + "  " + destintaionPoint.Y);
                for (int i = arr.Length - 1; i >= 0; i--)
                {
                    sw.WriteLine("Node  " + arr[i] + " at position x " + arr[i].X + " at position y   " + arr[i].Y);
                    c++;
                }
                sw.WriteLine(c);
            }
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

        /*
        * Check the size of the array of nodes, if the size is not enough then double it.
        * Add new node at the end of the array.
        * Compare each node with its parent (where parent_index = node_index/2). If(value of the node < its parent), then swap them.
        */
        public void add(double a, int b,int c)//O(logN).
        {
            if (last == 0)
                last++;
            //Check if the array size can have anymore elements or not.
            if (last == size)
            {
                //Double the size of the array.
                Array.Resize(ref arr, arr.Length * 2);
                //Set the size varialbe to the new size of the array.
                size = arr.Length;
            }
            //Put the new element after the last element in the array.
            arr[last] = new pair(a, b, c);
            int i = last;
            //Compare the new element with it's parent and swap them to keep it minimum tree.
            while (i != 1 && arr[i].first < arr[i / 2].first)//O(log(N)).
            {
                pair x = arr[i / 2];
                arr[i / 2] = arr[i];
                arr[i] = x;
                i /= 2;
            }
            last++;
        }
        /*
        * First node in the array is the minimum node. 
        * After virtually deleting the minimum node, we check if the array has other nodes.
        * If there is still other nodes, put the last node as the first node in the array.
        * Update the position of the first node, by looking at both {left(index*2) and right(index*2+1)} nodes. Then swap this node with minimum of (left and right).
        * When no children are left, or when the current node is already smaller than it's right and left, the loop breaks.
        * Finally return minimum node in the array.
        */

        public pair getmin()//O(LogN).
        {
            pair x = arr[1],y;
            last--;
            //Update the tree if it still have any elements.
            if (last != 0)
            {
                //Put the last element in the first place.
                arr[1] = arr[last];
                int i = 1;
                //Update the i-th element with it's children.
                while (i < last)//O(log(N)).
                {
                    //Finding minimum between i and it's children to update the tree.

                    //Check if valid right and left children.
                    if ((i * 2) + 1 < last)
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
                    //Check if valid left child.
                    else if (i*2<last)
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
                    //This node has no children.
                    else
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
        public int second,p;
        public pair(double a, int b,int c)
        {
            first = a;
            second = b;
            p = c;
        }
    }
}