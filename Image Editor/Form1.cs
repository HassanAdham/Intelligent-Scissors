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
        public Form1()
        {
            InitializeComponent();
            pointNumber = 0;
            is_menuStripMouseDown = false;
            is_maximized = true;
        }

        RGBPixel[,] ImageMatrix;
        int pointNumber;
        Point startPoint;
        Point freePoint;
        bool is_menuStripMouseDown;
        Point start;
        bool is_maximized;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                ImageOperations.BuildGraph(ImageMatrix);
                pictureBox1.Controls.Clear();
                pointNumber = 0;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                pictureBox1_CreatDot(e.Location.X, e.Location.Y);
                if (pointNumber == 0)
                {
                    pointNumber++;
                    startPoint = new Point(e.Location.X, e.Location.Y);
                    freePoint = new Point(e.Location.X, e.Location.Y);
                    freePoint = startPoint;

                }
                else if (pointNumber >= 1)
                {
                    startPoint = freePoint;
                    freePoint = new Point(e.Location.X, e.Location.Y);
                    int N = ImageOperations.GetWidth(ImageMatrix) * ImageOperations.GetHeight(ImageMatrix);
                    int S = ImageOperations.GetWidth(ImageMatrix) * startPoint.Y + startPoint.X;
                    int d = ImageOperations.GetWidth(ImageMatrix) * freePoint.Y + freePoint.X;
                    double[] arr= ImageOperations.shortestReach(N, ImageOperations.ImageGraph, S);
                    int []  arr1 = ImageOperations.line(S, d, arr, ImageOperations.ImageGraph);
                    Point[] points = new Point[arr1.Length];
                    for(int i = 0; i < arr1.Length; i++)
                    {
                        points[i].X = arr1[i] % ImageOperations.GetWidth(ImageMatrix);
                        points[i].Y = arr1[i] / ImageOperations.GetWidth(ImageMatrix);
                    }
                    drawLine(points, Color.Red, 1);
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //if(pointNumber >=1)
            //{
            //    startPoint = freePoint;
            //    freePoint = new Point(e.Location.X, e.Location.Y);
            //    drawLine(startPoint, freePoint, Color.Red, 1);
            //}
        }

        private void checkBox1_EnabledChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                checkBox1.Text = "On";
            else
                checkBox1.Text = "Off";
        }

        private void pictureBox1_CreatDot(int x, int y)
        {
            Label l = new Label();
            l.Size = new Size(3, 3);
            l.BackColor = Color.Blue;
            l.Location = new Point(x, y);

            pictureBox1.Controls.Add(l);
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
        private void drawLine(Point [] arr, Color C, int S)
        {
            Graphics g = pictureBox1.CreateGraphics();
            Rectangle r = new Rectangle();
            Pen pen = new Pen(C, S);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            PaintEventArgs p = new PaintEventArgs(g, r);
            p.Graphics.DrawCurve(pen, arr);
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
                ImageOperations.BuildGraph(ImageMatrix);
                pictureBox1.Controls.Clear();
                pointNumber = 0;
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

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }



}
