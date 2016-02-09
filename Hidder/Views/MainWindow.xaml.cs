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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hidder.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //ウインドウ全体でドラッグ可能にする。
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
        }

        //以下はすべてめんどくさいのでコードビハインドでやる。
        //そのうちViewModelに移す
        private void Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            //this.WindowState = WindowState.Hidden;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //var settingsWindow = new Settings(this.Top, this.Left);
            //settingsWindow.Show();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Copyright YoshinoriN 2016 All Right Reserved.");
        }
    }
}
