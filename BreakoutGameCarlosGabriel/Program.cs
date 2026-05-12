using OpenTK;
using OpenTK.Graphics;

namespace BreakoutGameCarlosGabriel
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int largeurFenetre = 1920;
            int hauteurFenetre = 1080;
            string titreFenetre = "Breakout Pro Edition - Carlos Gabriel";

            DisplayDevice moniteur = DisplayDevice.Default;
            if (DisplayDevice.Default == DisplayDevice.GetDisplay(DisplayIndex.Second))
            {
                moniteur = DisplayDevice.GetDisplay(DisplayIndex.First);
            }

            using (GameWindow window = new GameWindow(
                largeurFenetre,
                hauteurFenetre,
                GraphicsMode.Default,
                titreFenetre,
                GameWindowFlags.Default,
                moniteur))
            {
                new GestionJeu(window);
            }
        }
    }
}