using System.Windows;

namespace GitHubNotifier.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static private MainWindow window = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        static public void Execute()
        {
            if (window == null)
            {
                window = new MainWindow();
                window.Closed += (s, e) => window = null;
                window.Show();
            }
            window.Activate();
        }
    }
}
