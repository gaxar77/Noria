using Noria.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Noria.Misc
{
    public enum FileSystemItemClipboardStorageOperation
    {
        None,
        Copy,
        Cut
    }
    public class FileSystemItemClipboardItem
    {
        public string ItemPath { get; set; }
        public FileSystemItemClipboardStorageOperation Operation { get; set; }
        public FileSystemItemClipboardItem()
        {
            
        }

        public FileSystemItemClipboardItem(byte[] bytes)
        {
            using (var memStream = new MemoryStream(bytes))
            {
                var reader = new StreamReader(memStream);
                Operation = (FileSystemItemClipboardStorageOperation) reader.Read();
                ItemPath = reader.ReadLine();
            }
        }

        public byte[] ToBytes()
        {
            using (var memStream = new MemoryStream())
            {
                var writer = new StreamWriter(memStream);
                writer.Write((char)Operation);
                writer.WriteLine(ItemPath);
                writer.Flush();

                return memStream.ToArray();
            }
        }
    }
    public class FileSystemItemClipboard : IDisposable
    {
        private static readonly string MmfId = "{292570B3-FCB4-42F4-9FDF-B7B4AFA2DF29}";
        private MemoryMappedFile _mmf;
        private bool _isDisposed;
        
        public FileSystemItemClipboard()
        {
            _mmf = MemoryMappedFile.CreateOrOpen(MmfId, 1024);
        }

        public void Store(FileSystemItemClipboardItem item)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(FileSystemItemClipboard));

            using (var stream = _mmf.CreateViewStream())
            {
                var bytes = item.ToBytes();
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
        }

        public FileSystemItemClipboardItem Retrieve()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(FileSystemItemClipboard));

            using (var stream = _mmf.CreateViewStream())
            {
                var reader = new BinaryReader(stream);

                var bytes = reader.ReadBytes(1024);

                var item = new FileSystemItemClipboardItem(bytes);

                return item;
            }
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _mmf.Dispose();
                _isDisposed = true;
            }
        }
    }
}
