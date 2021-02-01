using GitHubNotifier.Views;
using GitHubNotifier.Views.Controls;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace GitHubNotifier.Services
{
    /// <summary>
    /// 定期的に通知を確認するサービスクラス
    /// </summary>
    public class NotificationService
    {
        private Timer timer;
        private readonly TimerCallback callback;
        private readonly int dueTime = 0;
        private readonly int period = 20000;
        private readonly int balloonTimeout = 5000;
        private bool hasError = false;
        private string linkUrl;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NotificationService()
        {
            // タイマーのコールバックを設定
            callback = new TimerCallback(GetNotifications);
            // バルーンチップクリックのイベントハンドラを設定
            ((App)App.Current).notifyIcon.BalloonTipClicked += BalloonTip_Click;
        }

        /// <summary>
        /// サービスを開始する。
        /// </summary>
        public void Run()
        {
            // 設定からアクセス情報を読み込み
            string strRootUrl = Properties.Settings.Default.RootURL;
            string strToken = Properties.Settings.Default.Token;

            // GitHubクライアントを設定
            ProductHeaderValue phv = new ProductHeaderValue("GitHubNotifier");
            Uri uri = new Uri(strRootUrl);
            Credentials tokenAuth = new Credentials(strToken);
            GitHubClient client = new GitHubClient(phv, uri)
            {
                Credentials = tokenAuth
            };
            timer = new Timer(callback, client, dueTime, period);
        }

        /// <summary>
        /// サービスを停止する。
        /// </summary>
        public void Stop()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);

            }
        }

        /// <summary>
        /// サービスを破棄する。
        /// </summary>
        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
            }
            
        }

        /// <summary>
        /// 通知を取得しバルーンチップを表示
        /// </summary>
        private async void GetNotifications(object state)
        {
            GitHubClient client = (GitHubClient)state;
            NotificationsRequest request = new NotificationsRequest();
            if (Properties.Settings.Default.LastCheckedTime == new DateTime(1753, 1, 1))
            {
                request.Since = new DateTimeOffset(DateTime.Now);
            }
            else
            {
                request.Since = new DateTimeOffset(Properties.Settings.Default.LastCheckedTime);
            }
            DateTime requestDateTime = DateTime.Now;
            IReadOnlyList<Notification> notifications;
            try
            {
                notifications = await client.Activity.Notifications.GetAllForCurrent(request);
            }
            catch
            {
                hasError = true;
                string errorTitle = Properties.Resources.NotificationErrorTitle;
                string errorText = Properties.Resources.NotificationErrorText;
                ((App)App.Current).notifyIcon.ShowBalloonTip(balloonTimeout, errorTitle, errorText, ToolTipIcon.Error);
                Stop();
                return;
            }
            hasError = false;
            // 問い合わせ時刻を保存
            Properties.Settings.Default.LastCheckedTime = requestDateTime;
            Properties.Settings.Default.Save();
            
            foreach(Notification notification in notifications)
            { 
                string strTitle = $"{notification.Repository.FullName} {notification.Subject.Title}";
                string strText;
                switch (notification.Reason)
                {
                    case "assign":
                        strText = Properties.Resources.AssignText;
                        break;
                    case "author":
                        strText = Properties.Resources.AuthorText;
                        break;
                    case "comment":
                        strText = Properties.Resources.CommentText;
                        break;
                    case "invitation":
                        strText = Properties.Resources.CommentText;
                        break;
                    case "manual":
                        strText = Properties.Resources.ManualText;
                        break;
                    case "mention":
                        strText = Properties.Resources.MentionText;
                        break;
                    case "review_requested":
                        strText = Properties.Resources.ReviewRequestedText;
                        break;
                    case "security_alert":
                        strText = Properties.Resources.SecurityAlertText;
                        break;
                    case "state_change":
                        strText = Properties.Resources.StateChangeText;
                        break;
                    case "subscribed":
                        strText = Properties.Resources.SubscribedText;
                        break;
                    case "team_mention":
                        strText = Properties.Resources.TeamMentionText;
                        break;
                    default:
                        continue;
                }
                linkUrl = notification.Subject.Url;
                ((App)App.Current).notifyIcon.ShowBalloonTip(balloonTimeout, strTitle, strText, ToolTipIcon.Info);
            }
        }

        /// <summary>
        /// ブラウザで通知を表示する。
        /// </summary>
        private void BalloonTip_Click(object sender, EventArgs e)
        {
            // エラーがある場合メインウィンドウを開く
            if (hasError)
            {
                MainWindow.Execute();
                return;
            }
            // ブラウザで通知ページを開く
            // Uri rootUrl = Properties.Settings.Default.IsEnterprise
            //             ? new Uri(Properties.Settings.Default.RootURL) : new Uri("https://github.com/");
            // string strUrl = new Uri(rootUrl, "notifications").ToString();
            Process.Start(new ProcessStartInfo(ConvertUrl()) { UseShellExecute = true });
        }

        /// <summary>
        /// APIエンドポイントのURLをGitHubのURLに変換する。
        /// </summary>
        /// <returns>変換後のURL</returns>
        private string ConvertUrl()
        {
            linkUrl = linkUrl.Replace("/pulls/", "/pull/");
            if (linkUrl == null)
            {
                return null;
            }
            if (Properties.Settings.Default.IsEnterprise)
            {
                return linkUrl.Replace("api/v3/", "");
            }
            else
            {
                return linkUrl.Replace("https://api.github.com/repos/", "https://github.com/");
            }
        }
    }
}
