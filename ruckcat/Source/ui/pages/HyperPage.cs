using Shapes2D;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Ruckcat
{


public class HyperPage : PageUI
{
    public override void Init()
    {
        base.Init();
    }

    public override void StartGame()
    {
        base.StartGame();

        foreach (CoreUI child in transform.GetComponentsInChildren<CoreUI>())
        {
            if (child != this)
            {
                child.EventTouch.AddListener(OnTouchChild);
            }
        }
    }


   
    public override void Update()
    {
        base.Update();
    }



    public override void OnTouch(Ruckcat.TouchUI touch)
    {
        base.OnTouch(touch);

    }
     
    public virtual void OnTouchChild(Ruckcat.TouchUI touch)
    {
       
    }





    public bool SetBarScale(string _barName, float _ratio)
    {

        bool isFoundObject = false;

        RectTransform rect = GetComponentByName<RectTransform>(_barName);
        if (rect)
        {
            isFoundObject = true;


                LeanTween.cancel(rect.gameObject.transform.GetChild(1).gameObject);
                LeanTween.scaleX(rect.gameObject.transform.GetChild(1).gameObject, _ratio, 0.2f).setEase(LeanTweenType.easeOutQuad).setDelay(0f);


        }

        return isFoundObject;
        
    }


    public bool SetText(string textMeshName, string value)
    {
        bool isFoundObject = false;

        TextMeshProUGUI textMesh = GetComponentByName<TextMeshProUGUI>(textMeshName);
        if (textMesh)
        {
            textMesh.SetText(value);
            isFoundObject = true;
        }

        return isFoundObject;
    }
    public TextMeshProUGUI GetTextfield(string textMeshName)
    {

        TextMeshProUGUI textMesh = GetComponentByName<TextMeshProUGUI>(textMeshName);
        
        return textMesh;
    }



    public T GetComponentByName<T>(string name) where T : Object
    {
        T result = null;
        foreach (T item in transform.GetComponentsInChildren<T>(true))
        {
            if (item != this)
            {
                if (item.name.ToLower() == name.ToLower()) result = item;
            }
                
               
                   
        }
        
        
        return result;
    }






}
}