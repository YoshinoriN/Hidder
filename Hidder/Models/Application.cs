using Hidder.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Hidder.Models
{
    /// <summary>
    /// 実行アプリケーションクラス。
    /// </summary>
    [Serializable]
    [XmlRoot("Application")]
    public class Application : PropertyChangeNotification
    {
        private System.Diagnostics.Process _process;

        /// <summary>
        /// ID (データグリッドと整合性とるためのIDでプロセスIDじゃない)
        /// Hack:xmlからリストアする場合に外部からIDを指定するためにPublicにする。
        /// </summary>
        [XmlIgnore]
        public int Id { get; set; }

        #region"プロセスに関する情報"

        private bool _isRunning = false;
        /// <summary>
        /// プロセス稼動有無
        /// Hack:Dispose済みかどうか確認するためにも使用する。_processの生成/破棄と同期を取る。
        /// </summary>
        [XmlIgnore]
        public bool IsRunning
        {
            get { return this._isRunning; }
            private set
            {
                this._isRunning = value;
                if (value)
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
        [XmlIgnore]
        public ApplicationStatusEnum ApplicationStatus
        {
            get { return this._applicationStatus; }
            private set
            {
                this._applicationStatus = value;
                this.OnPropertyChanged();
            }
        }

        private WindowStyleEnum _currentWindowStyle;
        /// <summary>
        /// 現在のウインドウサイズ
        /// </summary>
        [XmlIgnore]
        public WindowStyleEnum CurrentWindowStyle
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
        [XmlIgnore]
        public string ProcessId
        {
            get { return this._processId; }
            private set
            {
                this._processId = value;
                this.OnPropertyChanged();
            }
        }

        private string _title = "";
        /// <summary>
        /// タイトル
        /// </summary>
        [XmlElement]
        public string Title
        {
            get { return this._title; }
            set
            {
                this._title = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// パス
        /// </summary>
        [XmlElement]
        public string FullPath { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        [XmlIgnore]
        public string FileName { get { return Path.GetFileName(this.FullPath); } }

        private string _argument = "";
        /// <summary>
        /// 引数
        /// </summary>
        [XmlElement]
        public string Argument
        {
            get { return this._argument; }
            set
            {
                this._argument = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// アプリケーションのUI種別
        /// </summary>
        [XmlIgnore]
        private ApplicationUiType UiType { get; set; }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Application()
        {
            //シリアライズのために引数の存在しないコンストラクタが必要
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Application(int id, string title, string path, string argument)
        {
            this.Title = title;
            this.Id = id;
            this.FullPath = path;
            this.Argument = argument;
            this.CurrentWindowStyle = WindowStyleEnum.None;
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
                this.IsRunning = true;
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
                if (_process.MainModule.FileVersionInfo.InternalName == "cmd")
                {
                    this.UiType = ApplicationUiType.CUI;
                }
                else
                {
                    this.UiType = ApplicationUiType.GUI;
                    //アプリケーションが入力可能になるまで最大10秒待機する
                    _process.WaitForInputIdle(10000);
                }

                this.CurrentWindowStyle = WindowStyleEnum.Normal;
                this.FullPath = path;
                this.Argument = argument;
                this.ProcessId = _process.Id.ToString();
                this.ApplicationStatus = ApplicationStatusEnum.Running;
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
            if (this.CurrentWindowStyle == WindowStyleEnum.Hidden)
            {
                this.CurrentWindowStyle = WindowStyleChange.Normal(this._process);
            }
        }

        /// <summary>
        /// 起動中アプリケーションのウインドウを非表示にします。
        /// </summary>
        public void Hide()
        {
            if (this.CurrentWindowStyle != WindowStyleEnum.Hidden)
            {
                this.CurrentWindowStyle = WindowStyleChange.Hide(this._process);
            }
        }

        /// <summary>
        /// プロセスを終了します。
        /// </summary>
        public void Kill()
        {
            this.ApplicationStatus = ApplicationStatusEnum.Stopping;

            if (MessageBox.Show("Do you really want to kill it process force ?", "Warning"
                , MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                this.ApplicationStatus = ApplicationStatusEnum.Running;
                return;
            }

            try
            {
                //メッセージボックス確認中にプロセスが終了している可能性があるので、確認後にKillする。
                if (this.IsRunning == true) _process.CloseMainWindow();

                if (UiType == ApplicationUiType.GUI)
                {
                    KillGui();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show((string)ex.Message + Environment.NewLine + ex.StackTrace);
                this.ApplicationStatus = ApplicationStatusEnum.Faild;
            }
            finally
            {
                //IsRunningでプロセスが終了しているかどうか判断する。
                if (this.IsRunning == true) KillProcessFinalize();
            }
        }

        /// <summary>
        /// GUIアプリケーションのプロセスを終了します。
        /// </summary>
        private void KillGui()
        {
            //アプリケーションに終了メッセージ送信後に最大10秒待機する
            System.Threading.Thread.Sleep(10000);

            if (!_process.HasExited)
            {
                //GUIアプリケーションが終了しなかった場合、強制終了させるか再度確認する。
                if (MessageBox.Show("Do you really want to kill GUI application force ?", "Warning"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    this.ApplicationStatus = ApplicationStatusEnum.Running;
                    return;
                }
                //メッセージボックス確認中にプロセスが終了している可能性があるので、確認後にKillする。
                if (this.IsRunning == true) _process.Kill();
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
            this.CurrentWindowStyle = WindowStyleEnum.None;
            _process.Close();
            _process.Dispose();
        }
    }
}
