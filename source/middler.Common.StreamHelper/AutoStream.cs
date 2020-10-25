using System;
using System.IO;
using System.Threading;

namespace middler.Common.StreamHelper {

    public class AutoStream : Stream {

        public override bool CanRead => InnerStream.CanRead;
        public override bool CanSeek => InnerStream.CanSeek;
        public override bool CanWrite => InnerStream.CanWrite;
        public override long Length => InnerStream.Length;
        public override long Position {
            get => InnerStream.Position;
            set => InnerStream.Position = value;
        }

        public AutoStreamOptions Options { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        private Stream InnerStream { get; set; } = new MemoryStream();
        private bool IsFileStream { get; set; }


        public AutoStream(Action<AutoStreamOptionsBuilder> options, CancellationToken cancellationToken = default): this((AutoStreamOptionsBuilder)options, cancellationToken)
        {

        }

        public AutoStream(AutoStreamOptions options, CancellationToken cancellationToken = default)
        {
            Options = options;
            CancellationToken = cancellationToken;
            Options.TempDirectory ??= Directory.GetCurrentDirectory();
            Options.MemoryThreshold ??= 32 * 1024; // 32k
        }
        

        public override void Flush() {
            InnerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) {
            return InnerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) {
            return InnerStream.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            InnerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count) {

            if (!IsFileStream) {
                var allowMemoryBuffer = (Options.MemoryThreshold - count) >= InnerStream.Length;
                if (!allowMemoryBuffer) {
                    IsFileStream = true;
                    var tempStream = InnerStream;

                    EnsureFileStream();
                    tempStream.Seek(0, SeekOrigin.Begin);
                    int copyBufferSize = this.GetCopyBufferSize();
                    tempStream.CopyToAsync(InnerStream, copyBufferSize, CancellationToken);
                    InnerStream.Flush();
                    tempStream.Dispose();
                }
            }

            InnerStream.Write(buffer, offset, count);

        }

        private int GetCopyBufferSize()
        {
            int num = 81920;
            if (this.CanSeek)
            {
                long length = this.Length;
                long position = this.Position;
                if (length <= position)
                {
                    num = 1;
                }
                else
                {
                    long val2 = length - position;
                    if (val2 > 0L)
                        num = (int)Math.Min((long)num, val2);
                }
            }
            return num;
        }


        private void EnsureFileStream() {

            var tempFileName = Path.Combine(Options.TempDirectory, $"{Options.FilePrefix}_{Guid.NewGuid():n}.tmp");
            InnerStream = new FileStream(
                tempFileName,
                FileMode.Create,
                FileAccess.ReadWrite,
                FileShare.Delete,
                bufferSize: 1,
                FileOptions.Asynchronous | FileOptions.SequentialScan | FileOptions.DeleteOnClose);

        }


        protected override void Dispose(bool disposing) {
            InnerStream.Dispose();
            base.Dispose(disposing);
        }

    }
}
