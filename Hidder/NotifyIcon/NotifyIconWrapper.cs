using System;
using System.ComponentModel;
using System.Windows;

namespace Hidder.NotifyIcon
{
    public partial class NotifyIconWrapper : Component
    {
        private Views.MainWindow mainWindow { get; set; }

        public NotifyIconWrapper(Views.MainWindow _mainWindow)
        {
            // コンポーネントの初期化
            this.InitializeComponent();

            // コンテキストメニューのイベントを設定
            this.toolStripMenuItem_Open.Click += this.toolStripMenuItem_Open_Click;
            this.toolStripMenuItem_Exit.Click += this.toolStripMenuItem_Exit_Click;

            this.notifyIcon.Text = _mainWindow.title.Text;
            this.notifyIcon.Text += "\r\n\r\n" + "ダブルクリック:再表示" + "\r\n" + "右クリック:再表示もしくは終了";

            mainWindow = _mainWindow;
            mainWindow.Activate();
        }

        /// <summary>
        /// コンテキストメニュー "Display" を選択したとき呼ばれます。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void toolStripMenuItem_Open_Click(object sender, EventArgs e)
        {
            //ウインドウを表示して、タスクバーにも表示する。
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.ShowInTaskbar = true;
        }

        /// <summary>
        /// コンテキストメニュー "Exit" を選択したとき呼ばれます。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            // 現在のアプリケーションを終了
            Application.Current.Shutdown();
        }

    }
}
