using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2SaveFile.Interfaces
{
    public interface IStatisticData
    {
        uint GetStatistic(CharacterStatistic stat);
        void SetStatistic(CharacterStatistic stat, uint value);
    }
}
