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
    /// Logika interakcji dla klasy TicketPage.xaml
    /// </summary>
    public partial class TicketPage : Page
    {
        NavigationService navService;
        public TicketPage()
        {
            InitializeComponent();
            Get_DBcontent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.ActivateGrammar(MainWindow.bilety);
            navService = NavigationService.GetNavigationService(this);
            MainWindow.sre.SpeechRecognized += Sre_SpeechRecognized;
            MainWindow.ss.SpeakAsync("Wybierz liczbę i rodzaj biletów");
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            Console.WriteLine(txt);
            Choices(e);
            Singleton.Instance.number_of_all_tickets = Singleton.Instance.CountTickets();
            Singleton.Instance.MainNavigate(navService);
        }

        private void Get_DBcontent()
        {
            string sSQL = "SELECT * FROM Tickets";
            DataTable tbl = ClsDB.Get_DataTable(sSQL);

            ImageSource imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[0]["image"].ToString(), UriKind.Relative));
            normal_ticket.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[1]["image"].ToString(), UriKind.Relative));
            student_ticket.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[2]["image"].ToString(), UriKind.Relative));
            senior_ticket.Source = imgSource;
        }

        private void Choices(SpeechRecognizedEventArgs e)
        {
            bool anything = false;

            if (e.Result.Semantics.ContainsKey("ticket_type0"))
            {
                //Console.WriteLine("weszło1");
                string ticket_type = e.Result.Semantics["ticket_type0"].Value.ToString();
                SetTicketCount(ticket_type, e, "count0");
                anything = true;
            }

            if (e.Result.Semantics.ContainsKey("ticket_type1"))
            {
                //Console.WriteLine("weszło2");
                string ticket_type_2 = e.Result.Semantics["ticket_type1"].Value.ToString();
                SetTicketCount(ticket_type_2, e, "count1");
                anything = true;
            }

            if (e.Result.Semantics.ContainsKey("ticket_type2"))
            {
                //Console.WriteLine("weszło3");
                string ticket_type_3 = e.Result.Semantics["ticket_type2"].Value.ToString();
                SetTicketCount(ticket_type_3, e, "count2");
                anything = true;
            }

            if (anything)
            {
                MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
            }
        }
        private void SetTicketCount(string ticket_type, SpeechRecognizedEventArgs e, string key)
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

                number_of_student_tickets.Text = Singleton.Instance.number_of_student_ticket.ToString();
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

                number_of_normal_tickets.Text = Singleton.Instance.number_of_normal_ticket.ToString();
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

                number_of_senior_tickets.Text = Singleton.Instance.number_of_senior_ticket.ToString();
                Console.WriteLine("seniorski: " + Singleton.Instance.number_of_senior_ticket);
            }
        }
    }
}