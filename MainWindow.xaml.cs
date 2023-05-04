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
        MineGrid minefieldGrid;
        public MainWindow()
        {
            // Window and grid size declaration
            const int columns = 16;
            const int rows = 16;
            const int mines = 40;
            Application.Current.MainWindow.Height = 35 * rows + 70;
            Application.Current.MainWindow.Width = 35 * columns;

            InitializeComponent();

            char[,] minefield = populateMinefield(rows, columns, mines);
            GridButton[,] gameButtons = createButtons(rows, columns, minefield);
            minefieldGrid = createGrid(rows, columns, gameButtons);
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

        public GridButton[,] createButtons(int rows, int columns, char[,] minefield)
        {
            int count = 1;
            GridButton[,] gameButtons = new GridButton[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    GridButton btn = new GridButton();
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
                        case ' ':
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

        public MineGrid createGrid(int rows, int columns, GridButton[,] minefield)
        {
            MineGrid minesweeper = new MineGrid();
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

            return minesweeper;
        }

        public void buttonClick(object sender, RoutedEventArgs e)
        {
            GridButton pressedButton = sender as GridButton;
            if (!pressedButton.isFlag)
            {
                if (pressedButton.type == "number")
                {
                    pressedButton.revealIdentity();
                }
                else if (pressedButton.type == "zero")
                {
                    minefieldGrid.revealZeroes(pressedButton.xLoc, pressedButton.yLoc);
                }
                else
                {
                    minefieldGrid.showAllMines();
                }
            }
            
        }

        public void rightClick(object sender, RoutedEventArgs e)
        {
            GridButton pressedButton = sender as GridButton;
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

        public class GridButton : Button
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
                if (this.type == "number")
                {
                    this.pressed = true;
                    this.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#eff3ba");
                    this.Content = this.hiddenContent;
                }
                else if (this.type == "zero")
                {
                    this.pressed = true;
                    this.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#eff3ba");
                }
                else if (this.type == "mine")
                {
                    this.pressed = true;
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
    }
}
