using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace BreakoutGameCarlosGabriel
{
    class EffetDestructionBrique
    {
        private const int NombreColonnes = 3;
        private const int NombreLignes = 2;
        private const float DureeVie = 0.38f;
        private const float VitesseFragment = 135.0f;
        private const float Gravite = 240.0f;

        private readonly FragmentEffet[] fragments;
        private readonly Vector2 centre;
        private readonly Vector3 couleurPrincipale;
        private float tempsEcoule;

        private struct FragmentEffet
        {
            public Vector2 PositionInitiale;
            public Vector2 Vitesse;
            public float Largeur;
            public float Hauteur;
            public float Delai;
        }

        public EffetDestructionBrique(Brique brique)
        {
            centre = new Vector2(brique.Gauche + brique.Largeur / 2.0f, brique.Haut + brique.Hauteur / 2.0f);
            couleurPrincipale = brique.CouleurEffet;
            fragments = creerFragments(brique);
        }

        public bool EstTermine
        {
            get { return tempsEcoule >= DureeVie; }
        }

        public void MettreAJour(float deltaTime)
        {
            tempsEcoule += deltaTime;
        }

        public void Dessiner()
        {
            if (EstTermine)
            {
                return;
            }

            float progression = Math.Max(0.0f, Math.Min(1.0f, tempsEcoule / DureeVie));
            float alpha = 1.0f - progression;
            float alphaFlash = (1.0f - progression) * (1.0f - progression) * 0.65f;

            GL.Disable(EnableCap.Texture2D);
            dessinerFlash(alphaFlash, progression);

            for (int i = 0; i < fragments.Length; i++)
            {
                dessinerFragment(fragments[i], alpha, progression);
            }

            GL.Enable(EnableCap.Texture2D);
        }

        private static FragmentEffet[] creerFragments(Brique brique)
        {
            FragmentEffet[] nouveauxFragments = new FragmentEffet[NombreColonnes * NombreLignes];
            float largeurFragment = brique.Largeur / NombreColonnes;
            float hauteurFragment = brique.Hauteur / NombreLignes;
            Vector2 centreBrique = new Vector2(brique.Gauche + brique.Largeur / 2.0f, brique.Haut + brique.Hauteur / 2.0f);
            int index = 0;

            for (int ligne = 0; ligne < NombreLignes; ligne++)
            {
                for (int colonne = 0; colonne < NombreColonnes; colonne++)
                {
                    float x = brique.Gauche + colonne * largeurFragment;
                    float y = brique.Haut + ligne * hauteurFragment;
                    Vector2 centreFragment = new Vector2(x + largeurFragment / 2.0f, y + hauteurFragment / 2.0f);
                    Vector2 direction = centreFragment - centreBrique;

                    if (direction.LengthSquared < float.Epsilon)
                    {
                        direction = new Vector2(0.0f, -1.0f);
                    }
                    else
                    {
                        direction.Normalize();
                    }

                    float multiplicateur = 0.75f + index * 0.12f;
                    nouveauxFragments[index] = new FragmentEffet
                    {
                        PositionInitiale = new Vector2(x, y),
                        Vitesse = new Vector2(direction.X * VitesseFragment * multiplicateur, direction.Y * VitesseFragment * multiplicateur - 32.0f),
                        Largeur = largeurFragment - 2.0f,
                        Hauteur = hauteurFragment - 2.0f,
                        Delai = index * 0.012f
                    };
                    index++;
                }
            }

            return nouveauxFragments;
        }

        private void dessinerFlash(float alphaFlash, float progression)
        {
            float rayonX = 12.0f + progression * 18.0f;
            float rayonY = 8.0f + progression * 12.0f;

            GL.Color4(1.0f, 0.95f, 0.80f, alphaFlash);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(centre.X - rayonX, centre.Y - rayonY);
            GL.Vertex2(centre.X + rayonX, centre.Y - rayonY);
            GL.Vertex2(centre.X + rayonX, centre.Y + rayonY);
            GL.Vertex2(centre.X - rayonX, centre.Y + rayonY);
            GL.End();
        }

        private void dessinerFragment(FragmentEffet fragment, float alpha, float progression)
        {
            float tempsFragment = Math.Max(0.0f, tempsEcoule - fragment.Delai);
            Vector2 deplacement = fragment.Vitesse * tempsFragment + new Vector2(0.0f, 0.5f * Gravite * tempsFragment * tempsFragment);
            float echelle = Math.Max(0.35f, 1.0f - progression * 0.45f);
            float largeurCourante = fragment.Largeur * echelle;
            float hauteurCourante = fragment.Hauteur * echelle;
            float x = fragment.PositionInitiale.X + deplacement.X;
            float y = fragment.PositionInitiale.Y + deplacement.Y;

            GL.Color4(couleurPrincipale.X, couleurPrincipale.Y, couleurPrincipale.Z, alpha);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + largeurCourante, y);
            GL.Vertex2(x + largeurCourante, y + hauteurCourante);
            GL.Vertex2(x, y + hauteurCourante);
            GL.End();

            GL.Color4(1.0f, 1.0f, 1.0f, alpha * 0.55f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(x, y);
            GL.Vertex2(x + largeurCourante, y);
            GL.End();
        }
    }
}
