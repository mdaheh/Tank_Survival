#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Reflection;
using UnityEngine;

namespace TankSurvival.Dev
{
    public sealed class DevParam
    {
        private readonly object _target;
        private readonly FieldInfo _field;

        public readonly string Group;
        public readonly string Label;
        public readonly bool IsInteger;
        public readonly float Min;
        public readonly float Max;

        public DevParam(object target, FieldInfo field, string group, string label, bool isInteger)
        {
            _target = target;
            _field = field;

            Group = group;
            Label = label;
            IsInteger = isInteger;

            float current = Convert.ToSingle(_field.GetValue(_target));
            float max = Mathf.Max(10f, current * 4f);

            Min = 0f;
            Max = isInteger ? Mathf.RoundToInt(max) : max;
        }

        public float GetValue()
        {
            return Convert.ToSingle(_field.GetValue(_target));
        }

        public void SetValue(float value)
        {
            if (IsInteger)
            {
                _field.SetValue(_target, Mathf.RoundToInt(value));
                return;
            }

            _field.SetValue(_target, value);
        }
    }
}
#endif
