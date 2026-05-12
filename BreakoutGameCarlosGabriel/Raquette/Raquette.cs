using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace BreakoutGameCarlosGabriel
{
    class Raquette : BasePourObjets
    {
        #region Attributs
        private const float VitesseDeplacement = 9.0f;

        private readonly float largeurFenetre;
        private readonly float positionYInitiale;
        private int directionDeplacement;
        #endregion

        #region ConstructeursInitialisation
        public Raquette(float largeurFenetre, float hauteurFenetre)
        {
            this.largeurFenetre = largeurFenetre;
            largeur = 120.0f;
            hauteur = 18.0f;
            positionYInitiale = hauteurFenetre - 48.0f;
            AssignerTexture("raquette.png");
            Reinitialiser();
        }
        #endregion

        #region MethodesPubliques
        public void Reinitialiser()
        {
            positionX = largeurFenetre / 2.0f - largeur / 2.0f;
            positionY = positionYInitiale;
            directionDeplacement = 0;
            mettreAJourListePoints();
        }

        public override void MettreAJour()
        {
            KeyboardState clavier = Keyboard.GetState();
            directionDeplacement = 0;

            if (clavier.IsKeyDown(Key.A) || clavier.IsKeyDown(Key.Left))
            {
                directionDeplacement--;
            }
            if (clavier.IsKeyDown(Key.D) || clavier.IsKeyDown(Key.Right))
            {
                directionDeplacement++;
            }

            positionX += directionDeplacement * VitesseDeplacement;

            if (positionX < 0.0f)
            {
                positionX = 0.0f;
            }
            else if (positionX + largeur > largeurFenetre)
            {
                positionX = largeurFenetre - largeur;
            }

            mettreAJourListePoints();
        }

        public override void Dessiner()
        {
            if (UtiliseTexture)
            {
                base.Dessiner();
                return;
            }

            GL.Disable(EnableCap.Texture2D);

            GL.Color3(0.1f, 0.45f, 0.95f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(listePoints[0]);
            GL.Vertex2(listePoints[1]);
            GL.Vertex2(listePoints[2]);
            GL.Vertex2(listePoints[3]);
            GL.End();

            GL.Color3(0.75f, 0.9f, 1.0f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(positionX + 8.0f, positionY + 4.0f);
            GL.Vertex2(positionX + largeur - 8.0f, positionY + 4.0f);
            GL.End();

            GL.Enable(EnableCap.Texture2D);
        }
        #endregion

        #region MethodesPrivees
        private void mettreAJourListePoints()
        {
            listePoints[0] = new Vector2(positionX, positionY);
            listePoints[1] = new Vector2(positionX + largeur, positionY);
            listePoints[2] = new Vector2(positionX + largeur, positionY + hauteur);
            listePoints[3] = new Vector2(positionX, positionY + hauteur);
        }
        #endregion

        #region Proprietes
        public int DirectionDeplacement { get => directionDeplacement; }
        public float Gauche { get => positionX; }
        public float Droite { get => positionX + largeur; }
        public float Haut { get => positionY; }
        public float Bas { get => positionY + hauteur; }
        #endregion
    }
}
