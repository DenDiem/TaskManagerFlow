using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerFlow.ViewModels
{
    class TaskManagerViewModel
    {
        private Process[] _processList;
        public Process[] ProcessList
        {
            get
            {
                return _processList;

            }
            set
            {
                _processList = value;

            }
        }

        public TaskManagerViewModel()
        {
            ProcessList = GenerateProcessList();
        }
        private Process[] GenerateProcessList()
        {
            return Process.GetProcesses();
        }
    }
}
