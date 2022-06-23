using System;
using System.Collections;
using System.Collections.Generic;
using Ruckcat;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;
using Touch = Ruckcat.Touch;


[System.Serializable, Toggle("Enabled")]
public class PickupRuleTouch
{
    public bool Enabled;
}

[System.Serializable, Toggle("Enabled")]
public class PickupRuleAuto
{
    public bool Enabled;
    public float Time = 0;
}


[RequireComponent(typeof(BoxCollider))]
public class CoinPickup : HyperSceneObj
{
    [TitleGroup("Gameplay"), PropertyOrder(0)] [Tooltip("toplam ne kadar para(score) kazandiracagi")]
    public IntField Score;

    [TitleGroup("Spawn"), PropertyOrder(0)] [Tooltip("Coin prefabi")]
    public GameObject Prefab;

    [TitleGroup("Spawn"), PropertyOrder(0)] [Tooltip("Toplam coin sayisi")]
    public int Amount;

    [TitleGroup("Spawn"), PropertyOrder(0)] [Tooltip("spawn sonrasi itemlarin + birim kadar yerden gelmesi")]
    public Vector3 DropOffset;

    [TitleGroup("Spawn"), PropertyOrder(0)] [Tooltip("drop tween time")]
    public float DropTweenTime = 0.4f;

    [TitleGroup("Spawn"), PropertyOrder(0)] [Tooltip("drop tween time, itemlar arasi interval(delay)")]
    public float DropTweenTimeInterval = 0.05f;

    [TitleGroup("Pickup"), PropertyOrder(0)] [Tooltip("target viewport position")]
    public Vector2 TargetViewport;

    [TitleGroup("Pickup"), PropertyOrder(0)] [Tooltip("target viewport position")]
    public Vector3 TargetRotation;

    [TitleGroup("Pickup"), PropertyOrder(0)] [Tooltip("tween time")]
    public float PickupTweenTime = 0.4f;

    [TitleGroup("Pickup"), PropertyOrder(0)] [Tooltip("tween delay min ve max. x:min, y:max")]
    public Vector2 TweenTimeIntervalRange = new Vector2(0, 0.05f);

    [TitleGroup("Pickup"), PropertyOrder(0)] [Tooltip("UI Target noktasina geldiginde scale degeri")]
    public Vector3 ScaleOnTarget = new Vector3(1, 1, 1);

    public PickupRuleTouch RuleTouch = new PickupRuleTouch();
    public PickupRuleAuto RuleAuto = new PickupRuleAuto();

    private BoxCollider spawnInBox; //bu collider icine rasgele posistionlarda spoawn edilecek
    protected bool isDropAnimCompleted;
    protected bool isPickAnimStarted;
    protected bool isPickAnimCompleted;


    private List<HyperSceneObj> Items = new List<HyperSceneObj>();


    public override void Init()
    {
        base.Init();
        spawnInBox = GetComponent<BoxCollider>();
    }

    public override void StartGame()
    {
        base.StartGame();
    }

    public override void OnTouchChild(Touch touch)
    {
        base.OnTouchChild(touch);

        if (RuleTouch.Enabled && isDropAnimCompleted)
        {
            CollectCoin();
        }
    }

    public virtual void Create()
    {
        if (Items.Count > 0) return;

        //Debug.Log("Coinler düştü");
        for (int i = 0; i < Amount; i++)
        {
            Vector3 randomPosInBox = Utils.GetRandomPositionInBoxCollider(spawnInBox);
            randomPosInBox.y = spawnInBox.transform.position.y;

            HyperSceneObj item = spawnItem(randomPosInBox);
            dropAnim(item, DropTweenTimeInterval * i, i);
        }
    }

    //dtum itemlarin drop anim bittiginde
    protected virtual void OnDropCompleted()
    {
        isDropAnimCompleted = true;
        if (RuleAuto.Enabled)
        {
            float interval = (DropTweenTimeInterval * Amount) + RuleAuto.Time;
            Invoke("CollectCoin", interval);
        }
    }

    //tum itemlarin pick anim bittiginde
    protected virtual void OnPickCompleted()
    {
        isPickAnimCompleted = true;
        if (Score.GetValue() > 0)
        {
            HyperLevelCont.Instance.SetLevelScore(Score.GetValue());
        }
    }

    private HyperSceneObj spawnItem(Vector3 pos)
    {
        HyperSceneObj item = CoreSceneCont.Instance.SpawnItem<HyperSceneObj>(Prefab, pos, this.transform, true);
        Items.Add(item);
        item.transform.position = pos;
        item.gameObject.SetActive(false);
        return item;
    }

    private void dropAnim(HyperSceneObj item, float delay, int index)
    {
        item.gameObject.SetActive(true);
        Vector3 fromPos = item.transform.position + DropOffset;
        Vector3 toPos = item.transform.position;
        item.transform.position = fromPos;
        LeanTween.move(item.gameObject, toPos, DropTweenTime).setEase(LeanTweenType.easeInOutExpo).setDelay(delay)
            .setOnComplete(
                () =>
                {
                    if (index == Items.Count - 1)
                        OnDropCompleted();
                });

        Vector3 rotFrom =
            Utils.GetRandomPositionInVector(new Vector3[] {new Vector3(-90, -90, -90), new Vector3(90, 90, 90)});
        Vector3 rotTo = Vector3.zero;
        item.gameObject.transform.eulerAngles = rotFrom;
        LeanTween.rotate(item.gameObject, rotTo, DropTweenTime).setEase(LeanTweenType.easeInOutExpo).setDelay(delay);
    }


    public void CollectCoin()
    {
        if (!isPickAnimStarted)
        {
            StopAllCoroutines();
            for (int i = 0; i < Items.Count; i++)
            {
                HyperSceneObj item = Items[i];
                moveItemToTarget(item, i * Random.Range(TweenTimeIntervalRange.x, TweenTimeIntervalRange.y), i);
            }

            isPickAnimStarted = true;
            StartCoroutine(PickReset(TweenTimeIntervalRange.y));
        }
    }

    IEnumerator PickReset(float _time)
    {
        yield return new WaitForSeconds(_time);
        isPickAnimStarted = false;
    }

    private void moveItemToTarget(HyperSceneObj item, float delay, int index)
    {
        // if (UiTarget)
        {
            LeanTween.cancel(item.gameObject);
            // LeanTween.rotateX(item.gameObject, UiTarget.eulerAngles.z, 0.5f);
            LeanTween.rotate(item.gameObject, TargetRotation, PickupTweenTime);
            LeanTween.move(item.gameObject, getWorldPositionOnPlane(TargetViewport, 1), PickupTweenTime)
                .setEase(LeanTweenType.easeOutSine).setDelay(delay);
            LeanTween.scale(item.gameObject, ScaleOnTarget, PickupTweenTime).setEase(LeanTweenType.easeOutSine)
                .setDelay(delay).setOnComplete(
                    () =>
                    {
                        /*if ((index == Items.Count - 1))
                        {
                            OnPickCompleted();
                        }*/
                        OnPickCompleted();
                        Items.Remove(item);
                        Destroy(item.gameObject);
                    });
        }
    }


    private Vector3 getWorldPositionOnPlane(Vector3 viewport, float z)
    {
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3(viewport.x, viewport.y, z));
        return p;
    }


    // public void DestroyCoins()
    // {    
    //     foreach (var item in AllCoins)
    //     {
    //         item.DestroyThisCoin();
    //     }
    //     AllCoins.Clear();
    // }
}