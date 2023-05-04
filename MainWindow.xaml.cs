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
        public MainWindow()
        {
            // Window and grid size declaration
            const int columns = 10;
            const int rows = 10;
            const int mines = 10;
            Application.Current.MainWindow.Height = 35 * rows + 70;
            Application.Current.MainWindow.Width = 35 * columns;

            InitializeComponent();


            char[,] minefield = populateMinefield(rows, columns, mines);
            gridButton[,] gameButtons = createButtons(rows, columns, minefield);
            createGrid(rows, columns, gameButtons);
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

        public char[,] neighbouringMines(char[,] mineGrid, int row, int column, int totalRows, int totalColumns)
        {
            int mineCount = 0;
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = column - 1; j <= column + 1; j++)
                {
                    if (i >= 0 && i < totalRows && j >= 0 && j < totalColumns && mineGrid[i, j] == '*')
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

        public gridButton[,] createButtons(int rows, int columns, char[,] minefield)
        {
            int count = 1;
            gridButton[,] gameButtons = new gridButton[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    gridButton btn = new gridButton();
                    btn.xLoc = i;
                    btn.yLoc = j;
                    btn.Name = "button" + count.ToString();
                    btn.Click += buttonClick;
                    btn.MouseRightButtonUp += rightClick;
                    btn.pressed = false;
                    btn.isFlag = false;
                    btn.Content = '\0';
                    btn.hiddenContent = minefield[i, j];

                    switch (btn.hiddenContent)
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
                    gameButtons[i, j] = btn;
                    count++;
                }
            }

            return gameButtons;
        }

        public void createGrid(int rows, int columns, gridButton[,] minefield)
        {
            mineGrid minesweeper = new mineGrid();
            minesweeper.rows = rows;
            minesweeper.columns = columns;
            minesweeper.minefieldButtons = minefield;

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

        public void buttonClick(object sender, RoutedEventArgs e)
        {
            gridButton pressedButton = sender as gridButton;
            if (!pressedButton.isFlag)
            {
                pressedButton.pressed = true;
                if (pressedButton.type != "*")
                {
                    pressedButton.revealIdentity();
                }
                else
                {
                    mineGrid currentGame = sender as mineGrid;
                    currentGame.showAllMines();
                }
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

        public class gridButton : Button
        {
            public int xLoc;
            public int yLoc;
            public char hiddenContent;
            public bool pressed;
            public bool isFlag;
            public string type;
            string flagEmoji = "\uD83D" + "\uDEA9";

            public void revealIdentity()
            {
                if (this.hiddenContent != '*')
                {
                    this.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#eff3ba");
                    this.Content = this.hiddenContent;
                }
                else if (this.hiddenContent == ' ')
                {
                    this.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#eff3ba");
                }
                else if (this.hiddenContent == '*')
                {
                    this.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000");
                    this.Content = this.hiddenContent;

                }
                return;
            }

            public void placeFlag(bool isFlag)
            {
                switch (isFlag)
                {
                    case true:
                        this.Content = flagEmoji;
                        break;
                    case false:
                        this.Content = ' ';
                        break;
                }
            }
        }

        public class mineGrid : Grid
        {
            public int rows;
            public int columns;
            public gridButton[,] minefieldButtons;

            public void showAllMines()
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (minefieldButtons[i,j].type == "mine")
                        {
                            minefieldButtons[i, j].Content = minefieldButtons[i, j].hiddenContent;
                            minefieldButtons[i, j].Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000");
                        }
                    }
                }
                return;
            }
        }
    }
}
