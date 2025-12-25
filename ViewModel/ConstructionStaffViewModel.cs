using DocumentHub.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentHub.ViewModel
{
    public class ConstructionStaffViewModel
    {
        public ObservableCollection<ConstructionStaff> StaffList { get; }
    = new ObservableCollection<ConstructionStaff>
    {
        new ConstructionStaff { Id = 1, FullName = "Nguyễn Văn A", Position = "Chuyên viên" },
        new ConstructionStaff { Id = 2, FullName = "Trần Thị B", Position = "Giám đốc" },
        new ConstructionStaff { Id = 3, FullName = "Lê Văn C", Position = "Phó phòng" }
    };

    }
}
