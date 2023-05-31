using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMpegCore;
using NAudio.Wave;

namespace AudioConverter
{
    public class Converter
    {
        public readonly Process process = new Process();
        public Converter()
        {
        }
        public static void ConvertAudioto441Khz(string pathfile, string pathfile2)
        {
            int outRate = 44100;
            var inFile = pathfile;
            var outFile = pathfile2;
            using (var reader = new AudioFileReader(inFile))
            {
                var outFormat = new WaveFormat(outRate, reader.WaveFormat.Channels);
                using (var resampler = new MediaFoundationResampler(reader, outFormat))
                {
                    // resampler.ResamplerQuality = 60;
                    WaveFileWriter.CreateWaveFile(outFile, resampler);
                }
            }
            // Process process = new Process();
            // process.StartInfo.RedirectStandardOutput = true;
            // process.StartInfo.RedirectStandardError = true;
            // process.StartInfo.FileName = "ffmpeg";
            // process.StartInfo.Arguments = $"-i \"{pathfile}\" -ar 44100 \"{pathfile2}\"";
            // process.StartInfo.UseShellExecute = false;
            // process.StartInfo.CreateNoWindow = false;
            // process.Start();
            //// process.WaitForExit();
            // process.Close();
        }

    }
}
