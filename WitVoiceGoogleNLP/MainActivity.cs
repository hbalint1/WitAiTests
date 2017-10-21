using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Speech;
using Wit.Communication.WitAiComm;

namespace WitAiVoiceGoogleNLP
{
    [Activity(Label = "WitVoiceGoogleNLP", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        #region Local variables

        private bool isRecording;
        private const int VOICE = 10;

        #endregion

        #region View elements

        private Button recButton;
        private TextView tvSearch;
        private TextView tvResult;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            recButton = FindViewById<Button>(Resource.Id.btnRecord);
            tvSearch = FindViewById<TextView>(Resource.Id.tvSearch);
            tvResult = FindViewById<TextView>(Resource.Id.tvResult);

            SetMicrophone();
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        string search = matches[0];

                        // limit the output to 500 characters
                        if (search.Length > 500)
                            search = search.Substring(0, 500);

                        Request result = WitAiComm.SendRequest(search);
                        tvSearch.Text = result.Search;
                        tvResult.Text = result.Result;
                    }
                    else
                        tvSearch.Text = "No speech was recognised";
                    // change the text back on the button
                    recButton.Text = "Start Recording";

                    isRecording = !isRecording;
                }
            }

            base.OnActivityResult(requestCode, resultVal, data);
        }

        #region Private methods

        private void SetMicrophone()
        {
            // check to see if we can actually record - if we can, assign the event to the button
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                // no microphone, no recording. Disable the button and output an alert
                var alert = new AlertDialog.Builder(recButton.Context);
                alert.SetTitle("You don't seem to have a microphone to record with");
                alert.SetPositiveButton("OK", (sender, e) =>
                {
                    //textBox.Text = "No microphone present";
                    recButton.Enabled = false;
                    return;
                });

                alert.Show();
            }
            else
                recButton.Click += delegate
                {
                    // change the text on the button
                    recButton.Text = "End Recording";
                    isRecording = !isRecording;
                    if (isRecording)
                    {
                        // create the intent and start the activity
                        var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);

                        // put a message on the modal dialog
                        voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, Application.Context.GetString(Resource.String.messageSpeakNow));

                        // if there is more then 1.5s of silence, consider the speech over
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

                        // you can specify other languages recognised here, for example
                        // voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.German);
                        // if you wish it to recognise the default Locale language and German
                        // if you do use another locale, regional dialects may not be recognised very well

                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                        StartActivityForResult(voiceIntent, VOICE);
                    }
                };
        }

        #endregion
    }
}

