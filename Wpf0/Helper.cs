using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows;

namespace ColorPicker
{
    public static class Helper
    {
        /// <summary>
        /// 截屏
        /// </summary>
        /// <returns></returns>
        public static Bitmap GetScreenSnapshot()
        {
            Rectangle rc = SystemInformation.VirtualScreen;
            var bitmap = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }
        /// <summary>
        /// 截取屏幕的指定区域
        /// </summary>
        /// <param name="centerP"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Bitmap GetScreenSnapshot(System.Drawing.Point centerP,int size)
        {
            Rectangle rc = new Rectangle(
                new System.Drawing.Point(0,0), 
                new System.Drawing.Size(size, size));
            var bitmap = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(rc.X, rc.Y, (int)(centerP.X - size * 0.5), 
                    (int)(centerP.Y - size * 0.5), rc.Size, CopyPixelOperation.SourceCopy);
            }
            return bitmap;
        }

        /// <summary>
        /// 获取一张图片中的一部分 （.png图片是可行的，其余格式暂时还没有测试过）
        /// </summary>
        /// <param name="ImgUri">图片路径</param>
        /// <param name="XCoordinate">要截取部分的X坐标</param>
        /// <param name="YCoordinate">要截取部分的Y坐标</param>
        /// <param name="Width">截取的宽度</param>
        /// <param name="Height">截取的高度</param>
        /// <returns></returns>
        public static BitmapSource GetPartImage(string ImgUri, int XCoordinate, int YCoordinate, int Width, int Height)
        {
            return new CroppedBitmap(BitmapFrame.Create(new Uri(ImgUri, UriKind.Relative)), new Int32Rect(XCoordinate, YCoordinate, Width, Height));
        }

        public static BitmapSource GetPartImage(BitmapSource bmpSrc, int XCoordinate, int YCoordinate, int Width, int Height)
        {
            return new CroppedBitmap(bmpSrc, new Int32Rect(XCoordinate, YCoordinate, Width, Height));
        }

        public static BitmapSource ToBitmapSource(Bitmap bmp)
        {
            BitmapSource returnSource;
            try
            {
                returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                returnSource = null;
            }
            return returnSource;
        }
    }
}
