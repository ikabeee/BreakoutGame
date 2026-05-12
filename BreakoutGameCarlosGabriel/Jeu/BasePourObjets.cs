using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace BreakoutGameCarlosGabriel
{
    public abstract class BasePourObjets
    {
        #region Attributs
        protected float positionX;
        protected float positionY;
        protected float largeur;
        protected float hauteur;
        protected Vector2[] listePoints;
        protected Vector2[] coordonneesTextures;
        protected int textureID;
        protected string nomTexture;

        private static readonly Dictionary<string, int> cacheTextures = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        #endregion

        #region ConstructeursInitialisation
        protected BasePourObjets()
        {
            this.listePoints = new Vector2[4];
            setCoordonneesTextureCarre();
        }

        #endregion

        #region Texture
        protected void AssignerTexture(string nomTexture)
        {
            this.nomTexture = nomTexture;
            textureID = chargerTexturePartagee(nomTexture);
        }

        private static int chargerTexturePartagee(string nomTexture)
        {
            string cheminFichier = resoudreCheminTexture(nomTexture);
            if (!File.Exists(cheminFichier))
            {
                Console.WriteLine("Asset introuvable : " + nomTexture);
                return 0;
            }

            if (cacheTextures.TryGetValue(cheminFichier, out int texture))
            {
                return texture;
            }

            texture = chargerTextureDepuisFichier(cheminFichier);
            cacheTextures[cheminFichier] = texture;
            return texture;
        }

        private static int chargerTextureDepuisFichier(string cheminFichier)
        {
            using (Bitmap bmpImage = new Bitmap(cheminFichier))
            {
                int texture;
                GL.GenTextures(1, out texture);
                GL.BindTexture(TextureTarget.Texture2D, texture);

                BitmapData bmpData = bmpImage.LockBits(
                    new Rectangle(0, 0, bmpImage.Width, bmpImage.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                try
                {
                    GL.TexImage2D(
                        TextureTarget.Texture2D,
                        0,
                        PixelInternalFormat.Rgba,
                        bmpData.Width,
                        bmpData.Height,
                        0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte,
                        bmpData.Scan0);
                }
                finally
                {
                    bmpImage.UnlockBits(bmpData);
                }

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                return texture;
            }
        }

        private static string resoudreCheminTexture(string nomTexture)
        {
            string[] cheminsPossibles =
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", nomTexture),
                Path.Combine(Environment.CurrentDirectory, "assets", nomTexture),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\assets", nomTexture),
                Path.Combine(Environment.CurrentDirectory, @"..\..\assets", nomTexture)
            };

            for (int i = 0; i < cheminsPossibles.Length; i++)
            {
                string cheminComplet = Path.GetFullPath(cheminsPossibles[i]);
                if (File.Exists(cheminComplet))
                {
                    return cheminComplet;
                }
            }

            return Path.GetFullPath(cheminsPossibles[0]);
        }

        private void setCoordonneesTextureCarre()
        {
            coordonneesTextures = new Vector2[4];
            coordonneesTextures[0] = new Vector2(0.0f, 1.0f);
            coordonneesTextures[1] = new Vector2(1.0f, 1.0f);
            coordonneesTextures[2] = new Vector2(1.0f, 0.0f);
            coordonneesTextures[3] = new Vector2(0.0f, 0.0f);
        }
        #endregion

        public abstract void MettreAJour();

        public virtual void Dessiner()
        {
            if (textureID == 0)
            {
                return;
            }

            GL.PushMatrix();
            GL.Enable(EnableCap.Texture2D);
            GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.Begin(PrimitiveType.Quads);

            for (int i = 0; i < listePoints.Length; i++)
            {
                GL.TexCoord2(coordonneesTextures[i]);
                GL.Vertex2(listePoints[i]);
            }

            GL.End();
            GL.PopMatrix();
        }

        public float PositionX { get => positionX; set => positionX = value; }
        public float PositionY { get => positionY; set => positionY = value; }
        public float Largeur { get => largeur; set => largeur = value; }
        public float Hauteur { get => hauteur; set => hauteur = value; }
        protected bool UtiliseTexture { get => textureID != 0; }
    }
}
