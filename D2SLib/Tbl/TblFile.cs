using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace D2SLib.Tbl
{
    public class TblFile
    {
        private const int KEY_STRING_BUFFER_SIZE = 1024;
        private static readonly byte[] _stringBuffer = new byte[KEY_STRING_BUFFER_SIZE];


        // Header
        private ushort _crc = 0;
        private ushort _elementTableSize = 0;
        private int _hashTableSize = 0;
        private byte _version = 0;
        private int _stringListOffset = 0;
        private int _maxLoops = 0;
        private int _fileSize = 0;

        private List<ushort> _elementTable = null;

        public struct HashNode
        {
            public byte Active;
            public ushort ElementIndex;
            public int HashValue;
            public int KeyStringOffset;
            public int ValueStringOffset;
            public ushort ValueStringLength;
        }

        private List<HashNode> _hashTable = null;

        private List<KeyValuePair<string, string>> _stringList = null;

        public TblFile(string path)
        {
            using FileStream fs = new(path, FileMode.Open);
            using BinaryReader br = new(fs);

            // Header
            _crc = br.ReadUInt16();
            _elementTableSize = br.ReadUInt16();
            _hashTableSize = br.ReadInt32();
            _version = br.ReadByte();
            _stringListOffset = br.ReadInt32();
            _maxLoops = br.ReadInt32();
            _fileSize = br.ReadInt32();

            // Element table
            _elementTable = new(_elementTableSize);
            for (int i = 0; i < _elementTableSize; i++)
            {
                _elementTable.Add(br.ReadUInt16());
            }

            // Hash table
            _hashTable = new(_hashTableSize);
            for (int i = 0; i < _hashTableSize; i++)
            {
                _hashTable.Add(new()
                {
                    Active = br.ReadByte(),
                    ElementIndex = br.ReadUInt16(),
                    HashValue = br.ReadInt32(),
                    KeyStringOffset = br.ReadInt32(),
                    ValueStringOffset = br.ReadInt32(),
                    ValueStringLength = br.ReadUInt16(),
                });
            }

            // String list
            _stringList = new(_elementTableSize);
            foreach (var node in _hashTable)
            {
                if (node.Active == 0) continue;
                br.BaseStream.Seek(node.KeyStringOffset, SeekOrigin.Begin);
                var key = ReadString(br);
                br.BaseStream.Seek(node.ValueStringOffset, SeekOrigin.Begin);
                var value = ReadString(br, node.ValueStringLength);
                _stringList.Add(new(key, value));
            }
        }

        private static string ReadString(BinaryReader br)
        {
            if (br == null || br.BaseStream == null) return null;
            int index = 0;
            try
            {
                byte b = br.ReadByte();
                while (b != 0 && index < KEY_STRING_BUFFER_SIZE)
                {
                    _stringBuffer[index++] = b;
                    b = br.ReadByte();
                }
            }
            catch { }
            return Encoding.UTF8.GetString(_stringBuffer, 0, index);
        }

        private static string ReadString(BinaryReader br, int length)
        {
            if (br == null || br.BaseStream == null) return null;
            byte[] data = br.ReadBytes(length - 1);
            return Encoding.UTF8.GetString(data);
        }

        public ushort CRC { get => _crc; }
        public ushort ElementCount { get => _elementTableSize; }
        public int HashTableCount { get => _hashTableSize; }
        public byte Version { get => _version; }
        public int StringListOffset { get => _stringListOffset; }
        public int MaxLoops { get => _maxLoops; }
        public int FileSize { get => _fileSize; }

        public List<ushort> ElementTable { get => _elementTable; }

        public List<HashNode> HashTable { get => _hashTable; }

        public List<KeyValuePair<string, string>> StringList { get => _stringList; }

        public static void WriteToFile(string path, byte version, List<KeyValuePair<string, string>> stringList, bool calcCrc = false)
        {

        }

        private static int GetHashValue(string keyString, int hashTableSize)
        {
            return Math.Abs(keyString.GetHashCode()) % hashTableSize;
        }
    }
}
