using System;
using System.Collections.Generic;
using System.Text;

namespace D2Data
{
    /// <summary>
    /// Data file name enumeration.
    /// </summary>
    public enum DataFileEnum
    {
        Armor,
        Books,
        CharStats,
        CompCode,
        CubeMain,
        DifficultyLevels,
        ElemTypes,
        Experience,
        Gems,
        Hireling,
        Inventory,
        ItemRatio,
        ItemStatCost,
        ItemTypes,
        Levels,
        LvlMaze,
        LvlPrest,
        LvlSub,
        LvlTypes,
        MagicPrefix,
        MagicSuffix,
        Misc,
        MissCalc,
        Missiles,
        MonAi,
        MonEquip,
        MonLvl,
        MonMode,
        MonPreset,
        MonProp,
        MonSeq,
        MonSounds,
        MonStats,
        MonStats2,
        MonType,
        MonUMod,
        Npc,
        Objects,
        Overlay,
        PetType,
        PlrMode,
        Properties,
        Runes,
        SetItems,
        Sets,
        Shrines,
        SkillCalc,
        SkillDesc,
        Skills,
        SoundEnviron,
        Sounds,
        States,
        SuperUniques,
        TreasureClassEx,
        UniqueAppellation,
        UniqueItems,
        UniquePrefix,
        UniqueSuffix,
        UniqueTitle,
        Weapons,
    }

    public static class DataFileUtils
    {
        /// <summary>
        /// Gets data filename.
        /// </summary>
        /// <param name="file">Data file</param>
        /// <returns></returns>
        public static string GetDataFilename(DataFileEnum file)
        {
            return file.ToString() + ".txt";
        }
    }
}
