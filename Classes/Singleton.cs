using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Cinema.Classes
{
    public sealed class Singleton
    {
        private static readonly Singleton instance = new Singleton();

        public string movie_title;
        public string hour_of_movie;

        public int number_of_normal_ticket = 0;
        public int number_of_student_ticket = 0;
        public int number_of_senior_ticket = 0;
        public int number_of_all_tickets = 0;

        public Dictionary<int, List<int>> seats = new Dictionary<int, List<int>>();
        public string place;
        public int cinema_hall;

        public bool popcorn = false;
        public bool nachos = false;
        public bool drink = false;

        public int number_of_small_popcorn = 0;
        public int number_of_medium_popcorn = 0;
        public int number_of_large_popcorn = 0;

        public int number_of_small_nachos = 0;
        public int number_of_medium_nachos = 0;
        public int number_of_large_nachos = 0;

        public int number_of_cola = 0;
        public int number_of_fanta = 0;
        public int number_of_sprite = 0;
        public int number_of_juice = 0;
        public int number_of_water = 0;

        public int small_popcorn_prize;
        public int medium_popcorn_prize;
        public int large_popcorn_prize;

        public int small_nachos_prize;
        public int medium_nachos_prize;
        public int large_nachos_prize;

        public int cola_price;
        public int fanta_price;
        public int sprite_price;
        public int juice_price;
        public int water_price;

        public int normal_ticket_price;
        public int student_ticket_price;
        public int senior_ticket_price;

        public int total_cost = 0;
        public string order;

        public int CountTickets()
        {
            number_of_all_tickets = number_of_normal_ticket + number_of_student_ticket + number_of_senior_ticket;
            return number_of_all_tickets;
        }

        public int TotalCost()
        {
            int total_cost_of_tickets = (number_of_normal_ticket * normal_ticket_price) + (number_of_student_ticket * student_ticket_price) + (number_of_senior_ticket * senior_ticket_price);
            int total_cost_of_popcorn = (number_of_small_popcorn * small_popcorn_prize) + (number_of_medium_popcorn * medium_popcorn_prize) + (number_of_large_popcorn * large_popcorn_prize);
            int total_cost_of_nachos = (number_of_small_nachos * small_nachos_prize) + (number_of_medium_nachos * medium_nachos_prize) + (number_of_large_nachos * large_nachos_prize);
            int total_cost_of_drinks = (number_of_cola * cola_price) + (number_of_fanta * fanta_price) + (number_of_sprite * sprite_price) + (number_of_juice * juice_price) + (number_of_water * water_price);
            total_cost = total_cost_of_tickets + total_cost_of_popcorn + total_cost_of_nachos + total_cost_of_drinks;
            
            return total_cost;
        }

        public void MainNavigate(NavigationService navService)
        {
            if (movie_title == null)
            {
                navService.Navigate(new Uri("\\Pages\\MoviesPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else if (hour_of_movie == null)
            {
                navService.Navigate(new Uri("\\Pages\\MovieDetailsPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else if (CountTickets() == 0)
            {
                navService.Navigate(new Uri("\\Pages\\TicketPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else if (seats.Count == 0)
            {
                navService.Navigate(new Uri("\\Pages\\CinemaHallPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                total_cost = TotalCost();
                navService.Navigate(new Uri("\\Pages\\OrderPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        public void NavigateFood(NavigationService navService)
        {
            if (Singleton.Instance.popcorn == true && Singleton.Instance.number_of_small_popcorn == 0 && Singleton.Instance.number_of_medium_popcorn == 0 && Singleton.Instance.number_of_large_popcorn == 0)
            {
                navService.Navigate(new Uri("\\Pages\\PopcornPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else if (Singleton.Instance.nachos == true && Singleton.Instance.number_of_small_nachos == 0 && Singleton.Instance.number_of_medium_nachos == 0 && Singleton.Instance.number_of_large_nachos == 0)
            {
                navService.Navigate(new Uri("\\Pages\\NachosPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else if (Singleton.Instance.drink == true && Singleton.Instance.number_of_cola == 0 && Singleton.Instance.number_of_fanta == 0 && Singleton.Instance.number_of_sprite == 0 && Singleton.Instance.number_of_juice == 0 && Singleton.Instance.number_of_water == 0)
            {
                navService.Navigate(new Uri("\\Pages\\DrinksPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                navService.Navigate(new Uri("\\Pages\\OrderPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        public string CreatePlace()
        {
            // stworzenie stringa "Miejsce: 2,3 Rząd: 3"
            place = "";
            for(int i = 0; i < seats.Count; i++)
            {
                if (i != 0 && i < seats.Count - 1)
                {
                    place += place + " , ";
                }

                string list_of_seats = String.Join(",", seats.ElementAt(i).Value.Select(n => n.ToString()).ToArray());
                place += "Miejsce: " + list_of_seats + ", Rząd: " + seats.ElementAt(i).Key.ToString();
            }
            return place;
        }
        
        public List<int[]> IsSeatFree(Dictionary<int, List<int>> places, string movie_title, string hour_of_movie, int cinema_hall)
        {
            List<int[]> taken_seats = new List<int[]>();
            for (int i = 0; i < places.Count; i++)
            {
                for (int j = 0; j < places.ElementAt(i).Value.Count; j++)
                {
                    int seat = places.ElementAt(i).Value[j];
                    int row = places.ElementAt(i).Key;

                    int movie_id = GetMovieIdByTitle(movie_title);

                    string sSQL1 = "SELECT * FROM Hall WHERE movie_id=@movie_id AND cinema_hall=@cinema_hall AND row=@row AND seat=@seat AND hours LIKE @hour_of_movie;";
                    DataTable tbl1 = ClsDB.Get_DataTable(sSQL1, new List<object[]> { 
                        new object[] { "@movie_id", movie_id },
                        new object[] { "@cinema_hall", cinema_hall },
                        new object[] { "@row", row },
                        new object[] { "@seat", seat },
                        new object[] { "@hour_of_movie", "%"+hour_of_movie+"%" }});
                    
                    if (tbl1.Rows[0]["available"].ToString() == "N")
                    {
                        taken_seats.Add(new int[] {row, seat});
                    }
                }
            }
            return taken_seats;
        }

        public int GetMovieIdByTitle(string movie_title)
        {
            string sSQL = "SELECT * FROM Movies WHERE title LIKE @movie_title";
            DataTable tbl = ClsDB.Get_DataTable(sSQL, new List<object[]> { new object[] { "@movie_title", "%"+movie_title+"%" } });
            int movie_id = (int)tbl.Rows[0]["Id"];
            return movie_id;
        }

        public int SearchCinemaHall(string movie_title, string hour_of_movie)
        {
            int movie_id = GetMovieIdByTitle(movie_title);

            string sSQL1 = "SELECT * FROM Hours WHERE movie_id=@movie_id AND hours=@hour_of_movie";
            DataTable tbl1 = ClsDB.Get_DataTable(sSQL1, new List<object[]> { new object[] { "@movie_id", movie_id.ToString()}, new string[] { "@hour_of_movie", hour_of_movie.ToString() } }); ;
            if(tbl1.Rows.Count <= 0)
            {
                return -1;
            }
            cinema_hall = (int)tbl1.Rows[0]["cinema_hall"];
            
            return cinema_hall;
        }

        public void GetPrizeFromBase()
        {
            string sSQL = "SELECT * FROM Food WHERE type='nachos';";
            DataTable tbl = ClsDB.Get_DataTable(sSQL);
            small_nachos_prize = (int)tbl.Rows[0]["price"];
            medium_nachos_prize = (int)tbl.Rows[1]["price"];
            large_nachos_prize = (int)tbl.Rows[2]["price"];

            string sSQL1 = "SELECT * FROM Food WHERE type='popcorn';";
            DataTable tbl1 = ClsDB.Get_DataTable(sSQL1);
            small_popcorn_prize = (int)tbl1.Rows[0]["price"];
            medium_popcorn_prize = (int)tbl1.Rows[1]["price"];
            large_popcorn_prize = (int)tbl1.Rows[2]["price"];

            string sSQL2 = "SELECT * FROM Drinks;";
            DataTable tbl2 = ClsDB.Get_DataTable(sSQL2);
            cola_price = (int)tbl2.Rows[0]["price"];
            fanta_price = (int)tbl2.Rows[1]["price"];
            sprite_price = (int)tbl2.Rows[2]["price"];
            water_price = (int)tbl2.Rows[3]["price"];
            juice_price = (int)tbl2.Rows[4]["price"];


            string sSQL3 = "SELECT * FROM Tickets;";
            DataTable tbl3 = ClsDB.Get_DataTable(sSQL3);
            normal_ticket_price = (int)tbl3.Rows[0]["price"];
            student_ticket_price = (int)tbl3.Rows[0]["price"];
            senior_ticket_price = (int)tbl3.Rows[0]["price"];
        }

        static Singleton()
        {
            
        }

        private Singleton()
        {
        }

        public static Singleton Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
