using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ruckcat
{
    [Serializable]
    public class GameObjectField : BaseField
    {
        [BoxGroup("NoTitle", false)] [AssetSelector]
        public GameObject value;

        public GameObject GetValue()
        {
            return value;
        }
    }
}