using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ruckcat
{
    [Serializable]
    public class IntField : BaseField
    {
        [HorizontalGroup("1")] [HideLabel]
        public int defaultValue;

         [HorizontalGroup("1")]
        [BoxGroup("1/NoTitle", false)] [HideLabel]  public IntFloatValueType ValueType = IntFloatValueType.Constant;

        [ShowIfGroup("1/NoTitle/Curve/ValueType", Value = IntFloatValueType.Curve)] [BoxGroup("1/NoTitle/Curve", false)]
        public AnimationCurve animationCurve = AnimationCurve.Constant(0, 1, 0);

        [ShowIfGroup("1/NoTitle/MinMax/ValueType", Value = IntFloatValueType.Random)] [BoxGroup("1/NoTitle/MinMax", false)]
        public int minValue;

        [ShowIfGroup("1/NoTitle/MinMax/ValueType", Value = IntFloatValueType.Random)] [BoxGroup("1/NoTitle/MinMax", false)]
        public int maxValue;

        [ShowIfGroup("1/NoTitle/LevelMultiple/ValueType", Value = IntFloatValueType.LevelMultiple)]
        [BoxGroup("1/NoTitle/LevelMultiple", false)]
        public int multipleFactor;

        [ShowIfGroup("1/NoTitle/LevelAddition/ValueType", Value = IntFloatValueType.LevelAddition)]
        [BoxGroup("1/NoTitle/LevelAddition", false)]
        public int additionFactor;
        
        [ShowIfGroup("1/NoTitle/LevelMultipleX/ValueType", Value = IntFloatValueType.LevelMultipleX)]
        [BoxGroup("1/NoTitle/LevelMultipleX", false), Tooltip("LevelMultiple'den farkli olarakher zaman katlayarak ilerler")]
        public int multipleFactorX = 1;

        [ShowIfGroup("1/NoTitle/LevelAdditionX/ValueType", Value = IntFloatValueType.LevelAdditionX)]
        [BoxGroup("1/NoTitle/LevelAdditionX", false),Tooltip("LevelMultiple'den farkli olarakher zaman katlayarak ilerler")]
        public int additionFactorX = 0;


        public int GetValue(float time = 1)
        {
            int levelNumber =  GetLevel();
            int value = defaultValue;
            switch (ValueType)
            {
                case IntFloatValueType.Curve:
                    value = (int) animationCurve.Evaluate(time);
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
          

            return value;
        }

        private int RandomMinMaxValue()
        {
            return Random.Range(minValue, maxValue);
        }
    }
}