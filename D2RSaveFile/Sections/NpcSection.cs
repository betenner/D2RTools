using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2SaveFile.Sections
{
    public class NpcSection : IDiablo2FileSection
    {
        public byte[] Data { get; }
        public bool IsChanged { get; set; }
        public int Size => 51;

        public NpcSection(byte[] data, int offset)
        {
            Data = new byte[Size];
            Array.Copy(data, offset, Data, 0, Size);
        }
    }
}
