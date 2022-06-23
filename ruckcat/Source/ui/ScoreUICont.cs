using System;
using System.Collections;
using System.Collections.Generic;
using Ruckcat;
using TMPro;
using UnityEngine;

namespace Ruckcat
{


public class ScoreUICont : CoreUI
{
    [Header("New")] public FloatField time;
    public LeanTweenType easeType;
    public TextMeshProUGUI coinText;

    public RectTransform targetTransform;
    public GameObject coinPrefab;
    
    public void PlayCoinAnimation(Transform coinObject)
    {
        Vector3 uiPosition = Camera.main.WorldToScreenPoint(coinObject.position);
        GameObject createdObject = Instantiate(coinPrefab, uiPosition, Quaternion.identity, transform);
        Vector3 targetPosition = (targetTransform.position); // Camera.main.ScreenToViewportPoint

        LeanTween.move(createdObject, targetPosition, time.GetValue()).setEase(easeType).setOnComplete(o =>
        {
            Destroy(createdObject.gameObject);

            if (targetTransform.localScale == Vector3.one)
            {
                LeanTween.scale(targetTransform, new Vector3(1.75f, 1.75f, 1.75f), .2f).setLoopPingPong(1);
            }
        });
    }

    public void UpdateCoinUI(int coinCount)
    {
        coinText.text = coinCount.ToString();
    }

    // public void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         PlayCoinAnimation(testObject, 1);
    //     }
    // }
}
}
