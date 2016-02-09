using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Hidder.Behavior
{
    /// <summary>
    /// テキストボックスのビヘイビア
    /// </summary>
    public class TextBoxBehavior : Behavior<TextBox>
    {
        public string PlaceHolderText { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.GotFocus += AssociatedObject_GotForcus;
            this.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
            //テキストボックス描画時にプレースホルダーを表示する。
            this.AssociatedObject.Background = CreateVisualBrush();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.GotFocus -= AssociatedObject_GotForcus;
            this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }

        private void AssociatedObject_GotForcus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            //入力されている全ての文字を選択する。
            textBox.SelectAll();
        }

        private void AssociatedObject_TextChanged(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Background = CreateVisualBrush();
                return;
            }
            textBox.Background = new SolidColorBrush(Colors.Transparent);
        }

        private VisualBrush CreateVisualBrush()
        {
            var visual = new Label()
            {
                Content = this.PlaceHolderText,
                Padding = new Thickness(5, 1, 1, 1),
                Foreground = new SolidColorBrush(Colors.DarkGray),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            return new VisualBrush(visual)
            {
                Stretch = Stretch.None,
                TileMode = TileMode.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Center,
            };
        }
    }
}


