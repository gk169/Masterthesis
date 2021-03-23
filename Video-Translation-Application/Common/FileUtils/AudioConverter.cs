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
    }
}
