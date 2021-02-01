using System;
using System.Windows.Input;

namespace GitHubNotifier.ViewModels.Commands
{
    class RelayCommand : ICommand
    {
        private readonly Action action;
        private readonly Func<bool> canExecute;

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 常に実行可能なコマンドを作成する。
        /// </summary>
        /// <param name="action">実行するアクション</param>
        public RelayCommand(Action action) : this(action, null) { }

        /// <summary>
        /// 新しいコマンドを作成する。
        /// </summary>
        /// <param name="action">実行するアクション</param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action action, Func<bool> canExecute)
        {
            this.action = action ?? throw new ArgumentException();
            this.canExecute = canExecute;

        }

        /// <summary>
        /// コマンドを実行できるか判定する。
        /// </summary>
        /// <param name="parameter">コマンドが使用するデータ</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        /// <summary>
        /// コマンドを実行する。
        /// </summary>
        /// <param name="parameter">コマンドが使用するデータ</param>
        public void Execute(object parameter)
        {
            action();
        }

        /// <summary>
        /// <see cref="CanExecuteChanged"/>を発生させる。
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
