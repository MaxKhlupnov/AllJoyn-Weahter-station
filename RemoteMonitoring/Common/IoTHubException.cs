using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Client.Exceptions
{
    public class IoTHubException : ArgumentException
    {
        public bool IsTransient { get; set; }
    }
}
