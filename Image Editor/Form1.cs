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
        }

        RGBPixel[,] ImageMatrix;
        int pointNumber;
        Point startPoint;
        Point freePoint;

        private void button1_Click(object sender, EventArgs e)
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
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked==true)
            {
                pictureBox1_CreatDot(e.Location.X,e.Location.Y);
                if (pointNumber == 0)
                {
                    pointNumber++;
                    startPoint = new Point(e.Location.X,e.Location.Y);
                    freePoint = new Point(e.Location.X, e.Location.Y);
                    freePoint = startPoint;
                }      
            }     
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(pointNumber >=1)
            {
                startPoint = freePoint;
                freePoint = new Point(e.Location.X, e.Location.Y);
                drawLine(startPoint, freePoint, Color.Red, 1);
            }
        }

        private void checkBox1_EnabledChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
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
            PaintEventArgs p = new PaintEventArgs(g,r);
            Pen blackPen = new Pen(C, S);
            blackPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            p.Graphics.DrawLine(blackPen, p1, p2);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void menuStrip1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {

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
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void filesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }



}
