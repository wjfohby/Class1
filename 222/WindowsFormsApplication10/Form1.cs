using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using tessnet2;
using mshtml;
//using Captcha;

namespace WindowsFormsApplication10
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        private string pathname = string.Empty;
        private Bitmap img;
        private Color col;
       
        private int T = 150;
        private double BlackPoint;
        private int area;
        private System.Collections.Generic.List<int>
        XList = new List<int> { };
        private System.Collections.Generic.List<int>
        YList = new List<int> { };
        private bool isWhiteLine;
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;//choice all files
            open.InitialDirectory = ".";
            //file.Filter="所有文件(*.*)|*.*";
            open.Filter = "图片文件(*.jpg,*.gif,*.bmp,*.png)|*.jpg;*.gif;*.bmp;*.png";
            /*f (file.ShowDialog() == DialogResult.OK)
            {
                string path = file.FileName;
                
            }*/
            open.ShowDialog();
            if (open.FileName != string.Empty)
            {
                try
                {
                    pathname = open.FileName;
                    this.pictureBox1.Load(pathname);
                    // img = new Bitmap(pathname);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

   
        public Bitmap GetGray()//灰度化
        {
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    /*Color col = bit.GetPixel(i, j);
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bit.SetPixel(i, j, newColor);*/
                    var color = img.GetPixel(i, j);//隐式类型

                    var gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
    
                    //重新设置当前的像素点
                    Color newcol = Color.FromArgb(gray, gray, gray);
                    img.SetPixel(i, j, newcol);
                }
            }

            return img;
        }

         

        public  Bitmap GrayReverse()
        {
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = img.GetPixel(i, j);

                    Color newColor = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
                    img.SetPixel(i, j, newColor);
                }
            }
            return img;
        }

       

        public Bitmap GetBinaryZaTion()//二值化
        {
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                      col = img.GetPixel(x, y);
                      //平均值法求灰度值  
                      int gray = (col.R + col.G + col.B) / 3;
                      //大于阙值 黑色  
                      if (gray > T)
                      {
                          img.SetPixel(x, y, Color.FromArgb(col.A, 0, 0, 0));
                          //黑色点个数自加  
                          BlackPoint++;
                      }
                      //大于阙值 白色  
                      else
                      {
                          img.SetPixel(x, y, Color.FromArgb(col.A, 255, 255, 255));
                      }
                     
                }
            }
            return img;
        }


        public bool IsNeedInverseColor()
        {
            if ((BlackPoint * 1.0 / (img.Width * img.Height)) > 0.5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Bitmap InverseColor()
        {
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    col = img.GetPixel(x, y);
                    img.SetPixel(x, y, Color.FromArgb(col.A, 255 - col.R, 255 - col.G, 255 - col.B));
                }
            }
            return img;
        }


        private int dgGrayValue;
        private int MaxNearPoints;
        public Bitmap Noise()
        {
            Color piexl;
            int nearDots = 0;
           // int XSpan, YSpan, tmpX, tmpY;
            //逐点判断
            for (int i = 0; i < img.Width; i++)
                for (int j = 0; j < img.Height; j++)
                {
                    piexl = img.GetPixel(i, j);
                    if (piexl.R < dgGrayValue)
                    {
                        nearDots = 0;
                        //判断周围8个点是否全为空
                        if (i == 0 || i == img.Width - 1 || j == 0 || j == img.Height - 1)  //边框全去掉
                        {
                            img.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                        }
                        else
                        {
                            if (img.GetPixel(i - 1, j - 1).R < dgGrayValue) nearDots++;
                            if (img.GetPixel(i, j - 1).R < dgGrayValue) nearDots++;
                            if (img.GetPixel(i + 1, j - 1).R < dgGrayValue) nearDots++;
                            if (img.GetPixel(i - 1, j).R < dgGrayValue) nearDots++;
                            if (img.GetPixel(i + 1, j).R < dgGrayValue) nearDots++;
                            if (img.GetPixel(i - 1, j + 1).R < dgGrayValue) nearDots++;
                            if (img.GetPixel(i, j + 1).R < dgGrayValue) nearDots++;
                            if (img.GetPixel(i + 1, j + 1).R < dgGrayValue) nearDots++;
                        }

                        if (nearDots < MaxNearPoints)
                            img.SetPixel(i, j, Color.FromArgb(255, 255, 255));   //去掉单点 && 粗细小3邻边点
                    }
                    else  //背景
                        img.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                }
            return img;
        }


        public Bitmap ClearNoise()//去噪
        {
            int x, y;
            byte[] p = new byte[9];//min处理窗口3*3
            byte temp;
            int i, j;
            for (y = 1; y < img.Height - 1; y++)
            {
                for (x = 1; x < img.Width - 1; x++)
                {
                    p[0] = img.GetPixel(x - 1, y - 1).R;
                    p[1] = img.GetPixel(x, y - 1).R;
                    p[2] = img.GetPixel(x + 1, y - 1).R;
                    p[3] = img.GetPixel(x - 1, y).R;
                    p[4] = img.GetPixel(x, y).R;
                    p[5] = img.GetPixel(x + 1, y).R;
                    p[6] = img.GetPixel(x - 1, y + 1).R;
                    p[7] = img.GetPixel(x, y + 1).R;
                    p[8] = img.GetPixel(x + 1, y + 1).R;
                    for (j = 0; j < 5; j++)
                    {
                        for (i = j + 1; i < 9; i++)
                        {
                            if (p[j] > p[i])
                            {
                                temp = p[j];
                                p[j] = p[i];
                                p[i] = temp;
                            }
                        }
                    }
                    img.SetPixel(x, y, Color.FromArgb(p[4], p[4], p[4]));
                }
            }
            return img;
        }


      
        public Bitmap CutImg()
        {
            //Y轴分割  
            CutY();
            //区域个数  
            area = 0;
            if (XList.Count > 1)
            {
                //x起始值  
                int start = XList[0];
                //x结束值  
                int end = XList[XList.Count - 1];
                //x索引  
                int index = 0;
                while (start != end)
                {
                    //区域宽度  
                    int __w = start;
                    //区域个数自加  
                    area++;
                    while (XList.Contains(__w) && index < XList.Count)
                    {
                        //区域宽度自加  
                        __w++;
                        //x索引自加  
                        index++;
                    }
                    //区域X轴坐标  
                    int x = start;
                    //区域Y轴坐标  
                    int y = 0;
                    //区域宽度  
                    int width = __w - start;
                    //区域高度  
                    int height = img.Height;
                    /* 
                     * X轴分割当前区域 
                     */
                    CutX(img.Clone(new Rectangle(x, y, width, height), img.PixelFormat));
                    if (YList.Count > 1 && YList.Count != img.Height)
                    {
                        int y1 = YList[0];
                        int y2 = YList[YList.Count - 1];
                        if (y1 != 1)
                        {
                            y = y1 - 1;
                        }
                        height = y2 - y1 + 1;
                    }
                    //GDI+绘图对象  
                    Graphics g = Graphics.FromImage(img);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //画出验证码区域  
                    g.DrawRectangle(new Pen(Brushes.Green), new Rectangle(x, y, width, height));
                    g.Dispose();
                    //起始值指向下一组  
                    if (index < XList.Count)
                    {
                        start = XList[index];
                    }
                    else
                    {
                        start = end;
                    }

                }
            }
            return img;
        }


     //   #region Y轴字符分割图片
        /// <summary>  
        /// 得到Y轴分割点  
        /// 判断每一竖行是否有黑色  
        /// 有则添加  
        /// </summary>  
        /// <param name="img">要验证的图片</param>  
        private void CutY()
        {
            XList.Clear();
            for (int x = 0; x < img.Width; x++)
            {
                isWhiteLine = false;
                for (int y = 0; y < img.Height; y++)
                {
                    col = img.GetPixel(x, y);
                    if (col.R == 255)
                    {
                        isWhiteLine = true;
                    }
                    else
                    {
                        isWhiteLine = false;
                        break;
                    }
                }
                if (!isWhiteLine)
                {
                    XList.Add(x);
                }
            }
        }
       

        //#region X轴字符分割图片
        /// <summary>  
        /// 得到X轴分割点  
        /// 判断每一横行是否有黑色  
        /// 有则添加  
        /// </summary>  
        /// <param name="tempImg">临时区域</param>  
        private void CutX(Bitmap tempImg)
        {
            YList.Clear();
            for (int x = 0; x < tempImg.Height; x++)
            {
                isWhiteLine = false;
                for (int y = 0; y < tempImg.Width; y++)
                {
                    col = tempImg.GetPixel(y, x);
                    if (col.R == 255)
                    {
                        isWhiteLine = true;
                    }
                    else
                    {
                        isWhiteLine = false;
                        break;
                    }
                }
                if (!isWhiteLine)
                {
                    YList.Add(x);
                }
            }
            tempImg.Dispose();
        }



        private double PixlPercent(Bitmap tempimg)
        {
            int temp = 0;
            int w_h = tempimg.Width * tempimg.Height;
            for (int x = 0; x < tempimg.Width; x++)
            {
                for (int y = 0; y < tempimg.Height; y++)
                {
                    col = tempimg.GetPixel(x, y);
                    if (col.R == 0)
                    {
                        temp++;
                    }
                }
            }
            tempimg.Dispose();
            double result = temp * 1.0 / w_h;
            result = result.ToString().Length > 3 ? Convert.ToDouble(result.ToString().Substring(0, 3)) : result;
            return result;
        }

      //  private Image Getimage()
    //    {
    //        Bitmap bmp = (Bitmap)pictureBox5.Image;
    //        return bmp;
