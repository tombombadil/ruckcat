using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Ruckcat
{

public class HyperButtonDebug : HyperButton
{
    public override void Init()
    {
        base.Init();

        
            if (CoreGameCont.Instance.DebugMode > 0)
            {
                EventTouch.AddListener(onBtnDebugClicked);
                GetComponentInChildren<TextMeshProUGUI>().SetText(CoreGameCont.Instance.Version);
            }

            else
            {
                gameObject.SetActive(false);
            }
        



    }


    private void onBtnDebugClicked(TouchUI _touch)
    {
            ((HyperUICont)CoreUiCont.Instance).GetPageGame().OpenPopup<HyperPopupDebug>();
    }
}
}
