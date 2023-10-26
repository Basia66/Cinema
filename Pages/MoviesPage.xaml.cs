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
    /// Logika interakcji dla klasy MoviesPage.xaml
    /// </summary>
    public partial class MoviesPage : Page
    {
        NavigationService navService;
        public MoviesPage()
        {
            InitializeComponent();
            Get_DBcontent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.ActivateGrammar(MainWindow.filmy);
            navService = NavigationService.GetNavigationService(this);
            MainWindow.sre.SpeechRecognized += Sre_SpeechRecognized;
            MainWindow.ss.SpeakAsync("Który film chcesz obejrzeć?");
        }
        private void button_next_Click(object sender, RoutedEventArgs e)
        {
            navService.Navigate(new Uri("Pages/MoviesPage2.xaml", UriKind.RelativeOrAbsolute));
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

            movie0_button.Content = tbl.Rows[0]["title"].ToString();
            movie1_button.Content = tbl.Rows[1]["title"].ToString();
            movie2_button.Content = tbl.Rows[2]["title"].ToString();
            movie3_button.Content = tbl.Rows[3]["title"].ToString();
            movie4_button.Content = tbl.Rows[4]["title"].ToString();

            ImageSource imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[0]["poster"].ToString(), UriKind.Relative));
            im0.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[1]["poster"].ToString(), UriKind.Relative));
            im1.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[2]["poster"].ToString(), UriKind.Relative));
            im2.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[3]["poster"].ToString(), UriKind.Relative));
            im3.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[4]["poster"].ToString(), UriKind.Relative));
            im4.Source = imgSource;
        }
        private void Navigate(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Semantics.ContainsKey("operation"))
            {
                if (e.Result.Semantics["operation"].Value.ToString() == "dalej")
                {
                    MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
                    navService.Navigate(new Uri("\\Pages\\MoviesPage2.xaml", UriKind.RelativeOrAbsolute));
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