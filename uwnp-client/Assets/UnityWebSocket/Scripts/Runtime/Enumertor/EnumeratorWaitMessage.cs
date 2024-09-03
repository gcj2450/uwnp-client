using System.Collections;
using Google.Protobuf;

namespace ServerSDK.Network
{
    internal class EnumeratorWaitMessage : IEnumerator
    {
        public Zion.MsgCmd WaitMsg = null;

        public Zion.EnumCmdCode WaitCmd;
        internal EnumeratorWaitMessage(Zion.EnumCmdCode waitCmd)
        {
            this.WaitCmd = waitCmd;
        }

        public bool MoveNext()
        {
            return (WaitMsg == null);
        }

        public void Reset()
        {
            WaitMsg = null;
        }

        public object Current => WaitMsg;
    }
}
