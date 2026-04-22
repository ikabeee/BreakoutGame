using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Threading.Tasks;

namespace BreakoutGameCarlosGabriel
{
    class Program
    {
        static void Main(string[] args)
        {
            int largeurFenetre = 600;
            int hauteurFenetre = 300;
            string titreFenetre = "Breakout Game - Carlos Gabriel";

            DisplayDevice moniteur = DisplayDevice.Default;
            if (DisplayDevice.Default == DisplayDevice.GetDisplay(DisplayIndex.Second))
            {
                moniteur = DisplayDevice.GetDisplay(DisplayIndex.First);
            }
            GameWindow window = new GameWindow(largeurFenetre, hauteurFenetre, GraphicsMode.Default, titreFenetre, GameWindowFlags.Default, moniteur);
            GestionJeu fenetrePrincipale = new GestionJeu(window);
        }
    }
}
