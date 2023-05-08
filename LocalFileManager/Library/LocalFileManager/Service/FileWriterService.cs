namespace Rugal.Net.LocalFileManager.Service
{
    public class FileWriterService : IDisposable
    {
        public FileInfo Info { get; set; }
        public FileStream Stream { get; set; }
        public FileWriterService(string FullFileName)
        {
            Info = new FileInfo(FullFileName);
        }

        public FileWriterService OpenRead(long SeekLength = 0)
        {
            if (Stream != null)
                ClearStream();

            Stream = Info.OpenRead();
            if (SeekLength > 0)
                Seek(SeekLength);
            return this;
        }
        public FileWriterService OpenReadFromEnd()
        {
            OpenRead();
            SeekFromEnd();
            return this;
        }
        public FileWriterService OpenWrite(long SeekLength = 0)
        {
            if (Stream != null)
                ClearStream();

            Stream = Info.OpenWrite();
            if (SeekLength > 0)
                Seek(SeekLength);
            return this;
        }
        public FileWriterService OpenWriteFromEnd()
        {
            OpenWrite();
            SeekFromEnd();
            return this;
        }
        public FileWriterService Seek(long SeekLength)
        {
            Stream.Seek(SeekLength, SeekOrigin.Begin);
            return this;
        }
        public FileWriterService SeekFromEnd()
        {
            Stream.Seek(Stream.Length, SeekOrigin.Begin);
            return this;
        }

        public FileWriterService WriteBytes(byte[] Source, int MaxWriteLength = 1024)
        {
            if (Stream is null)
                OpenWriteFromEnd();

            using var SourceBuffer = new MemoryStream(Source);

            var Buffer = new byte[MaxWriteLength];
            while (SourceBuffer.Position < SourceBuffer.Length)
            {
                if (SourceBuffer.Position + MaxWriteLength > SourceBuffer.Length)
                    Buffer = new byte[SourceBuffer.Length - SourceBuffer.Position];

                SourceBuffer.Read(Buffer);
                Stream?.Write(Buffer);
            }

            return this;
        }
        public FileWriterService ReadBytes(Func<byte[], bool> ReadFunc, int MaxReadLength)
        {
            if (Stream is null)
                OpenRead();

            var Buffer = new byte[MaxReadLength];
            while (Stream.Position < Stream.Length)
            {
                if (Stream.Position + MaxReadLength > Stream.Length)
                    Buffer = new byte[Stream.Length - Stream.Position];

                Stream?.Read(Buffer);
                var IsCanNext = ReadFunc.Invoke(Buffer);
                if (!IsCanNext)
                    break;
            }

            return this;
        }

        private void ClearStream()
        {
            Stream?.Flush();
            Stream?.Close();
            Stream?.Dispose();
        }
        public void Dispose()
        {
            Stream.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}