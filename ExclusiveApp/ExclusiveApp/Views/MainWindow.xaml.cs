using System;
using System.Windows;
using System.Threading.Tasks;   // Task.Run
using System.Threading; // CancellationTokenSource
/*
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
*/

namespace ExclusiveApp.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        private CancellationTokenSource cancelTokensource1;//キャンセル判定用
        private CancellationTokenSource cancelTokensource2;//キャンセル判定用
        Object lockObj = new Object();
        int percent = 0;

        /// <summary>
        /// 初期化
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            task1_cancel_button.IsEnabled = false;  // MainWindows.xamlで宣言
            task2_cancel_button.IsEnabled = false;  // MainWindows.xamlで宣言
        }

        /// <summary>
        /// タスク１
        /// </summary>
        /// <param name="p">処理中に実行してほしいメソッド</param>
        /// <param name="cancelToken">キャンセル用のトークン</param>
        /// <returns>キャンセルの場合はfalse、そうでない場合はtrue</returns>
        public bool Task1(IProgress<int> p, CancellationToken cancelToken)
        {
            bool ret = true;
            // 排他処理
            lock (lockObj)
            {
                while (percent < 100)
                {
                    //ダミー負荷用ウエイトms スレッドを止める
                    Thread.Sleep(30);
                    //状況の報告
                    percent++;
                    p.Report(percent);
                    //キャンセルリクエストの確認
                    if (cancelToken.IsCancellationRequested)
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// タスク２
        /// </summary>
        /// <param name="p">処理中に実行してほしいメソッド</param>
        /// <param name="cancelToken">キャンセル用のトークン</param>
        /// <returns>キャンセルの場合はfalse、そうでない場合はtrue</returns>
        public bool Task2(IProgress<int> p, CancellationToken cancelToken)
        {
            bool ret = true;
            lock (lockObj)
            {
                while (percent > 0)
                {
                    //ダミー負荷用ウエイトms スレッドを止める
                    Thread.Sleep(30);
                    //状況の報告
                    percent--;
                    p.Report(percent);
                    //キャンセルリクエストの確認
                    if (cancelToken.IsCancellationRequested)
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 途中経過を表示するためのメソッド
        /// Text.Box を操作するので別スレッドから実行すると例外が生じてアプリが落ちてしまいます。
        /// </summary>
        /// <param name="percent"></param>
        public void SetText(int percent)
        {
            // textbox1 & ProgressBar1 の更新
            textBox1.Text = percent.ToString(); // MainWindows.xamlで宣言
            progressBar1.Value = percent;       // MainWindows.xamlで宣言
        }

        /// <summary>
        /// Task1キャンセル用のボタン処理
        /// cancelTokensource1.Cancel( )とするとTask1の処理ループの中の if 文で拾う事ができます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Task1_cancel_button_Click(object sender, RoutedEventArgs e)
        {
            if (cancelTokensource1 != null) cancelTokensource1.Cancel();
        }

        /// <summary>
        /// Task1キャンセル用のボタン処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Task2_cancel_button_Click(object sender, RoutedEventArgs e)
        {
            if (cancelTokensource2 != null) cancelTokensource2.Cancel();
        }


        /// <summary>
        /// Task1実行用のボタンクリック処理
        /// 途中経過表示用の処理とキャンセル用のトークンを引数としてTask1を起動します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Task1_button_Click(object sender, RoutedEventArgs e)
        {
            task1_button.IsEnabled = false;         // MainWindows.xamlで宣言
            task1_cancel_button.IsEnabled = true;   // MainWindows.xamlで宣言
            // 途中経過表示用の処理
            var p = new Progress<int>(SetText);
            cancelTokensource1 = new CancellationTokenSource();
            // キャンセル用のトークン
            var cToken = cancelTokensource1.Token;
            bool ret = await Task.Run(() => Task1(p, cToken));
            task1_button.IsEnabled = true;          // MainWindows.xamlで宣言
            task1_cancel_button.IsEnabled = false;  // MainWindows.xamlで宣言
            cancelTokensource1.Dispose();
            cancelTokensource1 = null;
        }

        /// <summary>
        /// Task1実行用のボタンクリック処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Task2_button_Click(object sender, RoutedEventArgs e)
        {
            task2_button.IsEnabled = false;         // MainWindows.xamlで宣言
            task2_cancel_button.IsEnabled = true;   // MainWindows.xamlで宣言
            var p = new Progress<int>(SetText);
            cancelTokensource2 = new CancellationTokenSource();
            var cToken = cancelTokensource2.Token;
            bool ret = await Task.Run(() => Task2(p, cToken));
            task2_button.IsEnabled = true;          // MainWindows.xamlで宣言
            task2_cancel_button.IsEnabled = false;  // MainWindows.xamlで宣言
            cancelTokensource2.Dispose();
            cancelTokensource2 = null;
        }
    }
}
