using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerFlow.Models
{
    class Process
    {
        public String Name { get; set; }
        public String ProcessId { get; set; }
        public bool Active { get; set; }
        public String ProcessGpu { get; set; }
        public String ProcessRam { get; set; }
        public String Threads { get; set; }
        public String UserName { get; set; }
        public String Path { get; set; }
        public String FileName { get; set; }
        public DateTime RunDate { get; set; }

        Process(DateTime runDate, string fileName, string path, string userName, string threads, bool active, string processId, string name)
        {
            RunDate = runDate;
            FileName = fileName;
            Path = path;
            UserName = userName;
            Threads = threads;
            Active = active;
            ProcessId = processId;
            Name = name;
        }

    }
}
