using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Hidder
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            //ぶっちゃけテーマの切替設定がないのでApp.xamlに書いた方がよい
            this.Resources.MergedDictionaries.Add(Application.LoadComponent
                (new Uri(String.Format(@"/Hidder;component/Themes/Dark.xaml"), UriKind.Relative)) as ResourceDictionary);
        }
    }
}
