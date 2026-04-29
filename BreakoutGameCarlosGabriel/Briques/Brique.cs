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
            AssignerTexture(typeBrique == TypeBrique.Indestructible ? "brique-indestructible.png" : "brique-destructible.png");

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

            if (UtiliseTexture)
            {
                base.Dessiner();
                GL.Disable(EnableCap.Texture2D);
                dessinerBordure();

                if (EstDynamique)
                {
                    dessinerTextureDynamique();
                }

                if (EstDestructible)
                {
                    dessinerPointsDeVie();
                }

                GL.Enable(EnableCap.Texture2D);
                return;
            }

            GL.Disable(EnableCap.Texture2D);
            dessinerFond();
            dessinerBordure();

            if (EstDynamique)
            {
                dessinerTextureDynamique();
            }

            if (EstDestructible)
            {
                dessinerPointsDeVie();
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

        private void dessinerPointsDeVie()
        {
            int pointsAffiches = Math.Max(0, Math.Min(pointsDeVie, PointsDeVieMaximum));
            if (pointsAffiches == 0)
            {
                return;
            }

            int nombreLignes = (pointsAffiches + 4) / 5;
            float taillePoint = Math.Max(4.0f, Math.Min(6.0f, largeur / 14.0f));
            float espacement = 2.0f;
            float hauteurTotale = nombreLignes * taillePoint + (nombreLignes - 1) * espacement;
            float yDepart = positionY + hauteur / 2.0f - hauteurTotale / 2.0f;

            float largeurFond = 5 * taillePoint + 4 * espacement + 8.0f;
            float hauteurFond = hauteurTotale + 6.0f;
            float xFond = positionX + largeur / 2.0f - largeurFond / 2.0f;
            float yFond = positionY + hauteur / 2.0f - hauteurFond / 2.0f;

            GL.Color4(0.03f, 0.05f, 0.08f, 0.45f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(xFond, yFond);
            GL.Vertex2(xFond + largeurFond, yFond);
            GL.Vertex2(xFond + largeurFond, yFond + hauteurFond);
            GL.Vertex2(xFond, yFond + hauteurFond);
            GL.End();

            int pointsDessines = 0;
            for (int ligne = 0; ligne < nombreLignes; ligne++)
            {
                int pointsCetteLigne = Math.Min(5, pointsAffiches - pointsDessines);
                float largeurLigne = pointsCetteLigne * taillePoint + (pointsCetteLigne - 1) * espacement;
                float xDepart = positionX + largeur / 2.0f - largeurLigne / 2.0f;
                float yLigne = yDepart + ligne * (taillePoint + espacement);

                for (int colonne = 0; colonne < pointsCetteLigne; colonne++)
                {
                    float xPoint = xDepart + colonne * (taillePoint + espacement);

                    if (EstDynamique)
                    {
                        GL.Color3(0.98f, 0.83f, 0.18f);
                    }
                    else
                    {
                        GL.Color3(0.95f, 0.98f, 1.0f);
                    }

                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex2(xPoint, yLigne);
                    GL.Vertex2(xPoint + taillePoint, yLigne);
                    GL.Vertex2(xPoint + taillePoint, yLigne + taillePoint);
                    GL.Vertex2(xPoint, yLigne + taillePoint);
                    GL.End();

                    GL.Color3(0.07f, 0.1f, 0.15f);
                    GL.Begin(PrimitiveType.LineLoop);
                    GL.Vertex2(xPoint, yLigne);
                    GL.Vertex2(xPoint + taillePoint, yLigne);
                    GL.Vertex2(xPoint + taillePoint, yLigne + taillePoint);
                    GL.Vertex2(xPoint, yLigne + taillePoint);
                    GL.End();
                }

                pointsDessines += pointsCetteLigne;
            }
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
