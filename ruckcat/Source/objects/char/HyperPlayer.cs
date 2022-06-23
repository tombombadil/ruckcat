using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ruckcat
{
    
    public class HyperPlayer : HyperMovement
    {
        private static HyperPlayer _instance;

        public  static HyperPlayer Instance
        {
            get
            {

                if (_instance == null)
                    _instance = (HyperPlayer)FindObjectOfType(typeof(HyperPlayer));

                if (_instance == null)
                    Debug.LogWarning("[Singleton] HyperPlayer not found!");

                return _instance;

            }
        }

        
    }

}