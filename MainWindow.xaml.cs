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
        MineGrid gameInstance;
        public MainWindow()
        {
            InitializeComponent();

            // Window and grid size declaration
            const int columns = 16;
            const int rows = 16;
            const int mines = 40;

            // Dynamically change window properties according to grid size
            Application.Current.MainWindow.Height = 35 * rows + 70;
            Application.Current.MainWindow.Width = 35 * columns;

            // Create a new game layout and add to view
            GameManager newGame = new GameManager();
            gameInstance = newGame.createGame(rows, columns, mines);
            minesweeper.Children.Add(gameInstance);
        }

        public void resetClick (object sender, RoutedEventArgs e)
        {
            minesweeper.Children.Clear();
            GameManager nextGame = new GameManager();
            MineGrid nextInstance = nextGame.createGame(16, 16, 40);
            minesweeper.Children.Add(nextInstance);
        }
    }
}
