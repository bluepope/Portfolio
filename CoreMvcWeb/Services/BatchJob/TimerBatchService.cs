using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreMvcWeb.Services.BatchJob
{
    public class TimerBatchService : ITimerBatchService, IDisposable
    {
        public int Count { get; set; } = 0;

        Timer _timer;

        public TimerBatchService()
        {
            Console.WriteLine("timer create");
            this.StartAsync();
        }

        public Task StartAsync()
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            
            return Task.CompletedTask;
        }

        public void DoWork(object state)
        {
            //동작
            this.Count++;

            Console.WriteLine("timer dowork");
        }

        public Task StopAsync()
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Console.WriteLine("timer dispose");
            _timer?.Dispose();
        }
    }
}
