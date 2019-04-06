using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerFlow.Models
{
    class Moduls
    {
        public Moduls(string modulPath, string modulName)
        {
            ModulPath = modulPath;
            ModulName = modulName;
        }

        public String ModulName { get; set; }
        public String ModulPath { get; set; }
    }
}
