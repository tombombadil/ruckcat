using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Ruckcat
{

    public class HyperPopupSettings : Popup
    {
        public CoreUI BtnVibration;
        public GameObject ImageDisableVibration;
        public override void Init()
        {
            base.Init();

            if (BtnVibration)
            {
                BtnVibration.EventTouch.AddListener(onClickBtnVibration);
                setVibration(HyperConfig.Instance.IsVibration);
            }

        }

        private void onClickBtnVibration(TouchUI info)
        {
            setVibration(!HyperConfig.Instance.IsVibration);
        }
        private void setVibration(bool isEnable)
        {
            HyperConfig.Instance.IsVibration = isEnable;
            if (ImageDisableVibration)
                ImageDisableVibration.SetActive(!isEnable);
        }
   
      
    }
}