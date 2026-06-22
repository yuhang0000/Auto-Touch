using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auto_Touch
{
    public partial class Preview : Form
    {
        public Preview()
        {
            InitializeComponent();
            this.label1.Parent = this.pictureBox1;
            //全屏
            this.Top = 0;
            this.Left = 0;
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            this.bitmap = new Bitmap(this.Width, this.Height);
            this.graphics = Graphics.FromImage(this.bitmap);
        }

        public Bitmap bitmap;
        public Graphics graphics;
        public Point LastPoint = new Point(int.MinValue, int.MinValue);
        public Pen pen = new Pen(Color.Red, 1);

        /// <summary>
        /// 绘制轨迹
        /// </summary>
        /// <param name="XY">点坐标</param>
        public void Draw(Point XY)
        {
            if(pen == null)
            {
                return;
            }
            if(this.LastPoint.X == int.MinValue)
            {
                this.LastPoint = XY;
            }
            else
            {
                this.graphics.DrawLine(this.pen, this.LastPoint, XY);
                this.pictureBox1.Image = this.bitmap;
                this.LastPoint = XY;
            }
        }

        private void Preview_KeyDown(object sender, KeyEventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        private void Preview_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.pen.Dispose();
            this.pen = null;
            this.graphics.Dispose();
            this.graphics = null;
            this.bitmap.Dispose();
            this.bitmap = null;
        }
    }
}
