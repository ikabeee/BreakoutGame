using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace BreakoutGameCarlosGabriel
{
    enum TypeBrique
    {
        Normale,
        Indestructible,
        Dynamique
    }

    class ContexteStrategieBrique
    {
        public int TableauActuel { get; set; }
        public int BriquesRestantes { get; set; }
        public int NombreRebondsBalle { get; set; }
    }

    interface IStrategieBriqueDynamique
    {
        void Appliquer(Brique brique, ContexteStrategieBrique contexte);
    }

    class StrategieSelonTableau : IStrategieBriqueDynamique
    {
        public void Appliquer(Brique brique, ContexteStrategieBrique contexte)
        {
            if (brique.DernierTableauStrategie != contexte.TableauActuel)
            {
                brique.AugmenterPointsDeVie(Math.Max(0, contexte.TableauActuel - 1));
                brique.DernierTableauStrategie = contexte.TableauActuel;
            }
        }
    }

    class StrategieSelonBriquesRestantes : IStrategieBriqueDynamique
    {
        public void Appliquer(Brique brique, ContexteStrategieBrique contexte)
        {
            if (contexte.BriquesRestantes <= 5 && brique.DernierSeuilRestantStrategie != contexte.TableauActuel)
            {
                brique.AugmenterPointsDeVie(1);
                brique.DernierSeuilRestantStrategie = contexte.TableauActuel;
            }
        }
    }

    class StrategieSelonRebondsBalle : IStrategieBriqueDynamique
    {
        public void Appliquer(Brique brique, ContexteStrategieBrique contexte)
        {
            int intervalleRebonds = contexte.NombreRebondsBalle / 8;
            if (intervalleRebonds > brique.DernierIntervalleRebondsStrategie)
            {
                brique.AugmenterPointsDeVie(1);
                brique.DernierIntervalleRebondsStrategie = intervalleRebonds;
            }
        }
    }

    class StrategieComposeeBrique : IStrategieBriqueDynamique
    {
        #region Attributs
        private readonly IStrategieBriqueDynamique[] strategies;
        #endregion

        #region ConstructeursInitialisation
        public StrategieComposeeBrique(params IStrategieBriqueDynamique[] strategies)
        {
            this.strategies = strategies;
        }
        #endregion

        #region MethodesPubliques
        public void Appliquer(Brique brique, ContexteStrategieBrique contexte)
        {
            for (int i = 0; i < strategies.Length; i++)
            {
                strategies[i].Appliquer(brique, contexte);
            }
        }
        #endregion
    }

    class Brique : BasePourObjets
    {
        #region Attributs
        private const int PointsDeVieMaximum = 9;

        private readonly TypeBrique typeBrique;
        private readonly IStrategieBriqueDynamique strategie;
        private int pointsDeVie;
        private bool estDetruite;
        #endregion

        #region ConstructeursInitialisation
        public Brique(float x, float y, float largeur, float hauteur, int pointsDeVie, TypeBrique typeBrique)
        {
            positionX = x;
            positionY = y;
            this.largeur = largeur;
            this.hauteur = hauteur;
            this.pointsDeVie = pointsDeVie;
            this.typeBrique = typeBrique;

            if (typeBrique == TypeBrique.Dynamique)
            {
                strategie = new StrategieComposeeBrique(
                    new StrategieSelonTableau(),
                    new StrategieSelonBriquesRestantes(),
                    new StrategieSelonRebondsBalle());
            }

            mettreAJourListePoints();
        }
        #endregion

        #region MethodesPubliques
        public void RecevoirDegat()
        {
            if (!EstDestructible)
            {
                return;
            }

            pointsDeVie--;
            if (pointsDeVie <= 0)
            {
                estDetruite = true;
            }
        }

        public void AugmenterPointsDeVie(int quantite)
        {
            if (quantite <= 0 || !EstDynamique || estDetruite)
            {
                return;
            }

            pointsDeVie += quantite;
            if (pointsDeVie > PointsDeVieMaximum)
            {
                pointsDeVie = PointsDeVieMaximum;
            }
        }

        public void AppliquerStrategie(ContexteStrategieBrique contexte)
        {
            if (EstDynamique && !estDetruite)
            {
                strategie.Appliquer(this, contexte);
            }
        }

        public override void MettreAJour()
        {
            mettreAJourListePoints();
        }

        public override void Dessiner()
        {
            if (estDetruite)
            {
                return;
            }

            GL.Disable(EnableCap.Texture2D);
            dessinerFond();
            dessinerBordure();

            if (EstDynamique)
            {
                dessinerTextureDynamique();
            }

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

        private void dessinerFond()
        {
            if (!EstDestructible)
            {
                GL.Color3(0.35f, 0.37f, 0.40f);
            }
            else if (EstDynamique)
            {
                GL.Color3(0.65f, 0.18f, 0.86f);
            }
            else if (pointsDeVie == 1)
            {
                GL.Color3(0.10f, 0.68f, 0.42f);
            }
            else if (pointsDeVie == 2)
            {
                GL.Color3(0.93f, 0.66f, 0.16f);
            }
            else
            {
                GL.Color3(0.87f, 0.18f, 0.18f);
            }

            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(listePoints[0]);
            GL.Vertex2(listePoints[1]);
            GL.Vertex2(listePoints[2]);
            GL.Vertex2(listePoints[3]);
            GL.End();
        }

        private void dessinerBordure()
        {
            GL.Color3(0.02f, 0.02f, 0.03f);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(listePoints[0]);
            GL.Vertex2(listePoints[1]);
            GL.Vertex2(listePoints[2]);
            GL.Vertex2(listePoints[3]);
            GL.End();
        }

        private void dessinerTextureDynamique()
        {
            GL.Color3(0.95f, 0.88f, 0.18f);
            GL.Begin(PrimitiveType.Lines);
            for (float x = positionX - hauteur; x < positionX + largeur; x += 12.0f)
            {
                GL.Vertex2(x, positionY + hauteur);
                GL.Vertex2(x + hauteur, positionY);
            }
            GL.End();
        }
        #endregion

        #region Proprietes
        public int PointsDeVie { get => pointsDeVie; }
        public bool EstDestructible { get => typeBrique != TypeBrique.Indestructible; }
        public bool EstDynamique { get => typeBrique == TypeBrique.Dynamique; }
        public bool EstDetruite { get => estDetruite; }
        public float Gauche { get => positionX; }
        public float Droite { get => positionX + largeur; }
        public float Haut { get => positionY; }
        public float Bas { get => positionY + hauteur; }
        public int DernierTableauStrategie { get; set; }
        public int DernierSeuilRestantStrategie { get; set; }
        public int DernierIntervalleRebondsStrategie { get; set; }
        #endregion
    }
}
