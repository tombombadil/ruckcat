using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ruckcat
{


public class MobileVibration : CoreObject
{
    public enum Level { LOW, NORMAL, HIGH };
    static public void Vibrate(Level _level, bool _removePrevious=false)
    {
        if (!HyperConfig.Instance.IsVibration) return;

        if (_removePrevious)
            Vibration.Cancel();


       if (Application.platform == RuntimePlatform.Android)
        {
            float vibValue = 0;
            if (_level == Level.LOW)
                vibValue = 100;
            if (_level == Level.NORMAL)
                vibValue = 400;
            if (_level == Level.HIGH)
                vibValue = 700;
            Vibration.Vibrate((long) vibValue);
        }
       if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (_level == Level.LOW)
                Vibration.VibratePop();
            if (_level == Level.NORMAL)
                Vibration.VibratePop();
            if (_level == Level.HIGH)
                Vibration.VibratePeek();
        }
       
        

    }

    static public void VibrateCustomAndroid(float _value, bool _removePrevious=false)
    {
        if (!HyperConfig.Instance.IsVibration) return;

        if (_removePrevious)
            Vibration.Cancel();


       if (Application.platform == RuntimePlatform.Android)
        {
           
            Vibration.Vibrate((long) _value);
        }
      
       
        

    }
}
}