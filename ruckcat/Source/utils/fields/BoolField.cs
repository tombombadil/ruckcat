using System;
using Sirenix.OdinInspector;

namespace Ruckcat
{
    [Serializable]
    public class BoolField : BaseField
    {
        [BoxGroup("NoTitle", false)]
        public bool value;

        public bool GetValue()
        {
            return value;
        }
    }
}