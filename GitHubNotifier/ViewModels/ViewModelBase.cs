using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GitHubNotifier.ViewModels
{
    /// <summary>
    /// ViewModelベースクラス
    /// </summary>
    class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティの変更を通知するためのマルチキャストイベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティを設定しリスナーに通知する。
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="storage">参照するプロパティ</param>
        /// <param name="value">プロパティに設定する値</param>
        /// <param name="propertyName">プロパティの名前</param>
        /// <returns>true: 値が変更された false: 変更が不要</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// プロパティが変更されたことをリスナーに通知する。
        /// </summary>
        /// <param name="propertyName">リスナーに通知するプロパティの名前</param>
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
