using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ColorPicker
{ 
    public partial class MainWindow : Window {
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS {
            public int cxLeftWidth;      // width of left border that retains its size
            public int cxRightWidth;     // width of right border that retains its size
            public int cyTopHeight;      // height of top border that retains its size
            public int cyBottomHeight;   // height of bottom border that retains its size
        };
        [StructLayout(LayoutKind.Sequential)]
        public class POINT {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd,ref MARGINS pMarInset);
        [DllImport("kernel32.dll"/*, CharSet = CharSet.Auto,CallingConvention = CallingConvention.StdCall*/)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn,IntPtr hInstance, int threadId);
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode,IntPtr wParam, IntPtr lParam);

        public const int WH_MOUSE = 7;
        public const int WH_MOUSE_LL = 14; //鼠标常量 
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;
        public bool picking = false;

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        static int hHook = 0;//定义钩子句柄
        HookProc MouseHookProcedure;//定义钩子处理函数

        DispatcherTimer timer = new DispatcherTimer();
        int ID = 0, count = 0;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public MainWindow() {
            InitializeComponent();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            try {
                IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

                MARGINS margins = new MARGINS();
                margins.cxLeftWidth = -1;// Convert.ToInt32(5 * (DesktopDpiX / 96));
                margins.cxRightWidth = -1;//Convert.ToInt32(5 * (DesktopDpiX / 96));
                margins.cyTopHeight = -1;// Convert.ToInt32(5 * (DesktopDpiX / 96));
                margins.cyBottomHeight = -1;// Convert.ToInt32(5 * (DesktopDpiX / 96));

                int hr = DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                if (hr < 0) {
                    Application.Current.MainWindow.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
                }
            }
            catch (DllNotFoundException) {  // If not Vista, paint background white.
                Application.Current.MainWindow.Background = new SolidColorBrush(Color.FromArgb(255,240,240,240)) ;
            }
            this.Background = Brushes.Transparent;
            rectangle.Fill = Brushes.White;
            R.Text = "255";
            G.Text = "255";
            B.Text = "255";
            colorBox.Text = "#FFFFFF";
            timer.Tick += new EventHandler(OnTimer);
            timer.Interval = TimeSpan.FromSeconds(0.4);
            //注册热键
            HotKey hotKey = new HotKey(this, HotKey.KeyFlags.MOD_CONTROL, System.Windows.Forms.Keys.X);
            hotKey.OnHotKey += new HotKey.OnHotKeyEventHandler(hotKey_OnHotKey);
        }

        void hotKey_OnHotKey()
        {
            //MessageBox.Show("事件注册成功");
            SetHook();
        }

        private void rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            SetHook();
        }

        private void SetHook()
        {
            picking = true;
            if (hHook == 0)
            {
                //显示失去颜色窗体
                //DXWindow1 dxwindow1 = new DXWindow1();
                ScreenWnd sWnd = new ScreenWnd();
                System.Drawing.Bitmap img = Helper.GetScreenSnapshot();
                BitmapSource bmpSrc = Helper.ToBitmapSource(img);
                sWnd.BmpSScreen = bmpSrc;
                sWnd.bmpScreen = img;
                sWnd.CanvaBg = new ImageBrush(bmpSrc);
                sWnd.Show();
                //this.Cursor = new Cursor(System.Windows.Forms.Application.StartupPath + @"\..\..\Images\b_2.ani");
                MouseHookProcedure = new HookProc(this.MouseHookProc); 
                Process curProcess = Process.GetCurrentProcess();
                ProcessModule curModule = curProcess.MainModule;
                hHook = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProcedure, GetModuleHandle(curModule.ModuleName), 0);
                if (hHook == 0)
                {
                    //this.Cursor = Cursors.Arrow;
                    MessageBox.Show("SetWindowsHookEx Failed");
                    return;
                }
            }
        }
        private void Window_Closed(object sender, EventArgs e) {
            bool retMouse = true;
            if (hHook != 0) {
                retMouse = UnhookWindowsHookEx(hHook);
                hHook = 0;
            }
        }

        public int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam) {
            MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
            if (nCode < 0) {
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else if (picking == true && wParam.ToInt32() == WM_LBUTTONDOWN) {
                System.Drawing.Point p = new System.Drawing.Point(MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y);//取置顶点坐标
                System.Drawing.Bitmap img = new System.Drawing.Bitmap(1, 1);
                System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(img);//这里的g充当img对象的画笔  
                gr.CopyFromScreen(p.X,p.Y,0,0,new System.Drawing.Size(1,1));
                System.Drawing.Color color = img.GetPixel(0, 0);
                Int32 c = color.ToArgb();
                colorBox.Text = "#" + (c & 0x00FFFFFF).ToString("X6");
                int r = (c >> 16) & 0xFF;//转换R
                int g = (c >> 8) & 0xFF;//转换G
                int b = c & 0xFF;//转换B
                R.Text = r.ToString();
                G.Text = g.ToString();
                B.Text = b.ToString();
                string rgbValue = r.ToString() + "," + g.ToString() +","+ b.ToString();
                rectangle.Fill = new SolidColorBrush(Color.FromRgb((byte)r,(byte)g,(byte)b));
                picking = false;
                if (this.WindowState != System.Windows.WindowState.Normal)
                {
                    this.Show();
                    this.WindowState = System.Windows.WindowState.Normal;
                }
                if(checkBoxRGB.IsChecked!= null &&(bool)checkBoxRGB.IsChecked)
                    Clipboard.SetDataObject(rgbValue);
                if (checkBox0x.IsChecked != null && (bool)checkBox0x.IsChecked)
                    Clipboard.SetDataObject(colorBox.Text);
            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        private void R_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) updateColor();
        }

        private void G_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) updateColor();
        }

        private void B_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) updateColor();
        }

        private void updateColor() {
            byte r, g, b;
            try {
                r = byte.Parse(R.Text);
                g = byte.Parse(G.Text);
                b = byte.Parse(B.Text);
            }
            catch {
                r = 255; g = 255; b = 255;
            }
            rectangle.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
            colorBox.Text="#"+r.ToString("X2")+g.ToString("X2")+b.ToString("X2");
        }
        
        private void labelRUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ID = 1; count = 0;
            Decide();
            timer.Start();
        }
        private void labelRDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ID = 2; count = 0;
            Decide();
            timer.Start();
        }
        private void labelGUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ID = 3; count = 0;
            Decide();
            timer.Start();
        }
        private void labelGDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ID = 4; count = 0;
            Decide();
            timer.Start();
        }
        private void labelBup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ID = 5; count = 0;
            Decide();
            timer.Start();
        }
        private void labelBDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ID = 6; count = 0;
            Decide();
            timer.Start();
        }

        private void labelRUp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            timer.Stop();
            timer.Interval = TimeSpan.FromSeconds(0.4);
        }
        private void labelRDown_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            timer.Stop();
            timer.Interval = TimeSpan.FromSeconds(0.4);
        }
        private void labelGUp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            timer.Stop();
            timer.Interval = TimeSpan.FromSeconds(0.4);
        }
        private void labelGDown_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            timer.Stop();
            timer.Interval = TimeSpan.FromSeconds(0.4);
        }
        private void labelBup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            timer.Stop();
            timer.Interval = TimeSpan.FromSeconds(0.4);
        }
        private void labelBDown_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            timer.Stop();
            timer.Interval = TimeSpan.FromSeconds(0.4);
        }
        
        void Decide() {
            switch (ID) {
                case 1: Change(ref R, 1); break;
                case 2: Change(ref R, -1); break;
                case 3: Change(ref G, 1); break;
                case 4: Change(ref G, -1); break;
                case 5: Change(ref B, 1); break;
                case 6: Change(ref B, -1); break;
                default: return;
            }
        }
        void Change(ref TextBox t, int flg) {
            byte tmp;
            try {
                tmp = byte.Parse(t.Text);
            }
            catch {
                if (flg == 1) tmp = 255;
                else tmp = 0;
            }
            t.Text = ((byte)(tmp + flg)).ToString();
            updateColor();
        }
        void OnTimer(object sender, EventArgs e) {
            Decide();
            count++;
            if (count >= 2) timer.Interval = TimeSpan.FromSeconds(0.1);
            else timer.Interval = TimeSpan.FromSeconds(0.4);
        }

        private void checkBoxRGB_Checked(object sender, RoutedEventArgs e)
        {
            if(checkBox0x != null)
                checkBox0x.IsChecked = false;
        }

        private void checkBox0x_Checked(object sender, RoutedEventArgs e)
        {
            if(checkBoxRGB!=null)
                checkBoxRGB.IsChecked = false;
        }
    }
}
