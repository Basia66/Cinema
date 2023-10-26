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
    /// Logika interakcji dla klasy PopcornPage.xaml
    /// </summary>
    public partial class PopcornPage : Page
    {
        NavigationService navService;
        public PopcornPage()
        {
            InitializeComponent();
            WritePrize();
            Get_DBcontent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            navService = NavigationService.GetNavigationService(this);
            MainWindow.ss.SpeakAsync("Ile i jaki rozmiar popkornu chcesz wybrać?");
            MainWindow.sre.SpeechRecognized += Sre_SpeechRecognized;
        }
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            Console.WriteLine(txt);

            if (e.Result.Semantics.ContainsKey("sizeOfFood") && e.Result.Semantics.ContainsKey("count"))
            {
                string food_size = e.Result.Semantics["sizeOfFood"].Value.ToString();
                Get_Food(food_size, e, "count");
                if(Singleton.Instance.number_of_small_popcorn != 0 || Singleton.Instance.number_of_medium_popcorn != 0 || Singleton.Instance.number_of_large_popcorn != 0)
                {
                    MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
                    Singleton.Instance.NavigateFood(navService);
                }
            }
            else if(e.Result.Semantics.ContainsKey("operation") && e.Result.Semantics["operation"].Value.ToString().Contains("nie"))
            {
                MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
                Singleton.Instance.popcorn = false;
                Singleton.Instance.NavigateFood(navService);
            }
        }
        private void WritePrize()
        {
            small_popcorn_price.Content = Singleton.Instance.small_popcorn_prize;
            medium_popcorn_price.Content = Singleton.Instance.medium_popcorn_prize;
            large_popcorn_price.Content = Singleton.Instance.large_popcorn_prize;
        }
        private void Get_DBcontent()
        {
            string sSQL = "SELECT * FROM Food WHERE type='popcorn';";
            DataTable tbl = ClsDB.Get_DataTable(sSQL);

            ImageSource imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[0]["image"].ToString(), UriKind.Relative));
            im0.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[1]["image"].ToString(), UriKind.Relative));
            im1.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[2]["image"].ToString(), UriKind.Relative));
            im2.Source = imgSource;
        }
        private void Get_Food(string food_size, SpeechRecognizedEventArgs e, string key)
        {
            if (food_size == "mały")
            {
                if (e.Result.Semantics.ContainsKey(key))
                {
                    Singleton.Instance.number_of_small_popcorn = Convert.ToInt32(e.Result.Semantics[key].Value);
                }
                else
                {
                    Singleton.Instance.number_of_small_popcorn = 1;
                }
                Console.WriteLine("mały: " + Singleton.Instance.number_of_small_popcorn);
            }
            else if (food_size == "średni")
            {
                if (e.Result.Semantics.ContainsKey(key))
                {
                    Singleton.Instance.number_of_medium_popcorn = Convert.ToInt32(e.Result.Semantics[key].Value);
                }
                else
                {
                    Singleton.Instance.number_of_medium_popcorn = 1;
                }
                Console.WriteLine("średni: " + Singleton.Instance.number_of_medium_popcorn);
            }
            else
            {
                if (e.Result.Semantics.ContainsKey(key))
                {
                    Singleton.Instance.number_of_large_popcorn = Convert.ToInt32(e.Result.Semantics[key].Value);
                }
                else
                {
                    Singleton.Instance.number_of_large_popcorn = 1;
                }
                Console.WriteLine("duży: " + Singleton.Instance.number_of_large_popcorn);
            }
        }
    }
}