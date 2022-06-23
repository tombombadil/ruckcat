using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ruckcat
{


public class HyperButton : CoreUI
{
    public void Show()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform rect = (RectTransform)transform.GetChild(i).transform;
            rect.gameObject.SetActive(true);
        }
    }

    /* @IsDisableTouch : false ise obje hide olsa bile touch target calismaya devam eder */
    public void Hide(bool IsDisableTouch=true)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform rect = (RectTransform) transform.GetChild(i).transform;
            rect.gameObject.SetActive(false);

            

            if (!IsDisableTouch)
            {
                Graphic graphic = transform.GetChild(i).GetComponent<Graphic>();
                if(graphic)
                {
                    if(graphic == TouchTarget)
                        rect.gameObject.SetActive(true);
                }
            }
            
        }
    }
    
    public void SetEnableTouch(bool isEnable)
    {
        if (TouchTarget)
        {
            //TouchTarget.gameObject.SetActive(isEnable);
            TouchTarget.raycastTarget = isEnable;
        }
            
        // foreach (Graphic g in GetComponentsInChildren<Graphic>())
        // {
        //     if (g.gameObject == TouchTarget.gameObject)
        //     {
        //         g.gameObject.gameObject.SetActive(isEnable);
        //     }
        //        
        // }
        
    }
}
}