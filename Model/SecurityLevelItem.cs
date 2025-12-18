using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentHub.Model
{
    public class SecurityLevelItem
    {
        public string Name { get; set; }      
        public string Abbreviation { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Abbreviation}";
        }
    }
}
