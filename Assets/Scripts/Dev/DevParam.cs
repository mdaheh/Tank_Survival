#if UNITY_EDITOR || DEBUG
using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TankSurvival.Dev
{
    public sealed class DevParam
    {
        private readonly object _target;
        private readonly FieldInfo _field;
        private readonly float _originalValue;

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

            _originalValue = ReadOriginalValue();
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

        public void Restore()
        {
            SetValue(_originalValue);
        }

#if UNITY_EDITOR
        private float ReadOriginalValue()
        {
            UnityEngine.Object unityTarget = _target as UnityEngine.Object;
            if (unityTarget != null)
            {
                SerializedObject so = new SerializedObject(unityTarget);
                SerializedProperty prop = so.FindProperty(_field.Name);
                if (prop != null)
                {
                    float value = prop.propertyType == SerializedPropertyType.Integer ? prop.intValue : prop.floatValue;
                    so.Dispose();
                    return value;
                }

                so.Dispose();
            }

            return Convert.ToSingle(_field.GetValue(_target));
        }
#else
        private float ReadOriginalValue()
        {
            return Convert.ToSingle(_field.GetValue(_target));
        }
#endif
    }
}
#endif
