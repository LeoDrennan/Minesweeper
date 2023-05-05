using System;
using System.Windows.Controls;
using System.Windows.Media;
using static Minesweeper.MainWindow;

public class MineGrid : Grid
{
    public int rows;
    public int columns;
    public GridButton[,] minefieldButtons;

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

    public void revealZeroes(int row, int column)
    {
        for (int i = row - 1; i <= row + 1; i++)
        {
            for (int j = column - 1; j <= column + 1; j++)
            {
                if (i >= 0 && i < rows && j >= 0 && j < columns && minefieldButtons[i, j].pressed == false && minefieldButtons[i, j].type != "zero")
                {
                    minefieldButtons[i, j].revealIdentity();
                }

                else if (i >= 0 && i < rows && j >= 0 && j < columns && minefieldButtons[i, j].pressed == false && minefieldButtons[i, j].type == "zero")
                {
                    minefieldButtons[i, j].revealIdentity();
                    revealZeroes(i, j);
                }
            }
        }
        return;
    }

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
