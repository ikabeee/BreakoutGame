using System;
using System.IO;
using OpenTK.Audio.OpenAL;

namespace BreakoutGameCarlosGabriel
{
    public class FichierWAV
    {
        #region Attributs
        private readonly string nomFichier;
        private int nbrCanaux;
        private int frequence;
        private int nbrBits;
        private int qteDonneesSonores;
        private byte[] donneesSonores;
        #endregion

        #region ConstructeursInitialisation
        public FichierWAV(string nomFichier)
        {
            this.nomFichier = nomFichier;

            using (Stream fichierAudio = File.Open(nomFichier, FileMode.Open, FileAccess.Read))
            {
                ChargerFichier(fichierAudio);
            }
        }
        #endregion

        #region ChargementFichier
        private void ChargerFichier(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (BinaryReader reader = new BinaryReader(stream))
            {
                string donneesFichier = new string(reader.ReadChars(4));
                if (donneesFichier != "RIFF")
                {
                    throw new NotSupportedException("N'est pas un fichier multimedia");
                }

                reader.ReadInt32();

                donneesFichier = new string(reader.ReadChars(4));
                if (donneesFichier != "WAVE")
                {
                    throw new NotSupportedException("Le fichier audio n'est pas au format WAVE");
                }

                donneesFichier = new string(reader.ReadChars(4));
                if (donneesFichier != "fmt ")
                {
                    throw new NotSupportedException("Fichier WAVE non supporte (fmt).");
                }

                int tailleSectionFormat = reader.ReadInt32();
                short formatAudio = reader.ReadInt16();
                if (formatAudio != 1)
                {
                    throw new NotSupportedException("Seuls les fichiers WAV PCM sont supportes.");
                }

                nbrCanaux = reader.ReadInt16();
                frequence = reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt16();
                nbrBits = reader.ReadInt16();

                int octetsFormatRestants = tailleSectionFormat - 16;
                if (octetsFormatRestants > 0)
                {
                    reader.ReadBytes(octetsFormatRestants);
                }

                donneesFichier = new string(reader.ReadChars(4));
                while (donneesFichier != "data")
                {
                    int tailleSection = reader.ReadInt32();
                    reader.ReadBytes(tailleSection);
                    donneesFichier = new string(reader.ReadChars(4));
                }

                qteDonneesSonores = reader.ReadInt32();
                donneesSonores = reader.ReadBytes(qteDonneesSonores);
            }
        }
        #endregion

        #region MethodesPubliques
        public ALFormat GetFormatSonAL()
        {
            switch (nbrCanaux)
            {
                case 1:
                    return nbrBits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2:
                    return nbrBits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default:
                    throw new NotSupportedException("Format non supporte.");
            }
        }

        public byte[] GetDonneesSonores()
        {
            return donneesSonores;
        }

        public int GetQteDonneesSonores()
        {
            return qteDonneesSonores;
        }

        public int GetFrequence()
        {
            return frequence;
        }

        public string GetNomFichier()
        {
            return nomFichier;
        }
        #endregion
    }
}
