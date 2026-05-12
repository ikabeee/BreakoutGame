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
        public int BriquesDetruites { get; set; }
        public int TotalBriquesCassables { get; set; }
    }

    interface IStrategieBriqueDynamique
    {
        void Appliquer(Brique brique, ContexteStrategieBrique contexte);
    }

    class StrategieSelonProgressionNiveau : IStrategieBriqueDynamique
    {
        private const float SeuilNiveau2 = 0.35f;
        private const float SeuilNiveau3 = 0.70f;

        public void Appliquer(Brique brique, ContexteStrategieBrique contexte)
        {
            if (contexte.TotalBriquesCassables <= 0 || brique.EstDetruite)
            {
                return;
            }

            float progression = (float)contexte.BriquesDetruites / contexte.TotalBriquesCassables;
            int palierProgression = 1;

            if (progression >= SeuilNiveau3)
            {
                palierProgression = 3;
            }
            else if (progression >= SeuilNiveau2)
            {
                palierProgression = 2;
            }

            if (palierProgression <= brique.DernierPalierProgressionStrategie)
            {
                return;
            }

            brique.AugmenterPointsDeVie(palierProgression - brique.DernierPalierProgressionStrategie);
            brique.DernierPalierProgressionStrategie = palierProgression;
        }
    }

    class Brique : BasePourObjets
    {
        #region Attributs
        private const int PointsDeVieMaximum = 3;
        private const string AssetBriqueIndestructible = "brique-indestructible.png";
        private const string PrefixeAssetBriqueNormale = "brique-destructible-nvie-";
        private const string PrefixeAssetBriqueDynamique = "brique-dynamique-nvie-";

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
            this.typeBrique = typeBrique;
            this.pointsDeVie = typeBrique == TypeBrique.Indestructible ? 1 : normaliserPointsDeVie(pointsDeVie);
            strategie = typeBrique == TypeBrique.Dynamique ? new StrategieSelonProgressionNiveau() : null;
            DernierPalierProgressionStrategie = EstDynamique ? this.pointsDeVie : 0;

            mettreAJourTextureSelonEtat();
            mettreAJourListePoints();
        }
        #endregion

        #region MethodesPubliques
        public void RecevoirDegat()
        {
            if (!EstDestructible || estDetruite)
            {
                return;
            }

            pointsDeVie--;
            if (pointsDeVie <= 0)
            {
                estDetruite = true;
                return;
            }

            mettreAJourTextureSelonEtat();
        }

        public void AugmenterPointsDeVie(int quantite)
        {
            if (quantite <= 0 || !EstDynamique || estDetruite)
            {
                return;
            }

            pointsDeVie = Math.Min(PointsDeVieMaximum, pointsDeVie + quantite);
            mettreAJourTextureSelonEtat();
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
                return;
            }

            GL.Disable(EnableCap.Texture2D);
            dessinerFond();
            dessinerBordure();

            if (EstDestructible)
            {
                dessinerPointsDeVie();
            }

            GL.Enable(EnableCap.Texture2D);
        }
        #endregion

        #region MethodesPrivees
        private static int normaliserPointsDeVie(int pointsDeVie)
        {
            return Math.Max(1, Math.Min(PointsDeVieMaximum, pointsDeVie));
        }

        private Vector3 obtenirCouleurPrincipale()
        {
            if (!EstDestructible)
            {
                return new Vector3(0.35f, 0.37f, 0.40f);
            }

            if (EstDynamique && pointsDeVie <= 1)
            {
                return new Vector3(0.18f, 0.62f, 0.72f);
            }

            if (EstDynamique && pointsDeVie == 2)
            {
                return new Vector3(0.83f, 0.61f, 0.12f);
            }

            if (EstDynamique)
            {
                return new Vector3(0.92f, 0.36f, 0.16f);
            }

            if (pointsDeVie <= 1)
            {
                return new Vector3(0.20f, 0.68f, 0.89f);
            }

            if (pointsDeVie == 2)
            {
                return new Vector3(0.58f, 0.36f, 0.74f);
            }

            return new Vector3(0.87f, 0.18f, 0.18f);
        }

        private void mettreAJourTextureSelonEtat()
        {
            if (!EstDestructible)
            {
                AssignerTexture(AssetBriqueIndestructible);
                return;
            }

            int niveauTexture = normaliserPointsDeVie(pointsDeVie);
            string prefixe = EstDynamique ? PrefixeAssetBriqueDynamique : PrefixeAssetBriqueNormale;
            AssignerTexture(prefixe + niveauTexture + ".png");
        }

        private void mettreAJourListePoints()
        {
            listePoints[0] = new Vector2(positionX, positionY);
            listePoints[1] = new Vector2(positionX + largeur, positionY);
            listePoints[2] = new Vector2(positionX + largeur, positionY + hauteur);
            listePoints[3] = new Vector2(positionX, positionY + hauteur);
        }

        private void dessinerFond()
        {
            Vector3 couleurPrincipale = obtenirCouleurPrincipale();
            GL.Color3(couleurPrincipale);

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

        private void dessinerPointsDeVie()
        {
            int pointsAffiches = Math.Max(0, Math.Min(pointsDeVie, PointsDeVieMaximum));
            if (pointsAffiches == 0)
            {
                return;
            }

            float taillePoint = Math.Max(4.0f, Math.Min(6.0f, largeur / 14.0f));
            float espacement = 2.0f;
            float largeurTotale = pointsAffiches * taillePoint + (pointsAffiches - 1) * espacement;
            float xDepart = positionX + largeur / 2.0f - largeurTotale / 2.0f;
            float yDepart = positionY + hauteur / 2.0f - taillePoint / 2.0f;

            for (int i = 0; i < pointsAffiches; i++)
            {
                float xPoint = xDepart + i * (taillePoint + espacement);

                if (EstDynamique)
                {
                    GL.Color3(1.0f, 0.95f, 0.72f);
                }
                else
                {
                    GL.Color3(0.95f, 0.98f, 1.0f);
                }

                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(xPoint, yDepart);
                GL.Vertex2(xPoint + taillePoint, yDepart);
                GL.Vertex2(xPoint + taillePoint, yDepart + taillePoint);
                GL.Vertex2(xPoint, yDepart + taillePoint);
                GL.End();
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
        public Vector3 CouleurEffet { get => obtenirCouleurPrincipale(); }
        public int DernierPalierProgressionStrategie { get; set; }
        #endregion
    }
}
