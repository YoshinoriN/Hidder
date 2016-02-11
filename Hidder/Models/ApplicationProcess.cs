using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Hidder.Models
{
    public class ApplicationProcess : ViewModels.ViewModelsBase
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private System.Diagnostics.Process _process;

        /// <summary>
        /// ID (データグリッドと整合性とるためのIDでプロセスIDじゃない)
        /// </summary>
        public int Id { get; private set; }

        #region"プロセスに関する情報"

        private bool _isRunning = false;
        /// <summary>
        /// プロセス稼動有無
        /// </summary>
        public bool IsRunning
        {
            get { return this._isRunning; }
            private set
            {
                this._isRunning = value;
                this.OnPropertyChanged();
            }
        }

        private ProcessWindowStyle _currentWindowStyle;
        /// <summary>
        /// 現在のウインドウサイズ
        /// </summary>
        public ProcessWindowStyle CurrentWindowStyle
        {
            get { return this._currentWindowStyle; }
            private set
            {
                this._currentWindowStyle = value;
                this.OnPropertyChanged();
            }
        }

        //以下、実行後変わらないはずなので、変更通知しない。

        private string _processId = "-";
        /// <summary>
        /// プロセスID
        /// </summary>
        public string ProcessId
        {
            get { return this._processId; }
            private set
            {
                this._processId = value;
                this.OnPropertyChanged();
            }
        }

        private string _fullPath;
        /// <summary>
        /// パス
        /// </summary>
        public string FullPath
        {
            get { return this._fullPath; }
            private set
            {
                this._fullPath = value;
            }
        }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get { return Path.GetFileName(this.FullPath); } }


        private string _argument;
        /// <summary>
        /// 引数
        /// </summary>
        public string Argument
        {
            get { return this._argument; }
            private set
            {
                this._argument = value;
            }
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ApplicationProcess(int id, string path, string argument)
        {
            this.Id = id;
            this.FullPath = path;
            this.Argument = argument;
        }

        /// <summary>
        /// プロセスを開始します。
        /// </summary>
        public void Start(string path, string argument)
        {
            _process = new System.Diagnostics.Process();
            _process.StartInfo.FileName = path;
            _process.StartInfo.Arguments = argument;
            _process.Exited += new EventHandler(ProcessExit);
            _process.EnableRaisingEvents = true;
            _process.Start();
            this.CurrentWindowStyle = _process.StartInfo.WindowStyle;
            this.IsRunning = true;
            this.FullPath = path;
            this.Argument = argument;
            this.ProcessId = _process.Id.ToString();
        }

        /// <summary>
        /// 起動中アプリケーションのウインドウサイズを変更します。
        /// Fixme:とりあえず、HiddenとNormalだけ
        /// </summary>
        public void ChangeWindowState()
        {
            if (this.CurrentWindowStyle != ProcessWindowStyle.Hidden)
            {
                ShowWindow(_process.MainWindowHandle, 1);
                this.CurrentWindowStyle = ProcessWindowStyle.Hidden;
                return;
            }
            ShowWindow(_process.MainWindowHandle, 0);
            this.CurrentWindowStyle = ProcessWindowStyle.Normal;
        }

        /// <summary>
        /// プロセスを終了します。
        /// Fixme:ここの例外処理考える。
        /// </summary>
        public void Kill()
        {
            var firstDialogResult = MessageBox.Show("Do you really want to kill it process force ?", "Warning",
                         MessageBoxButtons.YesNo,
                         MessageBoxIcon.Exclamation,
                         MessageBoxDefaultButton.Button2);

            if (firstDialogResult == DialogResult.No)
                return;

            //Hack:コンソールアプリケーションをCloseMainWindowでkillしてよいのか？
            _process.CloseMainWindow();
            if (_process.HasExited)
            {
                //GUIアプリケーションが終了しなかった場合、強制終了させるか再度確認する。
                var secondDialogResult = MessageBox.Show("Do you really want to kill GUI application force ?",
                                            "Warning",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Exclamation,
                                            MessageBoxDefaultButton.Button2);
                if (secondDialogResult == DialogResult.No)
                    return;

                _process.Kill();
            };
            this.IsRunning = false;
            this.ProcessId = "-";
            _process.Close();
            _process.Dispose();
        }

        /// <summary>
        /// アプリケーションを直接終了した場合に呼び出される。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessExit(Object sender, EventArgs e)
        {
            this.IsRunning = false;
            this.ProcessId = "-";
        }
    }
}
