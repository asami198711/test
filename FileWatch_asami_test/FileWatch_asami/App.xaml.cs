using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using FileWatch_asami.Views;

namespace FileWatch_asami
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {

        private void StartupApp(object sender, StartupEventArgs args)
        {
            // .Views.MainWindow.xaml を表示する
            var view = new FileWatch_asami.Views.MainWindow();
            view.Show();
        }
    }
}
