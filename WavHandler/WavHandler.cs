using Android.Media;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Wit.VoiceTestWitNLP.Wav
{
    public class WavHandler
    {
        private Task reading;
        private bool isRecording;
        public bool IsRecording
        {
            get
            {
                return isRecording;
            }
            set
            {
                isRecording = value;
            }
        }
        private byte[] audioBuffer;
        private AudioRecord audRecorder;
        private readonly string path;

        public WavHandler(string filePath)
        {
            path = filePath;
            audioBuffer = new byte[200000];

            InitializeAudioRecorder();
        }

        private void InitializeAudioRecorder()
        {
            audRecorder = new AudioRecord(
              // Hardware source of recording.
              AudioSource.Mic,
              // Frequency
              44100,
              // Mono or stereo
              ChannelIn.Mono,
              // Audio encoding
              Encoding.Pcm16bit,
              // Length of the audio clip.
              audioBuffer.Length
            );
        }

        public async Task RecordAudioAsync()
        {
            IsRecording = true;
            audRecorder.StartRecording();
            reading = new Task(ReadAudioAsync);
            reading.Start();
            //await Task.Run(() => WriteWavFileAsync());
        }

        private void ReadAudioAsync()
        {
            while (IsRecording)
            {
                try
                {
                    // Keep reading the buffer while there is audio input.
                    audRecorder.Read(audioBuffer, 0, audioBuffer.Length);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(ex.Message);
                    break;
                }
            }
        }
        
        public async Task StopRecordingAsync()
        {
            IsRecording = false;
            await reading;
            await Task.Run(() => WriteWavFileAsync());
        }

        private async Task WriteWavFileAsync()
        {
            try
            {
                byte[] realData = audioBuffer;
                audioBuffer = new byte[200000];

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    WaveHeaderWriter.WriteHeader(fs, realData.Length, 1, 44100);
                    await fs.WriteAsync(realData, 0, realData.Length);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public async Task<byte[]> ReadWavFileAsync()
        {
            try
            {
                byte[] data = await Task.Run(() => File.ReadAllBytes(path));
                return data;
            }
            catch (Exception ex)
            {
                return new byte[] { };
            }
        }
    }
}
