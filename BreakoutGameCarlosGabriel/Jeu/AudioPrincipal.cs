using System;
using System.IO;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace BreakoutGameCarlosGabriel
{
    public class AudioPrincipal : IDisposable
    {
        #region Attributs
        private readonly AudioContext audioContext;
        private int bufferRebond;
        private int bufferDestruction;
        private int bufferPerteBalle;
        private int sourceRebond;
        private int sourceDestruction;
        private int sourcePerteBalle;
        private float volume;
        private bool estDispose;
        #endregion

        #region ConstructeursInitialisation
        public AudioPrincipal()
        {
            audioContext = new AudioContext();
            init();
        }

        ~AudioPrincipal()
        {
            Dispose(false);
        }

        private void init()
        {
            bufferRebond = chargerBuffer("rebond.wav");
            bufferDestruction = chargerBuffer("destruction.wav");
            bufferPerteBalle = chargerBuffer("perteBalle.wav");

            sourceRebond = creerSource(bufferRebond);
            sourceDestruction = creerSource(bufferDestruction);
            sourcePerteBalle = creerSource(bufferPerteBalle);

            SetVolume(70);
        }
        #endregion

        #region ChargementAudio
        private int chargerBuffer(string nomFichier)
        {
            string cheminFichier = resoudreCheminAudio(nomFichier);
            if (!File.Exists(cheminFichier))
            {
                Console.WriteLine("Audio introuvable : " + nomFichier);
                return 0;
            }

            FichierWAV fichierAudio = new FichierWAV(cheminFichier);
            int buffer = AL.GenBuffer();
            AL.BufferData(
                buffer,
                fichierAudio.GetFormatSonAL(),
                fichierAudio.GetDonneesSonores(),
                fichierAudio.GetQteDonneesSonores(),
                fichierAudio.GetFrequence());

            return buffer;
        }

        private string resoudreCheminAudio(string nomFichier)
        {
            string[] cheminsPossibles =
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio", nomFichier),
                Path.Combine(Environment.CurrentDirectory, "audio", nomFichier),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\audio", nomFichier),
                Path.Combine(Environment.CurrentDirectory, @"..\..\audio", nomFichier)
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

        private int creerSource(int buffer)
        {
            if (buffer == 0)
            {
                return 0;
            }

            int source = AL.GenSource();
            AL.Source(source, ALSourcei.Buffer, buffer);
            AL.Source(source, ALSourceb.Looping, false);
            return source;
        }
        #endregion

        #region MethodesPubliques
        public void JouerSonRebond()
        {
            jouerSource(sourceRebond);
        }

        public void JouerSonDestruction()
        {
            jouerSource(sourceDestruction);
        }

        public void JouerSonPerteBalle()
        {
            jouerSource(sourcePerteBalle);
        }

        public void SetVolume(int valeur)
        {
            volume = Math.Max(0, Math.Min(100, valeur)) / 100.0f;
            AL.Listener(ALListenerf.Gain, volume);
        }
        #endregion

        #region Nettoyage
        private void jouerSource(int source)
        {
            if (source == 0)
            {
                return;
            }

            AL.SourceStop(source);
            AL.SourcePlay(source);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (estDispose)
            {
                return;
            }

            supprimerSource(sourceRebond);
            supprimerSource(sourceDestruction);
            supprimerSource(sourcePerteBalle);
            supprimerBuffer(bufferRebond);
            supprimerBuffer(bufferDestruction);
            supprimerBuffer(bufferPerteBalle);

            if (disposing)
            {
                audioContext.Dispose();
            }

            estDispose = true;
        }

        private void supprimerSource(int source)
        {
            if (source != 0)
            {
                AL.SourceStop(source);
                AL.DeleteSource(source);
            }
        }

        private void supprimerBuffer(int buffer)
        {
            if (buffer != 0)
            {
                AL.DeleteBuffer(buffer);
            }
        }
        #endregion
    }
}
