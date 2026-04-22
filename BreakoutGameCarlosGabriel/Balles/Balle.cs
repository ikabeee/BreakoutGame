using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace BreakoutGameCarlosGabriel
{
    class Balle : BasePourObjets
    {
        #region Attributs
        private const float VitesseInitialeX = 4.0f;
        private const float VitesseInitialeY = -6.0f;
        private const float VitesseMaxX = 11.0f;

        private Raquette raquette;
        private float vitesseX;
        private float vitesseY;
        private float anciennePositionX;
        private float anciennePositionY;
        private bool estEnMouvement;
        private int nombreRebonds;
        #endregion

        #region ConstructeursInitialisation
        public Balle(float largeur, float hauteur, Raquette raquette)
        {
            this.largeur = largeur;
            this.hauteur = hauteur;
            this.raquette = raquette;
            Reinitialiser(raquette);
        }
        #endregion

        #region MethodesPubliques
        public void Lancer()
        {
            if (!estEnMouvement)
            {
                vitesseX = VitesseInitialeX;
                vitesseY = VitesseInitialeY;
                estEnMouvement = true;
            }
        }

        public void Reinitialiser(Raquette nouvelleRaquette)
        {
            raquette = nouvelleRaquette;
            vitesseX = 0.0f;
            vitesseY = 0.0f;
            estEnMouvement = false;
            nombreRebonds = 0;
            placerSurRaquette();
            mettreAJourListePoints();
        }

        public void InverserVitesseX()
        {
            vitesseX = -vitesseX;
            nombreRebonds++;
        }

        public void InverserVitesseY()
        {
            vitesseY = -vitesseY;
            nombreRebonds++;
        }

        public void AugmenterVitesseHorizontale(int direction)
        {
            if (direction == 0)
            {
                return;
            }

            vitesseX += direction * 1.5f;
            if (vitesseX > VitesseMaxX)
            {
                vitesseX = VitesseMaxX;
            }
            else if (vitesseX < -VitesseMaxX)
            {
                vitesseX = -VitesseMaxX;
            }
        }

        public override void MettreAJour()
        {
            anciennePositionX = positionX;
            anciennePositionY = positionY;

            if (!estEnMouvement)
            {
                placerSurRaquette();
            }
            else
            {
                positionX += vitesseX;
                positionY += vitesseY;
            }

            mettreAJourListePoints();
        }

        public override void Dessiner()
        {
            float centreX = positionX + largeur / 2.0f;
            float centreY = positionY + hauteur / 2.0f;
            float rayon = largeur / 2.0f;

            GL.Disable(EnableCap.Texture2D);
            GL.Color3(0.96f, 0.96f, 0.86f);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex2(centreX, centreY);
            for (int i = 0; i <= 32; i++)
            {
                double angle = i * Math.PI * 2.0 / 32.0;
                GL.Vertex2(centreX + Math.Cos(angle) * rayon, centreY + Math.Sin(angle) * rayon);
            }
            GL.End();

            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Begin(PrimitiveType.LineLoop);
            for (int i = 0; i < 32; i++)
            {
                double angle = i * Math.PI * 2.0 / 32.0;
                GL.Vertex2(centreX + Math.Cos(angle) * rayon, centreY + Math.Sin(angle) * rayon);
            }
            GL.End();
            GL.Enable(EnableCap.Texture2D);
        }
        #endregion

        #region MethodesPrivees
        private void placerSurRaquette()
        {
            positionX = raquette.PositionX + raquette.Largeur / 2.0f - largeur / 2.0f;
            positionY = raquette.PositionY - hauteur - 2.0f;
        }

        private void mettreAJourListePoints()
        {
            listePoints[0] = new Vector2(positionX, positionY);
            listePoints[1] = new Vector2(positionX + largeur, positionY);
            listePoints[2] = new Vector2(positionX + largeur, positionY + hauteur);
            listePoints[3] = new Vector2(positionX, positionY + hauteur);
        }
        #endregion

        #region Proprietes
        public float VitesseX { get => vitesseX; set => vitesseX = value; }
        public float VitesseY { get => vitesseY; set => vitesseY = value; }
        public float AnciennePositionX { get => anciennePositionX; }
        public float AnciennePositionY { get => anciennePositionY; }
        public bool EstEnMouvement { get => estEnMouvement; }
        public int NombreRebonds { get => nombreRebonds; }
        public float Gauche { get => positionX; }
        public float Droite { get => positionX + largeur; }
        public float Haut { get => positionY; }
        public float Bas { get => positionY + hauteur; }
        public float AncienneGauche { get => anciennePositionX; }
        public float AncienneDroite { get => anciennePositionX + largeur; }
        public float AncienHaut { get => anciennePositionY; }
        public float AncienBas { get => anciennePositionY + hauteur; }
        #endregion
    }
}
