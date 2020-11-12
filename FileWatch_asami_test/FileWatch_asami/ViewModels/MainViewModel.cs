using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using FileWatch_asami.Base;
using FileWatch_asami.Common;
using FileWatch_asami.Models;
using MSAPI = Microsoft.WindowsAPICodePack; // Windowsファイル選択ダイアログ

namespace FileWatch_asami.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// 監視対象のファイルの追加/削除を担当する。
        /// </summary>
        public MainModel model { get; set; }

        /// <summary>
        /// 監視対象のファイル情報を持つをリスト。
        /// </summary>
        private ObservableCollection<WatchFileInfo> _items;
        public ObservableCollection<WatchFileInfo> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                // 値をsetしたことをViewModelに通知
                RaisePropertyChanged("Itmes");
            }
        }


        /// <summary>
        /// 選択行のファイル情報。
        /// </summary>
        //public WatchFileInfo selectedItem { get; set; }

        private WatchFileInfo _selectedItem;
        public WatchFileInfo SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                //行選択時に来る
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }


        /// <summary>
        /// インストラクタ。
        /// </summary>
        public MainViewModel()
        {
            Items = new ObservableCollection<WatchFileInfo>();
            model = new MainModel();
            model.PropertyChanged += this.DetectionModelPropertyChanged;
        }

        /// <summary>
        /// ファイル追加のコマンド
        /// </summary>
        private DelegateCommand addFileCommand;

        public DelegateCommand AddFileCommand
        {
            get
            {
                if (this.addFileCommand == null)
                {
                    this.addFileCommand = new DelegateCommand(AddFileExecute, CanAddFileExecute);
                }

                return this.addFileCommand;
            }
        }
        private bool CanAddFileExecute()
        {
            return true;
        }

        /// <summary>
        /// 「追加」ボタン押下時の処理。
        /// </summary>
        private void AddFileExecute()
        {
            // ファイル選択ダイアログ
            var dlg = new MSAPI::Dialogs.CommonOpenFileDialog();
            dlg.IsFolderPicker = false;
            dlg.Title = "監視するファイルを選択してください。";
            dlg.InitialDirectory = @"C:";

            if (dlg.ShowDialog() == MSAPI::Dialogs.CommonFileDialogResult.Ok)
            {
                // ファイルを選択したらココに来る
                WatchFileInfo fileInfo = new WatchFileInfo(dlg.FileName);

                // 重複チェック
                bool isExistFile = false;
                foreach (WatchFileInfo item in this.Items)
                {
                    if (item.FilePath.Equals(fileInfo.FilePath))
                    {
                        isExistFile = true;
                        break;
                    }
                }
                if (!isExistFile)
                {
                    // DataGridに表示
                    Items.Add(fileInfo);

                    // 監視開始
                    model.watchStart(fileInfo);
                }

            }
        }

        /// <summary>
        /// ファイル削除のコマンド。
        /// </summary>
        private DelegateCommand deleteFileCommand;

        public DelegateCommand DeleteFileCommand
        {
            get
            {
                if (this.deleteFileCommand == null)
                {
                    this.deleteFileCommand = new DelegateCommand(DeleteFileExecute, CanDeleteFileExecute);
                }

                return this.deleteFileCommand;
            }
        }
        private bool CanDeleteFileExecute()
        {
            return true;
        }

        /// <summary>
        /// 「削除」ボタン押下時の処理。
        /// </summary>
        private void DeleteFileExecute()
        {
            // DataGridから監視ファイルを削除
            if(null !=SelectedItem)
            {
                // マルチスレッドで監視する対象からも外す
                System.IO.FileInfo deleteFileInfo = new System.IO.FileInfo(SelectedItem.FilePath);
                SelectedItem.isWatching = false;
                Items.Remove(SelectedItem); // 実施後にSelectedItemがnullになる
            }
        }

        public void DetectionModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            // 変更されたプロパティに対応するVMのプロパティに、変更値を反映
            if (args.PropertyName == "Exist")
            {
                MainModel sendModel = (MainModel)sender;
                String changedFilePath = sendModel.Exist;
                ObservableCollection<WatchFileInfo> copyItems = new ObservableCollection<WatchFileInfo>(Items);

                foreach (WatchFileInfo item in copyItems)
                {
                    if (item.FilePath.Equals(changedFilePath))
                    {
                        int index = Items.IndexOf(item);
                        copyItems.Remove(item);
                        WatchFileInfo updataItem = new WatchFileInfo(item.FilePath);
                        copyItems.Insert(index, updataItem);
                        // 遷移先でRaisePropertyChanged("Itmes");を実施
                        Items = copyItems;

                        break;
                    }
                }
            }
            else if (args.PropertyName == "NotExist")
            {
                MainModel sendModel = (MainModel)sender;
                String deleteFilePath = sendModel.NotExist;
                ObservableCollection<WatchFileInfo> copyItems = new ObservableCollection<WatchFileInfo>(Items);

                foreach (WatchFileInfo item in copyItems)
                {
                    if (item.FilePath.Equals(deleteFilePath))
                    {
                        int index = Items.IndexOf(item);
                        copyItems.Remove(item);
                        WatchFileInfo updataItem = new WatchFileInfo(item.FilePath);
                        copyItems.Insert(index, updataItem);
                        // 遷移先でRaisePropertyChanged("Itmes");を実施
                        Items = copyItems;

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// デストラクタ。
        /// </summary>
        ~MainViewModel()
        {
            model.PropertyChanged -= this.DetectionModelPropertyChanged;
        }
    }
}
