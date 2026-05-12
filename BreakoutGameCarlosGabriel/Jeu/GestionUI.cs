using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace BreakoutGameCarlosGabriel
{
    public enum EtatJeu
    {
        Accueil,
        Tutoriel,
        EnJeu,
        Pause,
        FinDePartie
    }

    public class GestionUI
    {
        #region Attributs
        public const float HauteurHud = 42.0f;

        private readonly float largeurFenetre;
        private readonly float hauteurFenetre;
        private readonly Font policeHud;
        private readonly Font policeTitre;
        private readonly Font policeMessage;
        #endregion

        #region ConstructeursInitialisation
        public GestionUI(float largeurFenetre, float hauteurFenetre)
        {
            this.largeurFenetre = largeurFenetre;
            this.hauteurFenetre = hauteurFenetre;
            policeHud = new Font("Arial", 16.0f, FontStyle.Bold);
            policeTitre = new Font("Arial", 34.0f, FontStyle.Bold);
            policeMessage = new Font("Arial", 18.0f, FontStyle.Bold);
        }
        #endregion

        #region MethodesPubliques
        public void DessinerInterface(EtatJeu etatActuel, int score, int balles, int tableau)
        {
            if (etatActuel != EtatJeu.Accueil && etatActuel != EtatJeu.Tutoriel)
            {
                DessinerHUD(score, balles, tableau);
            }

            switch (etatActuel)
            {
                case EtatJeu.Accueil:
                    DessinerEcranAccueil();
                    break;
                case EtatJeu.Tutoriel:
                    DessinerEcranTutoriel();
                    break;
                case EtatJeu.Pause:
                    DessinerEcranPause();
                    break;
                case EtatJeu.FinDePartie:
                    DessinerEcranFin(score);
                    break;
            }
        }
        #endregion

        #region Affichage
        private void DessinerHUD(int score, int balles, int tableau)
        {
            dessinerRectangle(0.0f, 0.0f, largeurFenetre, HauteurHud, 0.02f, 0.02f, 0.03f, 0.82f);
            dessinerTexte("Score : " + score, 24.0f, 10.0f, policeHud, Color.White);
            dessinerTexte("Balles : " + balles, 210.0f, 10.0f, policeHud, Color.White);
            dessinerTexte("Tableau : " + tableau, 400.0f, 10.0f, policeHud, Color.White);
        }

        private void DessinerEcranAccueil()
        {
            dessinerTexteCentre("BREAKOUT PRO", largeurFenetre / 2, hauteurFenetre / 3, policeTitre, Color.White);
            dessinerTexteCentre("1. JOUER AU JEU ORIGINAL", largeurFenetre / 2, hauteurFenetre / 2, policeMessage, Color.LightGreen);
            dessinerTexteCentre("2. JOUER À LA DÉMO (COURT)", largeurFenetre / 2, hauteurFenetre / 2 + 40, policeMessage, Color.LightBlue);
            dessinerTexteCentre("ECHAP. QUITTER", largeurFenetre / 2, hauteurFenetre / 2 + 100, policeMessage, Color.Gray);
        }

        private void DessinerEcranTutoriel()
        {
            dessinerTexteCentre("TUTORIEL & COMMANDES", largeurFenetre / 2, 150, policeTitre, Color.White);
            dessinerTexteCentre("- A ou Flèche Gauche : Aller à gauche", largeurFenetre / 2, 250, policeMessage, Color.White);
            dessinerTexteCentre("- D ou Flèche Droite : Aller à droite", largeurFenetre / 2, 290, policeMessage, Color.White);
            dessinerTexteCentre("- ESPACE : Lancer la balle", largeurFenetre / 2, 330, policeMessage, Color.White);

            dessinerTexteCentre("LES BRIQUES :", largeurFenetre / 2, 400, policeMessage, Color.Yellow);
            dessinerTexteCentre("Vertes = 1 coup | Bleues = 2 coups | Rouges = 3 coups", largeurFenetre / 2, 440, policeMessage, Color.White);
            dessinerTexteCentre("Grises = Indestructibles", largeurFenetre / 2, 480, policeMessage, Color.Gray);

            dessinerTexteCentre("APPUYEZ SUR ESPACE POUR DÉMARRER", largeurFenetre / 2, 600, policeMessage, Color.Cyan);
        }

        private void DessinerEcranPause()
        {
            dessinerRectangle(330.0f, hauteurFenetre / 2.0f - 58.0f, 340.0f, 104.0f, 0.0f, 0.0f, 0.0f, 0.72f);
            dessinerTexte("PAUSE", 445.0f, hauteurFenetre / 2.0f - 45.0f, policeTitre, Color.White);
            dessinerTexte("P : reprendre", 430.0f, hauteurFenetre / 2.0f + 8.0f, policeMessage, Color.LightSkyBlue);
        }

        private void DessinerEcranFin(int scoreFinal)
        {
            dessinerRectangle(270.0f, hauteurFenetre / 2.0f - 78.0f, 460.0f, 142.0f, 0.0f, 0.0f, 0.0f, 0.72f);
            dessinerTexte("FIN DE PARTIE", 335.0f, hauteurFenetre / 2.0f - 60.0f, policeTitre, Color.White);
            dessinerTexte("Score final : " + scoreFinal, 405.0f, hauteurFenetre / 2.0f - 8.0f, policeMessage, Color.Gainsboro);
            dessinerTexte("Espace : recommencer", 385.0f, hauteurFenetre / 2.0f + 28.0f, policeMessage, Color.LightSkyBlue);
        }

        private void dessinerRectangle(float x, float y, float largeur, float hauteur, float rouge, float vert, float bleu, float alpha)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Color4(rouge, vert, bleu, alpha);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + largeur, y);
            GL.Vertex2(x + largeur, y + hauteur);
            GL.Vertex2(x, y + hauteur);
            GL.End();
            GL.Enable(EnableCap.Texture2D);
        }

        // Nouvelle méthode pour centrer le texte automatiquement
        private void dessinerTexteCentre(string texte, float centreX, float y, Font police, Color couleur)
        {
            SizeF tailleTexte;
            using (Bitmap mesure = new Bitmap(1, 1))
            using (Graphics graphMesure = Graphics.FromImage(mesure))
            {
                tailleTexte = graphMesure.MeasureString(texte, police);
            }

            // On décale le texte vers la gauche de la moitié de sa largeur pour le centrer
            float x = centreX - (tailleTexte.Width / 2.0f);
            dessinerTexte(texte, x, y, police, couleur);
        }

        private void dessinerTexte(string texte, float x, float y, Font police, Color couleur)
        {
            SizeF tailleTexte;
            using (Bitmap mesure = new Bitmap(1, 1))
            using (Graphics graphMesure = Graphics.FromImage(mesure))
            {
                tailleTexte = graphMesure.MeasureString(texte, police);
            }

            int largeurTexture = (int)tailleTexte.Width + 8;
            int hauteurTexture = (int)tailleTexte.Height + 8;

            using (Bitmap bitmap = new Bitmap(largeurTexture, hauteurTexture))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                using (Brush pinceau = new SolidBrush(couleur))
                {
                    graphics.DrawString(texte, police, pinceau, 0.0f, 0.0f);
                }

                int texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                BitmapData data = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    data.Width,
                    data.Height,
                    0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);

                bitmap.UnlockBits(data);

                GL.Enable(EnableCap.Texture2D);
                GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(x, y);
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(x + largeurTexture, y);
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(x + largeurTexture, y + hauteurTexture);
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(x, y + hauteurTexture);
                GL.End();

                GL.DeleteTexture(texture);
            }
        }
        #endregion
    }
}