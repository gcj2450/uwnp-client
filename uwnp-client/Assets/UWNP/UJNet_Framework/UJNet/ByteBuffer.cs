using System;
using System.Collections;
using System.IO;
using System.Text;

namespace UJNet
{
    public class ByteBuffer
    {
		// big order
        public MemoryStream memStream;

        public ByteBuffer()
        {
            memStream = new MemoryStream();
        }

        public ByteBuffer(int capacity)
        {
            memStream = new MemoryStream(capacity);
        }

        public static ByteBuffer Allocate(int capacity)
        {
            return new ByteBuffer(capacity);
        }
		// return cached byte
        public byte[] array()
        {
            return memStream.ToArray();
        }
		
		public long Position(){
			return memStream.Position;
		}
		
		public long Length(){
			return memStream.Length;
		}
		
		public void Flip(){
			memStream.Seek(0, SeekOrigin.Begin);
		}
		
		public void Position(long newPos){
			memStream.Seek(newPos, SeekOrigin.Begin);
		}

        // ===============PUT BYTE-ORDER==>Little(base type) -> Big==============//
        public ByteBuffer Put(byte[] val)
        {
			return Put(val, false);
        }
		
		// Put bytes big order, otherwise plat detach
		public ByteBuffer Put(byte[] val, bool bigOrder)
        {
			if (!bigOrder && BitConverter.IsLittleEndian)
               Array.Reverse(val);
			
            memStream.Write(val, 0, val.Length);
            return this;
        }
		
        public ByteBuffer Put(byte val){
            memStream.Write(new byte[]{val}, 0, 1);
            return this;
        }

        public ByteBuffer PutShort(short val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            memStream.Write(bytes, 0, bytes.Length);
            return this;
        }
        
        public ByteBuffer PutInt(int val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

             memStream.Write(bytes, 0, bytes.Length);
            return this;
        }

        public ByteBuffer PutLong(long val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

             memStream.Write(bytes, 0, bytes.Length);
            return this;
        }

        public ByteBuffer PutFloat(float val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            memStream.Write(bytes, 0, bytes.Length);
            return this;
        }

        public ByteBuffer PutDouble(double val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

             memStream.Write(bytes, 0, bytes.Length);
            return this;
        }

        //=================Get BYTE-ORDER=> BIG -> Little(base type) =================//
        public byte Get()
        {
            byte[] bytes = new byte[1];
            memStream.Read(bytes, 0, bytes.Length);
            return bytes[0];
        }

		public void Get(byte[] buf, int index, int count)
        {
            memStream.Read(buf, index, count);
        }

        public short GetShort()
        {
            byte[] bytes = new byte[2];
            memStream.Read(bytes, 0, bytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt16(bytes, 0);
        }

        public int GetInt()
        {
            byte[] bytes = new byte[4];
            memStream.Read(bytes, 0, bytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

        public long GetLong()
        {
            byte[] bytes = new byte[8];
            memStream.Read(bytes, 0, bytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt64(bytes, 0);
        }

        public float GetFloat()
        {
            byte[] bytes = new byte[4];
            memStream.Read(bytes, 0, bytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToSingle(bytes, 0);
        }

        public double GetDouble()
        {
            byte[] bytes = new byte[8];
			memStream.Read(bytes, 0, bytes.Length);
           	if (BitConverter.IsLittleEndian)
            	Array.Reverse(bytes);

            return BitConverter.ToDouble(bytes, 0);
        }
       
       
    }
}
