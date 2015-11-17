using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamitey;

namespace RemoteMonitoring.Common.DeviceSchema
{
    public static class WireCommandSchemaHelper
    {
        public static dynamic GetParameters(dynamic command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            IEnumerable<string> members = Dynamic.GetMemberNames(command);
            if (!members.Any(m => m == "Parameters"))
            {
                return null;
            }

            dynamic parameters = command.Parameters;

            return parameters;
        }
    }
}
