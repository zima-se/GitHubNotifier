using GitHubNotifier.Services;
using GitHubNotifier.Views.Controls;
using System;
using System.Windows;
using System.Windows.Forms;

namespace GitHubNotifier
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public NotifyIconWrapper notifyIcon;
        public NotificationService notificationService;

        /// <summary>
        /// アプリケーション開始時呼ばれる。
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // 前バージョンからのUpgradeを実行していないときは、Upgradeを実施する
            if (GitHubNotifier.Properties.Settings.Default.IsUpgraded == false)
            {
                // Upgradeを実行する
                GitHubNotifier.Properties.Settings.Default.Upgrade();
                // 「Upgradeを実行した」という情報を設定する
                GitHubNotifier.Properties.Settings.Default.IsUpgraded = true;
                // 現行バージョンの設定を保存する
                GitHubNotifier.Properties.Settings.Default.Save();
            }

            // NotifyIconを初期化
            Uri iconUri = new Uri("pack://application:,,,/GitHubNotifier;component/Resources/NotifyIconWhite.ico");
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem toolStripMenuItemOpen = new ToolStripMenuItem("Open", null, ToolStripMenuItemOpen_Click);
            ToolStripMenuItem toolStripMenuItemExit = new ToolStripMenuItem("Exit", null, ToolStripMenuItemExit_Click);
            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                toolStripMenuItemOpen,
                toolStripMenuItemExit
            });
            notifyIcon = new NotifyIconWrapper(iconUri, "GitHub Notifier", contextMenu);
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            // NotificationServiceを初期化
            notificationService = new NotificationService();
            // アクセス情報検証済みの場合通知の取得を開始
            if (GitHubNotifier.Properties.Settings.Default.IsValidated)
            {
                notificationService.Run();
            }
        }

        /// <summary>
        /// アプリケーション終了時呼ばれる。
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            notifyIcon.Dispose();
            notificationService.Dispose();
        }

        /// <summary>
        /// NotifyIconダブルクリック時呼ばれる。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // MainWindowを表示する
            Views.MainWindow.Execute();
        }

        /// <summary>
        /// コンテキストメニュー"Open"選択時呼ばれる。
        /// MainWindowを表示する
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void ToolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            // MainWindowを表示する
            Views.MainWindow.Execute();
        }

        /// <summary>
        /// コンテキストメニュー"Exit"選択時呼ばれる。
        /// アプリケーションを終了する。
        /// </summary>
        /// <param name="sender">呼び出し元オブジェクト</param>
        /// <param name="e">イベントデータ</param>
        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            // アプリケーションを終了する
            App.Current.Shutdown();
        }
    }
}
