using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using D2Data.DataFile;
using System.Drawing;

namespace D2Data
{
    /// <summary>
    /// Utilities.
    /// </summary>
    public static class Utils
    {
        public static Color GetItemQualityColor(ItemQuality quality)
        {
            switch (quality)
            {
                case ItemQuality.Craft: return Color.Orange;
                case ItemQuality.Magic: return Color.FromArgb(0x4850b8);
                case ItemQuality.Rare: return Color.Yellow;
                case ItemQuality.Set: return Color.FromArgb(0x00c400);
                case ItemQuality.Unique: return Color.FromArgb(0x908858);
                default: return Color.White;
            }
        }
    }

    /// <summary>
    /// Helper class for formulas.
    /// </summary>
    public static class FormulaHelper
    {
        private const int DROP_RATE_DIVISOR = 128;

        /// <summary>
        /// Calculates final drop rate for item quality.
        /// </summary>
        /// <param name="quality">Item quality</param>
        /// <param name="dropLevel">Drop level (area level)</param>
        /// <param name="itemLevel">Item level</param>
        /// <param name="bonusValue">Bonus value (in TreasureClass)</param>
        /// <param name="magicFind">Magic find (%)</param>
        /// <param name="uber">Is the item exceptional/elite</param>
        /// <param name="classSpecific">Is the item class-specific</param>
        /// <returns></returns>
        public static float CalcFinalDropRate(ItemQuality quality, int dropLevel, int itemLevel, 
            int bonusValue = 0, int magicFind = 0, bool uber = false, bool classSpecific = false)
        {
            // Base drop rate determined by ItemRatio.txt
            int chance = ItemRatio.Instance.CalcBaseDropBase(quality, dropLevel, itemLevel, out var min, 
                uber, classSpecific) * DROP_RATE_DIVISOR;

            if (chance <= 0) return 1f;

            // Apply MF
            chance = chance * 100 / (100 + GetEMF(magicFind, quality));

            // Minimum value
            if (min > 0) chance = Math.Max(chance, min);

            // Non-magical items don't have any bonus
            if (quality == ItemQuality.Normal || quality == ItemQuality.Superior || quality == ItemQuality.LowQuality)
            {
                return DROP_RATE_DIVISOR / chance;
            }

            // Bonus rate defined in TreasureClassEx.txt
            chance -= chance * bonusValue / 1024;

            return Math.Min((float)DROP_RATE_DIVISOR / chance, 1f);
        }

        // Get effective MF
        private static int GetEMF(int mf, ItemQuality quality)
        {
            if (mf < 10) return mf;
            switch (quality)
            {
                case ItemQuality.Unique:
                    return mf * 250 / (mf + 250);

                case ItemQuality.Set:
                    return mf * 500 / (mf + 500);

                case ItemQuality.Rare:
                    return mf * 600 / (mf + 600);

                default:
                    return mf;
            }
        }
    }

    /// <summary>
    /// Helper class for data.
    /// </summary>
    public static class DataHelper
    {
        /// <summary>
        /// Parse bool value.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static bool ParseBool(string value)
        {
            return value == "1";
        }

        /// <summary>
        /// Parse integer value.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default value when parsing fails</param>
        /// <returns></returns>
        public static int ParseInt(string value, int defaultValue = 0)
        {
            if (int.TryParse(value, out int result)) return result;
            return defaultValue;
        }

        /// <summary>
        /// Parse long value.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default value when parsing fails</param>
        /// <returns></returns>
        public static long ParseLong(string value, long defaultValue = 0L)
        {
            if (long.TryParse(value, out long result)) return result;
            return defaultValue;
        }

        /// <summary>
        /// Parse non-empty hash set.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="data">Raw data</param>
        /// <returns></returns>
        public static HashSet<T> ParseNonEmptyHashSet<T>(T[] data)
        {
            HashSet<T> result = new HashSet<T>();
            if (data == null || data.Length == 0) return result;
            foreach (var item in data)
            {
                if (item == null || string.IsNullOrEmpty(item.ToString())) continue;
                result.Add(item);
            }
            return result;
        }
    }

    /// <summary>
    /// Helper class for log.
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="logCallback">Log callback</param>
        /// <param name="message">Message</param>
        public static void Log(Action<string, LogLevel> logCallback, string message, LogLevel level = LogLevel.Log)
        {
            if (logCallback != null) logCallback(message, level);
        }
    }

    /// <summary>
    /// Log level.
    /// </summary>
    public enum LogLevel
    {
        Log,
        Warning,
        Error,
    }

    /// <summary>
    /// Item quality
    /// </summary>
    public enum ItemQuality
    {
        LowQuality,
        Normal,
        Superior,
        Magic,
        Rare,
        Set,
        Unique,
        Craft,
    }

    /// <summary>
    /// Difficulty
    /// </summary>
    public enum Difficulty
    {
        Normal,
        Nightmare,
        Hell,
    }

    /// <summary>
    /// Monster type
    /// </summary>
    public enum MonsterType
    {
        Regular,
        SuperUnique,
        ActBoss,
    }

    /// <summary>
    /// Player class
    /// </summary>
    public enum PlayerClass
    {
        Amazon,
        Sorceress,
        Necromancer,
        Paladin,
        Barbarian,
        Druid,
        Assassin,
    }
}
