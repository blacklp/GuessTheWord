using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.Xml;

namespace GuessIt {

    public partial class MainPage : PhoneApplicationPage {
        //Class attributes
        private int level;

        private string currentWord;

        private List<String> l;

        private char[] partial;

        private int counter = 0;

        private int maxChances;

        // Constructor
        public MainPage() {
            InitializeComponent();
            initializeWords();
        }

        private void initializeWords() {
            l = new List<string>();
            this.fillListWithWords();
        }

        private void fillListWithWords() {
            Uri url = new Uri("http://words.bighugelabs.com/apisample.php?v=2&format=xml", UriKind.Absolute);
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(url);
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e) {
            if (e.Error == null) {
                StringReader stream = new StringReader(e.Result);
                XmlReader reader = XmlReader.Create(stream);

                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Text) {
                        if (!reader.Value.Contains(" "))
                            l.Add(reader.Value.ToUpper());
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e) {

        }

        private void start_Click(object sender, RoutedEventArgs e) {
            initialActions();
        }

        private void initialActions() {
            counter = 0;
            setCurrentWord();

            level = currentWord.Length;
            maxChances = level;

            this.attempts.Visibility = Visibility.Visible;
            this.attemptsValue.Text = maxChances + "";

            partial = new char[level];

            solve.Visibility = Visibility.Visible;
            answerBox.Visibility = Visibility.Visible;
            solve.IsEnabled = true;
            this.textBlockEnter.Visibility = Visibility.Visible;
            this.textBlockEnter.Text = "Please enter a letter to start!";
            this.wordBlock.Text = "";
            this.answerBox.Text = "";
            this.answerBlock.Text = "";

            for (int i = 0; i < level; i++) {
                this.wordBlock.Text += "_ ";
                this.partial[i] = '_';
            }
        }

        private void setCurrentWord() {
            int randomIndex = new Random().Next(l.Count);
            currentWord = l.ElementAt(randomIndex);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) {

        }

        private void solve_Click(object sender, RoutedEventArgs e) {
            this.wordBlock.Text = this.currentWord;
        }

        private void answerBox_TextChanged(object sender, TextChangedEventArgs e) {
            if(counter < maxChances) {
                checkAnswer();
            }else {
                this.previousActions();
            }
            this.answerBox.Text = "";
        }

        private void previousActions() {
            this.attemptsValue.Text = "";
            this.attempts.Visibility = Visibility.Collapsed;
            this.textBlockEnter.Visibility = Visibility.Collapsed;
            counter = 0;
            
            solve.Visibility = Visibility.Collapsed;
            answerBox.Visibility = Visibility.Collapsed;

            this.answerBlock.Text = "You lost! Please start again!";
            this.wordBlock.Text = "";
            this.answerBox.Text = "";
        }

        private void checkAnswer() {
            if(this.answerBox.Text.Length > 0) {
                this.solve_Partial();    

                if (this.charsCoincide()) {
                    this.answerBlock.Text = "YES! Your answer is correct!";
                }
            }
        }

        private bool charsCoincide() {
            return (!wordBlock.Text.Contains("_"));
        }

        private void solve_Partial() {
            bool goodLetter = false;

            this.wordBlock.Text = "";
            char letter = answerBox.Text.ToUpper().ElementAt(0);
            char[] current = currentWord.ToUpper().ToCharArray();

            for (int i = 0; i < level; i++) {
                if (letter == current[i]) {
                    this.partial[i] = letter;
                    goodLetter = true;
                }
                this.wordBlock.Text += partial[i] + " ";
            }

            if(goodLetter) {
                this.answerBlock.Text = letter + ": This letter is correct!";
            }else {
                counter++;
                int attempts = (maxChances - counter);
                this.attemptsValue.Text = attempts.ToString();

                this.answerBlock.Text = letter + ": NO, Keep on trying!";
            }
        }
    }
}