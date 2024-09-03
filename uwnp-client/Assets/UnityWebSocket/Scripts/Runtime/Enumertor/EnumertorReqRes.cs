using System;
using System.Collections;
using UnityEngine;

namespace ServerSDK.Network
{
    public class EnumertorReqRes : IEnumerator
    {
        internal Zion.MsgCmd ResMsg = null;
        internal DateTime starttime;
        internal int timeout = 0;
        internal uint sn = 0;
        internal Net net;

        internal EnumertorReqRes()
        {
            timeout = 0;
        }
        internal EnumertorReqRes(Net net, uint sn, int vartimeout)
        {
            this.sn = sn;
            this.timeout = vartimeout;
            this.starttime = DateTime.Now;
            this.net = net;
        }

        public bool MoveNext()
        {
            if (timeout == 0)
            {
                return (ResMsg == null);
            }
            else
            {
                TimeSpan timespan = DateTime.Now - starttime;
                if (timespan.Seconds >= timeout)
                {
                    net.RemoveEnumReqRes(this.sn);
                    return false;
                } 
                else 
                {
                    return (ResMsg == null);
                }
            }
        }

        public void Reset()
        {
            ResMsg = null;
        }

        public object Current => ResMsg;
    }
}
