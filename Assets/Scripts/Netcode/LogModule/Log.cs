using System;
using System.Collections.Generic;

namespace Netcode.LogModule
{
    [Serializable]
    public class Log
    {
        public string timestamp;
        public string uuid;
        public string action;
        public string context;

        public Log(string timestamp, string uuid, string action, string context)
        {
            this.timestamp = timestamp;
            this.uuid = uuid;
            this.action = action;
            this.context = context;
        }
    }
    
    [Serializable]
    public class LogList
    {
        public List<Log> logs = new List<Log>();
    }
}