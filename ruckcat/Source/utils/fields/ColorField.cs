using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ruckcat 
{
    [Serializable]
    public class ColorField : BaseField
    {
        [BoxGroup("NoTitle", false)] [ColorPalette]
        public Color value;

        public Color GetValue()
        {
            return value;
        }
    }
}