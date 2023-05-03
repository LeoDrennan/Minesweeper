using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            const int columns = 16;
            const int rows = 16;
            const int mines = 40;

            Application.Current.MainWindow.Height = 35 * rows + 70;
            Application.Current.MainWindow.Width = 35 * columns;

            InitializeComponent();

            createGrid(rows, columns);
            char[,] minefield = populateMinefield(rows, columns, mines);
            createButtons(rows, columns, minefield);
        }

        public void createGrid(int rows, int columns)
        {
            mineGrid minesweeper = new mineGrid();
            minesweeper.HorizontalAlignment = HorizontalAlignment.Center;
            minesweeper.VerticalAlignment = VerticalAlignment.Center;

            for (int i = 0; i < rows; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(30);
                this.minesweeper.RowDefinitions.Add(rowDefinition);
            }

            for (int i = 0; i < columns; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(30);
                this.minesweeper.ColumnDefinitions.Add(columnDefinition);

            }
        }

        public char[,] populateMinefield(int rows, int columns, int bombs)
        {
            char[,] mineGrid = new char[rows, columns];
            Random rnd = new Random();

            int counter = 0;
            while (counter < bombs)
            {
                int row = rnd.Next(rows);
                int column = rnd.Next(columns);

                if (mineGrid[row, column] != '*')
                {
                    mineGrid[row, column] = '*';
                    counter++;
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (mineGrid[i, j] != '*')
                    {
                        neighbouringMines(mineGrid, i, j, rows, columns);
                    };
                };
            }
            return mineGrid;
        }

        public void createButtons(int rows, int columns, char[,] minefield)
        {
            int count = 1;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    gridButton btn = new gridButton();
                    btn.Name = "button" + count.ToString();
                    btn.Click += buttonClick;
                    btn.MouseRightButtonUp += rightClick;
                    btn.pressed = false;
                    btn.isFlag = false;
                    btn.Content = '\0';
                    btn.value = minefield[i,j];

                    switch (btn.value)
                    {
                        case '*':
                            btn.type = "mine";
                            break;
                        case '0':
                            btn.type = "zero";
                            break;
                        default:
                            btn.type = "number";
                            break;

                    }
                    Grid.SetColumn(btn, j);
                    Grid.SetRow(btn, i);
                    minesweeper.Children.Add(btn);
                    count++;
                }
            }
        }

        public char[,] neighbouringMines(char[,] mineGrid, int row, int column, int totalRows, int totalColumns)
        {
            int mineCount = 0;
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = column - 1; j <= column + 1; j++)
                {
                    if (i >= 0 && i < totalRows && j >= 0 && j < totalColumns && mineGrid[i,j] == '*')
                    {
                        mineCount++;
                    }
                }
            }

            if (mineCount == 0)
            {
                mineGrid[row, column] = ' ';
            }
            else
            {
                int asciiConversion = mineCount + 48;
                mineGrid[row, column] = Convert.ToChar(asciiConversion);
            }

            return mineGrid;
        }

        public void buttonClick(object sender, RoutedEventArgs e)
        {
            gridButton pressedButton = sender as gridButton;
            if (!pressedButton.isFlag && pressedButton.type != "mine")
            {
                pressedButton.pressed = true;
                pressedButton.revealIdentity(pressedButton.pressed);
            }
            else if (!pressedButton.isFlag && pressedButton.type == "mine")
            {
                pressedButton.pressed = true;
                pressedButton.revealIdentity(pressedButton.pressed);

            }
        }

        public void rightClick(object sender, RoutedEventArgs e)
        {
            gridButton pressedButton = sender as gridButton;
            if (pressedButton.pressed == false && pressedButton.isFlag == false)
            {
                pressedButton.isFlag = true;
                pressedButton.placeFlag(pressedButton.isFlag);
            } 
            else if (pressedButton.pressed == false && pressedButton.isFlag == true)
            {
                pressedButton.isFlag = false;
                pressedButton.placeFlag(pressedButton.isFlag);
            }
        }

        private void resetClick(object sender, RoutedEventArgs e)
        {

        }

        class gridButton : Button
        {
            public char value;
            public bool pressed;
            public bool isFlag;
            public string type;

            string iconName = "\uD83D" + "\uDEA9";

            public void revealIdentity(bool pressed)
            {
                if (pressed && this.value != '*')
                {
                    this.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#eff3ba");
                    this.Content = this.value;
                }
                else if (pressed && this.value == '*')
                {
                    this.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000");
                    this.Content = this.value;
                }
                return;
            }

            public void placeFlag(bool isFlag)
            {
                switch (isFlag)
                {
                    case true:
                        this.Content = iconName;
                        break;
                    case false:
                        this.Content = ' ';
                        break;
                }
            }
        }

        class mineGrid : Grid
        {
            
        }
    }
}
