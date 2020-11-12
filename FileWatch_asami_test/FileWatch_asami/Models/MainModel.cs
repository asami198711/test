using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

using FileWatch_asami.Base;
using FileWatch_asami.ViewModels;
using FileWatch_asami.Common;

namespace FileWatch_asami.Models
{
    class MainModel : ModelBase
    {
        /// <summary>
        /// 初期化。
        /// </summary>
        public MainModel()
        {
        }

        /// <summary>
        /// ファイルの監視開始
        /// </summary>
        /// <param name="addFileInfo">監視対象のファイル情報</param>
        public void watchStart(WatchFileInfo addWatchFileInfo)
        {
            // ファイル監視
            Task<string> task = fileWatch(addWatchFileInfo);
            addWatchFileInfo.task = task;

        }

        async Task<String> fileWatch(WatchFileInfo watchFileInfo)
        {
            
            while (watchFileInfo.isWatching)
            {
                // 3秒経過
                await Task.Delay(5000);


                System.IO.FileInfo fileInfo = new System.IO.FileInfo(watchFileInfo.FilePath);
                if (!fileInfo.Exists) {
                    // 削除、ファイル名変更時
                    watchFileInfo.isExist = false;
                    // 監視対象のファイルがなくなったため、背景色を灰色にするよう通知
                    NotExist = watchFileInfo.FilePath;
                } else
                {
                    // 更新日時の更新
                    // 「ファイルの監視->ファイル名変更（背景灰色）->ファイル名を元に戻す」の操作の場合もここにきてファイル監視を再開する
                    Exist = watchFileInfo.FilePath;
                }
            }
            return "finish";
        }

        private String _exist;
        /// <summary>
        /// 
        /// </summary>
        public String Exist
        {
            get
            {
                return _exist;
            }
            set
            {
                _exist = value;
                // 値をsetしたことをViewModelに通知
                RaisePropertyChanged("Exist");
            }
        }


        private String _notExist;
        /// <summary>
        /// 
        /// </summary>
        public String NotExist
        {
            get
            {
                return _notExist;
            }
            set
            {
                _notExist = value;
                // 値をsetしたことをViewModelに通知
                RaisePropertyChanged("NotExist");
            }
        }
    }
}
