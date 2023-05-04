using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using static Minesweeper.MainWindow;

namespace Minesweeper
{
    public class GameManager
    {
        public int totalRows;
        public int totalCols;
        public int totalMines;
        MineGrid minefieldGrid;
        MineGrid minesweeper = new MineGrid();

        public MineGrid createGame(int rows, int columns, int mines)
        {
            // Initialise game grid
            char[,] minefield = populateMinefield(rows, columns, mines);
            GridButton[,] gameButtons = createButtons(rows, columns, minefield);
            minefieldGrid = createGrid(rows, columns, gameButtons);
            
            return minefieldGrid;
        }

        private char[,] populateMinefield(int rows, int columns, int bombs)
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

        private char[,] neighbouringMines(char[,] mineGrid, int row, int column, int totalRows, int totalColumns)
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

        private GridButton[,] createButtons(int rows, int columns, char[,] minefield)
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

        private MineGrid createGrid(int rows, int columns, GridButton[,] minefield)
        {

            minesweeper.rows = rows;
            minesweeper.columns = columns;
            minesweeper.minefieldButtons = minefield;

            minesweeper.HorizontalAlignment = HorizontalAlignment.Center;
            minesweeper.VerticalAlignment = VerticalAlignment.Center;

            for (int i = 0; i < rows; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(30);
                minesweeper.RowDefinitions.Add(rowDefinition);
            }

            for (int i = 0; i < columns; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(30);
                minesweeper.ColumnDefinitions.Add(columnDefinition);
            }

            return minesweeper;
        }

        private void buttonClick(object sender, RoutedEventArgs e)
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

        private void rightClick(object sender, RoutedEventArgs e)
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

    }
}
