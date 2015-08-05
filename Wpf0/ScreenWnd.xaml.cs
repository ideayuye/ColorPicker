using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ColorPicker
{
    /// <summary>
    /// ScreenWnd.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenWnd : Window
    {
        public ScreenWnd()
        {
            InitializeComponent();
        }

        public Brush CanvaBg
        {
            get { return canvas1.Background; }
            set
            {
                canvas1.Background = (ImageBrush)value;
            }
        }
        /// <summary>
        /// 屏幕截图
        /// </summary>
        public BitmapSource BmpSScreen
        {
            get;
            set;
        }
        /// <summary>
        /// 选中的颜色
        /// </summary>
        public string SelectedRGBColor
        {
            get;
            set;
        }
        /// <summary>
        /// 选中的颜色
        /// </summary>
        public string Selected0xColor
        {
            get;
            set;
        }
        public event EventHandler ColorSelected;
        private void OnColorSelected(object sender, EventArgs e)
        {
            if (ColorSelected != null)
                ColorSelected(sender, e);
        }

        public System.Drawing.Bitmap bmpScreen;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        //鼠标移动 拾取当前区域图片 放大
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isLocked) return;   
            double mouseX = e.GetPosition(canvas1).X;
            double mouseY = e.GetPosition(canvas1).Y;

            Canvas.SetLeft(borderImg, mouseX + 20);
            Canvas.SetTop(borderImg, mouseY + 20);

            Canvas.SetTop(border3, Canvas.GetTop(borderImg) + borderImg.ActualHeight);
            double b3Height = canvas1.ActualHeight - Canvas.GetTop(border3);
            if (b3Height > 0)
                border3.Height = b3Height;
            else
                border3.Height = 0;

            Canvas.SetLeft(border4, Canvas.GetLeft(borderImg) + borderImg.ActualWidth);
            double b4Height = canvas1.ActualWidth - Canvas.GetLeft(border4);
            if (b4Height > 0)
                border4.Width = b4Height;
            else
                border4.Height = 0;

            int imgPartX = (int)mouseX - 15;
            int imgPartY = (int)mouseY - 12;
            if (imgPartX < 0) imgPartX = 0;
            if (imgPartY < 0) imgPartY = 0;
            image1.Source = Helper.GetPartImage(BmpSScreen, imgPartX, imgPartY, 30, 27);

            System.Drawing.Color color = bmpScreen.GetPixel((int)mouseX, (int)mouseY);
            tRgb.Text = "RGB:(" + ColorToRGB(color)+")";
            t0x.Text = "0x:" + ColorTo0X(color);
        }

        private string  ColorToRGB(System.Drawing.Color color)
        {
            Int32 c = color.ToArgb();
            int r = (c >> 16) & 0xFF;//转换R
            int g = (c >> 8) & 0xFF;//转换G
            int b = c & 0xFF;//转换B
            return r.ToString() + "," + g.ToString() + "," + b.ToString();
        }


        private string getRepeatChar(Match m)
        {
            var s = m.ToString();
            var s1 = "";
            if (s.Length == 2)
            {
                s1 = s[0].ToString();
            }
            return s1;
        }

        private string ColorTo0X(System.Drawing.Color color)
        {
            Int32 c = color.ToArgb();
            var clrStr = "#" + (c & 0x00FFFFFF).ToString("X6");
            Regex r = new Regex(@"^#([0-9a-zA-Z])\1([0-9a-zA-Z])\2([0-9a-zA-Z])\3$");
            if (r.IsMatch(clrStr))
            {
                Regex replace = new Regex(@"([0-9a-zA-Z])\1");
                clrStr = replace.Replace(clrStr, new MatchEvaluator(getRepeatChar));
            }
            return clrStr;
        }

        bool isLocked = false;
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {   //把颜色值 写入剪切板
            if (isLocked) return;
            double mouseX = e.GetPosition(canvas1).X;
            double mouseY = e.GetPosition(canvas1).Y;
            System.Drawing.Color color = bmpScreen.GetPixel((int)mouseX, (int)mouseY);
            string selrgbClr = ColorToRGB(color);
            string sel0xClr = ColorTo0X(color);
            Clipboard.SetDataObject(sel0xClr);
            tRgb.Text = "RGB:(" + selrgbClr  + ")";
            t0x.Text = "0x:" + sel0xClr ;
            Selected0xColor = t0x.Text;
            SelectedRGBColor = tRgb.Text;
            isLocked = true;
            OnColorSelected(this, e);
            this.Close();
        }

        private void tRgb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string selRgb = tRgb.Text.Substring(tRgb.Text.IndexOf('(')+1);
            selRgb = selRgb.Substring(0, selRgb.Length - 1);
            Clipboard.SetDataObject(selRgb);
            SelectedRGBColor = selRgb;
            OnColorSelected(this, e);
            this.Close();
        }

        private void t0x_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string sel0xColor = t0x.Text.Substring(t0x.Text.IndexOf(':')+1);
            Clipboard.SetDataObject(sel0xColor);
            SelectedRGBColor = sel0xColor;
            OnColorSelected(this, e);
            this.Close();
        }

    }
}
