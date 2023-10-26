using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cinema.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy GoodbyePage.xaml
    /// </summary>
    public partial class GoodbyePage : Page
    {
        NavigationService navService;
        public GoodbyePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            navService = NavigationService.GetNavigationService(this);
            MainWindow.ss.SpeakAsync("Dziękujemy za skorzystanie z naszego kina. Miłego seansu.");
            text2.Text = "Dziękujemy za skorzystanie z naszego kina. Miłego seansu.";

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(5000);
        }

    }
}
