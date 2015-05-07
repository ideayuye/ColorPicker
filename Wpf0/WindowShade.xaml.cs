using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ColorPicker
{
    /// <summary>
    /// WindowShade.xaml 的交互逻辑
    /// </summary>
    public partial class WindowShade : Window
    {
        public WindowShade()
        {
            InitializeComponent();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 图片放大
        /// </summary>
        /// <param name="originBmp">原始图片</param>
        /// <param name="iSize">放大系数</param>
        /// <returns>放大后的图片</returns>
        public Bitmap PicSized(Bitmap originBmp, double iSize)
        {
            int w = Convert.ToInt32(originBmp.Width * iSize);
            int h = Convert.ToInt32(originBmp.Height * iSize);
            Bitmap resizedBmp = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(resizedBmp);
            //设置高质量插值法  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //消除锯齿
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawImage(originBmp, new System.Drawing.Rectangle(0, 0, w, h),
                new System.Drawing.Rectangle(0, 0, originBmp.Width, originBmp.Height), GraphicsUnit.Pixel);
            g.Dispose();
            return resizedBmp;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Clip = P1.Data;
        }
    }
}
