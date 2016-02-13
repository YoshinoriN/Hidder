using Hidder.Command;
using Hidder.Common;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Hidder.ViewModels
{
    public class MainWindowViewModel : PropertyChangeNotification
    {
        //ファイル名とバージョン表示
        public string Title { get; private set; } = System.IO.Path.GetFileNameWithoutExtension(@System.Reflection.Assembly.GetExecutingAssembly().Location) + " " +
            System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).ProductVersion.ToString();

        private double _height = 200;
        /// <summary>
        /// ウィンドウの高さ
        /// </summary>
        public double Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
                this.OnPropertyChanged();
            }
        }

        private string _path;
        /// <summary>
        /// 実行アプリケーションのパス
        /// </summary>
        public string Path
        {
            get { return this._path; }
            set
            {
                this._path = value;
                if(!string.IsNullOrWhiteSpace(value))
                {
                    this.IsExistsFilePath = System.IO.File.Exists(value);
                }
                else
                {
                    this.IsExistsFilePath = true;
                }
                this.OnPropertyChanged();
            }
        }

        private string _argument;
        /// <summary>
        /// 実行アプリケーションの引数
        /// </summary>
        public string Argument
        {
            get { return this._argument; }
            set
            {
                this._argument = value;
                this.OnPropertyChanged();
            }
        }

        private bool _isExistsFilePath = true;
        /// <summary>
        /// テキストボックスに入力したパスが存在するかどうか。
        /// 空白の場合はtrueで扱う。
        /// Hack:もっと別の手段が取れそうだけど、いろいろな組み合わせの問題があって...
        /// </summary>
        public bool IsExistsFilePath
        {
            get { return this._isExistsFilePath; }
            private set
            {
                this._isExistsFilePath = value;
                this.OnPropertyChanged();
            }
        }

        private ObservableCollection<Models.ApplicationProcess> _applicationProcesses = new ObservableCollection<Models.ApplicationProcess>();
        public ObservableCollection<Models.ApplicationProcess> ApplicationProcesses
        {
            get { return this._applicationProcesses; }
            set
            {
                this._applicationProcesses = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.AddCommand = new DelegateCommand(Add);
            this.RunCommand = new DelegateCommand<int>(Run);
            this.KillCommand = new DelegateCommand<int>(Kill);
            this.RemoveCommand = new DelegateCommand<int>(Remove);
            this.ChangeVisibillityCommand = new DelegateCommand<int>(ChangeVisibillity);
            this.HideAllCommand = new DelegateCommand(HideAll);
            this.ExitCommand = new DelegateCommand(Exit);
        }

        #region "Command関連"

        private int _id;

        public ICommand AddCommand { get; private set; }

        /// <summary>
        /// アプリケーションの追加
        /// </summary>
        private void Add()
        {
            this.ApplicationProcesses.Add(new Models.ApplicationProcess(this._id, this.Path, this.Argument));
            this.Height = 500;
            _id++;
        }

        public ICommand RunCommand { get; private set; }

        /// <summary>
        /// アプリケーションの実行
        /// </summary>
        private void Run(int id)
        {
            try
            {
                var applicationProcess = this.ApplicationProcesses.Where(x => x.Id == id);
                applicationProcess.ToList().ForEach(x => x.Start(x.FullPath, x.Argument));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show((string)ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public ICommand KillCommand { get; private set; }

        /// <summary>
        /// プロセス終了時
        /// </summary>
        /// <param name="id"></param>
        private void Kill(int id)
        {
            try
            {
                var applicationProcess = this.ApplicationProcesses.Where(x => x.Id == id);
                applicationProcess.ToList().ForEach(x => x.Kill());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show((string)ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public ICommand ChangeVisibillityCommand { get; private set; }

        /// <summary>
        /// アプリケーションの可視/不可視を切り替える
        /// </summary>
        private void ChangeVisibillity(int id)
        {
            var applicationProcess = this.ApplicationProcesses.Where(x => x.Id == id);
            applicationProcess.ToList().ForEach(x => x.ChangeWindowState());                
        }

        public ICommand RemoveCommand { get; private set; }

        /// <summary>
        /// DataGridに表示されている行を削除する。
        /// </summary>
        private void Remove(int id)
        {
            var appProcess = this.ApplicationProcesses.Where(x => x.Id == id);
            //Fixme:このforeach何とかならんのか？
            foreach (Models.ApplicationProcess process in appProcess)
            {
                this.ApplicationProcesses.Remove(process);

                if(this.ApplicationProcesses.Count == 0)
                {
                    this.Height = 200;
                }
                return;
            }
        }

        public Action HideThis { get; set; }

        public ICommand HideAllCommand { get; private set; }

        /// <summary>
        /// 本アプリケーションを含めた全ての実行中アプリケーションを非表示にする。
        /// </summary>
        private void HideAll()
        {
            var applicationProcess = this.ApplicationProcesses.Where(x => x.CurrentWindowStyle
                                                                       != System.Diagnostics.ProcessWindowStyle.Hidden
                                                                       && x.ProcessId != "-");
            applicationProcess.ToList().ForEach(x => x.Hide());
            this.HideThis();
        }

        //View側からWindowのExitメソッドを登録する。
        public Action ExitThis { get; set; }

        public ICommand ExitCommand { get; private set; }

        /// <summary>
        /// 本アプリケーションの終了
        /// </summary>
        private void Exit()
        {
            var isExistsRunningApplication = this.ApplicationProcesses.Any(x => x.CurrentWindowStyle == System.Diagnostics.ProcessWindowStyle.Hidden);
            if(isExistsRunningApplication)
            {
                MessageBox.Show("Please exit all runnning applications or show all window, if some runnning application are hidden.");
                return;
            }
            this.ExitThis();
        }

        #endregion
    }
}
