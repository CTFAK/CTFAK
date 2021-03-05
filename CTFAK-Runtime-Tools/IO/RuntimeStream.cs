using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CTFAK_Runtime_Tools.IO
{
    public class RuntimeStream:Stream
    {
        private IntPtr _procHandle;
        public Process Process;
        private byte[] _buffer;

        public RuntimeStream(Process proc)
        {
            Process = proc;
            CanRead = true;
            CanSeek = false;
            CanWrite = true;
            Process.Refresh();
            _procHandle = Native.OpenProcess(Native.ProcessAccessFlags.All, false, Process.Id);
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    throw new NotImplementedException();
            }
            return Position;
        }


        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Native.ReadProcessMemory(_procHandle, new IntPtr(Position+offset),buffer, count, out var bytesRead);
            Position += count;
            // if(bytesRead.ToInt32()!=count)throw new NotImplementedException("Reading Failed");
            return count; //bytesRead.ToInt32();
        }

        


        public override void Write(byte[] buffer, int offset, int count)
        {
            Native.WriteProcessMemory(_procHandle,new IntPtr(Position+offset),buffer,(uint)count,out var bytesWritter);
        }

        public override bool CanRead { get; }
        public override bool CanSeek { get; }
        public override bool CanWrite { get; }

        public override long Length
        {
            get
            {
                Process.Refresh();
                return Process.PrivateMemorySize64;
            }
        }
        public override long Position { get; set; }
    }
}