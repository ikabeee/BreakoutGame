using System;
using System.Collections.Generic;
using System.ComponentModel;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace BreakoutGameCarlosGabriel
{
    public class GestionJeu
    {
        #region Attributs
        private const double NbrImagesParSecondes = 60.0;
        private const float LargeurJeu = 1000.0f;
        private const float HauteurJeu = 700.0f;
        private const int NombreTableaux = 3;
        private const float EpsilonCollision = 0.1f;

        private readonly GameWindow window;
        private List<Brique> listeBriquesCassables;
        private List<Brique> listeBriquesIndestructibles;
        private int nombreBriquesCassablesInitiales;

        private Raquette raquetteJoueur;
        private Balle balleJeu;
        private AudioPrincipal gestionAudio;
        private GestionUI gestionUI;
        #endregion

        private struct CollisionBriqueInfo
        {
            public Brique Brique;
            public int Index;
            public bool EstCassable;
            public bool UtiliseCollisionBalayee;
            public float TempsCollision;
            public Vector2 NormaleCollision;
        }

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
            window.Closing += destructeur;

            window.Run(dureeAffichageChaqueImage);
        }
        #endregion

        #region ChargementJeu
        private void chargement(object sender, EventArgs e)
        {
            GL.ClearColor(0.01f, 0.01f, 0.02f, 1.0f);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            listeBriquesCassables = new List<Brique>();
            listeBriquesIndestructibles = new List<Brique>();
            gestionAudio = new AudioPrincipal();
            gestionUI = new GestionUI(LargeurJeu, HauteurJeu);

            ReinitialiserPartie();
            afficherMenuConsole();
        }

        private void redimensionner(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, window.Width, window.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0, LargeurJeu, HauteurJeu, 0.0, -1.0, 1.0);
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
            }
        }

        private void destructeur(object sender, CancelEventArgs e)
        {
            gestionAudio?.Dispose();
        }
        #endregion

        #region InitialisationTableaux
        private void initialiserObjetsJoueur()
        {
            raquetteJoueur = new Raquette(LargeurJeu, HauteurJeu);
            balleJeu = new Balle(18.0f, 18.0f, raquetteJoueur);
        }

        private void genererTableauActuel() 
        {
            int tableau = ((TableauActuel - 1) % NombreTableaux) + 1;

            switch (tableau)
            {
                case 1:
                    GenererBriquesTableau1();
                    break;
                case 2:
                    GenererBriquesTableau2();
                    break;
                case 3:
                    GenererBriquesTableau3();
                    break;
            }

            nombreBriquesCassablesInitiales = listeBriquesCassables.Count;
        }

        public void GenererBriquesTableau1()
        {
            viderListesBriques();

            float largeurBrique = 78.0f;
            float hauteurBrique = 26.0f;
            float espace = 8.0f;
            float departX = 70.0f;
            float departY = 72.0f;
            int pointsDeVieBriqueNormale = 1;

            for (int ligne = 0; ligne < 5; ligne++)
            {
                for (int colonne = 0; colonne < 10; colonne++)
                {
                    int pointsDeVie = pointsDeVieBriqueNormale;
                    TypeBrique type = TypeBrique.Normale;

                    if (ligne == 2 && (colonne == 2 || colonne == 7))
                    {
                        type = TypeBrique.Dynamique;
                        pointsDeVie = 1;
                    }

                    ajouterBriqueCassable(departX + colonne * (largeurBrique + espace), departY + ligne * (hauteurBrique + espace), largeurBrique, hauteurBrique, pointsDeVie, type);
                }
            }
        }

        public void GenererBriquesTableau2()
        {
            viderListesBriques();

            float largeurBrique = 70.0f;
            float hauteurBrique = 25.0f;
            float espace = 8.0f;
            float departX = 85.0f;
            float departY = 70.0f;
            int pointsDeVieBriqueNormale = 2;

            for (int ligne = 0; ligne < 6; ligne++)
            {
                for (int colonne = 0; colonne < 10; colonne++)
                {
                    float x = departX + colonne * (largeurBrique + espace);
                    float y = departY + ligne * (hauteurBrique + espace);

                    if ((colonne == 0 || colonne == 9) && ligne > 0)
                    {
                        ajouterBriqueIndestructible(x, y, largeurBrique, hauteurBrique);
                    }
                    else if ((ligne + colonne) % 5 == 0)
                    {
                        ajouterBriqueCassable(x, y, largeurBrique, hauteurBrique, 1, TypeBrique.Dynamique);
                    }
                    else
                    {
                        ajouterBriqueCassable(x, y, largeurBrique, hauteurBrique, pointsDeVieBriqueNormale, TypeBrique.Normale);
                    }
                }
            }
        }

        public void GenererBriquesTableau3()
        {
            viderListesBriques();

            float largeurBrique = 62.0f;
            float hauteurBrique = 24.0f;
            float espace = 7.0f;
            float departY = 68.0f;
            int pointsDeVieBriqueNormale = 3;

            for (int ligne = 0; ligne < 7; ligne++)
            {
                int nombreColonnes = 8 + (ligne % 2);
                float departX = (LargeurJeu - nombreColonnes * largeurBrique - (nombreColonnes - 1) * espace) / 2.0f;

                for (int colonne = 0; colonne < nombreColonnes; colonne++)
                {
                    float x = departX + colonne * (largeurBrique + espace);
                    float y = departY + ligne * (hauteurBrique + espace);

                    if (ligne == 3 && colonne >= 2 && colonne <= nombreColonnes - 3)
                    {
                        ajouterBriqueIndestructible(x, y, largeurBrique, hauteurBrique);
                    }
                    else if (ligne == 0 || ligne == 6 || colonne == 0 || colonne == nombreColonnes - 1)
                    {
                        ajouterBriqueCassable(x, y, largeurBrique, hauteurBrique, 1, TypeBrique.Dynamique);
                    }
                    else
                    {
                        ajouterBriqueCassable(x, y, largeurBrique, hauteurBrique, pointsDeVieBriqueNormale, TypeBrique.Normale);
                    }
                }
            }
        }

        private void viderListesBriques()
        {
            listeBriquesCassables.Clear();
            listeBriquesIndestructibles.Clear();
        }

        private void ajouterBriqueCassable(float x, float y, float largeur, float hauteur, int pointsDeVie, TypeBrique type)
        {
            listeBriquesCassables.Add(new Brique(x, y, largeur, hauteur, pointsDeVie, type));
        }

        private void ajouterBriqueIndestructible(float x, float y, float largeur, float hauteur)
        {
            listeBriquesIndestructibles.Add(new Brique(x, y, largeur, hauteur, 1, TypeBrique.Indestructible));
        }
        #endregion

        #region LogiqueJeu
        public void DemarrerJeu()
        {
            if (EtatDuJeu == EtatJeu.FinDePartie)
            {
                ReinitialiserPartie();
            }

            if (EtatDuJeu == EtatJeu.Accueil)
            {
                balleJeu.Lancer();
                EtatDuJeu = EtatJeu.EnJeu;
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

            initialiserObjetsJoueur();
            genererTableauActuel();
        }

        private void preparerNouvelleBalle()
        {
            raquetteJoueur.Reinitialiser();
            balleJeu.Reinitialiser(raquetteJoueur);
            EtatDuJeu = EtatJeu.Accueil;
        }

        public void MettreAJourLeJeu()
        {
            if (EtatDuJeu != EtatJeu.EnJeu)
            {
                return;
            }

            raquetteJoueur.MettreAJour();
            balleJeu.MettreAJour();

            gererCollisionsFenetre();
            gererCollisionBalleRaquette();
            gererCollisionBalleBriques();
            appliquerStrategiesBriques();
            verifierPerteBalle();
            verifierTableauComplete();
        }

        private void gererCollisionsFenetre()
        {
            if (balleJeu.Gauche <= 0.0f)
            {
                balleJeu.DefinirPositionX(0.0f);
                balleJeu.InverserVitesseX();
                gestionAudio.JouerSonRebond();
            }
            else if (balleJeu.Droite >= LargeurJeu)
            {
                balleJeu.DefinirPositionX(LargeurJeu - balleJeu.Largeur);
                balleJeu.InverserVitesseX();
                gestionAudio.JouerSonRebond();
            }

            if (balleJeu.Haut <= 0.0f)
            {
                balleJeu.DefinirPositionY(0.0f);
                balleJeu.InverserVitesseY();
                gestionAudio.JouerSonRebond();
            }
        }

        private void gererCollisionBalleRaquette()
        {
            if (balleJeu.VitesseY <= 0.0f || !collisionBalleRaquette())
            {
                return;
            }

            if (balleJeu.AncienBas <= raquetteJoueur.Haut)
            {
                balleJeu.DefinirPositionY(raquetteJoueur.Haut - balleJeu.Hauteur - 1.0f);
                balleJeu.InverserVitesseY();
                appliquerEffetRaquette();
                gestionAudio.JouerSonRebond();
            }
        }

        private void appliquerEffetRaquette()
        {
            int directionRaquette = raquetteJoueur.DirectionDeplacement;
            int directionBalle = Math.Sign(balleJeu.VitesseX);

            if (directionRaquette == 0)
            {
                return;
            }

            if (directionBalle != 0 && directionBalle != directionRaquette)
            {
                balleJeu.InverserVitesseX();
            }
            else
            {
                balleJeu.AugmenterVitesseHorizontale(directionRaquette);
            }
        }

        private void gererCollisionBalleBriques()
        {
            if (!chercherPremiereCollisionBrique(out CollisionBriqueInfo collision))
            {
                return;
            }

            gererRebondBrique(collision);
            Score += 5;

            if (!collision.EstCassable)
            {
                gestionAudio.JouerSonRebond();
                return;
            }

            Brique brique = listeBriquesCassables[collision.Index];
            brique.RecevoirDegat();

            if (brique.EstDetruite)
            {
                listeBriquesCassables.RemoveAt(collision.Index);
                gestionAudio.JouerSonDestruction();
            }
            else
            {
                gestionAudio.JouerSonRebond();
            }
        }

        private bool chercherPremiereCollisionBrique(out CollisionBriqueInfo meilleureCollision)
        {
            meilleureCollision = default(CollisionBriqueInfo);
            bool collisionTrouvee = false;

            for (int i = 0; i < listeBriquesCassables.Count; i++)
            {
                if (detecterCollisionBalleBrique(listeBriquesCassables[i], out Vector2 normaleCollision, out float tempsCollision, out bool utiliseCollisionBalayee) &&
                    (!collisionTrouvee || tempsCollision < meilleureCollision.TempsCollision))
                {
                    meilleureCollision = new CollisionBriqueInfo
                    {
                        Brique = listeBriquesCassables[i],
                        Index = i,
                        EstCassable = true,
                        UtiliseCollisionBalayee = utiliseCollisionBalayee,
                        TempsCollision = tempsCollision,
                        NormaleCollision = normaleCollision
                    };
                    collisionTrouvee = true;
                }
            }

            for (int i = 0; i < listeBriquesIndestructibles.Count; i++)
            {
                if (detecterCollisionBalleBrique(listeBriquesIndestructibles[i], out Vector2 normaleCollision, out float tempsCollision, out bool utiliseCollisionBalayee) &&
                    (!collisionTrouvee || tempsCollision < meilleureCollision.TempsCollision))
                {
                    meilleureCollision = new CollisionBriqueInfo
                    {
                        Brique = listeBriquesIndestructibles[i],
                        Index = i,
                        EstCassable = false,
                        UtiliseCollisionBalayee = utiliseCollisionBalayee,
                        TempsCollision = tempsCollision,
                        NormaleCollision = normaleCollision
                    };
                    collisionTrouvee = true;
                }
            }

            return collisionTrouvee;
        }

        private bool detecterCollisionBalleBrique(Brique brique, out Vector2 normaleCollision, out float tempsCollision, out bool utiliseCollisionBalayee)
        {
            if (detecterCollisionBalayeeBrique(brique, out normaleCollision, out tempsCollision))
            {
                utiliseCollisionBalayee = true;
                return true;
            }

            if (collisionBalleBrique(brique))
            {
                normaleCollision = calculerNormaleChevauchement(brique);
                tempsCollision = 0.0f;
                utiliseCollisionBalayee = false;
                return true;
            }

            normaleCollision = Vector2.Zero;
            tempsCollision = 1.0f;
            utiliseCollisionBalayee = false;
            return false;
        }

        private bool detecterCollisionBalayeeBrique(Brique brique, out Vector2 normaleCollision, out float tempsCollision)
        {
            normaleCollision = Vector2.Zero;
            tempsCollision = 1.0f;

            float deltaX = balleJeu.PositionX - balleJeu.AnciennePositionX;
            float deltaY = balleJeu.PositionY - balleJeu.AnciennePositionY;

            if (Math.Abs(deltaX) < float.Epsilon && Math.Abs(deltaY) < float.Epsilon)
            {
                return false;
            }

            float gaucheEtendue = brique.Gauche - balleJeu.Largeur;
            float droiteEtendue = brique.Droite;
            float hautEtendu = brique.Haut - balleJeu.Hauteur;
            float basEtendu = brique.Bas;

            float entreeX = float.NegativeInfinity;
            float sortieX = float.PositiveInfinity;
            float entreeY = float.NegativeInfinity;
            float sortieY = float.PositiveInfinity;

            if (deltaX > 0.0f)
            {
                entreeX = (gaucheEtendue - balleJeu.AnciennePositionX) / deltaX;
                sortieX = (droiteEtendue - balleJeu.AnciennePositionX) / deltaX;
            }
            else if (deltaX < 0.0f)
            {
                entreeX = (droiteEtendue - balleJeu.AnciennePositionX) / deltaX;
                sortieX = (gaucheEtendue - balleJeu.AnciennePositionX) / deltaX;
            }
            else if (balleJeu.AnciennePositionX < gaucheEtendue || balleJeu.AnciennePositionX > droiteEtendue)
            {
                return false;
            }

            if (deltaY > 0.0f)
            {
                entreeY = (hautEtendu - balleJeu.AnciennePositionY) / deltaY;
                sortieY = (basEtendu - balleJeu.AnciennePositionY) / deltaY;
            }
            else if (deltaY < 0.0f)
            {
                entreeY = (basEtendu - balleJeu.AnciennePositionY) / deltaY;
                sortieY = (hautEtendu - balleJeu.AnciennePositionY) / deltaY;
            }
            else if (balleJeu.AnciennePositionY < hautEtendu || balleJeu.AnciennePositionY > basEtendu)
            {
                return false;
            }

            float tempsEntree = Math.Max(entreeX, entreeY);
            float tempsSortie = Math.Min(sortieX, sortieY);

            if (tempsEntree > tempsSortie || tempsEntree < 0.0f || tempsEntree > 1.0f)
            {
                return false;
            }

            if (entreeX > entreeY)
            {
                normaleCollision = deltaX > 0.0f ? new Vector2(-1.0f, 0.0f) : new Vector2(1.0f, 0.0f);
            }
            else if (entreeY > entreeX)
            {
                normaleCollision = deltaY > 0.0f ? new Vector2(0.0f, -1.0f) : new Vector2(0.0f, 1.0f);
            }
            else if (Math.Abs(deltaX) >= Math.Abs(deltaY))
            {
                normaleCollision = deltaX > 0.0f ? new Vector2(-1.0f, 0.0f) : new Vector2(1.0f, 0.0f);
            }
            else
            {
                normaleCollision = deltaY > 0.0f ? new Vector2(0.0f, -1.0f) : new Vector2(0.0f, 1.0f);
            }

            tempsCollision = tempsEntree;
            return true;
        }

        private Vector2 calculerNormaleChevauchement(Brique brique)
        {
            bool collisionNordSud = balleJeu.AncienBas <= brique.Haut || balleJeu.AncienHaut >= brique.Bas;
            bool collisionEstOuest = balleJeu.AncienneDroite <= brique.Gauche || balleJeu.AncienneGauche >= brique.Droite;

            if (collisionNordSud)
            {
                return balleJeu.VitesseY >= 0.0f ? new Vector2(0.0f, -1.0f) : new Vector2(0.0f, 1.0f);
            }

            if (collisionEstOuest)
            {
                return balleJeu.VitesseX >= 0.0f ? new Vector2(-1.0f, 0.0f) : new Vector2(1.0f, 0.0f);
            }

            float penetrationGauche = Math.Abs(balleJeu.Droite - brique.Gauche);
            float penetrationDroite = Math.Abs(balleJeu.Gauche - brique.Droite);
            float penetrationHaut = Math.Abs(balleJeu.Bas - brique.Haut);
            float penetrationBas = Math.Abs(balleJeu.Haut - brique.Bas);
            float penetrationHorizontale = Math.Min(penetrationGauche, penetrationDroite);
            float penetrationVerticale = Math.Min(penetrationHaut, penetrationBas);

            if (penetrationVerticale <= penetrationHorizontale)
            {
                return penetrationHaut <= penetrationBas ? new Vector2(0.0f, -1.0f) : new Vector2(0.0f, 1.0f);
            }

            return penetrationGauche <= penetrationDroite ? new Vector2(-1.0f, 0.0f) : new Vector2(1.0f, 0.0f);
        }

        private void gererRebondBrique(CollisionBriqueInfo collision)
        {
            replacerBalleApresCollision(collision);

            if (collision.NormaleCollision.Y != 0.0f)
            {
                balleJeu.InverserVitesseY();
            }
            else if (collision.NormaleCollision.X != 0.0f)
            {
                balleJeu.InverserVitesseX();
            }
        }

        private void replacerBalleApresCollision(CollisionBriqueInfo collision)
        {
            if (collision.UtiliseCollisionBalayee)
            {
                float deltaX = balleJeu.PositionX - balleJeu.AnciennePositionX;
                float deltaY = balleJeu.PositionY - balleJeu.AnciennePositionY;
                float positionImpactX = balleJeu.AnciennePositionX + deltaX * collision.TempsCollision + collision.NormaleCollision.X * EpsilonCollision;
                float positionImpactY = balleJeu.AnciennePositionY + deltaY * collision.TempsCollision + collision.NormaleCollision.Y * EpsilonCollision;
                balleJeu.DefinirPosition(positionImpactX, positionImpactY);
                return;
            }

            float nouvellePositionX = balleJeu.PositionX;
            float nouvellePositionY = balleJeu.PositionY;

            if (collision.NormaleCollision.X < 0.0f)
            {
                nouvellePositionX = collision.Brique.Gauche - balleJeu.Largeur - EpsilonCollision;
            }
            else if (collision.NormaleCollision.X > 0.0f)
            {
                nouvellePositionX = collision.Brique.Droite + EpsilonCollision;
            }
            else if (collision.NormaleCollision.Y < 0.0f)
            {
                nouvellePositionY = collision.Brique.Haut - balleJeu.Hauteur - EpsilonCollision;
            }
            else if (collision.NormaleCollision.Y > 0.0f)
            {
                nouvellePositionY = collision.Brique.Bas + EpsilonCollision;
            }

            balleJeu.DefinirPosition(nouvellePositionX, nouvellePositionY);
        }

        private void appliquerStrategiesBriques()
        {
            ContexteStrategieBrique contexte = new ContexteStrategieBrique
            {
                BriquesDetruites = Math.Max(0, nombreBriquesCassablesInitiales - listeBriquesCassables.Count),
                TotalBriquesCassables = nombreBriquesCassablesInitiales
            };

            for (int i = 0; i < listeBriquesCassables.Count; i++)
            {
                listeBriquesCassables[i].AppliquerStrategie(contexte);
            }
        }

        private void verifierPerteBalle()
        {
            if (balleJeu.Haut <= HauteurJeu)
            {
                return;
            }

            BallesDisponibles--;
            gestionAudio.JouerSonPerteBalle();

            if (BallesDisponibles <= 0)
            {
                EtatDuJeu = EtatJeu.FinDePartie;
            }
            else
            {
                preparerNouvelleBalle();
            }
        }

        private void verifierTableauComplete()
        {
            if (EtatDuJeu != EtatJeu.EnJeu || listeBriquesCassables.Count > 0)
            {
                return;
            }

            BallesDisponibles += 2;
            Score += 50;
            TableauActuel++;
            genererTableauActuel();
            preparerNouvelleBalle();
        }
        #endregion

        #region Collisions
        private bool collisionBalleRaquette()
        {
            return balleJeu.Droite >= raquetteJoueur.Gauche &&
                   balleJeu.Gauche <= raquetteJoueur.Droite &&
                   balleJeu.Bas >= raquetteJoueur.Haut &&
                   balleJeu.Haut <= raquetteJoueur.Bas;
        }

        private bool collisionBalleBrique(Brique brique)
        {
            return balleJeu.Droite >= brique.Gauche &&
                   balleJeu.Gauche <= brique.Droite &&
                   balleJeu.Bas >= brique.Haut &&
                   balleJeu.Haut <= brique.Bas;
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
            dessinerObjetsJeu();
            gestionUI?.DessinerInterface(EtatDuJeu, Score, BallesDisponibles, TableauActuel);
        }

        private void dessinerObjetsJeu()
        {
            for (int i = 0; i < listeBriquesCassables.Count; i++)
            {
                listeBriquesCassables[i].Dessiner();
            }

            for (int i = 0; i < listeBriquesIndestructibles.Count; i++)
            {
                listeBriquesIndestructibles[i].Dessiner();
            }

            raquetteJoueur?.Dessiner();
            balleJeu?.Dessiner();
        }

        private void afficherMenuConsole()
        {
            Console.WriteLine();
            Console.WriteLine("=== BREAKOUT ===");
            Console.WriteLine("Espace : lancer la balle / reprendre");
            Console.WriteLine("A ou <- : deplacer la raquette a gauche");
            Console.WriteLine("D ou -> : deplacer la raquette a droite");
            Console.WriteLine("P      : pause / reprendre");
            Console.WriteLine("R      : recommencer");
            Console.WriteLine("Echap  : quitter");
            Console.WriteLine();
        }
        #endregion
    }
}
