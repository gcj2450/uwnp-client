using System;
using System.Collections;
using System.Text;
using UJNet.Data;

namespace UJNet
{
    interface UJSerializer
    {
        byte[] Obj2Bin(UJObject obj);

        UJObject Bin2Obj(byte[] data);

        byte[] Array2Bin(UJArray array);

        UJArray Bin2Array(byte[] data);
		
		UJObject Json2Obj(string s);
		
		UJArray Json2Array(string s);
	}
}
