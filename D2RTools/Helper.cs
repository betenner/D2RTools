using System;
using System.Collections.Generic;
using System.Text;

namespace D2RTools
{
    /// <summary>
    /// Helper class
    /// </summary>
    public static class Helper
    {
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
            else if (falseAction != null) falseAction();
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
        public static bool Assert<T>(bool condition, Action<T> trueAction = null, T trueParam = default(T), Action<T> falseAction = null, T falseParam = default(T))
        {
            if (condition && trueAction != null) trueAction(trueParam);
            else if (falseAction != null) falseAction(falseParam);
            return condition;
        }
    }
}
