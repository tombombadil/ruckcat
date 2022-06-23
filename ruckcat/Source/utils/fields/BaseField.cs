namespace Ruckcat
{
    public abstract class BaseField
    {
        public virtual int GetLevel()
        {
            return HyperLevelCont.Instance.CurrLevel;
        }
    }
    
    public enum VectorValueType
    {
        Constant,
        Random,
        LevelMultiple,
        LevelAddition
    }

    public enum IntFloatValueType
    {
        Curve,
        Constant,
        Random,
        LevelMultiple,
        LevelAddition,
        LevelMultipleX,
        LevelAdditionX,
    }
}