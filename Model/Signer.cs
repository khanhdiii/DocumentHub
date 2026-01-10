using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentHub.Model
{
    public class Signer : INotifyPropertyChanged 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        private string _fullName; 
        public string FullName 
        { 
            get => _fullName;
            set { _fullName = value;
                OnPropertyChanged(nameof(FullName));
            } 
        } 
        private string _position; 
        public string Position 
        { 
            get => _position;
            set { _position = value;
                OnPropertyChanged(nameof(Position));
            } 
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
