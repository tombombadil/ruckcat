using System.Collections;
using System.Collections.Generic;
using Ruckcat;
using UnityEngine;
using UnityEngine.UI;

namespace Ruckcat
{
public class Emoji : CoreUI
{
    public Texture[] ListImages;
    private RawImage image;

    public override void Init()
    {
        base.Init();

        image = GetComponent<RawImage>();
    }

    public void ShowRandom()
    {
        if (ListImages.Length > 0)
        {
            int index = Random.Range(0, ListImages.Length - 1);
            showImage(index);
        }
    }

    private void showImage(int _index, float _animTime = 1f)
    {

        image.texture = ListImages[_index];
        gameObject.SetActive(true);
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), _animTime).setEase(LeanTweenType.easeOutElastic);

    }
}
}

