using System;

namespace BreakoutGameCarlosGabriel
{
    public abstract class BasePourObjets
    {
        // Attributs communs (protégés pour être accessibles par les enfants)
        protected float positionX;
        protected float positionY;
        protected float largeur;
        protected float hauteur;

        // Propriétés publiques
        public float PositionX { get => positionX; set => positionX = value; }
        public float PositionY { get => positionY; set => positionY = value; }
        public float Largeur { get => largeur; set => largeur = value; }
        public float Hauteur { get => hauteur; set => hauteur = value; }

        // Méthodes communes abstraites
        public abstract void MettreAJour();

        // La méthode Dessiner nécessitera probablement un objet graphique (ex: Graphics g) selon votre cadriciel
        public abstract void Dessiner();
    }
}