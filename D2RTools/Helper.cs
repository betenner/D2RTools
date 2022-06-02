using System;
using System.Collections.Generic;
using System.Text;

namespace D2RTools
{
    /// <summary>
    /// Character attribute
    /// </summary>
    public enum CharAttr
    {
        Strength,
        Energy,
        Dexterity,
        Vitality,
        StatsLeft,
        SkillLeft,
        Life,
        MaxLife,
        Mana,
        MaxMana,
        Stamina,
        MaxStamina,
        Level,
        Experience,
        Gold,
        StashGold,
    }

    /// <summary>
    /// Helper class
    /// </summary>
    public static class Helper
    {
        private static readonly Dictionary<string, CharAttr> _str2AttrMap;
        private static readonly Dictionary<CharAttr, string> _attr2StrMap;

        static Helper()
        {
            _str2AttrMap = new Dictionary<string, CharAttr>()
            {
                { "strength", CharAttr.Strength },
                { "energy", CharAttr.Energy },
                { "dexterity", CharAttr.Dexterity },
                { "vitality", CharAttr.Vitality },
                { "statpts", CharAttr.StatsLeft },
                { "newskills", CharAttr.SkillLeft },
                { "hitpoints", CharAttr.Life },
                { "maxhp", CharAttr.MaxLife },
                { "mana", CharAttr.Mana },
                { "maxmana", CharAttr.MaxMana },
                { "stamina", CharAttr.Stamina },
                { "maxstamina", CharAttr.MaxStamina },
                { "level", CharAttr.Level },
                { "experience", CharAttr.Experience },
                { "gold", CharAttr.Gold },
                { "goldbank", CharAttr.StashGold },
            };

            _attr2StrMap = new Dictionary<CharAttr, string>();
            foreach (var key in _str2AttrMap.Keys)
            {
                _attr2StrMap.Add(_str2AttrMap[key], key);
            }
        }

        /// <summary>
        /// Converts character attribute to inner string.
        /// </summary>
        /// <param name="attr">Character attribute</param>
        /// <returns></returns>
        public static string CharAttr2Str(CharAttr attr)
        {
            if (_attr2StrMap.TryGetValue(attr, out string str)) return str;
            return null;
        }

        /// <summary>
        /// Converts inner string to character attribute.
        /// </summary>
        /// <param name="str">Inner string</param>
        /// <returns></returns>
        public static CharAttr? Str2CharAttr(string str)
        {
            if (_str2AttrMap.TryGetValue(str, out CharAttr attr)) return attr;
            return null;
        }

        /// <summary>
        /// Asserts specified condition then do certain action.
        /// </summary>
        /// <param name="condition">Condition to assert</param>
        /// <param name="trueAction">Action to do if condition is true</param>
        /// <param name="falseAction">Action to do if condition is false</param>
        /// <returns></returns>
        public static bool Assert(bool condition, Action trueAction = null, Action falseAction = null)
        {
            if (condition && trueAction != null) trueAction();
            else falseAction?.Invoke();
            return condition;
        }

        /// <summary>
        /// Asserts specified condition then do certain action.
        /// </summary>
        /// <typeparam name="T">Type of action param</typeparam>
        /// <param name="condition">Condition to assert</param>
        /// <param name="trueAction">Action to do if condition is true</param>
        /// <param name="trueParam">Param for true action</param>
        /// <param name="falseAction">Action to do if condition is false</param>
        /// <param name="falseParam">Param for false action</param>
        /// <returns></returns>
        public static bool Assert<T>(bool condition, Action<T> trueAction = null, T trueParam = default, Action<T> falseAction = null, T falseParam = default)
        {
            if (condition && trueAction != null) trueAction(trueParam);
            else falseAction?.Invoke(falseParam);
            return condition;
        }
    }
}
