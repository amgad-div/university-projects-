using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics_Project
{
    public partial class Form1 : Form
    {
       
        int[] matrix1 = new int[3];
        int[,] matrix2 = new int[3, 3];
        Point[] line = new Point[1000000];
        Point[] clp = new Point[10000];
        Point[] circ = new Point[100000];
        Point[] newpoint = new Point[100];
        int prevx = -1, prevy = -1;
        int x, y;
        int nx = -1, ny = -1;
        Point[] rot = new Point[100];
        int r = 0;
        int nr = 0;
        int cr = 0;
        public float steps;
        public int q = 0;
        public Bitmap bt, clip,fil;
        int shp = 0, shx1, shx2, shy1, shy2;
        int mxy = 0, mnx = 1000;
        int[] dx = { 1, -1, 0, 0, 1, 1, -1, -1 };
        int[] dy = { 0, 0, 1, -1, 1, -1, 1, -1 };
        Color cl;
         public Form1()
        {
            InitializeComponent();
            bt = new Bitmap(pic.Width, pic.Height);
            clip = new Bitmap(pic.Width, pic.Height);
            cl = new Color();
            cl = Color.Brown;
        }
        static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
        Point mirror(Point p, int x0, int y0, int x1, int y1)
        {

            double dx, dy, a, b;
            double x2, y2;
            Point p1=new Point(); //reflected point to be returned

            dx = (double)(x1 - x0);
            dy = (double)(y1 - y0);

            a = (dx * dx - dy * dy) / (dx * dx + dy * dy);
            b = 2 * dx * dy / (dx * dx + dy * dy);

            x2 = Math.Round(a * (p.X - x0) + b * (p.Y - y0) + x0);
            y2 = Math.Round(b * (p.X - x0) - a * (p.Y - y0) + y0);

            p1.X = (int)x2;
            p1.Y = (int)y2;

            return p1;
        }
        private void fill(int x, int y)
        {
            if (!valid(x, y))
                return;
            Color c = bt.GetPixel(x, y);
            if (c.R != 0||c.G!=0||c.B!=0)
                return;
            bt.SetPixel(x, y, cl);
            for (int i = 0; i < 4; i++)
            {
                fill(x + dx[i], y + dy[i]);
            }
        }
        bool valid(int x, int y)
        {
            return (0 <= x && x < pic.Width && 0 <= y && y < pic.Height);
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            //drawline_DDA2(0, 160, 320, 160);
            //drawline_DDA2(160, 0, 160, 320);
        }
        public int[] mul_matrix(int[] mat1, int[,] mat2)
        {
            int[] arr = new int[3];
            for (int i = 0; i < 3; i++)
            {
                int sum = 0;
                for (int j = 0; j < 3; j++)
                {
                    sum += mat1[j] * mat2[i, j];
                }
                arr[i] = sum;
            }
            return arr;
        }
        public Point translation1p(Point p1,Point p2)
        {
            Point res=new Point();
            matrix1[0] = p1.X; matrix1[1] = p1.Y; matrix1[2] = 1;
            matrix2[0, 0] = 1; matrix2[0, 1] = 0; matrix2[0, 2] = p2.X;
            matrix2[1, 0] = 0; matrix2[1, 1] = 1; matrix2[1, 2] = p2.Y;
            matrix2[2, 0] = 0; matrix2[2, 1] = 0; matrix2[2, 2] = 1;

            int[] arr = mul_matrix(matrix1, matrix2);
            res.X = arr[0]; res.Y = arr[1];
            return res;
        }
        public Point rotation1p(Point p1, int ang)
        {
            Point res = new Point();
            matrix1[0] = p1.X; matrix1[1] = p1.Y; matrix1[2] = 1;
            matrix2[0, 0] = (int)(Math.Cos(ang)); matrix2[0, 1] = (int)(-Math.Sin(ang)); matrix2[0, 2] = 0;
            matrix2[1, 0] = (int)(Math.Sin(ang)); matrix2[1, 1] = (int)(Math.Cos(ang)); matrix2[1, 2] = 0;
            matrix2[2, 0] = 0; matrix2[2, 1] = 0; matrix2[2, 2] = 1;

            int[] arr = mul_matrix(matrix1, matrix2);
            res.X = arr[0]; res.Y = arr[1];
            return res;
        }
        public void drawline_DDA(int x1, int y1, int x2, int y2)
        {
            
            int dx = x2 - x1;
            int dy = y2 - y1;
            clip = new Bitmap(pic.Width, pic.Height);
            steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
            float xinc = dx / steps;
            float yinc = dy / steps;
            float x = x1, y = y1;
            for (int i = 0; i < steps; i++)
            {
                DataGridViewRow r = new DataGridViewRow();
                
                    
                r.CreateCells(dgv);
                r.Cells[0].Value = i;
                r.Cells[1].Value = x;
                r.Cells[2].Value = y;
               // System.Threading.Thread.Sleep(5);
                if (valid((int)Math.Round(x + 2), (int)Math.Round(y + 2)) == true && valid((int)Math.Round(x - 2), (int)Math.Round(y - 2)) == true)
                {
                    bt.SetPixel((int)Math.Round(x), (int)Math.Round(y), cl);
                    bt.SetPixel((int)Math.Round(x) + 1, (int)Math.Round(y), cl);
                    bt.SetPixel((int)Math.Round(x), (int)Math.Round(y) + 1, cl);
                    bt.SetPixel((int)Math.Round(x) + 2, (int)Math.Round(y), cl);
                    bt.SetPixel((int)Math.Round(x), (int)Math.Round(y) + 2, cl);
                }
                //clip.SetPixel((int)Math.Round(x), (int)Math.Round(y), Color.Blue);
                x += xinc;
                y += yinc;
                r.Cells[3].Value = (int)Math.Round(x);
                r.Cells[4].Value = (int)Math.Round(y);
                //line[q].X =(int)Math.Round(x); line[q].Y =(int)Math.Round(y);
                mxy = Math.Max((int)Math.Round(y), mxy);
                mnx = Math.Min((int)Math.Round(x), mnx);
                q++;
                dgv.Rows.Add(r);
            }
            pic.Image = bt;
        }
        public void drawline_DDA2(int x1, int y1, int x2, int y2)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
            float xinc = dx / steps;
            float yinc = dy / steps;
            float x = x1, y = y1;
            for (int i = 0; i < steps; i++)
            {
                DataGridViewRow r = new DataGridViewRow();
                r.CreateCells(dgv);
                r.Cells[0].Value = i;
                r.Cells[1].Value = x;
                r.Cells[2].Value = y;

                bt.SetPixel((int)Math.Round(x), (int)Math.Round(y), cl);
                clip.SetPixel((int)Math.Round(x), (int)Math.Round(y), Color.Blue);
                x += xinc;
                y += yinc;
                r.Cells[3].Value = (int)Math.Round(x);
                r.Cells[4].Value = (int)Math.Round(y);
                //clip[q].X = (int)Math.Round(x); clip[q].Y = (int)Math.Round(y);
                q++;
                dgv.Rows.Add(r);
                
            }
            pic.Image = bt;
        }
        public void drawline_Bresenham(int x1, int y1, int x2, int y2)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            Bitmap bt = new Bitmap(pic.Width, pic.Height);
            bool s = false;
            if (dy > dx)
            {
                int t = dx;
                dx = dy;
                dy = t;
                s = true;
            }
            int dx2 = 2 * dx;
            int dy2 = 2 * dy;

            int pi = dy2 - dx;
            int x = x1, y = y1;

            for (int i = 0; i < dx; i++)
            {
                DataGridViewRow r = new DataGridViewRow();
                r.CreateCells(dgv);
                r.Cells[0].Value = i;
                r.Cells[1].Value = x;
                r.Cells[2].Value = y;
                r.Cells[3].Value = pi;
                if (pi > 0)
                {
                    y++;
                    pi = pi + dy2 - dx2;
                }
                else
                    pi = pi + dy2;
                x++;
                r.Cells[4].Value = x;
                r.Cells[5].Value = y;
                if (!s)
                    bt.SetPixel(x, y, Color.Blue);
                else
                {
                    bt.SetPixel(y, x, Color.Blue);
                    r.Cells[1].Value = y;
                    r.Cells[2].Value = x;
                    r.Cells[4].Value = y;
                    r.Cells[5].Value = x;
                }
                dgv.Rows.Add(r);
            }
            pic.Image = bt;
        }
        public void drawCircle(int radus,int xc,int yc)
        {
                int x = 0, y = radus;
                int i=0;
                int p = 3 - 2 * radus;
                bt.SetPixel(x + xc, yc + y, Color.Blue);
                bt.SetPixel(-x + xc, yc + y, Color.Blue);
                bt.SetPixel(x + xc, yc - y, Color.Blue);
                bt.SetPixel(xc - x, yc - y, Color.Blue);

                bt.SetPixel(xc + y, yc + x, Color.Blue);
                bt.SetPixel(xc - y, yc + x, Color.Blue);
                bt.SetPixel(xc + y, yc - x, Color.Blue);
                bt.SetPixel(xc - y, yc - x, Color.Blue);
                while (y>=x)
                {
                    DataGridViewRow r = new DataGridViewRow();
                    r.CreateCells(dgv);
                    r.Cells[0].Value = i;
                    r.Cells[1].Value = x;
                    r.Cells[2].Value = y;
                    r.Cells[3].Value = p;
                    x++;
                    if (p < 0)
                        p = p + 4 * x + 6;
                    else
                    {
                        y--;
                        p = p + 4 * (x - y) + 10;
                    }
                    bt.SetPixel(x+xc,yc+y, Color.Blue);
                    bt.SetPixel(-x+xc, yc+y, Color.Blue);
                    bt.SetPixel(x+xc, yc-y, Color.Blue);
                    bt.SetPixel(xc-x,yc-y, Color.Blue);

                    bt.SetPixel(xc+y, yc+x, Color.Blue);
                    bt.SetPixel(xc-y, yc+x, Color.Blue);
                    bt.SetPixel(xc+y, yc-x, Color.Blue);
                    bt.SetPixel(xc-y, yc-x, Color.Blue);
                    r.Cells[4].Value = x;
                    r.Cells[5].Value = y;
                    dgv.Rows.Add(r);
                    i++;
                }
                pic.Image = bt;
        }
        public void drawEllipce(int rx, int ry)
        {
            int x = 0, y = ry;
            int d0 = (ry * ry) - rx * rx * ry + ((1 / 4) * rx * rx);
            int i = 0;
            while ((2 * ry * ry * x) < (2 * rx * rx * y))
            {
                x++;
                if (d0 < 0)
                    d0 = d0 + 2 * ry * ry * x + ry * ry;
                else
                {
                    y--;
                    d0 = d0 + (2 * ry * ry * x) - (2 * rx * rx) * y + ry * ry;
                }
                bt.SetPixel(x + 100, y + 100, Color.Blue);
                bt.SetPixel(-x + 100, y + 100, Color.Blue);
                bt.SetPixel(-x + 100, -y + 100, Color.Blue);
                bt.SetPixel(x + 100, -y + 100, Color.Blue);
                DataGridViewRow r = new DataGridViewRow();
                r.CreateCells(dgv);
                r.Cells[0].Value = i;
                r.Cells[1].Value = d0;
                r.Cells[2].Value = x;
                r.Cells[3].Value = y;
                r.Cells[4].Value = 2 * ry * ry * x;
                r.Cells[5].Value = 2 * rx * rx * y;
                dgv.Rows.Add(r);
                i++;
            }

            double p0 = ry * ry * (x + 0.5) * (x + 0.5) + rx * rx * (y - 1) * (y - 1) - rx * rx * ry * ry;
            while (y > 0)
            {
                y--;
                if (p0 < 0)
                {
                    x++;
                    p0 = p0 + 2 * ry * ry * x - 2 * rx * rx * y + rx * rx;
                }
                else
                {
                    p0 = p0 - 2 * rx * rx * y + rx * rx;
                }
                bt.SetPixel(x + 100, y + 100, Color.Blue);
                bt.SetPixel(-x + 100, y + 100, Color.Blue);
                bt.SetPixel(-x + 100, -y + 100, Color.Blue);
                bt.SetPixel(x + 100, -y + 100, Color.Blue);
                DataGridViewRow r = new DataGridViewRow();
                r.CreateCells(dgv);
                r.Cells[0].Value = i;
                r.Cells[1].Value = p0;
                r.Cells[2].Value = x;
                r.Cells[3].Value = y;
                r.Cells[4].Value = 2 * ry * ry * x;
                r.Cells[5].Value = 2 * rx * rx * y;
                dgv.Rows.Add(r);
                i++;
            }
            pic.Image = bt;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            dgv.Rows.Clear();
                int x1 = int.Parse(px1.Text);
                int y1 = int.Parse(py1.Text);
                int x2 = int.Parse(px2.Text);
                int y2 = int.Parse(py2.Text);

                Bitmap bt = new Bitmap(pic.Width, pic.Height);
            if (comboBox1.Text == "DDA")
            {
                drawline_DDA(x1, y1, x2, y2);
            }
            else if(comboBox1.Text=="Bresenham")
            {
                drawline_Bresenham(x1, y1, x2, y2);
            }
            else if(comboBox1.Text=="Circle")
            {
                int radus = int.Parse(R1.Text);
                drawCircle(radus,100,100);
            }
            else if (comboBox1.Text == "ellipce")
            {
                int rx = int.Parse(R1.Text);
                int ry = int.Parse(R2.Text);
                drawEllipce(rx, ry);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.Text=="Circle")
            {
                dgv.Columns[0].HeaderText = "i";
                dgv.Columns[1].HeaderText = "x";
                dgv.Columns[2].HeaderText = "y";
                dgv.Columns[3].HeaderText = "p";
                dgv.Columns[4].HeaderText = "x+1";
                dgv.Columns[5].HeaderText = "y+1";
                Rad1.Visible = true; R1.Visible = true;
                px1.ReadOnly = true; px2.ReadOnly = true; py1.ReadOnly = true; py2.ReadOnly = true;
            }
            else if(comboBox1.Text=="DDA")
            {
                dgv.Columns[0].HeaderText = "i";
                dgv.Columns[1].HeaderText = "x";
                dgv.Columns[2].HeaderText = "y";
                dgv.Columns[3].HeaderText = "Round x";
                dgv.Columns[4].HeaderText = "Round y";
                dgv.Columns[5].HeaderText = "";
                Rad1.Visible = false; R1.Visible = false; Rad2.Visible = false; R2.Visible = false;
                px1.ReadOnly = false; px2.ReadOnly = false; py1.ReadOnly = false; py2.ReadOnly = false;
            }
            else if(comboBox1.Text=="Bresenham")
            {
                dgv.Columns[0].HeaderText = "i";
                dgv.Columns[1].HeaderText = "x";
                dgv.Columns[2].HeaderText = "y";
                dgv.Columns[3].HeaderText = "p";
                dgv.Columns[4].HeaderText = "x+1";
                dgv.Columns[5].HeaderText = "y+1";
                Rad1.Visible = false; R1.Visible = false; Rad2.Visible = false; R2.Visible = false;
                px1.ReadOnly = false; px2.ReadOnly = false; py1.ReadOnly = false; py2.ReadOnly = false;
            }
            else if(comboBox1.Text=="ellipce")
            {
                dgv.Columns[0].HeaderText = "i";
                dgv.Columns[1].HeaderText = "pi";
                dgv.Columns[2].HeaderText = "xi+1";
                dgv.Columns[3].HeaderText = "yi+1";
                dgv.Columns[4].HeaderText = "2ry2Xi+1";
                dgv.Columns[5].HeaderText = "2rx2Yi+1";
                Rad2.Visible = true; R2.Visible = true;
                Rad1.Visible = true; R1.Visible = true;

            }
        }

        static void SwapNum(ref int a, ref int b)
        {

            int tempswap = a;
            a = b;
            b = tempswap;
        }  
        
        private void pic_Click(object sender, EventArgs e)
        {
            
        }
        
        private void trans_Click(object sender, EventArgs e)
        {
            
            
            if(comboBox2.Text=="translation")
            {

                int incx = int.Parse(angl.Text);
                int incy = int.Parse(trany.Text);
                //Point res=new Point();
                for (int i = 0; i < r; i++)
                {
                    //Point center = new Point(0, 0);
                    Point nPoint = new Point();
                    nPoint.X = rot[i].X + incx;
                    nPoint.Y = rot[i].Y+ incy;
                    rot[i] = nPoint;
                }
                bt = new Bitmap(pic.Width, pic.Height);
                for (int i = 0; i < r - 1; i++)
                {
                    drawline_DDA(rot[i].X, rot[i].Y, rot[i + 1].X, rot[i + 1].Y);
                }
                pic.Image = bt;
            }
            else if(comboBox2.Text=="rotation")
            {
                double ang = double.Parse(angl.Text);

                //Point res=new Point();
                for (int i = 0; i < r; i++)
                {
                    //Point center = new Point(0, 0);
                    Point nPoint = RotatePoint(rot[i], rot[1], ang);
                    rot[i] = nPoint;
                    nr++;
                }
                bt = new Bitmap(pic.Width, pic.Height);
                for (int i = 0; i < r-1; i++) 
                {
                    drawline_DDA(rot[i].X, rot[i].Y, rot[i + 1].X, rot[i + 1].Y);
                }
                //drawline_DDA(rot[0].X, rot[0].Y, rot[r -1].X, rot[r - 1].Y);
                pic.Image = bt;
            }
            else if(comboBox2.Text=="Clipping")
            {
                bt = new Bitmap(pic.Width, pic.Height);
            }
            else if(comboBox2.Text=="scaling")
            {
                {

                    int incx = int.Parse(angl.Text);
                    int incy = int.Parse(trany.Text);
                    //Point res=new Point();
                    for (int i = 0; i < r; i++)
                    {
                        //Point center = new Point(0, 0);
                        Point nPoint = new Point();
                        nPoint.X = rot[i].X * incx;
                        nPoint.Y = rot[i].Y * incy;
                        rot[i] = nPoint;
                    }
                    bt = new Bitmap(pic.Width, pic.Height);
                    for (int i = 0; i < r - 1; i++)
                    {
                        drawline_DDA(rot[i].X, rot[i].Y, rot[i + 1].X, rot[i + 1].Y);
                    }
                    pic.Image = bt;
                }
            }
            else if (comboBox2.Text == "reflection")
            {
                int lix1, lix2, liy1, liy2;
                if (comboBox3.Text == "Y")
                {
                     lix1 = mnx - 1; liy1 = 0;
                     lix2 = mnx - 1; liy2 = pic.Height;
                }
                else
                {
                     lix1 = 0; liy1 = mxy + 1;
                     lix2 = pic.Width; liy2 = mxy + 1;
                }
                for (int i = 0; i < r; i++) 
                {
                    Point nPoint = mirror(rot[i], lix1, liy1, lix2, liy2);
                    rot[i] = nPoint;
                }
                bt = new Bitmap(pic.Width, pic.Height);
                for (int i = 0; i < r - 1; i++)
                {
                    drawline_DDA(rot[i].X, rot[i].Y, rot[i + 1].X, rot[i + 1].Y);
                }
                pic.Image = bt;
                //drawline_DDA(newpoint[0].X, newpoint[0].Y, newpoint[nr - 1].X, newpoint[nr - 1].Y);
            }
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox2.Text=="Clipping")
            {
                MessageBox.Show("Select the area you want to clip");
                label9.Visible = false;
                angl.Visible = false;
                trany.Visible = false;
                label10.Visible = false;
                comboBox3.Visible = false;
            }
            else if (comboBox2.Text == "rotation")
            {
                label9.Text = "angle";
                label9.Visible = true;
                angl.Visible = true;
                trany.Visible = false;
                label10.Visible = false;
                comboBox3.Visible = false;
            }
            else if (comboBox2.Text == "translation")
            {
                label9.Text = "X";
                label9.Visible = true;
                angl.Visible = true;
                trany.Visible = true;
                label10.Visible = true;
                comboBox3.Visible = false;
            }
            else if(comboBox2.Text=="scaling")
            {
                label9.Text = "X";
                label9.Visible = true;
                angl.Visible = true;
                trany.Visible = true;
                label10.Visible = true;
                comboBox3.Visible = false;
            }
            else if(comboBox2.Text=="reflection")
            {
                label9.Visible = false;
                angl.Visible = false;
                trany.Visible = false;
                label10.Visible = false;
                comboBox3.Visible = true;
            }
            else if (comboBox2.Text == "Filling")
            {
                label9.Visible = false;
                angl.Visible = false;
                trany.Visible = false;
                label10.Visible = false;
                comboBox3.Visible = false;
                //drawline_DDA(rot[0].X, rot[0].Y, rot[r - 1].X, rot[r - 1].Y);
            }
        }
        private void clipping(int x1,int y1,int x2,int y2)
        {
            if (y1 > y2)
            {
                int t = y1; y1 = y2; y2 = t;
            }
            if (x1 > x2)
            {
                int t = x1; x1 = x2; x2 = t;
            }

            for(int i=0;i<pic.Width;i++)
            {
                for(int j=0;j<pic.Height;j++)
                {
                    if (x1 <= i && i <= x2 && y1 <= j && j <= y2)
                        continue;
                    bt.SetPixel(i, j, Color.White);
                }
            }
            pic.Image = bt;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bt = new Bitmap(pic.Width, pic.Height);
            matrix1 = new int[3];
            matrix2 = new int[3, 3];
            line = new Point[10000];
            clp = new Point[10000];
            shp = 0;
            mxy = 0;
            mnx = 1000;
            newpoint = new Point[100];
            nr = 0;
            prevx = -1; prevy = -1;
            nx = -1; ny = -1; cr = 0;
            rot = new Point[100];
            r = 0;
            pic.Image = bt;
            comboBox2.Text = "";
            dgv.Rows.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();
            cl = cd.Color;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            prevx = prevy = -1;
            nx = ny = -1;
        }

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right)
            {
                prevx = prevy = -1;
                nx = ny = -1;
                return;
            }
            MouseEventArgs eM = (MouseEventArgs)e;

            if (comboBox1.Text == "DDA" && comboBox2.Text == "")
            {
                if (prevx != -1)
                {
                    x = eM.X;
                    y = eM.Y;
                    label7.Text = x.ToString();
                    label8.Text = y.ToString();
                    rot[r].X = x; rot[r].Y = y;
                    r++;
                    drawline_DDA(x, y, prevx, prevy);
                    pic.Image = bt;
                    SwapNum(ref x, ref prevx);
                    SwapNum(ref y, ref prevy);
                }
                else
                {
                    x = eM.X;
                    y = eM.Y;
                    rot[r].X = x; rot[r].Y = y;
                    r++;
                    prevx = x; prevy = y;
                }
            }
            else if (comboBox1.Text == "Circle" && comboBox2.Text == "")
            {
                if (nx != -1)
                {
                    x = eM.X;
                    y = eM.Y;
                    int rad = (int)(Math.Sqrt((Math.Abs(nx - x) * Math.Abs(nx - x)) + (Math.Abs(ny - y) * Math.Abs(ny - y))));
                    //drawline_DDA(x, y, nx, ny);
                    drawCircle(rad, x, y);
                }
                else
                {
                    x = eM.X;
                    y = eM.Y;
                    nx = x; ny = y;
                }
            }
            if (comboBox2.Text == "Clipping")
            {
                if (shp == 0)
                {
                    shx1 = eM.X;
                    shy1 = eM.Y;
                    shp++;
                }
                else if (shp == 1)
                {
                    shx2 = eM.X;
                    shy2 = eM.Y;
                    clipping(shx1, shy1, shx2, shy2);
                    shp = 0;
                }

            }
            else if (comboBox2.Text == "Filling" && comboBox1.Text == "DDA")
            {
                fill(eM.X, eM.Y);
                pic.Image = bt;

            }
        }


    }
}
