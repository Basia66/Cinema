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
    /// Logika interakcji dla klasy MovieDetailsPage.xaml
    /// </summary>
    public partial class MovieDetailsPage : Page
    {
        NavigationService navService;
        List<string> hours = new List<string>();
        public MovieDetailsPage()
        {
            InitializeComponent();
            FillDataByMovieTitle();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            navService = NavigationService.GetNavigationService(this);
            MainWindow.ss.SpeakAsync("O jakiej godzinie chcesz obejrzeć film?");
            MainWindow.sre.SpeechRecognized += Sre_SpeechRecognized;
        }
        
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            Choices(e);
        }

        public void FillDataByMovieTitle()
        {
            string movie_title = Singleton.Instance.movie_title;

            string sSQL = "SELECT * FROM Movies WHERE title LIKE @movie_title";
            DataTable tbl = ClsDB.Get_DataTable(sSQL, new List<object[]> { new object[] { "@movie_title", "%" + movie_title + "%" } });
            string description = tbl.Rows[0]["description"].ToString();
            string poster = tbl.Rows[0]["poster"].ToString();

            movieTitle.Content = movie_title;
            movieDesc.Text = description;

            ImageSource imgSource = new BitmapImage(new Uri("../Graphics/" + poster, UriKind.Relative));
            movieImage.Source = imgSource;

            int movie_id = Singleton.Instance.GetMovieIdByTitle(movie_title);

            string sSQL1 = "SELECT * FROM Hours WHERE movie_id=@movie_id";
            DataTable tbl1 = ClsDB.Get_DataTable(sSQL1, new List<object[]> { new object[] { "@movie_id", movie_id.ToString() } });

            for (int i = 0; i < 5; i++)
            {
                hours.Add(tbl1.Rows[i]["hours"].ToString());
            }

            hour0.Content = hours[0];
            hour1.Content = hours[1];
            hour2.Content = hours[2];
            hour3.Content = hours[3];
            hour4.Content = hours[4];
        }

        private void Choices(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Semantics.ContainsKey("time"))
            {
                string hour = e.Result.Semantics["time"].Value.ToString();
                if (hours.Contains(hour))
                {
                    Singleton.Instance.hour_of_movie = hour;
                    Singleton.Instance.cinema_hall = Singleton.Instance.SearchCinemaHall(Singleton.Instance.movie_title, hour);
                    Console.WriteLine(Singleton.Instance.hour_of_movie);
                    MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
                    Singleton.Instance.MainNavigate(navService);
                }
                else
                {
                    MainWindow.ss.SpeakAsync("Nie ma filmu o takiej godzinie");
                    MainWindow.sre.SpeechRecognized += Sre_SpeechRecognized;
                }

            }
        }

    }
}
