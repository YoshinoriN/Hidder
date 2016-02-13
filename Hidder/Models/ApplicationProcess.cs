using Hidder.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Hidder.Models
{
    public class ApplicationProcess : PropertyChangeNotification
    {
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
                if(value)
                {
                    this.ApplicationStatus = ApplicationStatusEnum.Running;
                }
                this.OnPropertyChanged();
            }
        }

        private ApplicationStatusEnum _applicationStatus;
        /// <summary>
        /// プロセス稼動詳細ステータス
        /// </summary>
        public ApplicationStatusEnum ApplicationStatus
        {
            get { return this._applicationStatus; }
            private set
            {
                this._applicationStatus = value;
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

        /// <summary>
        /// パス
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get { return Path.GetFileName(this.FullPath); } }

        /// <summary>
        /// 引数
        /// </summary>
        public string Argument { get; private set; }

        /// <summary>
        /// GUIかCUIか
        /// Fixme:Enumにする。
        /// </summary>
        private string UIType { get; set; }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ApplicationProcess(int id, string path, string argument)
        {
            this.Id = id;
            this.FullPath = path;
            this.Argument = argument;
            if (!System.IO.File.Exists(path))
            {
                this.ApplicationStatus = ApplicationStatusEnum.DoesntExist;
                return;
            }
            this.ApplicationStatus = ApplicationStatusEnum.Ready;
        }

        /// <summary>
        /// プロセスを開始します。
        /// </summary>
        public void Start(string path, string argument)
        {
            //Hack:ここでTry~Catchしたくないんだけどな～
            try
            {
                this.ApplicationStatus = ApplicationStatusEnum.Starting;
                _process = new System.Diagnostics.Process();
                _process.StartInfo.FileName = path;
                _process.StartInfo.Arguments = argument;
                _process.Exited += new EventHandler(ProcessExit);
                _process.EnableRaisingEvents = true;
                _process.Start();

                //Hack:GUIの場合次のInternalNameが取得できないケースがあるので、0.5秒停止させる
                System.Threading.Thread.Sleep(500);

                //Hack:InternalNameがcmdかどうかでCUIかGUIを判断する。
                if (_process.MainModule.FileVersionInfo.InternalName != "cmd")
                {
                    this.UIType = "GUI";
                    _process.WaitForInputIdle(10000);
                }
                else
                {
                    this.UIType = "CUI";
                }

                this.CurrentWindowStyle = _process.StartInfo.WindowStyle;
                this.FullPath = path;
                this.Argument = argument;
                this.ProcessId = _process.Id.ToString();
                this.IsRunning = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show((string)ex.Message + Environment.NewLine + ex.StackTrace);
                this.IsRunning = false;
                this.ApplicationStatus = ApplicationStatusEnum.Faild;
            }
        }

        /// <summary>
        /// 起動中アプリケーションのウインドウを表示します。
        /// </summary>
        public void Show()
        {
            if (this.CurrentWindowStyle == ProcessWindowStyle.Hidden)
            {
                this.CurrentWindowStyle = WindowStyleChange.Normal(this._process);
            }
        }

        /// <summary>
        /// 起動中アプリケーションのウインドウを非表示にします。
        /// </summary>
        public void Hide()
        {
            if (this.CurrentWindowStyle != ProcessWindowStyle.Hidden)
            {
                this.CurrentWindowStyle = WindowStyleChange.Hide(this._process);
            }
        }

        /// <summary>
        /// プロセスを終了します。
        /// Fixme:ここの例外処理考える。
        /// </summary>
        public void Kill()
        {
            this.ApplicationStatus = ApplicationStatusEnum.Stopping;
            try
            {
                if (MessageBox.Show("Do you really want to kill it process force ?", "Warning"
                   , MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    this.ApplicationStatus = ApplicationStatusEnum.Running;
                    return;
                }

                _process.CloseMainWindow();

                if (UIType == "CUI")
                {
                    KillProcessFinalize();
                    return;
                }

                if (!_process.HasExited)
                {
                    //GUIアプリケーションが終了しなかった場合、強制終了させるか再度確認する。
                    if (MessageBox.Show("Do you really want to kill GUI application force ?", "Warning"
                        , MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        this.ApplicationStatus = ApplicationStatusEnum.Running;
                        return;
                    }
                    _process.Kill();
                };
                KillProcessFinalize();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show((string)ex.Message + Environment.NewLine + ex.StackTrace);
                this.ApplicationStatus = ApplicationStatusEnum.Faild;
            }
        }

        /// <summary>
        /// アプリケーションを直接終了した場合に呼び出される。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessExit(Object sender, EventArgs e)
        {
            KillProcessFinalize();
        }

        /// <summary>
        /// プロセスKill後の処理
        /// </summary>
        private void KillProcessFinalize()
        {
            this.IsRunning = false;
            this.ApplicationStatus = ApplicationStatusEnum.Ready;
            this.ProcessId = "-";
            _process.Close();
            _process.Dispose();
        }

    }
}
