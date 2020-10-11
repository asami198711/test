using ExclusiveApp.Views;
using Prism.Ioc;
using System.Windows;

namespace ExclusiveApp
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Appの継承抽象メンバー
        /// </summary>
        /// <returns></returns>
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// Appの継承抽象メンバー
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
