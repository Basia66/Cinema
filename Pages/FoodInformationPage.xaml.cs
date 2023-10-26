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

namespace Cinema.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy FoodInformationPage.xaml
    /// </summary>
    public partial class FoodInformationPage : Page
    {
        NavigationService navService;
        public FoodInformationPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.ActivateGrammar(MainWindow.jedzonko);
            navService = NavigationService.GetNavigationService(this);
            MainWindow.ss.SpeakAsync("Czy chcesz zamówić jakieś jedzenie lub napoje?");
            MainWindow.ss.SpeakAsync("Do wyboru jest popkorn, naczosy oraz napoje");
            text1.Text = "Czy chcesz zamówić jakieś jedzenie? " + "\n" +
                "Do wyboru jest: " + "\n" +
                "- popcorn " + "\n" +
                "- nachosy " + "\n" +
                "- napoje";
            MainWindow.sre.SpeechRecognized += Sre_SpeechRecognized;
        }
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            Console.WriteLine(txt);
            Navigate(e);
            Singleton.Instance.NavigateFood(navService);
        }

        private void Navigate(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Semantics.ContainsKey("operation1") && e.Result.Semantics["operation1"].Value.ToString() == "nie")
            {
                MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
                navService.Navigate(new Uri("\\Pages\\OrderPage.xaml", UriKind.RelativeOrAbsolute));
                return;
            }

            if (e.Result.Semantics.ContainsKey("typeOfFood1"))
            {

                if (e.Result.Semantics["typeOfFood1"].Value.ToString().Contains("popco"))
                {
                    Singleton.Instance.popcorn = true;
                    Console.WriteLine("popcorn wszedł");
                }
                if (e.Result.Semantics["typeOfFood1"].Value.ToString().Contains("nachosy"))
                {
                    Singleton.Instance.nachos = true;
                    Console.WriteLine("nachosy wszedł");
                }
            }
            if (e.Result.Semantics.ContainsKey("typeOfFood2"))
            {
                if (e.Result.Semantics["typeOfFood2"].Value.ToString().Contains("popco"))
                {
                    Singleton.Instance.popcorn = true;
                    Console.WriteLine("popcorn wszedł");
                }
                if (e.Result.Semantics["typeOfFood2"].Value.ToString().Contains("nacho"))
                {
                    Singleton.Instance.nachos = true;
                    Console.WriteLine("nachosy wszedł");
                }
            }
            if (e.Result.Semantics.ContainsKey("food"))
            {
                Singleton.Instance.drink = true;
                Console.WriteLine("picie wszedł");
            }
            MainWindow.sre.SpeechRecognized -= Sre_SpeechRecognized;
            Singleton.Instance.NavigateFood(navService);
        }
    }
}