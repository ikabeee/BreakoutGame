using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace BreakoutGameCarlosGabriel
{
    public class GestionJeu
    {
        #region Attributs
        private const double NbrImagesParSecondes = 60.0;

        private readonly GameWindow window;
        private List<Brique> listeBriquesCassables;
        private List<Brique> listeBriquesIndestructibles;

        private Raquette raquetteJoueur;
        private Balle balleJeu;
        private GestionAudio gestionAudio;
        private GestionUI gestionUI;
        #endregion

        #region EtatJeu
        public int BallesDisponibles { get; private set; }
        public int Score { get; private set; }
        public int TableauActuel { get; private set; }
        public EtatJeu EtatDuJeu { get; private set; }
        #endregion

        #region ConstructeurInitialisation
        public GestionJeu(GameWindow window)
        {
            this.window = window;
            start();
        }

        private void start()
        {
            double dureeAffichageChaqueImage = 1.0 / NbrImagesParSecondes;

            window.Load += chargement;
            window.Resize += redimensionner;
            window.KeyDown += clavierKeyDown;
            window.UpdateFrame += update;
            window.RenderFrame += rendu;

            window.Run(dureeAffichageChaqueImage);
        }
        #endregion

        #region ChargementJeu
        private void chargement(object sender, EventArgs e)
        {
            GL.ClearColor(0.03f, 0.03f, 0.05f, 1.0f);
            GL.Enable(EnableCap.Texture2D);

            listeBriquesCassables = new List<Brique>();
            listeBriquesIndestructibles = new List<Brique>();
            gestionAudio = new GestionAudio();
            gestionUI = new GestionUI();

            BallesDisponibles = 3;
            Score = 0;
            TableauActuel = 1;
            EtatDuJeu = EtatJeu.Accueil;

            afficherMenuConsole();
            GenererBriquesTableau1();
        }

        private void redimensionner(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, window.Width, window.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0, window.Width, window.Height, 0.0, -1.0, 1.0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        private void clavierKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    DemarrerJeu();
                    break;
                case Key.P:
                    MettreEnPause();
                    break;
                case Key.R:
                    ReinitialiserPartie();
                    break;
                case Key.Escape:
                    window.Exit();
                    break;
                case Key.Left:
                case Key.A:
                    Console.WriteLine("Raquette gauche");
                    break;
                case Key.Right:
                case Key.D:
                    Console.WriteLine("Raquette droite");
                    break;
            }
        }
        #endregion

        #region LogiqueJeu
        public void GenererBriquesTableau1()
        {
            listeBriquesCassables.Clear();
            listeBriquesIndestructibles.Clear();

            // TODO : Creer des classes concretes de Brique avant instanciation.
        }

        public void DemarrerJeu()
        {
            if (EtatDuJeu == EtatJeu.Accueil || EtatDuJeu == EtatJeu.FinDePartie)
            {
                EtatDuJeu = EtatJeu.EnJeu;
                Console.WriteLine("Debut partie");
            }
            else if (EtatDuJeu == EtatJeu.Pause)
            {
                EtatDuJeu = EtatJeu.EnJeu;
            }
        }

        public void MettreEnPause()
        {
            if (EtatDuJeu == EtatJeu.EnJeu)
            {
                EtatDuJeu = EtatJeu.Pause;
            }
            else if (EtatDuJeu == EtatJeu.Pause)
            {
                EtatDuJeu = EtatJeu.EnJeu;
            }
        }

        private void ReinitialiserPartie()
        {
            BallesDisponibles = 3;
            Score = 0;
            TableauActuel = 1;
            EtatDuJeu = EtatJeu.Accueil;
            GenererBriquesTableau1();
        }

        public void MettreAJourLeJeu()
        {
            if (EtatDuJeu != EtatJeu.EnJeu)
            {
                return;
            }

            raquetteJoueur?.MettreAJour();
            balleJeu?.MettreAJour();

            // TODO : Collisions balle/raquette, balle/briques, murs, perte de balle.
        }
        #endregion

        #region GestionAffichage
        private void update(object sender, FrameEventArgs e)
        {
            MettreAJourLeJeu();
        }

        private void rendu(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DessinerLeJeu();
            window.SwapBuffers();
        }

        public void DessinerLeJeu()
        {
            if (EtatDuJeu != EtatJeu.Accueil)
            {
                raquetteJoueur?.Dessiner();
                balleJeu?.Dessiner();

                foreach (Brique brique in listeBriquesCassables)
                {
                    brique.Dessiner();
                }

                foreach (Brique brique in listeBriquesIndestructibles)
                {
                    brique.Dessiner();
                }
            }

            gestionUI?.DessinerInterface(EtatDuJeu, Score, BallesDisponibles, TableauActuel);
        }

        private void afficherMenuConsole()
        {
            Console.WriteLine();
            Console.WriteLine("=== BREAKOUT ===");
            Console.WriteLine("Espace : demarrer / reprendre");
            Console.WriteLine("P      : pause");
            Console.WriteLine("A/<-   : raquette gauche");
            Console.WriteLine("D/->   : raquette droite");
            Console.WriteLine("R      : recommencer");
            Console.WriteLine("Echap  : quitter");
            Console.WriteLine();
        }
        #endregion
    }
}
