using System.Collections;

namespace ServerSDK.Network
{
    internal class EnumertorConnect : IEnumerator
    {
        internal bool IsConnected = false;

        internal EnumertorConnect()
        {
        }

        public bool MoveNext()
        {
            return !IsConnected;
        }

        public void Reset()
        {
            IsConnected = false;
        }

        public object Current => IsConnected;
    }
}
