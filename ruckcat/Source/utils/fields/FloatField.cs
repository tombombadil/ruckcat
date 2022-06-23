using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Ruckcat
{
    [Serializable]
    public class FloatField : BaseField
    {
        [HorizontalGroup("1")] [HideLabel]
        public float defaultValue;

        [HorizontalGroup("1")]
        [BoxGroup("1/NoTitle", false)] [HideLabel] public IntFloatValueType ValueType = IntFloatValueType.Constant;

        [ShowIfGroup("1/NoTitle/Curve/ValueType", Value = IntFloatValueType.Curve)]
        [BoxGroup("1/NoTitle/Curve", false)]
        public AnimationCurve animationCurve = AnimationCurve.Constant(0, 1, 0);

        [FormerlySerializedAs("minValue")]
        [ShowIfGroup("1/NoTitle/MinMax/ValueType", Value = IntFloatValueType.Random)]
        [BoxGroup("1/NoTitle/MinMax", false)]
        public float randomMin;

        [FormerlySerializedAs("maxValue")]
        [ShowIfGroup("1/NoTitle/MinMax/ValueType", Value = IntFloatValueType.Random)]
        [BoxGroup("1/NoTitle/MinMax", false)]
        public float randomMax;

        [ShowIfGroup("1/NoTitle/LevelMultiple/ValueType", Value = IntFloatValueType.LevelMultiple)]
        [BoxGroup("1/NoTitle/LevelMultiple", false)]
        public float multipleFactor;

        [ShowIfGroup("1/NoTitle/LevelAddition/ValueType", Value = IntFloatValueType.LevelAddition)]
        [BoxGroup("1/NoTitle/LevelAddition", false)]
        public float additionFactor;
        
        
        [ShowIfGroup("1/NoTitle/LevelMultipleX/ValueType", Value = IntFloatValueType.LevelMultipleX)]
        [BoxGroup("1/NoTitle/LevelMultipleX", false), Tooltip("LevelMultiple'den farkli olarakher zaman katlayarak ilerler")]
        public float multipleFactorX = 1;

        [ShowIfGroup("1/NoTitle/LevelAdditionX/ValueType", Value = IntFloatValueType.LevelAdditionX)]
        [BoxGroup("1/NoTitle/LevelAdditionX", false),Tooltip("LevelMultiple'den farkli olarakher zaman katlayarak ilerler")]
        public float additionFactorX = 0;
        
        
        [BoxGroup("1/NoTitle/Limit", false)]
        public float min = 0;
        [BoxGroup("1/NoTitle/Limit", false)]
        public float max = 0;


        public void SetValue(float _value)
        {
            defaultValue = _value;
        }

        public void AppendValue(float _append)
        {
            SetValue(GetValue() + _append);
        }


        public float GetValue(float time = 1)
        {
            int levelNumber = GetLevel();
            float value = defaultValue;
            switch (ValueType)
            {
                case IntFloatValueType.Curve:
                    value = animationCurve.Evaluate(time);
                    break;
                case IntFloatValueType.Constant:
                    value =  defaultValue; 
                    break;
                case IntFloatValueType.Random:
                    value =  RandomMinMaxValue(); 
                    break;
                case IntFloatValueType.LevelMultiple:
                    value =  levelNumber == 1 ? defaultValue : (levelNumber - 1) * multipleFactor * defaultValue; 
                    break;
                case IntFloatValueType.LevelAddition:
                    value =  levelNumber == 1 ? defaultValue : (levelNumber - 1) * additionFactor + defaultValue; 
                    break;
                case IntFloatValueType.LevelMultipleX:
                    value = defaultValue;
                    for (int i = 0; i < (levelNumber - 1); i++)
                    {
                        value *= multipleFactorX ; 
                    }
                    break;
                case IntFloatValueType.LevelAdditionX:
                    value = defaultValue;
                    for (int i = 0; i < (levelNumber - 1); i++)
                    {
                        value += multipleFactorX ; 
                    }
                    break;
                
            
            }
            if (max > min)
            {
                value = Mathf.Clamp(value, min, max);
            }
            
          

            return value;
        }

        private float RandomMinMaxValue()
        {
            return Random.Range(randomMin, randomMax);
        }
    }
}