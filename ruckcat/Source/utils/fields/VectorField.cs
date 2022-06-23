using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Ruckcat
{
    [Serializable]
    public class VectorField : BaseField
    {
        [HorizontalGroup("1")] [HideLabel] public Vector3 defaultValue;

        [HorizontalGroup("1")]
        [BoxGroup("1/NoTitle", false)] [HideLabel]  public VectorValueType ValueType = VectorValueType.Constant;

        [ShowIfGroup("1/NoTitle/MinMax/ValueType", Value = VectorValueType.Random)] [BoxGroup("1/NoTitle/MinMax", false)]
        public Vector3 minValue;

        [ShowIfGroup("1/NoTitle/MinMax/ValueType", Value = VectorValueType.Random)] [BoxGroup("1/NoTitle/MinMax", false)]
        public Vector3 maxValue;

        [ShowIfGroup("1/NoTitle/LevelMultiple/ValueType", Value = VectorValueType.LevelMultiple)]
        [BoxGroup("1/NoTitle/LevelMultiple", false)]
        public float multipleFactor;

        [ShowIfGroup("1/NoTitle/LevelAddition/ValueType", Value = VectorValueType.LevelAddition)]
        [BoxGroup("1/NoTitle/LevelAddition", false)]
        public float additionFactor;

        public Vector3 GetValue(float time = 1)
        {
            int levelNumber =  GetLevel();
            switch (ValueType)
            {
                case VectorValueType.Constant:
                    return defaultValue;
                case VectorValueType.Random:
                    return RandomMinMaxValue();
                case VectorValueType.LevelMultiple:
                    return levelNumber == 1 ? defaultValue : (levelNumber - 1) * multipleFactor * defaultValue;
                case VectorValueType.LevelAddition:
                    return levelNumber == 1
                        ? defaultValue
                        : (Vector3.one * (levelNumber - 1) * additionFactor) + defaultValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Vector3 RandomMinMaxValue()
        {
            float x = Random.Range(minValue.x, maxValue.x);
            float y = Random.Range(minValue.y, maxValue.y);
            float z = Random.Range(minValue.z, maxValue.z);

            return new Vector3(x, y, z);
        }
    }
}