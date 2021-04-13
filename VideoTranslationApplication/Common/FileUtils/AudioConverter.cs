// Code from: https://markheath.net/post/converting-mp3-to-wav-with-naudio //

using NAudio.Wave;

namespace VideoTranslationTool.FileUtils
{
    public class AudioConverter
    {
        public static void Mp3ToWav(string mp3File, string outputFile)
        {
            using (Mp3FileReader reader = new Mp3FileReader(mp3File))
            {
                using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    WaveFileWriter.CreateWaveFile(outputFile, pcmStream);
                }
            }
        }
        public static void Mp4ToMp3(string mp4File, string outputFile)
        {
            using (MediaFoundationReader mediaReader = new MediaFoundationReader(mp4File))
            {
                MediaFoundationEncoder.EncodeToMp3(mediaReader, outputFile);
            }
        }

        public static void WavToMp3(string wavFile, string outputFile)
        {
            using (WaveFileReader waveReader = new WaveFileReader(wavFile))
            {
                MediaFoundationEncoder.EncodeToMp3(waveReader, outputFile);
            }
        }

        public static void WavToWavPcm(string inputFile, string outputFile)
        {
            using(MediaFoundationReader mediaReader = new MediaFoundationReader(inputFile))
            {
                using(WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(mediaReader))
                {
                    WaveFileWriter.CreateWaveFile(outputFile, pcmStream);
                }
            }
        }
    }
}
