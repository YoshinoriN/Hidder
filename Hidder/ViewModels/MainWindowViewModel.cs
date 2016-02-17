using Hidder.Command;
using Hidder.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace Hidder.ViewModels
{
    public class MainWindowViewModel : PropertyChangeNotification
    {
        //ファイル名とバージョン表示
        public string WindowTitle { get; private set; } = System.IO.Path.GetFileNameWithoutExtension(@System.Reflection.Assembly.GetExecutingAssembly().Location) + " " +
            System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).ProductVersion.ToString();

        private double _height = 230;
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

        private string _appTitle;
        /// <summary>
        /// 実行アプリケーションに付けるタイトル
        /// </summary>
        public string AppTitle
        {
            get { return this._appTitle; }
            set
            {
                this._appTitle = value;
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
                if (!string.IsNullOrWhiteSpace(value))
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

        private ObservableCollection<Models.Application> _applications = new ObservableCollection<Models.Application>();
        public ObservableCollection<Models.Application> Applications
        {
            get { return this._applications; }
            set
            {
                this._applications = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.BrowseCommand = new DelegateCommand(Browse);
            this.AddCommand = new DelegateCommand(Add);
            this.RunCommand = new DelegateCommand<int>(Run);
            this.KillCommand = new DelegateCommand<int>(Kill);
            this.RemoveCommand = new DelegateCommand<int>(Remove);
            this.ChangeVisibillityCommand = new DelegateCommand<int>(ChangeVisibillity);
            this.HideAllCommand = new DelegateCommand(HideAll);
            this.ExitCommand = new DelegateCommand(Exit);
            this.SaveListCommand = new DelegateCommand(SaveList);
            this.RestoreListCommand = new DelegateCommand(RestoreList);
        }

        #region "Command関連"

        public ICommand BrowseCommand { get; private set; }

        /// <summary>
        /// ファイルを開く
        /// </summary>
        private void Browse()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Open the file";
            dialog.Filter = "All file(*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                this.Path = dialog.FileName;
            }
        }

        private int _id;

        public ICommand AddCommand { get; private set; }

        /// <summary>
        /// アプリケーションの追加
        /// </summary>
        private void Add()
        {
            this.Applications.Add(new Models.Application(this._id, this.AppTitle, this.Path, this.Argument));
            this.Height = 500;
            _id++;
        }

        public ICommand RunCommand { get; private set; }

        /// <summary>
        /// アプリケーションの実行
        /// </summary>
        private void Run(int id)
        {
            var applicationProcess = this.Applications.Where(x => x.Id == id);
            applicationProcess.ToList().ForEach(x => x.Start(x.FullPath, x.Argument));
        }

        public ICommand KillCommand { get; private set; }

        /// <summary>
        /// プロセス終了時
        /// </summary>
        /// <param name="id"></param>
        private void Kill(int id)
        {
            var application = this.Applications.Where(x => x.Id == id);
            application.ToList().ForEach(x => x.Kill());
        }

        public ICommand ChangeVisibillityCommand { get; private set; }

        /// <summary>
        /// アプリケーションの可視/不可視を切り替える
        /// </summary>
        private void ChangeVisibillity(int id)
        {
            var application = this.Applications.Where(x => x.Id == id);
            foreach (Models.Application app in application)
            {
                if (app.CurrentWindowStyle == WindowStyleEnum.Hidden)
                {
                    app.Show();
                }
                else
                {
                    app.Hide();
                }
            }
        }

        public ICommand RemoveCommand { get; private set; }

        /// <summary>
        /// DataGridに表示されている行を削除する。
        /// </summary>
        private void Remove(int id)
        {
            var application = this.Applications.Where(x => x.Id == id);
            //Fixme:このforeach何とかならんのか？
            foreach (Models.Application app in application)
            {
                this.Applications.Remove(app);
                if (this.Applications.Count == 0)
                {
                    this.Height = 230;
                }
                return;
            }
        }

        //View側からWindowのHideメソッドを登録する。
        public Action HideThis { get; set; }

        public ICommand HideAllCommand { get; private set; }

        /// <summary>
        /// 本アプリケーションを含めた全ての実行中アプリケーションを非表示にする。
        /// </summary>
        private void HideAll()
        {
            var applications = this.Applications.Where(x => x.CurrentWindowStyle
                                                       != WindowStyleEnum.Hidden
                                                       && x.ProcessId != "-");
            applications.ToList().ForEach(x => x.Hide());
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
            var isExistsRunningApplication = this.Applications.Any(x => x.CurrentWindowStyle == WindowStyleEnum.Hidden);
            if (isExistsRunningApplication)
            {
                MessageBox.Show("Please show all runnning application's window.");
                return;
            }
            this.ExitThis();
        }

        #endregion

        #region"リストの保存/復元"

        public ICommand SaveListCommand { get; private set; }

        /// <summary>
        /// アプリケーションリストの保存
        /// </summary>
        private void SaveList()
        {
            if (this.Applications.Count == 0)
            {
                MessageBox.Show("Some applications doesn't exists on the datagrid.");
                return;
            }
            try
            {
                Models.XmlFileSerializer.Serialize(this.Applications, "./AppData/applicationlist.xml");
                MessageBox.Show("Saved");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show((string)ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public ICommand RestoreListCommand { get; private set; }

        /// <summary>
        /// アプリケーションリストの復元
        /// </summary>
        private void RestoreList()
        {
            try
            {
                var applications = Models.XmlFileSerializer.Deserialize(this.Applications, "./AppData/applicationlist.xml");
                if (applications == null)
                {
                    MessageBox.Show("Couldn't find the file.");
                    return;
                }

                foreach (Models.Application application in applications)
                {
                    application.Id = this._id;
                    this.Applications.Add(application);
                    this._id++;
                }
                this.Height = 500;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show((string)ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        #endregion
    }
}
