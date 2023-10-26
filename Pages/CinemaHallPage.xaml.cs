using Cinema.Classes;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
    /// Logika interakcji dla klasy CinemaHallPage.xaml
    /// </summary>
    public partial class CinemaHallPage : Page
    {
        NavigationService navService;
        string movie_title = Singleton.Instance.movie_title;
        string hours = Singleton.Instance.hour_of_movie;
        int cinema_hall = Singleton.Instance.cinema_hall;

        int seats_to_allocate;
        Dictionary<int, List<int>> ss;

        int row = 0;
        List<int> seats;
        //int seat1 = 0;
        //int seat2 = 0;

        public CinemaHallPage()
        {
            InitializeComponent();
            ColorTakenSeatsFromBase(movie_title, hours, cinema_hall);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.ActivateGrammar(MainWindow.miejsca);
            navService = NavigationService.GetNavigationService(this);

            // TODO: sprawdzić możliwości sali

            seats_to_allocate = Singleton.Instance.number_of_normal_ticket + 
                Singleton.Instance.number_of_senior_ticket + 
                Singleton.Instance.number_of_student_ticket;
            ss = new Dictionary<int, List<int>>();

            MainWindow.ss.SpeakAsync("Liczba miejsc do wybrania to: "+seats_to_allocate+".Wybierz miejsca i rząd.");
            MainWindow.sre.SpeechRecognized += Sre_SpeechRecognized;
        }
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            seats = new List<int>();
            
            string txt = e.Result.Text;
            Console.WriteLine(txt);

            if (e.Result.Semantics.ContainsKey("place_in_cinema1") && e.Result.Semantics.ContainsKey("place_in_cinema2"))
            {
                string name_seat = e.Result.Semantics["place_in_cinema1"].Value.ToString();
                //Console.WriteLine("typ: " + name_seat);
                string name_seat2 = e.Result.Semantics["place_in_cinema2"].Value.ToString();
                //Console.WriteLine("typ: " + name_seat2);

                if (e.Result.Semantics.ContainsKey("count"))
                {
                    List<string> numbers = e.Result.Semantics["count"].Value.ToString().Split(' ').ToList();
                    numbers.RemoveAt(0); // na początku zawsze będzie undefined
                    foreach(string number in numbers)
                    {
                        int seat = Convert.ToInt32(number);
                        if (seat < 0 || seat > 5 || seat < 0 || seat > 5)
                        {
                            MainWindow.ss.Speak("Numery miejsc są błędne, wybierz jeszcze raz");
                            return;
                        }

                        Console.WriteLine("Miejsce: " + number);
                        seats.Add(seat);
                    }
                }
                else
                {
                    MainWindow.ss.SpeakAsync("Wybierz jeszcz raz miejsca i rząd");
                    return;
                }

                if (e.Result.Semantics.ContainsKey("count1"))
                {
                    int number_seat2 = Convert.ToInt32(e.Result.Semantics["count1"].Value);
                    row = number_seat2;
                    if (row < 0 || row > 5)
                    {
                        MainWindow.ss.Speak("Numer rzędy jest błędy, wybierz jeszcze raz");
                        return;
                    }
                    Console.WriteLine("Rząd: " + row.ToString());
                }
                else
                {
                    MainWindow.ss.SpeakAsync("Wybierz jeszcz raz miejsca i rząd");
                    return;
                }
            }
            else
            {
                MainWindow.ss.SpeakAsync("Wybierz jeszcz raz miejsca i rząd");
                return;
            }

            // sprawdzenie czy wybrano już te miejsca; normalnie złożonośc przepiękna :)
            foreach (int id in ss.Keys)
            {
                if(id == row)
                {
                    foreach (int seat in ss[id])
                    {
                        foreach (int se in seats)
                        {
                            if (seat == se)
                            {
                                MainWindow.ss.Speak("Wybrałeś już to miejsce, podaj poprawne");
                                return;
                            }
                        }
                    }
                }
            }
            
            if (seats.Count > seats_to_allocate)
            {
                MainWindow.ss.Speak("Wybrałeś za dużo miejsc");
                return;
            }

            bool result = ChoiceSeats(row, seats);
            seats_to_allocate -= seats.Count;
            if (result && seats_to_allocate <= 0)
            {
                MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
                if (ss.ContainsKey(row))
                {
                    ss[row].Concat(seats);
                }
                else
                {
                    ss.Add(row, seats);
                }
                Singleton.Instance.seats = ss;
                AllocateChoise();
                navService.Navigate(new Uri("\\Pages\\FoodInformationPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else if (result)
            {
                if(ss.ContainsKey(row))
                {
                    ss[row].Concat(seats);
                } 
                else
                {
                    ss.Add(row, seats);
                }
                
                MainWindow.ss.SpeakAsync("Pozostało jeszcze " + seats_to_allocate + " miejs do wybrania");
            }
            else
            {
                MainWindow.ss.Speak("Miejsce pokolorowane na czerwono są zajęte. Wybierz inne miejsca.");
            }
        }

        public void AllocateChoise()
        {
            Singleton.Instance.seats = ss;
            Singleton.Instance.place = Singleton.Instance.CreatePlace();
            int movie_id = Singleton.Instance.GetMovieIdByTitle(movie_title);
            string sSQL = "UPDATE Hall SET available = 'N' WHERE movie_id=@movie_id AND cinema_hall=@cinema_hall AND row=@row AND seat=@seat AND hours LIKE @hour_of_movie;";

            foreach (int id in ss.Keys)
            {
                foreach (int seat in ss[id])
                {
                    ClsDB.Update_Data(sSQL, new List<object[]> {
                            new object[] { "@movie_id", movie_id },
                            new object[] { "@cinema_hall", cinema_hall },
                            new object[] { "@row", id },
                            new object[] { "@seat", seat },
                            new object[] { "@hour_of_movie", "%"+Singleton.Instance.hour_of_movie+"%" }
                        });
                }
            }
        }

        public bool ChoiceSeats(int row, List<int> seats)
        {

            Dictionary<int, List<int>> places = new Dictionary<int, List<int>>();
            places.Add(row, seats);

            Dictionary<int, List<int>> free_places = new Dictionary<int, List<int>>(places);

            List<int[]> taken_seats = Singleton.Instance.IsSeatFree(places, movie_title, hours, cinema_hall);

            foreach (var taken_seat in taken_seats)
            {
                var taken_row = taken_seat[0];
                var taken_seat_in_row = taken_seat[1];
                free_places[taken_row].Remove(taken_seat_in_row);
            }

            //Singleton.Instance.seats = free_places;

            if (taken_seats.Count == 0)
            {
                ColorSeats(places, Colors.Red);
                ColorSeats(free_places, Colors.Gray);
                return true;
            }
            else
            {
                return false;
            }

        }

        public void ColorSeats(Dictionary<int, List<int>> seats, Color color)
        {
            for (int i = 0; i < seats.Count; i++)
            {
                for (int j = 0; j < seats.ElementAt(i).Value.Count; j++)
                {
                    int seat = seats.ElementAt(i).Value[j];
                    int row = seats.ElementAt(i).Key;

                    if (row == 1 && seat == 1)
                    {
                        seat11.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 1 && seat == 2)
                    {
                        seat12.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 1 && seat == 3)
                    {
                        seat13.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 1 && seat == 4)
                    {
                        seat14.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 1 && seat == 5)
                    {
                        seat15.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 2 && seat == 1)
                    {
                        seat21.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 2 && seat == 2)
                    {
                        seat22.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 2 && seat == 3)
                    {
                        seat23.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 2 && seat == 4)
                    {
                        seat24.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 2 && seat == 5)
                    {
                        seat25.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 3 && seat == 1)
                    {
                        seat31.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 3 && seat == 2)
                    {
                        seat32.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 3 && seat == 3)
                    {
                        seat33.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 3 && seat == 4)
                    {
                        seat34.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 3 && seat == 5)
                    {
                        seat35.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 4 && seat == 1)
                    {
                        seat41.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 4 && seat == 2)
                    {
                        seat42.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 4 && seat == 3)
                    {
                        seat43.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 4 && seat == 4)
                    {
                        seat44.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 4 && seat == 5)
                    {
                        seat45.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 5 && seat == 1)
                    {
                        seat51.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 5 && seat == 2)
                    {
                        seat52.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 5 && seat == 3)
                    {
                        seat53.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 5 && seat == 4)
                    {
                        seat54.Fill = new SolidColorBrush(color);
                    }
                    else if (row == 5 && seat == 5)
                    {
                        seat55.Fill = new SolidColorBrush(color);
                    }
                }
            }
        }

        public void ColorTakenSeatsFromBase(string movie_title, string hour_of_movie, int cinema_hall)
        {
            string sSQL1 = @"SELECT * FROM Hall WHERE 
                                        movie_id=@movie_id AND 
                                        hours LIKE @hour_of_movie AND 
                                        cinema_hall=@cinema_hall";

            int movie_id = Singleton.Instance.GetMovieIdByTitle(movie_title);

            DataTable tbl1 = ClsDB.Get_DataTable(sSQL1, new List<object[]> {
                        new object[] { "@movie_id", movie_id },
                        new object[] { "@hour_of_movie", "%"+Singleton.Instance.hour_of_movie+"%" },
                        new object[] { "@cinema_hall", cinema_hall }});

            for (int i = 0; i < tbl1.Rows.Count; i++)
            {
                int seat = (int)tbl1.Rows[i]["seat"];
                int row = (int)tbl1.Rows[i]["row"];
                string available = tbl1.Rows[i]["available"].ToString();

                if (available == "N")
                {
                    if (row == 1 && seat == 1)
                    {
                        seat11.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 1 && seat == 2)
                    {
                        seat12.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 1 && seat == 3)
                    {
                        seat13.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 1 && seat == 4)
                    {
                        seat14.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 1 && seat == 5)
                    {
                        seat15.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 2 && seat == 1)
                    {
                        seat21.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 2 && seat == 2)
                    {
                        seat22.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 2 && seat == 3)
                    {
                        seat23.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 2 && seat == 4)
                    {
                        seat24.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 2 && seat == 5)
                    {
                        seat25.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 3 && seat == 1)
                    {
                        seat31.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 3 && seat == 2)
                    {
                        seat32.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 3 && seat == 3)
                    {
                        seat33.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 3 && seat == 4)
                    {
                        seat34.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 3 && seat == 5)
                    {
                        seat35.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 4 && seat == 1)
                    {
                        seat41.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 4 && seat == 2)
                    {
                        seat42.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 4 && seat == 3)
                    {
                        seat43.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 4 && seat == 4)
                    {
                        seat44.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 4 && seat == 5)
                    {
                        seat45.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 5 && seat == 1)
                    {
                        seat51.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 5 && seat == 2)
                    {
                        seat52.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 5 && seat == 3)
                    {
                        seat53.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 5 && seat == 4)
                    {
                        seat54.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    else if (row == 5 && seat == 5)
                    {
                        seat55.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                }
            }
        }
    }
}
