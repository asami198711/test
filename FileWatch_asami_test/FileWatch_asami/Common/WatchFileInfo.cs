using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatch_asami.Common
{
    /// <summary>
    /// 監視対象のファイル情報
    /// </summary>
    class WatchFileInfo
    {
        // ファイルフルパス
        public String FilePath { get; set; }
        // 更新日時
        public String UpdateDay { get; set; }
        // 存在有無
        public bool isExist { get; set; }
        // ファイル監視タスク
        public Task<string> task { get; set; }
        // ファイル監視中ならtrue,削除ボタン押下で監視を解除したらにfalse
        public bool isWatching = false;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="FilePath">ファイルフルパス</param>
        /// <param name="UpdateDay">更新日時</param>
        public WatchFileInfo(String FilePath)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(FilePath);
            this.FilePath = FilePath;
            this.UpdateDay = info.LastWriteTime.ToString();
            isExist = true;
            isWatching = true;
        }

        public void refresh ()
        {
            System.IO.FileInfo info = new System.IO.FileInfo(this.FilePath);
            if (null != info) {
                this.UpdateDay = info.LastWriteTime.ToString();
                isExist = true;
            } else
            {
                isExist = false;
            }
        }
    }
}
