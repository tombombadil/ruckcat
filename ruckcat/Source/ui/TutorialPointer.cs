using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ruckcat
{

public class TutorialPointer : CoreUI
{
    
    public override void Init()
    {
        base.Init();

        SetVisible(false);
        
    }
    public void SetVisible(bool isVisible)
    {
        // gameObject.SetActive(isVisible);
         this.GetComponentInChildren<Image>().enabled = isVisible;
    }
    public void SetPosition(Vector3 _3dWorldPos)
    {
        SetVisible(true);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_3dWorldPos);
         this.GetComponent<RectTransform>().position = screenPos;
    }

    
}
}