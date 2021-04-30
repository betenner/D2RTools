using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2SaveFile.Sections
{
    public interface IDiablo2FileSection
    {
        byte[] Data { get; }
        int Size { get; }

        bool IsChanged { get; set; }
    }
}
