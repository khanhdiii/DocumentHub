using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using DocumentHub.Model;

namespace DocumentHub.ViewModel
{
    public class RecipientsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Recipient> RecipientList { get; }
            = new ObservableCollection<Recipient>
            {
                new Recipient { Id = 1, Name = "Sở Nội vụ" },
                new Recipient { Id = 2, Name = "Phòng Tài chính" },
                new Recipient { Id = 3, Name = "Ban Giám đốc" }
            };

        private Recipient _selectedRecipient;
        public Recipient SelectedRecipient
        {
            get => _selectedRecipient;
            set
            {
                _selectedRecipient = value;
                OnPropertyChanged(nameof(SelectedRecipient));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public RecipientsViewModel()
        {
            SaveCommand = new RelayCommand(param => SaveRecipient());
            EditCommand = new RelayCommand(param => EditRecipient(param as Recipient));
            DeleteCommand = new RelayCommand(param => DeleteRecipient(param as Recipient));
        }

        private void SaveRecipient()
        {
            if (SelectedRecipient == null) return;
            var existing = RecipientList.FirstOrDefault(r => r.Id == SelectedRecipient.Id);
            if (existing != null)
            {
                existing.Name = SelectedRecipient.Name;
                OnPropertyChanged(nameof(RecipientList));
            }
        }

        private void EditRecipient(Recipient recipient)
        {
            if (recipient != null)
                SelectedRecipient = recipient;
        }

        private void DeleteRecipient(Recipient recipient)
        {
            if (recipient != null)
                RecipientList.Remove(recipient);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
