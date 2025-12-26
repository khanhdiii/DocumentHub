using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using DocumentHub.Model;

namespace DocumentHub.ViewModel
{
    public class RecipientsViewModel : INotifyPropertyChanged
    {
        // List Recipient
        public ObservableCollection<Recipient> RecipientList { get; set; }
            = new ObservableCollection<Recipient>();

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

        //Command function
        public ICommand SaveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public RecipientsViewModel()
        {
            // Data test
            RecipientList.Add(new Recipient { Id = 1, Name = "Sở Nội vụ" });
            RecipientList.Add(new Recipient { Id = 2, Name = "Phòng Tài chính" });
            RecipientList.Add(new Recipient { Id = 3, Name = "Ban Giám đốc" });

            SaveCommand = new RelayCommand(param => SaveRecipient());
            EditCommand = new RelayCommand(param => EditRecipient(param as Recipient));
            DeleteCommand = new RelayCommand(param => DeleteRecipient(param as Recipient));
        }

        private void SaveRecipient()
        {
            if (SelectedRecipient == null) return;

            // If not in list -> add new
            if (!RecipientList.Contains(SelectedRecipient))
            {
                SelectedRecipient.Id = RecipientList.Count + 1;
                RecipientList.Add(SelectedRecipient);
            }
            else
            {
                // If in list -> edit
                OnPropertyChanged(nameof(RecipientList));
            }
        }

        private void EditRecipient(Recipient recipient)
        {
            if (recipient != null)
            {
                SelectedRecipient = recipient;
                recipient.Name += " (đã chỉnh sửa)";
                OnPropertyChanged(nameof(RecipientList));
            }
        }

        private void DeleteRecipient(Recipient recipient)
        {
            if (recipient != null)
            {
                RecipientList.Remove(recipient);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
