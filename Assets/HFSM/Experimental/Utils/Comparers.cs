using System;
using System.Collections.Generic;
using UnityEngine;

namespace HFSM.Experimental.Utils
{
    public static class FloatComparer
    {
        public enum Mode
        {
            LessThan,
            LessThanOrEqualTo,
            EqualTo,
            MoreThanOrEqualTo,
            MoreThan
        }
        
        private static Dictionary<Mode, Func<float, float, bool>> _comparers = new Dictionary<Mode, Func<float, float, bool>>
        {
            {Mode.LessThan, (a, b) => a < b},
            {Mode.LessThanOrEqualTo, (a, b) => a <= b},
            {Mode.EqualTo, Mathf.Approximately},
            {Mode.MoreThanOrEqualTo, (a, b) => a >= b},
            {Mode.MoreThan, (a, b) => a > b},
        };

        public static bool Compare(float a, float b, Mode mode)
        {
            return _comparers[mode].Invoke(a, b);
        }
    }
}