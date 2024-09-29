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
using System.Windows.Shapes;

namespace Kursova_GAME
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public int difficulty = 0;
        
        public Window1()
        {
            InitializeComponent();
        }

        private void Difficult1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            difficulty = 0;
            this.Close();
        }

        private void Difficult2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            difficulty = 1;
            this.Close();
        }

        private void Difficult3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            difficulty = 2;
            this.Close();
        }

        private void Difficult4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            difficulty = 3;
            this.Close();
        }

        private void Difficult5_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            difficulty = 4;
            this.Close();
        }

        private void Difficult6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            difficulty = 5;
            this.Close();
        }

        private void Difficult7_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            difficulty = 6;
            this.Close();
        }
    }
}
