using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Ruckcat
{

public class HyperTutorial : CoreCont<HyperTutorial>
{

    [System.Serializable, Toggle("Enabled")]
    public class FollowObject
    {
        public bool Enabled;
        [Tooltip("Gameobject bir HyperSceneObj olmak zorunda")]  public string GameObjectName;

        [Tooltip("hand cursor ve object arasindaki  offset")]
        public Vector3 Offset;
        [HideInInspector] public GameObject go;
    }
    
    [System.Serializable]
    public class AnimElement
    {
        public FollowObject FollowSceneObject = new FollowObject();
        
        public Vector3 StartViewportLocation;
        public Vector3 EndViewportLocation;
        public float AnimTime = 1;

        public float DelayLoopTime = 0.2f; //repeaticin kac sn bekleyecek?
        public bool IsPingPong = false;
        public int Loop = 0; //0(sonsuz) 

        private int loopCounter = 0;

    }
    [System.Serializable]
    public class StepElement
    {
        public string Id;
        public List<AnimElement> Anims = new List<AnimElement>();
        [Tooltip("opsiyonel olarak farkli bir cursor kullanmak icin, pagegame icindeki cursor referansi. bu deger nullise default olan kullanilir (CursorPrefab)")]
        public Transform UseAnotherCursor;
        [HideInInspector] public bool isStarted;
        public float TimeScale = -1f; //anim zamaninda time.scale degeri, -1 oldugundan timescale degeri degistirilmez
    }

    public List<StepElement> Steps = new List<StepElement>();
    [HideInInspector] public UnityEvent EventStepStarted = new UnityEvent();
    [HideInInspector] public UnityEvent EventStepCompleted = new UnityEvent();
    [HideInInspector] public UnityEvent EventAnimStarted = new UnityEvent();
    [HideInInspector] public UnityEvent EvnetAnimCompleted = new UnityEvent();

    public Transform UICursor;
    [HideInInspector] protected int currStep = -1;
    [HideInInspector] public int currStepAnim;
    [HideInInspector] public GameObject cursor;

    public override void Init()
    {
        base.Init();
        
        currStep = -1;
        // cursor = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity);
        cursor = UICursor.gameObject;
        cursor.transform.parent = CoreUiCont.Instance.transform;
        cursor.transform.localScale = Vector3.one;
        cursor.SetActive(false);

        foreach (StepElement step in Steps)
        {
            foreach (AnimElement anim in step.Anims)
            {
                if (anim.FollowSceneObject.Enabled)
                {
                    GameObject go = getObjectByName(anim.FollowSceneObject.GameObjectName);
                    if (go)
                    {
                        anim.FollowSceneObject.go = go;
                      
                        
                    }
                }
            }
        }


       setActiveCursor(false);
       enabled = true;
    }

    public override void StartGame()
    {
        base.StartGame();
        if(currStep > -1)
        Stop();

    }

    public override void Update()
    {
        base.Update();

        if (GetCurrentStep() != null)
        {
            if (GetCurrentStep().isStarted)
            {
                AnimElement currAnim = Steps[currStep].Anims[currStepAnim];
                if (currAnim.FollowSceneObject.go)
                {
                    Vector3 worldPos = currAnim.FollowSceneObject.go.transform.position +
                                       currAnim.FollowSceneObject.Offset;
                    Vector3 posScreen = Camera.main.WorldToScreenPoint(worldPos);
                    cursor.transform.position = posScreen;
                }
            }
          
           
        }
    }

    /*--------------------------------------------------------------------------------------------*/
    /*-----------------------------------------| PUBLIC |-----------------------------------------*/
    /*--------------------------------------------------------------------------------------------*/

    /* Play ilgili step indexesahip step'i baslatir */
    public virtual void Play(int _step)
    {

        if (_step < Steps.Count)
        {
            currStep = _step;

            cursor = (Steps[currStep].UseAnotherCursor!=null)? Steps[currStep].UseAnotherCursor.gameObject: UICursor.gameObject ;
            setActiveCursor(true);
           if( Steps[currStep].TimeScale > -1) Time.timeScale = Steps[currStep].TimeScale;
            playStep(_step);
        }

    }
    /* I'ye gore ilgili step'i baslsatir */
     public void PlayById(string _id)
    {

       int index = GetStepIndexById(_id);
       if(index > -1)
       {
           Play(index);
       }

    }
    public virtual void Stop()
    {
        if (Time.timeScale != 1)
        {
            LeanTween.value(gameObject, updateValueExampleCallback, Time.timeScale, 1, 0.5f).setEase(LeanTweenType.linear);
            void updateValueExampleCallback(float val)
            {
                Time.timeScale = val;
            }
        }
        if (currStep < Steps.Count - 1)
        {
            Steps[currStep].isStarted = false;
        }
        LeanTween.cancel(cursor);
        setActiveCursor(false);
    }

    public void SavePositions(int _step, int _animIndex, Vector3 _startPos, Vector3 _endPos, float _time = 1, float _delayLoop = 0.2f)
    {
        AnimElement elem = new AnimElement();
        elem.AnimTime = _time;
        elem.StartViewportLocation = _startPos;
        elem.EndViewportLocation = _endPos;
        elem.DelayLoopTime = _delayLoop;

        StepElement step = new StepElement();
        if (_animIndex >= Steps.Count)
        {
            step.Anims.Add(elem);
        }
        else
        {
            step.Anims[_animIndex] = elem;
        }


        if (_step >= Steps.Count)
        {
            Steps.Add(step);
        }
        else
        {
            Steps[_step] = step;
        }

    }



    public int GetStepIndexById(string Id)
    {
        int index = -1;
        StepElement elem = Steps.Find((x) => x.Id == Id);
        if(elem != null)
        index = Steps.IndexOf(elem);
        return index;
    }

    public StepElement GetCurrentStep()
    {
        StepElement s = null;
        if(currStep > -1)
            s = Steps[currStep];
        return s;
    }


    /*-----------------------------------------| private |-----------------------------------------*/
    private void playStep(int _step)
    {

        if (_step < Steps.Count)
        {
            Steps[_step].isStarted = true;
            //   moveTo(Steps[_step]);
            if (Steps[_step].Anims.Count > 0)
            {
                playStepAnim(0);
                EventStepStarted.Invoke();
            }

        }

    }

    private void playStepAnim(int _animIndex)
    {
        if (Steps[currStep].Anims.Count > 0 && _animIndex < Steps[currStep].Anims.Count)
        {
            currStepAnim = _animIndex;
            moveTo(Steps[currStep].Anims[_animIndex]);
            EventAnimStarted.Invoke();
        }

    }

    private void moveTo(AnimElement _element)
    {
        LeanTween.cancel(cursor, false);

        if (!_element.FollowSceneObject.go)
        {
            cursor.transform.position = Camera.main.ViewportToScreenPoint(_element.StartViewportLocation);


            Vector3 toScreenPoint = Camera.main.ViewportToScreenPoint(_element.EndViewportLocation);

            LTDescr tw = LeanTween.move(cursor, toScreenPoint, _element.AnimTime).setIgnoreTimeScale(true).setOnComplete(moveToCompleted).setOnUpdate(onUpdate);

            if (_element.IsPingPong) tw.setLoopPingPong(_element.Loop);
            else tw.setLoopCount(_element.Loop);
        }
        

        

    }

    protected virtual void onUpdate(float value)
    {

    }

    private void moveToCompleted()
    {
        AnimElement currAnim = Steps[currStep].Anims[currStepAnim];

        float delay = currAnim.DelayLoopTime * Time.timeScale;
        if (Steps[currStep].isStarted) StartCoroutine(moveToCompletedDelay(delay));

    }

    private IEnumerator moveToCompletedDelay(float time)
    {
        yield return new WaitForSeconds(time);
        if (currStepAnim < Steps[currStep].Anims.Count - 1)
        {
         
            playStepAnim(currStepAnim + 1);
            EvnetAnimCompleted.Invoke();
        }
        else
        {
            EventStepCompleted.Invoke();
            playStepAnim(0);

        }

    }


    private void setActiveCursor(bool isActive)
    {
        UICursor.gameObject.SetActive(false);
         foreach (StepElement step in Steps)
        {
            if(step.UseAnotherCursor != null) step.UseAnotherCursor.gameObject.SetActive(false);
        }

        if(cursor)
        cursor.SetActive(isActive);
        

        
    }

    private GameObject getObjectByName(string _name)
    {
        GameObject go = null;
        MonoBehaviour[] all = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour mono in all)
        {
            if (mono.name == _name)
                go = mono.gameObject;

        }

        return go;
    }
}
}