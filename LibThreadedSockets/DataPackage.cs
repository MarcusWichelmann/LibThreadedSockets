using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibThreadedSockets
{
    public class DataPackage
    {
        public byte[] Bytes
        {
            get
            {
                return data.ToArray();
            }
        }

        private List<byte> data = new List<byte>();

        public DataPackage(params object[] parts)
        {
            foreach(object part in parts)
                Add(part);
        }

        public void Add(object part)
        {
            if(part is Array)
                foreach(object element in (Array)part)
                    Add(element);
            else if(part is byte)
                Add((byte)part);
            else if(part is bool)
                Add((bool)part);
            else if(part is char)
                Add((char)part);
            else if(part is short)
                Add((short)part);
            else if(part is ushort)
                Add((ushort)part);
            else if(part is int)
                Add((int)part);
            else if(part is uint)
                Add((uint)part);
            else if(part is long)
                Add((long)part);
            else if(part is ulong)
                Add((ulong)part);
            else if(part is float)
                Add((float)part);
            else if(part is double)
                Add((double)part);
            else if(part is string)
                Add((string)part);
            else if(part is DataPackage)
                Add((DataPackage)part);
            else
                throw new ArgumentException("Unsupported argument type");
        }

        public void Add(byte value) => data.Add(value);
        public void Add(bool value) => data.AddRange(BitConverter.GetBytes(value));
        public void Add(char value) => data.AddRange(BitConverter.GetBytes(value));
        public void Add(short value) => data.AddRange(BitConverter.GetBytes(value));
        public void Add(ushort value) => data.AddRange(BitConverter.GetBytes(value));
        public void Add(int value) => data.AddRange(BitConverter.GetBytes(value));
        public void Add(uint value) => data.AddRange(BitConverter.GetBytes(value));
        public void Add(long value) => data.AddRange(BitConverter.GetBytes(value));
        public void Add(ulong value) => data.AddRange(BitConverter.GetBytes(value));
        public void Add(float value) => data.AddRange(BitConverter.GetBytes(value));
        public void Add(double value) => data.AddRange(BitConverter.GetBytes(value));
        public void Add(string value) => data.AddRange(Encoding.UTF8.GetBytes(value));
        public void Add(DataPackage value) => data.AddRange(value.Bytes);

        public override string ToString() => BitConverter.ToString(Bytes);
    }
}
