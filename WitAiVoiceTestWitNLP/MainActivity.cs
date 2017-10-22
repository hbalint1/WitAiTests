using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Media;
using System.Threading.Tasks;
using WitAiVoiceTestWitNLP.Model;
using System.IO;
using Wit.Communication.WitAiComm;

namespace WitAiVoiceTestWitNLP
{
    [Activity(Label = "WitAiVoiceTestWitNLP", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private bool isRecording = false;
        private byte[] audioBuffer;
        private AudioRecord audRecorder;
        private string path = @"/sdcard/SendToWit.wav";
        private ProgressDialog progress;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            progress = new ProgressDialog(this);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button btnRecord = FindViewById<Button>(Resource.Id.btnRecord);
            TextView tvSearch = FindViewById<TextView>(Resource.Id.tvSearch);
            TextView tvResult = FindViewById<TextView>(Resource.Id.tvResult);

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
                    byte[] d = await ReadWavFileAsync();
                    progress.SetMessage("Plesase wait, processing request...");
                    progress.SetCancelable(false);
                    progress.Show();
                    Request r = await WitAiComm.PostVoiceAsync(d);
                    tvSearch.Text = r.Search;
                    tvResult.Text = r.Result;
                    progress.Hide();
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

        private async Task WriteWavFileAsync()
        {
            byte[] realData = audioBuffer;

            using (FileStream fs = new FileStream(path, FileMode.Create)) 
            {
                WaveHeaderWriter.WriteHeader(fs, realData.Length, 1, 44100);
                await fs.WriteAsync(realData, 0, realData.Length);
            }
        }

        private async Task<byte[]> ReadWavFileAsync()
        {
            byte[] data = await Task.Run(() => File.ReadAllBytes(path));
            
            return data;
        }
    }
}

