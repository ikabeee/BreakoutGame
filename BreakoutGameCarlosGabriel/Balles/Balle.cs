using System;

namespace BreakoutGameCarlosGabriel
{
    public class Balle : BasePourObjets
    {
        private float vitesseX;
        private float vitesseY;
        private bool estEnMouvement;

        public Balle(float largeur, float hauteur)
        {
            this.largeur = largeur;
            this.hauteur = hauteur;
            this.estEnMouvement = false;
        }

        public void Lancer(float vitesseInitialeX, float vitesseInitialeY)
        {
            this.vitesseX = vitesseInitialeX;
            this.vitesseY = vitesseInitialeY;
            this.estEnMouvement = true;
        }

        public void Reinitialiser()
        {
            this.estEnMouvement = false;
            this.vitesseX = 0;
            this.vitesseY = 0;
        }

        public void InverserVitesseX() { vitesseX = -vitesseX; }
        public void InverserVitesseY() { vitesseY = -vitesseY; }

        public override void MettreAJour()
        {
            if (!estEnMouvement)
            {
                // TODO : La balle doit suivre le centre de la raquette
                return;
            }

            // Application de la vitesse
            positionX += vitesseX;
            positionY += vitesseY;

            // TODO : Gérer les rebonds sur les murs (haut, gauche, droite) en appelant InverserVitesseX() ou InverserVitesseY()
        }

        public override void Dessiner()
        {
            // TODO : Dessiner la balle
        }
    }
}