using System;
using System.Windows.Controls;
using System.Windows.Media;
using static Minesweeper.MainWindow;

public class MineGrid : Grid
{
    public int rows;
    public int columns;
    public GridButton[,] minefieldButtons;

    // Reveal all mines in the current grid
    public void showAllMines()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (minefieldButtons[i, j].type == "mine")
                {
                    minefieldButtons[i, j].Content = minefieldButtons[i, j].hiddenContent;
                    minefieldButtons[i, j].Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000");
                }
            }
        }
        return;
    }

    // Reveal 1x1 area surrounding a location, recursively doing the same for all neighbouring zeroes found
    public void revealZeroes(int row, int column)
    {
        for (int i = row - 1; i <= row + 1; i++)
        {
            for (int j = column - 1; j <= column + 1; j++)
            {
                // Check cell is within bounds. If yes and the cell is not a zero, reveal it.
                if (i >= 0 && i < rows && j >= 0 && j < columns && minefieldButtons[i, j].pressed == false && minefieldButtons[i, j].type != "zero")
                {
                    minefieldButtons[i, j].revealIdentity();
                }

                // Check cell is within bounds. If yes and the cell is a zero, reveal it and call function on that cell.
                else if (i >= 0 && i < rows && j >= 0 && j < columns && minefieldButtons[i, j].pressed == false && minefieldButtons[i, j].type == "zero")
                {
                    minefieldButtons[i, j].revealIdentity();
                    revealZeroes(i, j);
                }
            }
        }
        return;
    }

    // Check current game state and return true or false for whether the game has been won
    public bool checkWinCon(int targetScore, int totalMines)
    {
        bool won = false;
        int pressedCount = 0;
        int flaggedCount = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (minefieldButtons[i, j].pressed == true)
                {
                    pressedCount++;
                }
                if (minefieldButtons[i, j].isFlag == true)
                {
                    flaggedCount++;
                }
            }
        }
        if (pressedCount == targetScore && flaggedCount == totalMines)
        {
            won = true;
        }
        return won;
    }
}
