using System;
using System.Collections.Generic;

namespace BreakoutGameCarlosGabriel
{
    public class GestionJeu
    {
        // Listes
        private List<Brique> listeBriquesCassables;
        private List<Brique> listeBriquesIndestructibles;

        // Objets du jeu
        private Raquette raquetteJoueur;
        private Balle balleJeu;
        private GestionAudio gestionAudio;
        private GestionUI gestionUI;

        // Variables d'état
        public int BallesDisponibles { get; private set; }
        public int Score { get; private set; }
        public int TableauActuel { get; private set; }
        public EtatJeu EtatDuJeu { get; private set; }

        public GestionJeu()
        {
            listeBriquesCassables = new List<Brique>();
            listeBriquesIndestructibles = new List<Brique>();
            gestionAudio = new GestionAudio();
            gestionUI = new GestionUI();

            BallesDisponibles = 3;
            Score = 0;
            TableauActuel = 1;
            EtatDuJeu = EtatJeu.Accueil;

            // TODO : Instancier la raquette et la balle avec leurs dimensions de départ
        }

        public void GenererBriquesTableau1()
        {
            // TODO : Utiliser des boucles pour remplir les listes de briques (effort maximum)
        }

        public void DemarrerJeu()
        {
            if (EtatDuJeu == EtatJeu.Accueil || EtatDuJeu == EtatJeu.FinDePartie)
            {
                EtatDuJeu = EtatJeu.EnJeu;
                // TODO : Lancer la balle
            }
        }

        public void MettreEnPause()
        {
            if (EtatDuJeu == EtatJeu.EnJeu) EtatDuJeu = EtatJeu.Pause;
            else if (EtatDuJeu == EtatJeu.Pause) EtatDuJeu = EtatJeu.EnJeu;
        }

        public void MettreAJourLeJeu()
        {
            if (EtatDuJeu != EtatJeu.EnJeu) return;

            raquetteJoueur.MettreAJour();
            balleJeu.MettreAJour();

            // TODO : Gérer les collisions (balle vs raquette, balle vs briques)
            // TODO : Si balle sort par le bas -> BallesDisponibles--, reinitialiser balle/raquette, jouer son.
            // TODO : Si BallesDisponibles == 0 -> EtatDuJeu = EtatJeu.FinDePartie
        }

        public void DessinerLeJeu()
        {
            // 1. Dessiner les objets du jeu seulement si on n'est pas à l'accueil
            if (EtatDuJeu != EtatJeu.Accueil)
            {
                raquetteJoueur.Dessiner();
                balleJeu.Dessiner();

                foreach (Brique brique in listeBriquesCassables)
                {
                    brique.Dessiner();
                }
                foreach (Brique brique in listeBriquesIndestructibles)
                {
                    brique.Dessiner();
                }
            }

            // 2. Toujours dessiner l'interface par-dessus
            gestionUI.DessinerInterface(EtatDuJeu, Score, BallesDisponibles, TableauActuel);
        }
    }
}