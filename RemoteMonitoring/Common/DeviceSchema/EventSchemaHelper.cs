﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dynamitey;

namespace RemoteMonitoring.Common.DeviceSchema
{
    /// <summary>
    /// Helper class to encapsulate interactions with the event schema (messages from the device).
    /// </summary>
    public static class EventSchemaHelper
    {
        /// <summary>
        /// Returns ObjectType for a message. Used by ASA and EventProcessor processing.
        /// </summary>
        /// <param name="eventData">Message data</param>
        /// <returns>ObjectType value, or empty string if not found</returns>
        public static string GetObjectType(dynamic eventData)
        {
            try {
                if (eventData == null)
                {
                    throw new ArgumentNullException("eventData");
                }

                IEnumerable<string> members = Dynamic.GetMemberNames(eventData);

                if (!members.Any(m => m == "ObjectType"))
                {
                    return "";
                }

                dynamic objectType = eventData.ObjectType;

                if (objectType == null)
                {
                    return "";
                }

                return objectType.ToString();
            }catch(Exception ex)
            {
                String exs = ex.ToString();
                return null;
            }
        }
    }
}
