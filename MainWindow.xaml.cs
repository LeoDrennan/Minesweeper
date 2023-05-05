using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
using static System.Reflection.Metadata.BlobBuilder;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int rows;
        int columns;
        int mines;
        MineGrid gameInstance;

        public MainWindow()
        {
            InitializeComponent();

            // Window and grid size declaration
            rows = 16;
            columns = 16;
            mines = 40;
            formatWindow();
        }

        private void formatWindow()
        {
            // Dynamically change window properties according to grid size
            Application.Current.MainWindow.Height = 40 * rows + 100;
            Application.Current.MainWindow.Width = 35 * columns;

            // Create a new game layout and add to view
            GameManager newGame = new GameManager();
            newGame.totalRows = rows;
            newGame.totalCols = columns;
            newGame.totalMines = mines;
            gameInstance = newGame.createGame();
            minesweeper.Children.Add(gameInstance);
        }

        // Delete current game and create a new game
        public void resetClick(object sender, RoutedEventArgs e)
        {
            minesweeper.Children.Clear();
            GameManager nextGame = new GameManager();
            nextGame.totalRows = rows;
            nextGame.totalCols = columns;
            nextGame.totalMines = mines;
            MineGrid nextInstance = nextGame.createGame();
            minesweeper.Children.Add(nextInstance);
        }

        // Check for currently selected difficulty and create a new game with that criteria
        public void changeDifficulty(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem test = (ComboBoxItem)difficultySelect.SelectedItem;

            switch (test.Name)
            {
                case "Beginner":
                    rows = 8;
                    columns = 8;
                    mines = 10;
                    minesweeper.Children.Clear();
                    formatWindow();
                    break;
                case "Intermediate":
                    rows = 16;
                    columns = 16;
                    mines = 40;
                    minesweeper.Children.Clear();
                    formatWindow();
                    break;
                case "Expert":
                    rows = 16;
                    columns = 30;
                    mines = 99;
                    minesweeper.Children.Clear();
                    formatWindow();
                    break;
            }
            return;
        }
    }
}
