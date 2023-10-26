using Cinema.Classes;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
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
using static System.Net.Mime.MediaTypeNames;

namespace Cinema.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        NavigationService navService;
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Singleton.Instance.GetPrizeFromBase();
            navService = NavigationService.GetNavigationService(this);
            string welcome = "Witamy w Kinie Wisła. W czym mogę pomóc?";
            MainWindow.sre.SpeechRecognized += Sre_SpeechRecognized;

            MainWindow.ss.SpeakAsync(welcome);
            text0.Text = welcome;
        }
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            Console.WriteLine(txt);
            Choices(e);

            MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
            Singleton.Instance.MainNavigate(navService);
        }
        private void Choices(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Semantics.ContainsKey("ticket_type")) //ulgowy/normalny/seniorski oraz ich liczba
            {
                string ticket_type = e.Result.Semantics["ticket_type"].Value.ToString();
                if (e.Result.Semantics.ContainsKey("count"))
                {
                    setTicketCount(ticket_type, e, "count");
                }
            }

            if (e.Result.Semantics.ContainsKey("movie")) //chce obejrzeć film
            {
                Singleton.Instance.movie_title = e.Result.Semantics["movie"].Value.ToString();
                Console.WriteLine(e.Result.Semantics["movie"].Value.ToString());
                
                if (e.Result.Semantics.ContainsKey("time")) // czas filmu
                {
                    string hour = e.Result.Semantics["time"].Value.ToString();
                    Singleton.Instance.hour_of_movie = hour;
                    Singleton.Instance.cinema_hall = Singleton.Instance.SearchCinemaHall(Singleton.Instance.movie_title, hour);
                    if(Singleton.Instance.cinema_hall == -1)
                    {
                        Singleton.Instance.hour_of_movie = null;
                    }

                    Console.WriteLine(Singleton.Instance.hour_of_movie);
                }
            }
        }
        private void setTicketCount(string ticket_type, SpeechRecognizedEventArgs e, string key)
        {
            if (ticket_type == "ulgowy")
            {
                if (e.Result.Semantics.ContainsKey(key))
                {
                    Singleton.Instance.number_of_student_ticket = Convert.ToInt32(e.Result.Semantics[key].Value);
                }
                else
                {
                    Singleton.Instance.number_of_student_ticket = 1;
                }
                Console.WriteLine("ulgowych: " + Singleton.Instance.number_of_student_ticket);
            }

            if (ticket_type == "normalny")
            {
                if (e.Result.Semantics.ContainsKey(key))
                {
                    Singleton.Instance.number_of_normal_ticket = Convert.ToInt32(e.Result.Semantics[key].Value);
                }
                else
                {
                    Singleton.Instance.number_of_normal_ticket = 1;
                }
                Console.WriteLine("normalnych: " + Singleton.Instance.number_of_normal_ticket);
            }

            if (ticket_type == "seniorski")
            {
                if (e.Result.Semantics.ContainsKey(key))
                {
                    Singleton.Instance.number_of_senior_ticket = Convert.ToInt32(e.Result.Semantics[key].Value);
                }
                else
                {
                    Singleton.Instance.number_of_senior_ticket = 1;
                }
                Console.WriteLine("seniorski: " + Singleton.Instance.number_of_senior_ticket);
            }
        }
    }
}