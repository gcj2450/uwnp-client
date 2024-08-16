using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UJNet.Data;

namespace NetCoreServer.UJNet_Framework.UJNet.Data
{
    public class SFSDataWrapper
    {
        private int type;

        private object data;

        /// <exclude />
        public int Type
        {
            get
            {
                return type;
            }
        }

        /// <exclude />
        public object Data
        {
            get
            {
                return data;
            }
        }

        /// <exclude />
        public SFSDataWrapper(int type, object data)
        {
            this.type = type;
            this.data = data;
        }

        /// <exclude />
        public SFSDataWrapper(SFSDataType tp, object data)
        {
            type = (int)tp;
            this.data = data;
        }
    }
}
