﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreServer.UJNet_Framework.UJNet.Data
{
    public class SFSArrayEnumerator : IEnumerator
    {
        private List<SFSDataWrapper> data;

        private int cursorIndex;

        object IEnumerator.Current
        {
            get
            {
                if (cursorIndex < 0 || cursorIndex == data.Count)
                {
                    throw new InvalidOperationException();
                }
                return data[cursorIndex].Data;
            }
        }

        /// <exclude />
        public SFSArrayEnumerator(List<SFSDataWrapper> data)
        {
            this.data = data;
            cursorIndex = -1;
        }

        void IEnumerator.Reset()
        {
            cursorIndex = -1;
        }

        bool IEnumerator.MoveNext()
        {
            if (cursorIndex < data.Count)
            {
                cursorIndex++;
            }
            return cursorIndex != data.Count;
        }
    }
}