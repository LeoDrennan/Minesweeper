using System;
using System.Windows.Controls;
using System.Windows.Media;
using static Minesweeper.MainWindow;

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
