using System;

namespace GitHubNotifier.Views.Controls
{
    public enum ToolTipIcon
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    public class NotifyIconWrapper: IDisposable
    {
        private readonly System.Windows.Forms.NotifyIcon notifyIcon;

        public string Text
        {
            get { return notifyIcon.Text; }
            set { notifyIcon.Text = value; }
        }

        public bool Visible
        {
            get { return notifyIcon.Visible; }
            set { notifyIcon.Visible = value; }
        }

        public Uri IconUri
        {
            set
            {
                if (value == null)
                {
                    return;
                }
                System.IO.Stream iconStream = System.Windows.Application.GetResourceStream(value).Stream;
                notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            }
        }

        public System.Windows.Forms.ContextMenuStrip ContextMenu {
            get { return notifyIcon.ContextMenuStrip; }
            set { notifyIcon.ContextMenuStrip = value; }
        }

        public ToolTipIcon BaloonTipIcon
        {
            get { return (ToolTipIcon)notifyIcon.BalloonTipIcon; }
            set { notifyIcon.BalloonTipIcon = (System.Windows.Forms.ToolTipIcon)value; }
        }

        public string BalloonTipTitle
        {
            get { return notifyIcon.BalloonTipTitle; }
            set { notifyIcon.BalloonTipTitle = value; }
        }

        public string BalloonTipText
        {
            get { return notifyIcon.BalloonTipText; }
            set { notifyIcon.BalloonTipText = value; }
        }

        public void ShowBalloonTip(int timeOut, string tipTitle, string tipText, ToolTipIcon tipIcon)
        {
            System.Windows.Forms.ToolTipIcon icon = (System.Windows.Forms.ToolTipIcon)tipIcon;
            notifyIcon.ShowBalloonTip(timeOut, tipTitle, tipText, icon);
        }

        public NotifyIconWrapper() : this(null) { }
        public NotifyIconWrapper(Uri iconUri) : this(iconUri, null) { }
        public NotifyIconWrapper(Uri iconUri, string text) : this(iconUri, text, null) { }
        public NotifyIconWrapper(Uri iconUri, string text, System.Windows.Forms.ContextMenuStrip contextMenu)
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            IconUri = iconUri;
            Text = text;
            ContextMenu = contextMenu;
            notifyIcon.Visible = true;
        }

        #region NotifyIcon Events
        public event EventHandler BalloonTipClicked
        {
            add { notifyIcon.BalloonTipClicked += value; }
            remove { notifyIcon.BalloonTipClicked -= value; }
        }

        public event EventHandler BalloonTipClosed
        {
            add { notifyIcon.BalloonTipClosed += value; }
            remove { notifyIcon.BalloonTipClosed -= value; }
        }

        public event EventHandler BalloonTipShown
        {
            add { notifyIcon.BalloonTipShown += value; }
            remove { notifyIcon.BalloonTipShown -= value; }
        }

        public event EventHandler Click
        {
            add { notifyIcon.Click += value; }
            remove { notifyIcon.Click -= value; }
        }

        public event EventHandler DoubleClick
        {
            add { notifyIcon.DoubleClick += value; }
            remove { notifyIcon.DoubleClick -= value; }
        }

        public event EventHandler Disposed
        {
            add { notifyIcon.Disposed += value; }
            remove { notifyIcon.Disposed -= value; }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    notifyIcon.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }
        #endregion

    }
}