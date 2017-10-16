using System.Windows;
using Wit.Communication.WitAiComm;
using Wit.TextTestWPF.WitAiTextComm.ViewModels;

namespace Wit.TextTestWPF.WitAiTextComm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WitViewModel ViewModel { get; set; } = new WitViewModel();
        public string tbTxt { get; set; } = "Text to send";

        public MainWindow()
        {
            DataContext = this;
            ViewModel.Requests.Add(new Request
            {
                Search = "search1",
                Result = "result1"
            });

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Request req = WitAiComm.SendRequest(tbTxt);
            ViewModel.Requests.Add(req);
        }
    }
}
