using Hidder.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Hidder.ViewModels
{
    public class MainWindowViewModel : ViewModelsBase
    {
        //ファイル名とバージョン表示
        public string Title { get; private set; } = System.IO.Path.GetFileNameWithoutExtension(@System.Reflection.Assembly.GetExecutingAssembly().Location) + " " +
            System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).ProductVersion.ToString();

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

        private ObservableCollection<Models.ApplicationProcess> _applicationProcesses = new ObservableCollection<Models.ApplicationProcess>();
        public ObservableCollection<Models.ApplicationProcess> ApplicationProcesses
        {
            get { return this._applicationProcesses; }
            set
            {
                this._applicationProcesses = value;
                //コレクション1つめ追加時に高さを広げる
                if (this._height != this.Height)
                {
                    this.Height = 500;
                }
                OnPropertyChanged();
            }
        }

        private double _height = 200;
        public double Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.RunCommand = new DelegateCommand(Run);
            this.ReRunCommand = new DelegateCommand<int>(ReRun);
            this.KillCommand = new DelegateCommand<int>(Kill);
            this.RemoveCommand = new DelegateCommand<int>(Remove);
            this.ChangeVisibillityCommand = new DelegateCommand<int>(ChangeVisibillity);
        }

        private string _changeVisibilityButtonContent = "Hide";
        /// <summary>
        /// 実行アプリケーションの可視/不可視
        /// </summary>
        public string ChangeVisibilityButtonContent
        {
            get { return this._changeVisibilityButtonContent; }
            set
            {
                this._changeVisibilityButtonContent = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// アプリケーション管理用ID
        /// </summary>
        private int _id;

        public ICommand RunCommand { get; private set; }

        /// <summary>
        /// Run(否データグリッド)押下時
        /// </summary>
        private void Run()
        {
            this.ApplicationProcesses.Add(new Models.ApplicationProcess(this._id));
            try
            {
                this.ApplicationProcesses[_id].Start(this.Path, this.Argument);
                _id++;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show((string)ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public ICommand ReRunCommand { get; private set; }

        /// <summary>
        /// Run(データグリッド)押下時
        /// </summary>
        private void ReRun(int id)
        {
            try
            {
                var applicationProcess = this.ApplicationProcesses.Where(x => x.Id == id);
                foreach (Models.ApplicationProcess appProcess in applicationProcess)
                    appProcess.Start(appProcess.FullPath, appProcess.Argument);
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
        /// アプリケーションの可視/不可視を切り替える
        /// </summary>
        private void Remove(int id)
        {
            //Fixme:正直ここ怪しいのでなおす。
            var appProcess = this.ApplicationProcesses.Where(x => x.Id == id);

            //Fixme:このforeach何とかならんのか？
            foreach (Models.ApplicationProcess process in appProcess)
            {
                this.ApplicationProcesses.Remove(process);
                return;
            }

        }

    }
}
