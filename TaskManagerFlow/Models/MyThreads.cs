using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerFlow.Models
{
    class MyThreads
    {
        public MyThreads(String avalible, DateTime thrreadStart, int threadId)
        {
            Avalible = avalible;
            ThrreadStart = thrreadStart;
            ThreadId = threadId;
        }

        public int ThreadId { get; set; }
        public DateTime ThrreadStart{get;set;}
        public String Avalible { get; set; }
    }
}
