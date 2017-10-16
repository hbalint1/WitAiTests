using System.Collections.ObjectModel;
using System.ComponentModel;
using Wit.Communication.WitAiComm;

namespace Wit.TextTestWPF.WitAiTextComm.ViewModels
{
    public class WitViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Request> _requests;

        public event PropertyChangedEventHandler PropertyChanged;

        public WitViewModel()
        {
            _requests = new ObservableCollection<Request>();
        }

        public ObservableCollection<Request> Requests
        {
            get
            {
                return _requests;
            }
            set
            {
                if (_requests != value)
                {
                    _requests = value;

                    OnPropertyCanhged(nameof(_requests));
                }
            }
        }

        public void OnPropertyCanhged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
