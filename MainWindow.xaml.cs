using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Cinema
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SpeechSynthesizer ss = new SpeechSynthesizer();
        public static SpeechRecognitionEngine sre;
        public static CultureInfo ci = new CultureInfo("pl-PL");
        
        public static Grammar poczatek = new Grammar(".\\Grammars\\Poczatek.xml", "rootRule");
        public static Grammar filmy = new Grammar(".\\Grammars\\Filmy.xml", "rootRule");
        public static Grammar bilety = new Grammar(".\\Grammars\\Bilety.xml", "rootRule");
        public static Grammar miejsca = new Grammar(".\\Grammars\\Miejsca.xml", "rootRule");
        public static Grammar jedzonko = new Grammar(".\\Grammars\\Jedzonko.xml", "rootRule");

        public MainWindow()
        {
            InitializeComponent();
            ss.SetOutputToDefaultAudioDevice();
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            MainWindow.ActivateGrammar(poczatek);
            MainWindow.sre.LoadGrammar(MainWindow.poczatek);
            MainWindow.sre.LoadGrammar(MainWindow.filmy);
            MainWindow.sre.LoadGrammar(MainWindow.bilety);
            MainWindow.sre.LoadGrammar(MainWindow.miejsca);
            MainWindow.sre.LoadGrammar(MainWindow.jedzonko);
            sre.RecognizeAsync(RecognizeMode.Multiple);
            Main.Navigate(new Uri("\\Pages\\WelcomePage.xaml", UriKind.Relative));
        }

        public static void ActivateGrammar(Grammar grammar)
        {
            poczatek.Enabled = false;
            filmy.Enabled = false;
            bilety.Enabled = false;
            miejsca.Enabled = false;
            jedzonko.Enabled = false;

            grammar.Enabled = true;
        }
    }
}
