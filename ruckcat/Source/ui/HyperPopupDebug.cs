using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


namespace Ruckcat
{


public class HyperPopupDebug : Popup
{
    //public TextMeshProUGUI TxtLevel;
    public TMP_InputField TxtLevel;
    public CoreUI BtnLevel;
    public override void Init()
    {
        base.Init();

        if(BtnLevel)
        {
            BtnLevel.EventTouch.AddListener(onClickBtnLevel);
            TxtLevel.text =  HyperLevelCont.Instance.CurrLevel.ToString();
        }
       

    }

    private void onClickBtnLevel(TouchUI info)
    {
        int level;
        
        if (int.TryParse(TxtLevel.text.ToString(), out level))
        {
            Debug.Log("int.parse.ok " +level);
            setLevel(level);

        }


    }
    private void setLevel(int level)
    {
        HyperLevelCont.Instance.SetLevel(level);
    }
}

}