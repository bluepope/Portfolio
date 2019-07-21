using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreMvcWeb.Services.BatchJob
{
    public interface ITimerBatchService
    {
        int Count { get; set; }

        Task StartAsync();
        void DoWork(object state);
        Task StopAsync();
    }
}
