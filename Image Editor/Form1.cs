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
        }

        RGBPixel[,] ImageMatrix;
        int pointNumber = 0;
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
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked==true)
            {
                pictureBox1_CreateDash(e.Location.X,e.Location.Y);
                if (pointNumber == 0)
                {
                     pointNumber++;
                     startPoint = new Point(e.Location.X,e.Location.Y);
                }
                else if(pointNumber == 1)
                {
                    pointNumber++;
                    freePoint = new Point(e.Location.X, e.Location.Y);
                    drawLine(startPoint, freePoint);
                }
                else
                {
                    startPoint = freePoint;
                    freePoint = new Point(e.Location.X, e.Location.Y);
                    drawLine(startPoint, freePoint);
                }       
            }     
        }

        private void checkBox1_EnabledChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
                checkBox1.Text = "On";
            else
                checkBox1.Text = "Off";
        }

        private void pictureBox1_CreateDash(int x, int y)
        {
            Label l = new Label();
            l.Text = "-";
            l.Size = new Size(3, 3);
            l.BackColor = Color.Blue;
            l.Location = new Point(x, y);

            pictureBox1.Controls.Add(l);
        }

        private void drawLine(Point p1, Point p2)
        {
            Graphics g = pictureBox1.CreateGraphics();
            Rectangle r = new Rectangle();
            PaintEventArgs p = new PaintEventArgs(g,r);
            Pen blackPen = new Pen(Color.Black, 3);
            p.Graphics.DrawLine(blackPen, p1,p2);
        }
    }
}
