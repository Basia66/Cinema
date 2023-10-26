using Cinema.Classes;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
    /// Logika interakcji dla klasy MoviesPage2.xaml
    /// </summary>
    public partial class MoviesPage2 : Page
    {
        NavigationService navService;
        public MoviesPage2()
        {
            InitializeComponent();
            Get_DBcontent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            navService = NavigationService.GetNavigationService(this);
            MainWindow.sre.SpeechRecognized += Sre_SpeechRecognized;
        }
        private void button_previous_Click(object sender, RoutedEventArgs e)
        {
            navService.Navigate(new Uri("Pages/MoviesPage.xaml", UriKind.RelativeOrAbsolute));
        }
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            Navigate(e);
        }

        private void Get_DBcontent()
        {
            string sSQL = "SELECT * FROM Movies;";
            DataTable tbl = ClsDB.Get_DataTable(sSQL);

            movie5_button.Content = tbl.Rows[5]["title"].ToString();
            movie6_button.Content = tbl.Rows[6]["title"].ToString();
            movie7_button.Content = tbl.Rows[7]["title"].ToString();
            movie8_button.Content = tbl.Rows[8]["title"].ToString();
            movie9_button.Content = tbl.Rows[9]["title"].ToString();

            ImageSource imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[5]["poster"].ToString(), UriKind.Relative));
            im5.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[6]["poster"].ToString(), UriKind.Relative));
            im6.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[7]["poster"].ToString(), UriKind.Relative));
            im7.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[8]["poster"].ToString(), UriKind.Relative));
            im8.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[9]["poster"].ToString(), UriKind.Relative));
            im9.Source = imgSource;
        }
        private void Navigate(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Semantics.ContainsKey("operation"))
            {
                if (e.Result.Semantics["operation"].Value.ToString() == "wstecz")
                {
                    MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
                    navService.Navigate(new Uri("\\Pages\\MoviesPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }
            if (e.Result.Semantics.ContainsKey("titles"))
            {
                Singleton.Instance.movie_title = e.Result.Semantics["titles"].Value.ToString();
                Console.WriteLine(Singleton.Instance.movie_title);
                MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
                navService.Navigate(new Uri("\\Pages\\MovieDetailsPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}