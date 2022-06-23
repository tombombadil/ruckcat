using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ruckcat
{
    public class CoreCont<T> : CoreObject where T : CoreObject
    {
        private static T _instance;

        public  static T Instance
        {
            get
            {

                if (_instance == null)
                    _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null)
                    Debug.LogWarning("[Singleton] Instance not found!");

                return _instance;

            }
        }

        public static C Cast<C>() where C : T
        {
               if (_instance == null)
                    _instance = (T)FindObjectOfType(typeof(T));

                return (C) _instance;
            
            
        }

        public override void Init()
        {
            if (!IsInited)
            {
                base.Init();

                if(CoreInputCont.Instance)
                {
                    CoreInputCont.Instance.EventTouch.AddListener(OnTouch);
                    CoreInputCont.Instance.EventSwipe.AddListener(OnTouch);
                }
            }
           
            

            
        }

        public virtual void OnTouch(Touch touch)
        {
        }


    }
}
 