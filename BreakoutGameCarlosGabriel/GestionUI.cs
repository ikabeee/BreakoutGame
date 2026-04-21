using System;

namespace BreakoutGameCarlosGabriel
{
    public enum EtatJeu
    {
        Accueil,
        EnJeu,
        Pause,
        FinDePartie
    }

    public class GestionUI
    {
        public GestionUI()
        {
            // TODO : Initialiser les polices/couleurs
        }

        public void DessinerInterface(EtatJeu etatActuel, int score, int balles, int tableau)
        {
            switch (etatActuel)
            {
                case EtatJeu.Accueil:
                    DessinerEcranAccueil();
                    break;
                case EtatJeu.EnJeu:
                    DessinerHUD(score, balles, tableau);
                    break;
                case EtatJeu.Pause:
                    DessinerHUD(score, balles, tableau);
                    DessinerEcranPause();
                    break;
                case EtatJeu.FinDePartie:
                    DessinerEcranFin(score);
                    break;
            }
        }

        private void DessinerHUD(int score, int balles, int tableau)
        {
            // TODO : Afficher "Score: X", "Balles: Y", "Tableau: Z" en haut de l'écran
        }

        private void DessinerEcranAccueil()
        {
            // TODO : Afficher "CASSE-BRIQUES - Appuyez sur ESPACE pour commencer"
        }

        private void DessinerEcranPause()
        {
            // TODO : Afficher "PAUSE - Appuyez sur P pour reprendre"
        }

        private void DessinerEcranFin(int scoreFinal)
        {
            // TODO : Afficher "FIN DE PARTIE - Score : X"
        }
    }
}