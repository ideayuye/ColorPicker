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
using System.Threading;

namespace ColorPicker
{
    /// <summary>
    /// Window2.xaml 的交互逻辑
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            IsShowScreenWnd = true;
            this.Hide();
            Thread.Sleep(100);
            ScreenWnd sWnd = new ScreenWnd();
            sWnd.Closed += new EventHandler(sWnd_Closed);
            sWnd.ColorSelected += new EventHandler(sWnd_ColorSelected);
            System.Drawing.Bitmap img = Helper.GetScreenSnapshot();
            BitmapSource bmpSrc = Helper.ToBitmapSource(img);
            sWnd.BmpSScreen = bmpSrc;
            sWnd.bmpScreen = img;
            sWnd.CanvaBg = new ImageBrush(bmpSrc);
            sWnd.Show();
        }

        void sWnd_ColorSelected(object sender, EventArgs e)
        {
            ScreenWnd sWnd = (ScreenWnd ) sender ;
            txt0x.Text = sWnd.Selected0xColor;
            txtRGB.Text = sWnd.SelectedRGBColor; 
        }

        void sWnd_Closed(object sender, EventArgs e)
        {
            IsShowScreenWnd = false;
            this.Show();
        }

        private bool IsShowScreenWnd = false;

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsShowScreenWnd)
                return;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void textBlock1_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            MessageBox.Show(" Fuck !What are you doing?");
        }

        private void txt0x_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetDataObject(txt0x.Text.Substring(txt0x.Text.IndexOf(':') + 1));
        }

        private void txtRGB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string selRgb = txtRGB.Text.Substring(txtRGB.Text.IndexOf('(') + 1);
            selRgb = selRgb.Substring(0, selRgb.Length - 1);
            Clipboard.SetDataObject(selRgb);
        }
    }
}
