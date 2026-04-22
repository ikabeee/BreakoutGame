using System.Drawing;
using System.Drawing.Imaging;
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
        #endregion

        #region ConstructeursInitialisation
        protected BasePourObjets()
        {
            this.listePoints = new Vector2[4];
            setCoordonneesTextureCarre();
        }

        protected BasePourObjets(string nomTexture)
            : this()
        {
            this.nomTexture = nomTexture;
            chargerTexture();
        }
        #endregion

        #region Texture
        protected void chargerTexture()
        {
            GL.GenTextures(1, out textureID);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            BitmapData textureData = chargerImage();
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgb,
                textureData.Width,
                textureData.Height,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgr,
                PixelType.UnsignedByte,
                textureData.Scan0);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        private BitmapData chargerImage()
        {
            Bitmap bmpImage = new Bitmap(nomTexture);
            Rectangle rectangle = new Rectangle(0, 0, bmpImage.Width, bmpImage.Height);
            BitmapData bmpData = bmpImage.LockBits(rectangle, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            bmpImage.UnlockBits(bmpData);
            return bmpData;
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
            GL.PushMatrix();
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
    }
}
