﻿using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
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
        int Difficulty = 0;

        bool ShowError2x2 = false;
        bool NightTema = true;
        bool ShowTimer = false;
        bool ResolvedNumbers = false;
        bool WrongNumbers = false;
        bool WrongNumbersColor = false;

        int size = 5;
        List<List<char>> userGrid = new List<List<char>>();
        List<List<int>> solution = new List<List<int>>();
        List<List<int>> numbers = new List<List<int>>();

        List<TimeSpan> times = new List<TimeSpan>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeGrids();
            ReadFromFile("TopResults.txt");
        }

        private void InitializeGrids()
        {
            // Ініціалізація solution
            solution = new List<List<int>>()
        {
            new List<int> { -1, 0, 0, 0, -1 },
            new List<int> { 0, 1, -1, 0, 1 },
            new List<int> { 0, 0, 0, 0, 1 },
            new List<int> { 0, 1, -1, 0, 0 },
            new List<int> { -1, 0, 0, 0, -1 }
        };

            // Ініціалізація numbers
            numbers = new List<List<int>>()
        {
            new List<int> { 1, 0, 0, 0, 3 },
            new List<int> { 0, 0, 2, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0 },
            new List<int> { 0, 0, 2, 0, 0 },
            new List<int> { 1, 0, 0, 0, 1 }
        };

            //Ініціалізація userGrid
            for (int i = 0; i < size; i++)
            {
                userGrid.Add(new List<char>());
                for (int j = 0; j < size; j++)
                {
                    userGrid[i].Add('-'); // Ініціалізація порожніх значень
                }
            }

            // Заповнюємо список DateTime.MinValue
            for (int i = 0; i < 6; i++)
            {
                times.Add(TimeSpan.Zero);
            }
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
            bt = new Button[rows, cols];
            double baseFontSize = 52; // Базовий розмір шрифту
            double buttonFontSize = baseFontSize * (5.0 / Math.Max(rows, cols)); // Зменшуємо розмір шрифту при збільшенні кількості кнопок

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    bt[i, j] = new Button();
                    bt[i, j].Background = Brushes.LightGray;
                    bt[i, j].FontSize = buttonFontSize;
                    bt[i, j].Click += bt_Click;
                    bt[i, j].BorderBrush = Brushes.Black;
                    bt[i, j].MouseRightButtonDown += bt_RightClick;
                    bt[i, j].MouseEnter += OnMouseEnterHandler;
                    bt[i, j].MouseLeave += OnMouseLeaveHandler;
                    // Якщо є числа у поточній позиції, то встановлюємо їх
                    if (numbers[i][j] != 0)
                    {
                        bt[i, j].Content = numbers[i][j];
                        bt[i, j].IsEnabled = false;
                        bt[i, j].Background = Brushes.White;
                    }

                    Grid.SetRow(bt[i, j], i);
                    Grid.SetColumn(bt[i, j], j);
                    buttonGrid.Children.Add(bt[i, j]);
                }
            }
            userGrid = new List<List<char>>();
            for (int i = 0; i < rows; i++)
            {
                userGrid.Add(new List<char>());
                for (int j = 0; j < cols; j++)
                {
                    userGrid[i].Add('-');
                }
            }
        }
        private void New_Game_Click(object sender, RoutedEventArgs e)
        {
            USQLCSharpProject_My_Game.My_Game.GenerateGame(solution, numbers, Difficulty); //Відбувається присвоєння масивам значень
            buttonGrid.IsEnabled = true;
            //Очищення елементів
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    userGrid[i][j] = '-';
            //Очищення кнопок
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    bt[i, j].Background = Brushes.LightGray;
                    if (numbers[i][j] != 0)
                        bt[i, j].Content = numbers[i][j];
                    else bt[i, j].Content = ' ';
                }
            CreateGrid();
            date = DateTime.Now;
            timer.Start();
            NewGame_Pause.Visibility = Visibility.Hidden;
        }

        private bool CheckSolution()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (userGrid[i][j] == '*' && solution[i][j] == 0)
                    {
                        return false;
                    }

                    if (userGrid[i][j] == '#' && solution[i][j] != 0)
                    {
                        return false; // Є невірно розміщена чорна клітинка
                    }

                    if (userGrid[i][j] == '-' && numbers[i][j] == 0)
                    {
                        return false; // Пропущено чорну клітинку
                    }
                }
            }
            timer.Stop();
            ChangeTopResult();
            return true; // Все вірно
        }
        private void Restart_Game_Click(object sender, RoutedEventArgs e)
        {
            buttonGrid.IsEnabled = true;

            //Очищення елементів
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    userGrid[i][j] = '-';

            //Очищення кнопок
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    bt[i, j].Background = Brushes.LightGray;
                    if (numbers[i][j] != 0)
                        bt[i, j].Content = numbers[i][j];
                    else bt[i, j].Content = ' ';
                }
            date = DateTime.Now;
            timer.Start();
            NewGame_Pause.Visibility = Visibility.Hidden;
        }
        private void SelectDifficulty_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Time_Pause.Visibility = Visibility.Visible;
            Window1 window1 = new Window1();

            LinearGradientBrush linearGradientBrush = new LinearGradientBrush();

            linearGradientBrush.StartPoint = new Point(0.5, 0);
            linearGradientBrush.EndPoint = new Point(0.5, 1);

            if (NightTema)
            {
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Colors.Black, 0.138));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(14, 34, 46), 0.389));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(127, 207, 253), 0.916));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(33, 90, 124), 0.569));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(53, 124, 168), 0.682));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(75, 161, 212), 0.799));

            }
            else
            {
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(127, 207, 253), 0.628)); // #FF7FCFFD
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(33, 90, 124), 0.176));   // #FF215A7C
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(53, 124, 168), 0.31));   // #FF357CA8
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(75, 161, 212), 0.444));  // #FF4BA1D4
            }
            window1.TopGrid.Background = linearGradientBrush;

            window1.Left = this.Left + this.Width - 18;
            window1.Top = this.Top;
            window1.ShowDialog();
            timer.Start();
            Time_Pause.Visibility = Visibility.Hidden;
            Difficulty = window1.difficulty;
            bool rez = false;
            while (rez == false)
            {
                switch (Difficulty)
                {
                    case 0:
                        rows = 5;
                        cols = 5;
                        rez = true;
                        break;
                    case 1:
                        rows = 7;
                        cols = 7;
                        rez = true;
                        break;
                    case 2:
                        rows = 10;
                        cols = 10;
                        rez = true;
                        break;
                    case 3:
                        rows = 12;
                        cols = 12;
                        rez = true;
                        break;
                    case 4:
                        rows = 15;
                        cols = 15;
                        rez = true;
                        break;
                    case 5:
                        rows = 20;
                        cols = 20;
                        rez = true;
                        break;
                    case 6:
                        Difficulty = GenerateRandomNumber(0, 5);
                        // Треба доробити
                        break;
                }
            }
            solution.Clear();
            numbers.Clear();
            userGrid.Clear();
            for (int i = 0; i < rows; i++)
            {
                userGrid.Add(new List<char>());
                solution.Add(new List<int>());
                numbers.Add(new List<int>());
                for (int j = 0; j < cols; j++)
                {
                    userGrid[i].Add('-'); // Ініціалізація порожніх значень
                    solution[i].Add(0);
                    numbers[i].Add(0);
                }
            }
            USQLCSharpProject_My_Game.My_Game.GenerateGame(solution, numbers, Difficulty); //Відбувається присвоєння масивам значень
            CreateGrid();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - date;  // date - це час початку
            label1.Content = String.Format("{0:hh\\:mm\\:ss\\.fff}", elapsed);
        }

        private void ChangeTopResult()
        {
            switch (Difficulty)
            {
                case 0:
                    if (times[0] > TimeSpan.Parse(label1.Content.ToString()) || (times[1] == TimeSpan.Zero))
                    {
                        times[0] = TimeSpan.Parse(label1.Content.ToString());
                        Result1.Content = label1.Content;
                    }
                    break;
                case 1:
                    if (times[1] > TimeSpan.Parse(label1.Content.ToString()) || (times[1] == TimeSpan.Zero))
                    {
                        Result2.Content = label1.Content;
                        times[1] = TimeSpan.Parse(label1.Content.ToString());
                    }
                    break;
                case 2:
                    if (times[2] > TimeSpan.Parse(label1.Content.ToString()) || (times[1] == TimeSpan.Zero))
                    {
                        Result3.Content = label1.Content;
                        times[2] = TimeSpan.Parse(label1.Content.ToString());
                    }
                    break;
                case 3:
                    if (times[3] > TimeSpan.Parse(label1.Content.ToString()) || (times[1] == TimeSpan.Zero))
                    {
                        Result4.Content = label1.Content;
                        times[3] = TimeSpan.Parse(label1.Content.ToString());
                    }
                    break;
                case 4:
                    if (times[4] > TimeSpan.Parse(label1.Content.ToString()) || (times[1] == TimeSpan.Zero))
                    {
                        Result5.Content = label1.Content;
                        times[4] = TimeSpan.Parse(label1.Content.ToString());
                    }
                    break;
                case 5:
                    if (times[5] > TimeSpan.Parse(label1.Content.ToString()) || (times[1] == TimeSpan.Zero))
                    {
                        Result6.Content = label1.Content;
                        times[5] = TimeSpan.Parse(label1.Content.ToString());
                    }
                    break;
            }
        }

        private void bt_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            int row = Grid.GetRow(clickedButton);    // Рядок, на якому знаходиться кнопка
            int column = Grid.GetColumn(clickedButton);  // Стовпець, на якому знаходиться кнопка
            if (clickedButton.Background == Brushes.Black || clickedButton.Background == Brushes.LightGreen || clickedButton.Background == Brushes.MediumVioletRed
                || clickedButton.Background == Brushes.White || clickedButton.Background == Brushes.Red || clickedButton.Background == Brushes.Blue)
            {
                clickedButton.Background = Brushes.LightGray;
                clickedButton.Content = ' ';
                userGrid[row][column] = '-';
            }
            else if (clickedButton.Background != Brushes.Black)
            {
                clickedButton.Background = Brushes.Black;
                clickedButton.Content = ' ';
                userGrid[row][column] = '#';
                Brush brush;
                if (WrongNumbersColor) brush = Brushes.Blue;
                else brush = Brushes.Red;
                if (ShowError2x2) Search2x2Blocks(brush);
            }
            
            if (CheckSolution())
            {
                MessageBox.Show("Ви успішно вирішили гру Nurikabe! Вітаємо!");
                buttonGrid.IsEnabled = false;
                NewGame_Pause.Visibility = Visibility.Visible;
                MessageBox.Show("Гра завершена.");
            }
        }

        private void bt_RightClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            int row = Grid.GetRow(clickedButton);    // Рядок, на якому знаходиться кнопка
            int column = Grid.GetColumn(clickedButton);  // Стовпець, на якому знаходиться кнопка
            if (clickedButton.Background == Brushes.LightGreen || clickedButton.Background == Brushes.MediumVioletRed 
                || clickedButton.Background == Brushes.White || clickedButton.Background == Brushes.Blue)
            {
                clickedButton.Background = Brushes.LightGray;
                clickedButton.Content = ' ';
                userGrid[row][column] = '-';
            }
            else if (clickedButton.Background != Brushes.White)
            {
                clickedButton.Background = Brushes.White;
                clickedButton.Content = '*';
                userGrid[row][column] = '*';
                if (ResolvedNumbers) if (userGrid[row][column] == '*' && solution[row][column] == 1) bt[row, column].Background = Brushes.LightGreen;
                if (WrongNumbers) if (userGrid[row][column] == '*' && solution[row][column] != 1) bt[row, column].Background = Brushes.MediumVioletRed;
            }
            if (CheckSolution())
            {
                MessageBox.Show("Ви успішно вирішили гру Nurikabe! Вітаємо!");
                buttonGrid.IsEnabled = false;
                NewGame_Pause.Visibility = Visibility.Visible;
                MessageBox.Show("Гра завершена.");
            }
        }
        void OnMouseEnterHandler(object sender, MouseEventArgs e)
        {
            Button hoveredButton = sender as Button;
            hoveredButton.BorderBrush = Brushes.Red;
            hoveredButton.BorderThickness = new Thickness(5); // Товщина краю
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
                Dif1.Visibility = Visibility.Visible;
                Dif2.Visibility = Visibility.Visible;
                Dif3.Visibility = Visibility.Visible;
                Dif4.Visibility = Visibility.Visible;
                Dif5.Visibility = Visibility.Visible;
                Dif6.Visibility = Visibility.Visible;
                star1.Visibility = Visibility.Visible;
                star2.Visibility = Visibility.Visible;
                star3.Visibility = Visibility.Visible;
                star4.Visibility = Visibility.Visible;
                star5.Visibility = Visibility.Visible;
                star6.Visibility = Visibility.Visible;
                Result1.Visibility = Visibility.Visible;
                Result2.Visibility = Visibility.Visible;
                Result3.Visibility = Visibility.Visible;
                Result4.Visibility = Visibility.Visible;
                Result5.Visibility = Visibility.Visible;
                Result6.Visibility = Visibility.Visible;
                TopForm.LayoutTransform = new RotateTransform(0);
                LinearGradientBrush linearGradientBrush = new LinearGradientBrush();

                linearGradientBrush.StartPoint = new Point(0.5, 0);
                linearGradientBrush.EndPoint = new Point(0.5, 1);

                if (NightTema)
                {
                    linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Colors.Black, 0.138));
                    linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(14, 34, 46), 0.389));
                    linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(127, 207, 253), 0.916));
                    linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(33, 90, 124), 0.569));
                    linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(53, 124, 168), 0.682));
                    linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(75, 161, 212), 0.799));

                }
                else
                {
                    linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(127, 207, 253), 0.628)); // #FF7FCFFD
                    linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(33, 90, 124), 0.176));   // #FF215A7C
                    linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(53, 124, 168), 0.31));   // #FF357CA8
                    linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(75, 161, 212), 0.444));  // #FF4BA1D4
                }
                TopGrid.Background = linearGradientBrush;

            }
            else
            {
                TopLabel.Visibility = Visibility.Hidden;
                Dif1.Visibility = Visibility.Hidden;
                Dif2.Visibility = Visibility.Hidden;
                Dif3.Visibility = Visibility.Hidden;
                Dif4.Visibility = Visibility.Hidden;
                Dif5.Visibility = Visibility.Hidden;
                Dif6.Visibility = Visibility.Hidden;
                star1.Visibility = Visibility.Hidden;
                star2.Visibility = Visibility.Hidden;
                star3.Visibility = Visibility.Hidden;
                star4.Visibility = Visibility.Hidden;
                star5.Visibility = Visibility.Hidden;
                star6.Visibility = Visibility.Hidden;
                Result1.Visibility = Visibility.Hidden;
                Result2.Visibility = Visibility.Hidden;
                Result3.Visibility = Visibility.Hidden;
                Result4.Visibility = Visibility.Hidden;
                Result5.Visibility = Visibility.Hidden;
                Result6.Visibility = Visibility.Hidden;
                TopForm.LayoutTransform = new RotateTransform(180);

                if (NightTema) TopGrid.Background = Brushes.Black;
                else
                {
                    Color col = Color.FromRgb(33, 90, 124);
                    TopGrid.Background = new SolidColorBrush(col);
                }
            }
        }

        static int GenerateRandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max + 1); // max + 1, щоб включити верхню межу
        }

        public void WriteToFile(string filePath)
        {
            try
            {
                // Відкриваємо файл для запису
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (TimeSpan ts in times) // times тепер містить TimeSpan
                    {
                        // Записуємо кожен елемент списку у окремому рядку
                        writer.WriteLine(ts.ToString()); // TimeSpan конвертується в строку
                    }
                }
                Console.WriteLine("Дані успішно записано до файлу.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при записі до файлу: {ex.Message}");
            }
        }

        public void ReadFromFile(string filePath)
        {
            try
            {
                // Очищаємо список перед зчитуванням
                times.Clear();

                // Відкриваємо файл для зчитування
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    int i = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Кожен рядок з файлу конвертуємо у TimeSpan і додаємо до списку
                        if (TimeSpan.TryParse(line, out TimeSpan ts))
                        {
                            times.Add(ts); // Додаємо TimeSpan до списку
                                           // Оновлюємо відповідні результати
                            if (i == 0) Result1.Content = ts.ToString(@"hh\:mm\:ss\.fff");
                            else if (i == 1) Result2.Content = ts.ToString(@"hh\:mm\:ss\.fff");
                            else if (i == 2) Result3.Content = ts.ToString(@"hh\:mm\:ss\.fff");
                            else if (i == 3) Result4.Content = ts.ToString(@"hh\:mm\:ss\.fff");
                            else if (i == 4) Result5.Content = ts.ToString(@"hh\:mm\:ss\.fff");
                            else if (i == 5) Result6.Content = ts.ToString(@"hh\:mm\:ss\.fff");
                            i++;
                        }
                        else
                        {
                            Console.WriteLine($"Не вдалося конвертувати рядок '{line}' в TimeSpan.");
                        }
                    }
                }
                Console.WriteLine("Дані успішно зчитано з файлу.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при зчитуванні з файлу: {ex.Message}");
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WriteToFile("TopResults.txt");
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Time_Pause.Visibility = Visibility.Visible;
            Settings settings = new Settings();

            if (ShowError2x2) settings.Wrong_2x2_Box.IsChecked = true;
            if (!NightTema) settings.night_Box.IsChecked = false;
            if (ShowTimer) settings.Timer_Box.IsChecked = true;
            if (ResolvedNumbers) settings.Numbers_Box.IsChecked = true;
            if (WrongNumbers) settings.WrongNumbers_Box.IsChecked = true;
            if (WrongNumbersColor) settings.WrongBlue_Box.IsChecked = true;

            LinearGradientBrush linearGradientBrush = new LinearGradientBrush();

            linearGradientBrush.StartPoint = new Point(0.5, 0);
            linearGradientBrush.EndPoint = new Point(0.5, 1);

            if (NightTema)
            {
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Colors.Black, 0.138));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(14, 34, 46), 0.389));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(127, 207, 253), 0.916));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(33, 90, 124), 0.569));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(53, 124, 168), 0.682));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(75, 161, 212), 0.799));
                settings.TopGrid.Background = linearGradientBrush;

            }
            else
            {
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(127, 207, 253), 0.628)); // #FF7FCFFD
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(33, 90, 124), 0.176));   // #FF215A7C
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(53, 124, 168), 0.31));   // #FF357CA8
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(75, 161, 212), 0.444));  // #FF4BA1D4
            }
            settings.TopGrid.Background = linearGradientBrush;

            settings.Left = this.Left - this.Width + 127;
            settings.Top = this.Top;
            settings.ShowDialog();

            if (settings.Wrong_2x2_Box.IsChecked == true)
            {
                Brush brush;
                if (WrongNumbersColor) brush = Brushes.Blue;
                else brush = Brushes.Red;
                ShowError2x2 = true;
                Search2x2Blocks(brush);
            } //Ok
            else
            {
                ShowError2x2 = false;
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        if (bt[i, j].Background == Brushes.Red) bt[i, j].Background = Brushes.Black;
            }
            if (settings.Timer_Box.IsChecked == true)
            {
                ShowTimer = true;
                label1.Visibility = Visibility.Hidden;
            } //Ok
            else
            {
                ShowTimer = false;
                label1.Visibility = Visibility.Visible;
            }
            if (settings.night_Box.IsChecked == true)
            {
                NightTema = true;
                TopGrid.Background = Brushes.Black;

                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Colors.Black, 0.138));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(14, 34, 46), 0.389));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(127, 207, 253), 0.916));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(33, 90, 124), 0.569));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(53, 124, 168), 0.682));
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(75, 161, 212), 0.799));

                this.Grid1.Background = linearGradientBrush;
            } //Ok
            else
            {
                NightTema = false;
                Color col = Color.FromRgb(33, 90, 124);
                TopGrid.Background = new SolidColorBrush(col);

                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(127, 207, 253), 0.628)); // #FF7FCFFD
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(33, 90, 124), 0.176));   // #FF215A7C
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(53, 124, 168), 0.31));   // #FF357CA8
                linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(75, 161, 212), 0.444));  // #FF4BA1D4

                this.Grid1.Background = linearGradientBrush;
            }
            if (settings.Numbers_Box.IsChecked == true)
            {
                ResolvedNumbers = true;
            } //Ok
            else
            {
                ResolvedNumbers = false;
            }
            if (settings.WrongNumbers_Box.IsChecked == true)
            {
                WrongNumbers = true;
            } //Ok
            else
            {
                WrongNumbers = false;
            }
            if (settings.WrongBlue_Box.IsChecked == true) WrongNumbersColor = true; //Ok
            else WrongNumbersColor = false;

            timer.Start();
            Time_Pause.Visibility = Visibility.Hidden;
        }

        public void Search2x2Blocks(Brush brush)
        {
            for (int row = 0; row < rows - 1; row++)
            {
                for (int col = 0; col < cols - 1; col++)
                {
                    if (userGrid[row][col] == '#')
                    {
                        if (IsIsolated(row, col))
                            bt[row, col].Background = brush;
                        if (IsTwoByTwoGroup(row, col))
                        {
                            bt[row, col].Background = brush;
                            bt[row + 1, col].Background = brush;
                            bt[row, col + 1].Background = brush;
                            bt[row + 1, col + 1].Background = brush;
                        }
                    }
                }
            }
        }

        // Функція для перевірки ізольованої чорної клітинки
        private bool IsIsolated(int row, int col)
        {
            // Перевіряємо чотири сторони навколо клітини (верх, низ, ліворуч, праворуч)
            int[][] directions = new int[][]
            {
        new int[] {-1, 0}, // верх
        new int[] {1, 0},  // низ
        new int[] {0, -1}, // ліворуч
        new int[] {0, 1}   // праворуч
            };

            foreach (var dir in directions)
            {
                int newRow = row + dir[0];
                int newCol = col + dir[1];

                if (IsValidCell(newRow, newCol) && userGrid[newRow][newCol] == '#')
                {
                    return false; // Якщо є сусідня чорна клітинка, то не ізольована
                }
            }
            return true; // Якщо немає сусідів чорних клітин, то клітинка ізольована
        }

        // Функція для перевірки, чи є біла клітинка частиною 2x2 групи
        private bool IsTwoByTwoGroup(int row, int col)
        {
            if (IsValidCell(row + 1, col) && IsValidCell(row, col + 1) && IsValidCell(row + 1, col + 1))
            {
                return userGrid[row][col] == '#' &&
                       userGrid[row + 1][col] == '#' &&
                       userGrid[row][col + 1] == '#' &&
                       userGrid[row + 1][col + 1] == '#';
            }
            return false;
        }

        // Функція для перевірки, чи є клітина дійсною в межах сітки
        private bool IsValidCell(int row, int col)
        {
            return row >= 0 && row < rows && col >= 0 && col < cols;
        }

    }
}