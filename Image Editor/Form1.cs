using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Editor
{
    public partial class Form1 : Form
    {
        RGBPixel[,] ImageMatrix;
        int pointNumber;
        Point startPoint;
        Point firstPoint;
        Point freePoint;
        Label firstdot;
        int[] firstarr;
        bool is_menuStripMouseDown;
        Point start;
        bool is_maximized;


        public Form1()
        {
            InitializeComponent();
            pointNumber = 0;
            is_menuStripMouseDown = false;
            is_maximized = true;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (selectTool.Checked == true)
            {
                pictureBox1_CreateDot(e.Location.X, e.Location.Y);

                if (pointNumber == 0)
                {
                    startPoint = new Point(e.Location.X, e.Location.Y);
                    //freePoint = new Point(e.Location.X, e.Location.Y);
                    freePoint = startPoint;
                    firstPoint = startPoint;

                }
                else if (pointNumber >= 1)
                {
                    startPoint = freePoint;
                    freePoint = new Point(e.Location.X, e.Location.Y);
                    int N = ImageOperations.GetWidth(ImageMatrix) * ImageOperations.GetHeight(ImageMatrix);
                    int S = ImageOperations.GetWidth(ImageMatrix) * startPoint.Y + startPoint.X;
                    int d = ImageOperations.GetWidth(ImageMatrix) * freePoint.Y + freePoint.X;
                    int[] arr= ImageOperations.shortestReach(N, ImageOperations.ImageGraph, S);
                    if (pointNumber == 1)
                        firstarr = arr;
                    int []  arr1 = ImageOperations.line(d, arr);
                    Point[] points = new Point[arr1.Length];
                    for(int i = 0; i < arr1.Length; i++)
                    {
                        points[i].X = arr1[i] % ImageOperations.GetWidth(ImageMatrix);
                        points[i].Y = arr1[i] / ImageOperations.GetWidth(ImageMatrix);
                    }
                    ImageOperations.outputShortestPath(points, S, startPoint, d, freePoint);
                    drawLine(points, Color.Red, 1);
                }
                pointNumber++;
            }
        }
        private void firstdot_Click(object sender, System.EventArgs e)
        {
            int S = ImageOperations.GetWidth(ImageMatrix) * firstPoint.Y + firstPoint.X;
            int d = ImageOperations.GetWidth(ImageMatrix) * freePoint.Y + freePoint.X;
            int[] arr1 = ImageOperations.line(d, firstarr);
            Point[] points = new Point[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                points[i].X = arr1[i] % ImageOperations.GetWidth(ImageMatrix);
                points[i].Y = arr1[i] / ImageOperations.GetWidth(ImageMatrix);
            }
            ImageOperations.outputShortestPath(points, S, startPoint, d, freePoint);
            drawLine(points, Color.Red, 1);
        }
        private void pictureBox1_CreateDot(int x, int y)
        {
            Label l = new Label();
            l.Size = new Size(4, 4);
            l.BackColor = Color.Blue;
            l.Location = new Point(x, y);
            if (pointNumber == 0)
            {
                firstdot = l;
                firstdot.Click += new EventHandler(firstdot_Click);
            }

            pictureBox1.Controls.Add(l);
        }
        private void drawLine(Point[] arr, Color C, int S)//O(N)
        {
            Graphics g = pictureBox1.CreateGraphics();
            Rectangle r = new Rectangle();
            Pen pen = new Pen(C, S);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            PaintEventArgs p = new PaintEventArgs(g, r);
            p.Graphics.DrawCurve(pen, arr); //O(N)
        }

        private void drawLine(Point p1, Point p2, Color C, int S)
        {
            Graphics g = pictureBox1.CreateGraphics();
            Rectangle r = new Rectangle();
            PaintEventArgs p = new PaintEventArgs(g, r);
            Pen blackPen = new Pen(C, S);
            blackPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            p.Graphics.DrawLine(blackPen, p1, p2);
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                pictureBox1.Controls.Clear();
                pointNumber = 0;
                selectTool.Enabled = true;
                pictureBox1.Width = pictureBox1.Image.Width;
                pictureBox1.Height = pictureBox1.Image.Height;
            }
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void menuStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            is_menuStripMouseDown = true;
            start = new Point(e.X, e.Y);
        }
        private void menuStrip1_MouseMove(object sender, MouseEventArgs e)
        {
            if (is_menuStripMouseDown)
            {
                Point p = PointToScreen(e.Location);
                this.Location = new Point(p.X - start.X, p.Y - start.Y);
            }
        }
        private void menuStrip1_MouseUp(object sender, MouseEventArgs e)
        {
            is_menuStripMouseDown = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (is_maximized == false)
            {
                this.WindowState = FormWindowState.Maximized;
                is_maximized = true;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                is_maximized = false;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            selectTooltip.SetToolTip(selectTool,"Makes a selection by snapping to image edges");
        }
        private void selectTool_CheckedChanged(object sender, EventArgs e)
        {
            if(selectTool.Checked == true)
            {
                ImageOperations.BuildGraph(ImageMatrix);
                ImageOperations.output();
               
            }
        }
    }



}
