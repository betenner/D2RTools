using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2SaveFile.Interfaces
{
    public interface IBasicCharacterData
    {
        int Level { get; set; }
        HeroClass HeroClass { get; set; }
        string CharacterName { get; set; }

        void SetCharacterProgression(Difficulty difficulty, Act act);
    }
}
