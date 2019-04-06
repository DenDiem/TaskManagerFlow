
using Binarysharp.MemoryManagement.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Input;
using TaskManagerFlow.Models;
using TaskManagerFlow.Tools;
using Process = System.Diagnostics.Process;


namespace TaskManagerFlow.ViewModels
{
    class TaskManagerViewModel : INotifyPropertyChanged
    {
        private ICommand _deleteCommand;
        private ICommand _openCommand;

        private static Timer timer;
        private static Timer timerSec;

        private List<Models.Process> _processList;
        private List<Models.MyThreads> _threadList;
        private List<Models.Moduls> _modulList;
        private static Models.Process _selectedItem;
        private int _count;
        private static int _checker;
        public Models.Process SelectedItem
        {
            get { return _selectedItem; }
            set
            {

                _selectedItem = value;
                Checker = ProcessList.IndexOf(value);
                NotifyPropertyChanged();


            }
        }
        public List<Models.MyThreads> ThreadList
        {
            get { return _threadList; }
            set
            {
                _threadList = value;
                NotifyPropertyChanged();

            }
        }
        public List<Models.Moduls> ModulList
        {
            get { return _modulList; }
            set
            {
                _modulList = value;
                NotifyPropertyChanged();

            }
        }
        public List<Models.Process> ProcessList
        {
            get { return _processList; }
            set
            {
                _processList = value;
                NotifyPropertyChanged();

            }
        }



        /*public String TotalRam
        {
            get
            {
                return My.Computer.Info.AvailablePhysicalMemory;
            }
        }*/
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                NotifyPropertyChanged();

            }
        }
        public static int Checker
        {
            get => _checker;
            set
            {


                _checker = value;




            }
        }
        public TaskManagerViewModel()
        {
            Checker = 0;





            timer = new Timer(_ => RefreshMainTable(), null, 0, 8000);

        }

        void RefreshMainTable()
        {


            ProcessList = GenerateProcessList();
            Count++;

            SelectedItem = ProcessList[Checker];
            if (timerSec != null) 
            timerSec.Dispose();
            timerSec = new Timer(_ => RefreshSecondTable(), null, 0, 2000);

        }

        private List<Models.Process> GenerateProcessList()
        {

            String mashineName, threadCount, processRAM, processName, pathTODO;
            DateTime start;
            int processID;
            var counters = new List<PerformanceCounter>();
            bool active;
            List<Models.Process> processes = new List<Models.Process>();
            try
            {
                foreach (Process process in Process.GetProcesses())
                {

                    start = process.StartTime.Date;
                    processID = process.Id;


                    pathTODO = GetExecutablePathAboveVista((UIntPtr)processID);// process.MainModule.FileName;
                    processRAM = ((process.WorkingSet64) / 1024.0) / 1024.0 + "MB";
                    mashineName = process.MachineName;
                    threadCount = process.Threads.Count.ToString();
                    active = process.Responding;

                    processName = process.ProcessName;

                    var counter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);
                    counter.NextValue();
                    counters.Add(counter);
                    processes.Add(new Models.Process(start, pathTODO, mashineName, threadCount, active,
                        processID.ToString(), processName, processRAM));


                }
            }
            catch (Exception e)
            {
                //MessageBox.Show("ERROR:", e.Message);
            }
            int i = 0;

            Thread.Sleep(1000);

            foreach (var counter in counters)
            {
                try
                {
                    processes[i].ProcessGpu = Math.Round(counter.NextValue(), 1) + "";
                }
                catch (Exception e)
                {

                }

                ++i;
            }


            if (ProcessList != null)
                SelectedItem = ProcessList[Checker];
            // MessageBox.Show(SelectedItem.Name);
            return processes;
        }
        public ICommand OpenCommand => (_openCommand) ?? (_openCommand = new RelayCommand<object>(o => Open(), o => Checker!=-1));

        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand<object>(o => Delete(), o => Checker != -1));

        public void Delete()
        {
            Process.GetProcesses()[Checker].Kill();
        }
        public void Open()
        {
            var str = ProcessList[Checker].Path;
            Process.Start(@str.Substring(0,str.LastIndexOf(@"\")) + @"\");
        }
        private List<Models.MyThreads> GenerateThreads()
        {
            List<Models.MyThreads> res = new List<MyThreads>();

            try
            {
                foreach (ProcessThread thread in Process.GetProcesses()[Checker].Threads)
                {
                    res.Add(new MyThreads(thread.ThreadState.ToString(), thread.StartTime.Date, thread.Id));

                }

            }
            catch
            {

            }

            return res;
        }
        private List<Models.Moduls> GenerateModuls()
        {
            List<Models.Moduls> res = new List<Moduls>();
            try
            {
                foreach (ProcessModule module in Process.GetProcesses()[Checker].Modules)
                {
                    res.Add(new Moduls(module.FileName, module.ModuleName));

                }

            }
            catch
            {

            }



            return res;
        }

        private void RefreshSecondTable()
        {
            ThreadList = GenerateThreads();
            ModulList = GenerateModuls();

        }
        private static string GetExecutablePathAboveVista(UIntPtr dwProcessId)
        {
            StringBuilder buffer = new StringBuilder(1024);
            IntPtr hprocess = OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, (int)dwProcessId);
            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    int size = buffer.Capacity;
                    if (QueryFullProcessImageName(hprocess, 0, buffer, out size))
                    {
                        return buffer.ToString();
                    }
                }
                finally
                {
                    CloseHandle(hprocess);
                }
            }
            return string.Empty;
        }
        [DllImport("kernel32.dll")]
        private static extern bool QueryFullProcessImageName(IntPtr hprocess, int dwFlags,
            StringBuilder lpExeName, out int size);
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
            bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);
        #region OnProperetyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(
            [CallerMemberName] String propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
