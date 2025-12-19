using DocumentHub.Data;
using DocumentHub.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DocumentHub.ViewModel
{
    class IncomingDocViewModel
    {
        public ObservableCollection<IncomingDocument> IncomingDocs { get; set; }

        public IncomingDocViewModel()
        {
            using var _context = new AppDbContext();
            _context.Database.EnsureCreated();

            var list = _context.IncomingDocuments.AsNoTracking().ToList();
            IncomingDocs = new ObservableCollection<IncomingDocument>(list);
        }
    }
}
