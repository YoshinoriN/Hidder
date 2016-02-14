using System.Windows;
using System.Windows.Controls;

namespace Hidder.Behavior
{
    /// <summary>
    /// テキストボックスのビヘイビア拡張
    /// </summary>
    public class TextBoxBehaviorExtensions : TextBoxBehavior
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewDragOver += textBox_PreviewDragOver;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewDragOver -= textBox_PreviewDragOver;
        }

        private void textBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = DragDropEffects.Copy;
                var dropFiles = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
                if (dropFiles == null) return;
                var textBox = sender as TextBox;
                textBox.Text = dropFiles[0];
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }
    }
}


