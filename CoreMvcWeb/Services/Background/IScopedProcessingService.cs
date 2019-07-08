using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcWeb.Services.Background
{
    internal interface IScopedProcessingService
    {
        void DoWork();
    }
}
