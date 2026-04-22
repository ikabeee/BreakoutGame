using System;

namespace BreakoutGameCarlosGabriel
{
    abstract class Brique : BasePourObjets
    {
        // Attributs spécifiques
        private int pointsDeVie;
        private bool estDestructible;
        private bool estDynamique;

        public int PointsDeVie { get => pointsDeVie; }
        public bool EstDestructible { get => estDestructible; }

        public Brique(float x, float y, float largeur, float hauteur, int pv, bool destructible, bool dynamique)
        {
            this.positionX = x;
            this.positionY = y;
            this.largeur = largeur;
            this.hauteur = hauteur;
            this.pointsDeVie = pv;
            this.estDestructible = destructible;
            this.estDynamique = dynamique;
        }

        public void RecevoirDegat()
        {
            if (estDestructible && pointsDeVie > 0)
            {
                pointsDeVie--;
                // TODO : Mettre à jour l'apparence ou la couleur de la brique ici
            }
        }

        public override void MettreAJour()
        {
            // TODO : Stratégie pour les briques dynamiques (ex: augmenter les PV avec le temps)
        }

        public override void Dessiner()
        {
            // TODO : Dessiner la brique avec la couleur correspondant à ses pointsDeVie
        }
    }
}