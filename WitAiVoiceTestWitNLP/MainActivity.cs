using Android.App;
using Android.Widget;
using Android.OS;
using Wit.Communication.WitAiComm;
using Wit.VoiceTestWitNLP.Wav;

namespace WitAiVoiceTestWitNLP
{
    [Activity(Label = "WitAiVoiceTestWitNLP", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private bool isRecording = false;
        private const string wavFilePath = @"/sdcard/SendToWit.wav";
        private ProgressDialog progress;
        private WavHandler wavHandler;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            progress = new ProgressDialog(this);
            wavHandler = new WavHandler(wavFilePath);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button btnRecord = FindViewById<Button>(Resource.Id.btnRecord);
            TextView tvSearch = FindViewById<TextView>(Resource.Id.tvSearch);
            TextView tvResult = FindViewById<TextView>(Resource.Id.tvResult);

            //byte[] da = await wavHandler.ReadWavFileAsync();
            //Request ra = await WitAiComm.PostVoiceAsync(da);

            btnRecord.Click += async (o, i) =>
            {
                isRecording = !isRecording;
                //wavHandler.IsRecording = isRecording;
                if (isRecording)
                {
                    btnRecord.Text = Resources.GetString(Resource.String.stopRecording);
                    await wavHandler.RecordAudioAsync();
                }
                else
                {
                    await wavHandler.StopRecordingAsync();
                    btnRecord.Text = Resources.GetString(Resource.String.startRecording);
                    progress.SetMessage("Plesase wait, processing request...");
                    progress.SetCancelable(false);
                    progress.Show();
                    //byte[] d = await wavHandler.ReadWavFileAsync();
                    //Request r = await WitAiComm.PostVoiceAsync(d);
                    //tvSearch.Text = r.Search;
                    //tvResult.Text = r.Result;
                    progress.Hide();
                }
            };
        }
    }
}

