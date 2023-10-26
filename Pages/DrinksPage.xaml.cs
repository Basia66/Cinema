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
    /// Logika interakcji dla klasy DrinksPage.xaml
    /// </summary>
    public partial class DrinksPage : Page
    {
        NavigationService navService;
        public DrinksPage()
        {
            InitializeComponent();         
            WritePrize();
            Get_DBcontent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            navService = NavigationService.GetNavigationService(this);
            MainWindow.ss.SpeakAsync("Ile i jaki rodzaj napojów chcesz wybrać?");
            MainWindow.sre.SpeechRecognized += Sre_SpeechRecognized;

        }
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            Console.WriteLine(txt);

            if (e.Result.Semantics.ContainsKey("typeOfDrink") && e.Result.Semantics.ContainsKey("count"))
            {
                string drink_size = e.Result.Semantics["typeOfDrink"].Value.ToString();
                GetDrink(drink_size, e, "count");
                if (Singleton.Instance.number_of_cola != 0 || 
                    Singleton.Instance.number_of_fanta != 0 || 
                    Singleton.Instance.number_of_sprite != 0 || 
                    Singleton.Instance.number_of_juice != 0 ||
                    Singleton.Instance.number_of_water != 0)
                {
                    MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
                    Singleton.Instance.NavigateFood(navService);
                }
            }
            else if (e.Result.Semantics.ContainsKey("operation") && e.Result.Semantics["operation"].Value.ToString().Contains("nie"))
            {
                MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
                Singleton.Instance.drink = false;
                Singleton.Instance.NavigateFood(navService);
            }
        }
        private void WritePrize()
        {
            cola_price.Content = Singleton.Instance.cola_price;
            fanta_price.Content = Singleton.Instance.fanta_price;
            sprite_price.Content = Singleton.Instance.sprite_price;
            water_price.Content = Singleton.Instance.water_price;
            juice_price.Content = Singleton.Instance.juice_price;

        }
        private void Get_DBcontent()
        {
            string sSQL = "SELECT * FROM Drinks;";
            DataTable tbl = ClsDB.Get_DataTable(sSQL);

            ImageSource imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[0]["image"].ToString(), UriKind.Relative));
            im0.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[1]["image"].ToString(), UriKind.Relative));
            im1.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[2]["image"].ToString(), UriKind.Relative));
            im2.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[3]["image"].ToString(), UriKind.Relative));
            im3.Source = imgSource;

            imgSource = new BitmapImage(new Uri("../Graphics/" + tbl.Rows[4]["image"].ToString(), UriKind.Relative));
            im4.Source = imgSource;
        }
        private void GetDrink(string drink_type, SpeechRecognizedEventArgs e, string key)
        {
            if (drink_type == "cola")
            {
                if (e.Result.Semantics.ContainsKey(key))
                {
                    Singleton.Instance.number_of_cola = Convert.ToInt32(e.Result.Semantics[key].Value);
                }
                else
                {
                    Singleton.Instance.number_of_cola = 1;
                }
                Console.WriteLine("cola: " + Singleton.Instance.number_of_cola);
            }
            if (drink_type == "fanta")
            {
                if (e.Result.Semantics.ContainsKey(key))
                {
                    Singleton.Instance.number_of_fanta = Convert.ToInt32(e.Result.Semantics[key].Value);
                }
                else
                {
                    Singleton.Instance.number_of_fanta = 1;
                }
                Console.WriteLine("fanta: " + Singleton.Instance.number_of_fanta);
            }
            if (drink_type == "sprite")
            {
                if (e.Result.Semantics.ContainsKey(key))
                {
                    Singleton.Instance.number_of_sprite = Convert.ToInt32(e.Result.Semantics[key].Value);
                }
                else
                {
                    Singleton.Instance.number_of_sprite = 1;
                }
                Console.WriteLine("sprite: " + Singleton.Instance.number_of_sprite);
            }
            if (drink_type == "sok")
            {
                if (e.Result.Semantics.ContainsKey(key))
                {
                    Singleton.Instance.number_of_juice = Convert.ToInt32(e.Result.Semantics[key].Value);
                }
                else
                {
                    Singleton.Instance.number_of_juice = 1;
                }
                Console.WriteLine("sok: " + Singleton.Instance.number_of_juice);
            }
            if (drink_type == "woda")
            {
                if (e.Result.Semantics.ContainsKey(key))
                {
                    Singleton.Instance.number_of_water = Convert.ToInt32(e.Result.Semantics[key].Value);
                }
                else
                {
                    Singleton.Instance.number_of_water = 1;
                }
                Console.WriteLine("woda: " + Singleton.Instance.number_of_water);
            }
        }
    }
}