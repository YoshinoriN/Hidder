using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hidder.NotifyIcon
{
    public partial class NotifyIconWrapper : Component
    {
        private Views.MainWindow _mainWindow { get; set; }

        public NotifyIconWrapper(Views.MainWindow mainWindos)
        {
            InitializeComponent();
            this.Display.Click += this.Display_Click;
            this._mainWindow = mainWindos;
            this._mainWindow.Activate();
        }

        /// <summary>
        /// コンテキストメニュー "Display" を選択したとき呼ばれます。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void Display_Click(object sender, EventArgs e)
        {
            //ウインドウを表示して、タスクバーにも表示する。
            this._mainWindow.WindowState = WindowState.Normal;
            this._mainWindow.ShowInTaskbar = true;
        }
    }
}
