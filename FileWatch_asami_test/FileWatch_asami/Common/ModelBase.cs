using System.ComponentModel;

namespace FileWatch_asami.Common
{
    /// <summary>
    /// Modelの基本クラス。
    /// ModelからViewModelへの通知を実装。
    /// </summary>
    public class ModelBase
    {
        /// <summary>
        /// プロパティ変更をViewModelに通知するためのイベントハンドラ
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティ変更をViewModelに通知する処理
        /// </summary>
        /// <paramname="propertyName">プロパティ名</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
