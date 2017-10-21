using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Media;
using System.Threading.Tasks;
using WitAiVoiceTestWitNLP.Model;
using System.IO;

namespace WitAiVoiceTestWitNLP
{
    [Activity(Label = "WitAiVoiceTestWitNLP", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private bool isRecording = false;
        private byte[] audioBuffer;
        private AudioRecord audRecorder;
        private string path = @"/sdcard/SendToWit.wav";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button btnRecord = FindViewById<Button>(Resource.Id.btnRecord);

            audioBuffer = new byte[200000];
            InitializeAudioRecorder();

            btnRecord.Click += async (o, i) =>
            {
                isRecording = !isRecording;
                if (isRecording)
                {
                    btnRecord.Text = Resources.GetString(Resource.String.stopRecording);
                    await RecordAudioAsync();
                }
                else
                {
                    btnRecord.Text = Resources.GetString(Resource.String.startRecording);
                }
            };
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

        private async Task RecordAudioAsync()
        {
            audRecorder.StartRecording();
            await Task.Run(() => ReadAudioAsync());
            await Task.Run(() => WriteWavFileAsync());
        }

        private void ReadAudioAsync()
        {
            while (isRecording)
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

        private void WriteWavFileAsync()
        {
            byte[] realData = audioBuffer;

            using (FileStream fs = new FileStream(path, FileMode.Create)) 
            {
                WaveHeaderWriter.WriteHeader(fs, realData.Length, 1, 44100);
                fs.Write(realData, 0, realData.Length);
            }
        }
    }
}