//
    //    }






        private void button2_Click(object sender, EventArgs e)
        {
            //Image imag;
            //img=Image.FromHbitmap(img.GetHbitmap());//获得句柄
            img = new Bitmap(pictureBox1.Image);

            pictureBox2.Image = GetGray();//灰度化
            img = new Bitmap(pictureBox2.Image);

            pictureBox3.Image = GetBinaryZaTion();//二值化
            img = new Bitmap(pictureBox3.Image);

            //pictureBox4.Image = GetBinaryZaTion();
           // img = new Bitmap(pictureBox4.Image);
            IsNeedInverseColor();
           // pictureBox5.Image = ClearNoise();
            pictureBox4.Image = InverseColor();//反色
            img = new Bitmap(pictureBox4.Image);

            pictureBox5.Image = ClearNoise();//去噪
            img = new Bitmap(pictureBox5.Image);

            pictureBox6.Image = CutImg();//分割
            img = new Bitmap(pictureBox6.Image);
            //textBox1.Text = Convert.ToString(PixlPercent(img));
            //UseOrc(pictureBox7.Image);

            //return Security_Code;



            /*tessnet2.Tesseract ocr = new tessnet2.Tesseract();//声明一个OCR类
            ocr.SetVariable("tessedit_char_whitelist", "0123456789"); //设置识别变量，当前只能识别数字。
            ocr.Init(Application.StartupPath + @"\\tmpe", "eng", true); //应用当前语言包。注，Tessnet2是支持多国语的。语言包下载链接：http://code.google.com/p/tesseract-ocr/downloads/list
            List<tessnet2.Word> result = ocr.DoOCR(img, Rectangle.Empty);//执行识别操作
            string code = result[0].Text;
            textBox1.Text = code;*/
            
             
        }
        //string Security_Code;
        

        private void button3_Click(object sender, EventArgs e)
        {
            //img = new Bitmap(pictureBox6.Image);
            //pictureBox6.Image = img;
            Bitmap bmp = (Bitmap)pictureBox5.Image;
         
            tessnet2.Tesseract ocr = new tessnet2.Tesseract();//声明一个OCR类
           // ocr.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"); 
            ocr.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            ocr.Init(Application.StartupPath + @"\tessdata", "eng", false); //应用当前语言包。
            List<tessnet2.Word> result = ocr.DoOCR(img,Rectangle.Empty);
          //  List<tessnet2.Word> result = ocr.DoOCR(img, Rectangle.Empty);//执行识别操作
            string code = result[0].Text;
            textBox1.Text = code;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }
    }
}