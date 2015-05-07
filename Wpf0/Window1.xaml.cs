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

namespace ColorPicker
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            BindCBX();
        }

        private void BindCBX()
        {
            comboBox1.Items.Add(Cursors.AppStarting);
            comboBox1.Items.Add(Cursors.Arrow);
            comboBox1.Items.Add(Cursors.ArrowCD);
            comboBox1.Items.Add(Cursors.Cross);
            comboBox1.Items.Add(Cursors.Hand);
            comboBox1.Items.Add(Cursors.Help);
            comboBox1.Items.Add(Cursors.IBeam);
            comboBox1.Items.Add(Cursors.No);
            comboBox1.Items.Add(Cursors.None);
            comboBox1.Items.Add(Cursors.Pen);
            comboBox1.Items.Add(Cursors.ScrollAll);
            comboBox1.Items.Add(Cursors.ScrollE);
            comboBox1.Items.Add(Cursors.ScrollN);
            comboBox1.Items.Add(Cursors.ScrollNE);
            comboBox1.Items.Add(Cursors.ScrollNW);
            comboBox1.Items.Add(Cursors.ScrollS);
            comboBox1.Items.Add(Cursors.ScrollSE);
            comboBox1.Items.Add(Cursors.ScrollSW);
            comboBox1.Items.Add(Cursors.ScrollW);
            comboBox1.Items.Add(Cursors.ScrollWE);
            comboBox1.Items.Add(Cursors.SizeAll);
            comboBox1.Items.Add(Cursors.SizeNESW);
            comboBox1.Items.Add(Cursors.SizeNS);
            comboBox1.Items.Add(Cursors.SizeNWSE);
            comboBox1.Items.Add(Cursors.SizeWE);
            comboBox1.Items.Add(Cursors.UpArrow);
            comboBox1.Items.Add(Cursors.Wait);
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Cursor = (Cursor)comboBox1.SelectedItem;
        }



    }
}
