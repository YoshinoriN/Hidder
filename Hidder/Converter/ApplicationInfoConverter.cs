using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hidder.Converter
{
    /// <summary>
    /// アプリケーションの情報をまとめるクラス
    /// </summary>
    public class ApplicationInfoConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var dataRow = values[0] as DataGridRow;
            Models.Application application = (Models.Application)dataRow.Item;

            TextBlock textBlock = new TextBlock();
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.Text = "Title:"+ application.Title
                           + "\r\n" + "FileName:" + application.FileName
                           + "\r\n" + "Path:" + application.FullPath
                           + "\r\n" + "Argument:" + application.Argument
                           ;
            return textBlock;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return value.ToString().Split();
        }
    }
}
