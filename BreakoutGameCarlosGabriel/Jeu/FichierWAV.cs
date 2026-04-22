using System;
using System.IO;

namespace BreakoutGameCarlosGabriel
{
    public class FichierWAV
    {
        private string nomFichier;
        private int nbrCanaux;
        private int frequence;
        private int nbrBits;
        private int qteDonneesSonores;
        private byte[] donneesSonores;

        public FichierWAV(string nomFichier)
        {
            this.nomFichier = nomFichier;
            // TODO : Appeler ChargerFichier avec un flux (Stream)
        }

        private void ChargerFichier(Stream stream)
        {
            // TODO : Implémenter la lecture du fichier WAV
        }

        public byte[] GetDonneesSonores() { return donneesSonores; }
        public int GetQteDonneesSonores() { return qteDonneesSonores; }
        public int GetFrequence() { return frequence; }

        // Note: Le type ALFormat dépend de la librairie audio que vous utilisez (comme OpenTK)
        // public ALFormat GetFormatSonAL() { ... }
    }
}