using System.Windows;

namespace Anime_Stiker;
public partial class CharactersWindow : Window
{
    public CharactersWindow()
    {
        InitializeComponent();
        SetWindowPosition();
        InitListBoxOfCharacters();
    }

    private void SetWindowPosition()
    {
        this.WindowStartupLocation = WindowStartupLocation.Manual;

        double screenWidth = SystemParameters.PrimaryScreenWidth;
        //double screenHeight = SystemParameters.PrimaryScreenHeight;

        this.Left = screenWidth - this.Width;
        this.Top = 250;
    }
    private void InitListBoxOfCharacters()
    {

    }
}
