using GitHubNotifier.ViewModels.Commands;
using Octokit;
using System;
using System.Collections.Generic;

namespace GitHubNotifier.ViewModels
{
    /// <summary>
    /// メインウィンドウViewModelクラス
    /// </summary>
    class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// GitHubサーバのルートURL
        /// </summary>
        private string strRootUrl;
        public string StrRootUrl
        {
            get { return strRootUrl; }
            set
            {
                SetProperty(ref strRootUrl, value);
                RegisterCommand.RaiseCanExecuteChanged();
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// 個人アクセストークン
        /// </summary>
        private string strToken;
        public string StrToken
        {
            get { return strToken; }
            set
            {
                SetProperty(ref strToken, value);
                RegisterCommand.RaiseCanExecuteChanged();
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        private string strError;
        public string StrError
        {
            get { return strError; }
            set { SetProperty(ref strError, value); }
        }

        /// <summary>
        /// Registerボタン押下時のコマンド
        /// </summary>
        private RelayCommand registerCommand;
        public RelayCommand RegisterCommand
        {
            get { return registerCommand ??= new RelayCommand(Register, CanRegister); }
        }

        private bool isRegistering;

        public MainWindowViewModel()
        {
            string strRootUrl = Properties.Settings.Default.RootURL;
            if (!string.IsNullOrEmpty(strRootUrl))
            {
                StrRootUrl = strRootUrl;
            }
            string strToken = Properties.Settings.Default.Token;
            if (!string.IsNullOrEmpty(strToken))
            {
                StrToken = strToken;
            }
            isRegistering = false;
        }

        /// <summary>
        /// GitHubへのアクセス情報を登録する。
        /// </summary>
        private async void Register()
        {
            isRegistering = true;
            bool hasError = false;
            RegisterCommand.RaiseCanExecuteChanged();
            ((App)App.Current).notificationService.Stop();

            ProductHeaderValue phv = new ProductHeaderValue("GitHubNotifier");
            Uri uri = new Uri(StrRootUrl);
            Credentials tokenAuth = new Credentials(StrToken);

            // URLが有効かチェック
            if (!uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase) &&
                !uri.Host.Equals("api.github.com", StringComparison.OrdinalIgnoreCase))
            {
                Properties.Settings.Default.IsEnterprise = true;
                EnterpriseProbe probe = new EnterpriseProbe(phv);
                EnterpriseProbeResult result = await probe.Probe(uri);
                switch (result)
                {
                    case EnterpriseProbeResult.Failed:
                        StrError = Properties.Resources.NetworkErrorText;
                        hasError = true;
                        break;
                    case EnterpriseProbeResult.NotFound:
                        StrError = Properties.Resources.ServerErrorText;
                        hasError = true;
                        break;
                }
            }
            else
            {
                Properties.Settings.Default.IsEnterprise = false;
            }
            
            if (!hasError)
            {
                // 通知を取得する
                GitHubClient client = new GitHubClient(phv, uri)
                {
                    Credentials = tokenAuth
                };
                try
                {
                    IReadOnlyList<Notification> notifications = await client.Activity.Notifications.GetAllForCurrent();
                }
                catch (AuthorizationException)
                {
                    StrError = Properties.Resources.TokenErrorText;
                    hasError = true;
                }
                catch (ForbiddenException)
                {
                    StrError = Properties.Resources.PermissionErrorText;
                    hasError = true;
                }
            }

            if (hasError)
            {
                Properties.Settings.Default.IsValidated = false;
            }
            else
            {
                // 入力値を保存する
                Properties.Settings.Default.RootURL = StrRootUrl;
                Properties.Settings.Default.Token = StrToken;
                Properties.Settings.Default.IsValidated = true;
                Properties.Settings.Default.Save();
                // NotificationServiceを開始する
                ((App)App.Current).notificationService.Run();
                // ウィンドウを閉じる
                App.Current.MainWindow.Close();
            }

            isRegistering = false;
            RegisterCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// ボタンを押下できるか判定する。
        /// </summary>
        /// <returns>true: 可能 false: 不可</returns>
        private bool CanRegister()
        {
            if (string.IsNullOrEmpty(StrRootUrl) || string.IsNullOrEmpty(StrToken))
            {
                return false;
            }
            if (isRegistering == true)
            {
                return false;
            }
            return true;
        }
        
    }
}
