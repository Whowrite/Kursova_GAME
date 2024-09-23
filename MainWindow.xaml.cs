using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using USQLCSharpProject_My_Game;

namespace Kursova_GAME
{
    public partial class MainWindow : Window
    {
        int rows = 5; // Кількість рядків
        int cols = 5; // Кількість стовпців
        Button[,] bt;
        DateTime date = new DateTime();
        DispatcherTimer timer = new DispatcherTimer();

        const int size = 5;
        char[,] userGrid = new char[size, size];

        int[,] solution = new int[size, size]
        {
        { -1, 0, 0, 0, -1 },
        { 0, 1, -1, 0, 1 },
        { 0, 0, 0, 0, 1 },
        { 0, 1, -1, 0, 0 },
        { -1, 0, 0, 0, -1 },
        };

        int[,] numbers = new int[size, size]
        {
        { 1, 0, 0, 0, 3 },
        { 0, 0, 2, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 2, 0, 0 },
        { 1, 0, 0, 0, 1 },
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bt = new Button[rows, cols]; // Ініціалізуємо масив кнопок
            CreateGrid(); // Викликаємо метод для створення сітки
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(1);
            timer.Start();
            date = DateTime.Now;
        }

        private void CreateGrid()
        {
            buttonGrid.RowDefinitions.Clear();
            buttonGrid.ColumnDefinitions.Clear();
            buttonGrid.Children.Clear();

            for (int i = 0; i < rows; i++)
            {
                buttonGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int j = 0; j < cols; j++)
            {
                buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Додаємо кнопки в сітку
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    bt[i, j] = new Button();
                    bt[i, j].Background = Brushes.LightGray;
                    bt[i, j].FontSize = 52;
                    bt[i, j].Click += bt_Click;
                    bt[i, j].BorderBrush = Brushes.Black;
                    bt[i, j].MouseRightButtonDown += bt_RightClick;
                    bt[i, j].MouseEnter += OnMouseEnterHandler;
                    bt[i, j].MouseLeave += OnMouseLeaveHandler;
                    if (numbers[i, j] != 0)
                    {
                        bt[i, j].Content = numbers[i, j];
                        bt[i, j].IsEnabled = false;
                    }

                    // Вказуємо позицію кнопки в сітці
                    Grid.SetRow(bt[i, j], i);
                    Grid.SetColumn(bt[i, j], j);

                    buttonGrid.Children.Add(bt[i, j]);
                }
            }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    userGrid[i, j] = '-';
        }
        private void New_Game_Click(object sender, RoutedEventArgs e)
        {
            USQLCSharpProject_My_Game.My_Game.GenerateGame(solution, numbers); //Відбувається присвоєння масивам значень
            buttonGrid.IsEnabled = true;
            //Очищення елементів
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    userGrid[i, j] = '-';

            //Очищення кнопок
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    bt[i, j].Background = Brushes.LightGray;
                    if (numbers[i, j] != 0)
                        bt[i, j].Content = numbers[i, j];
                    else bt[i, j].Content = ' ';
                }
            CreateGrid();
            date = DateTime.Now;
            timer.Start();
        }

        private bool CheckSolution()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (userGrid[i, j] == '#' && solution[i, j] != 0)
                    {
                        return false; // Є невірно розміщена чорна клітинка
                    }

                    if (userGrid[i, j] == '-' && numbers[i, j] == 0)
                    {
                        return false; // Пропущено чорну клітинку
                    }
                }
            }
            timer.Stop();
            return true; // Все вірно
        }
        private void Restart_Game_Click(object sender, RoutedEventArgs e)
        {
            buttonGrid.IsEnabled = true;

            //Очищення елементів
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    userGrid[i, j] = '-';

            //Очищення кнопок
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    bt[i, j].Background = Brushes.LightGray;
                    if (numbers[i, j] != 0)
                        bt[i, j].Content = numbers[i, j];
                    else bt[i, j].Content = ' ';
                }
            date = DateTime.Now;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            long tick = DateTime.Now.Ticks - date.Ticks;
            DateTime stopwatch = new DateTime();

            stopwatch = stopwatch.AddTicks(tick);
            label1.Content = String.Format("{0:HH:mm:ss:ff}", stopwatch);
        }

        private void bt_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            int row = Grid.GetRow(clickedButton);    // Рядок, на якому знаходиться кнопка
            int column = Grid.GetColumn(clickedButton);  // Стовпець, на якому знаходиться кнопка
            if (clickedButton.Background != Brushes.Black)
            {
                clickedButton.Background = Brushes.Black;
                clickedButton.Content = ' ';
                userGrid[row, column] = '#';
            }
            else
            {
                clickedButton.Background = Brushes.LightGray;
                clickedButton.Content = ' ';
                userGrid[row, column] = '-';
            }
            if (CheckSolution())
            {
                MessageBox.Show("Ви успішно вирішили гру Nurikabe! Вітаємо!");
                buttonGrid.IsEnabled = false;
                MessageBox.Show("Гра завершена.");
            }
            //else
            //{
            //    MessageBox.Show("Невірне рішення. Спробуйте знову.");
            //}
        }

        private void bt_RightClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            int row = Grid.GetRow(clickedButton);    // Рядок, на якому знаходиться кнопка
            int column = Grid.GetColumn(clickedButton);  // Стовпець, на якому знаходиться кнопка
            if (clickedButton.Background != Brushes.White)
            {
                clickedButton.Background = Brushes.White;
                clickedButton.Content = '*';
                userGrid[row, column] = '*';
            }
            else
            {
                clickedButton.Background = Brushes.LightGray;
                clickedButton.Content = ' ';
                userGrid[row, column] = '-';
            }
            if (CheckSolution())
            {
                MessageBox.Show("Ви успішно вирішили гру Nurikabe! Вітаємо!");
                buttonGrid.IsEnabled = false;
                MessageBox.Show("Гра завершена.");
            }
            //else
            //{
            //    MessageBox.Show("Невірне рішення. Спробуйте знову.");
            //}
        }
        void OnMouseEnterHandler(object sender, MouseEventArgs e)
        {
            Button hoveredButton = sender as Button;
            hoveredButton.BorderBrush = Brushes.Red;
            hoveredButton.BorderThickness = new Thickness(3); // Товщина краю
        }
        void OnMouseLeaveHandler(object sender, MouseEventArgs e)
        {
            Button hoveredButton = sender as Button;
            hoveredButton.BorderBrush = Brushes.Black;
            hoveredButton.BorderThickness = new Thickness(1); // Край не підсвічується
        }

        private void TopGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (TopGrid.ActualHeight == 471.519)
            {
                TopLabel.Visibility = Visibility.Visible;
                TopForm.LayoutTransform = new RotateTransform(0);
            }
            else
            {
                TopLabel.Visibility = Visibility.Hidden;
                TopForm.LayoutTransform = new RotateTransform(180);
            }
        }
    }
}
