using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using static Minesweeper.MainWindow;

namespace Minesweeper
{
    public class GameManager
    {
        public int totalRows;
        public int totalCols;
        public int totalMines;
        private int targetScore;
        MineGrid minefieldGrid;
        MineGrid minesweeper = new MineGrid();

        public MineGrid createGame()
        {
            // Initialise game grid
            char[,] minefield = populateMinefield(totalRows, totalCols, totalMines);
            GridButton[,] gameButtons = createButtons(totalRows, totalCols, minefield);
            minefieldGrid = createGrid(totalRows, totalCols, gameButtons);
            this.targetScore = winCondition();
            
            return minefieldGrid;
        }

        private int winCondition()
        {
            int targetScore = totalRows * totalCols - totalMines;
            return targetScore;
        }

        private char[,] populateMinefield(int rows, int columns, int bombs)
        {
            char[,] mineGrid = new char[rows, columns];
            Random rnd = new Random();

            // Randomly distibute bombs
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

            // Fill empty squares with number of adjacent bombs
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
            // Search in 1x1 grid around cell for bombs
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

            // Format cell content for game
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
                    // Create button and assign properties
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

                    // Define button type 
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
            // set 
            minesweeper.rows = rows;
            minesweeper.columns = columns;
            minesweeper.minefieldButtons = minefield;

            // Formatting for view
            minesweeper.HorizontalAlignment = HorizontalAlignment.Center;
            minesweeper.VerticalAlignment = VerticalAlignment.Center;

            // Define X and Y for grid in view
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

        private void disableButtons()
        {
            for (int i = 0; i < totalRows; i++)
            {
                for (int j = 0; j < totalCols; j++)
                {
                    minefieldGrid.minefieldButtons[i, j].Click -= buttonClick;
                    minefieldGrid.minefieldButtons[i, j].MouseRightButtonUp -= rightClick;
                }
            }
        }

        private void showWinScreen()
        {
            bool hasWon = minefieldGrid.checkWinCon(targetScore, totalMines);
            if (hasWon)
            {
                disableButtons();
                MessageBoxButton winscreen = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, "You have won!");
            }
        }

        private void buttonClick(object sender, RoutedEventArgs e)
        {
            // Logic for dealing with all cases when a tile is clicked
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
                    disableButtons();
                }
            }
            showWinScreen();
        }

        private void rightClick(object sender, RoutedEventArgs e)
        {
            // Place or remove a flag when a tile is right clicked
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
            showWinScreen();
        }
    }
}
