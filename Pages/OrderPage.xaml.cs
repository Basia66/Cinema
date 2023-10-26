using Cinema.Classes;
using Microsoft.Speech.Recognition;
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
    /// Logika interakcji dla klasy OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        NavigationService navService;

        public OrderPage()
        {

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            navService = NavigationService.GetNavigationService(this);
            MainWindow.ss.SpeakAsync("Twoje zamówienie wyświetla się na ekranie.");
            Singleton.Instance.order = CreateOrder();

            ClsDB.Update_Data("Insert into Orders (order_text) values (@data);",
                new List<object[]> { new object[] { "@data", Singleton.Instance.order.Replace('\n', ' ') } });


            Console.WriteLine(Singleton.Instance.order);

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Singleton.Instance.order != null)
            {
                navService.Navigate(new Uri("\\Pages\\GoodbyePage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(10000);
        }

        public string CreateOrder()
        {
            string order = "Tytuł filmu: " + Singleton.Instance.movie_title + "\n"
                    + "Godzina seansu:" + Singleton.Instance.hour_of_movie + "\n"
                    + Singleton.Instance.place + "\n";

            if (Singleton.Instance.number_of_small_popcorn != 0)
            {
                order += "Mały popcorn: " + Singleton.Instance.number_of_small_popcorn + "\n";
            }
            if (Singleton.Instance.number_of_medium_popcorn != 0)
            {
                order += "Średni popcorn: " + Singleton.Instance.number_of_medium_popcorn + "\n";
            }
            if (Singleton.Instance.number_of_large_popcorn != 0)
            {
                order += "Duży popcorn: " + Singleton.Instance.number_of_large_popcorn + "\n";
            }
            if (Singleton.Instance.number_of_small_nachos != 0)
            {
                order += "Małe nachosy: " + Singleton.Instance.number_of_small_nachos + "\n";
            }
            if (Singleton.Instance.number_of_medium_nachos != 0)
            {
                order += "Średnie nachosy: " + Singleton.Instance.number_of_medium_nachos + "\n";
            }
            if (Singleton.Instance.number_of_large_nachos != 0)
            {
                order += "Duże nachosy: " + Singleton.Instance.number_of_large_nachos + "\n";
            }
            if (Singleton.Instance.number_of_cola != 0)
            {
                order += "Cola: " + Singleton.Instance.number_of_cola + "\n";
            }
            if (Singleton.Instance.number_of_fanta != 0)
            {
                order += "Fanta: " + Singleton.Instance.number_of_fanta + "\n";
            }
            if (Singleton.Instance.number_of_sprite != 0)
            {
                order += "Sprite: " + Singleton.Instance.number_of_sprite + "\n";
            }
            if (Singleton.Instance.number_of_juice != 0)
            {
                order += "Sok: " + Singleton.Instance.number_of_juice + "\n";
            }
            if (Singleton.Instance.number_of_water != 0)
            {
                order += "Woda: " + Singleton.Instance.number_of_water + "\n";
            }

            order += "Całkowita kwota do zapłaty: " + Singleton.Instance.TotalCost().ToString("0.00") + " zł";
            text1.Text = order;
            return order;
        }
    }
}
