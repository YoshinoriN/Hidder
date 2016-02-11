using System.Windows;

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
