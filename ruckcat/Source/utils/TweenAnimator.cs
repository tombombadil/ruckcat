using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

/* 
Linear loop animasyonlar vermek icin, ya da basit animasyonlar yaratmak icin her seferinde prefaba ozel class yaratmak yerine bu class kullanilabilir 
*/


namespace Ruckcat
{

    public class TweenAnimEventInfo
    {
        public int StepIndex;
        public int AnimIndex;
        public float AnimRatio; // 0 - 1 arasinda onUpdate degeri

        public TweenAnimEventInfo(int stepIndex, int animIndex) { StepIndex = stepIndex; AnimIndex = animIndex; }

        // public override string ToString() { return "@Target: " + Target.name + ",@Order: " + Order; }
    }

    public class TweenEventStart : UnityEvent<TweenAnimEventInfo> { };


    [System.Serializable, Toggle("Enabled")]
    public class AnimVector
    {
        [Tooltip("true oldugunda transformun su anki degerinden > Value degerine gecis animasyonu yapilir. false oldugunda Value'dan su an ki degere gecis yapilir.")] 
        public bool IsTo  = true;
        public bool Enabled;
        public Vector3 Value;
        public bool X = true;
        public bool Y = true;
        public bool Z = true;
        public bool IsLocalSpace = false;
        [HideInInspector] public Vector3 initValue;
        [HideInInspector] public Vector3 initValueLocal;
    }

     public class AnimFloat
    {
        public bool Enabled;
        public Vector3 Value;
        [HideInInspector] public Vector3 initValue;
        [HideInInspector] public Vector3 initValueLocal;
    }

    [System.Serializable]
    public class TweenAnim
    {



        /* params */
        public AnimVector Position = new AnimVector();
        public AnimVector Rotation = new AnimVector();
        public AnimVector Scale = new AnimVector();
        public LeanTweenType Ease;
        public float AnimTime = 1;
        public float AnimDelayStart = 0; //animasyon baslamadan once delay suresi
        public bool IsPingPong = false;
        /*  private */
        [HideInInspector] public int index;
        [HideInInspector] public int stepIndex;
        [HideInInspector] public bool isPlaying;
    }
    [System.Serializable]
    public class TweenStep
    {
        /* params */
        public TweenAnim[] Animations;  //animasyonlar sirayla oynatilir
        public bool IsAutoPlayNext = true;//bir sonraki animasyona otomatik gecis yapilip yapilmayacagi. repeat = 0 ise bu deger true olsa bile gecis yapilamaz. repeat > 1 ise repeatin bitmesi beklenir.
        public int StepRepeat = 1; //0(sonsuz)  1(repeat yok, animasyonlar bir kez oynatilir)
        public float DelayStart = 0;
        public float DelayRepeat = 0; //repeat varsa, diger repeat kac sn sonra baslayacak
        /* privates */
        [HideInInspector] public int repeatCounter = 0;
        [HideInInspector] public int index;
        [HideInInspector] public bool isStarted;

    }

    public enum AutoStartEnum { DISABLED, ON_INIT, ON_STARTGAME }


    public class TweenAnimator : HyperSceneObj
    {
        public AutoStartEnum AutoStart;
        public TweenStep[] Steps;
        [HideInInspector] public TweenEventStart EventStepStart = new TweenEventStart();
        [HideInInspector] public TweenEventStart EventStepEnd = new TweenEventStart();
        [HideInInspector] public TweenEventStart EventAnimStarted = new TweenEventStart();
        [HideInInspector] public TweenEventStart EventAnimUpdated = new TweenEventStart();


        private TweenStep currStep;
        private TweenAnim currAnim;
        public override void Init()
        {
            for (int i = 0; i < Steps.Length; i++)
            {
                Steps[i].index = i;
                for (int a = 0; a < Steps[i].Animations.Length; a++)
                {
                    TweenAnim anim = Steps[i].Animations[a];
                    anim.index = a;
                    anim.stepIndex = i;
                    anim.Position.initValue = transform.position;
                    anim.Position.initValueLocal = transform.localPosition;
                    anim.Rotation.initValue = transform.rotation.eulerAngles;
                    anim.Rotation.initValueLocal = transform.localRotation.eulerAngles;
                    anim.Scale.initValue = transform.localScale;
                    anim.Scale.initValueLocal = transform.localScale;
                }
            }




            base.Init();
             if (AutoStart == AutoStartEnum.ON_INIT) 
             {
                 enabled = true;
                 PlayStep(0);
             }



        }

        public override void StartGame()
        {
            base.StartGame();

            if (AutoStart == AutoStartEnum.ON_STARTGAME) PlayStep(0);
        }







        /*--------------------------------------------------------------------------------------------*/
        /*-----------------------------------------| PUBLIC |-----------------------------------------*/
        /*--------------------------------------------------------------------------------------------*/

        public virtual void PlayStep(int stepIndex)
        {

            if (stepIndex < Steps.Length)
            {
                currStep = Steps[stepIndex];

                currStep.isStarted = true;
                currStep.repeatCounter = 0;
                if (currStep.Animations.Length > 0)
                {

                    EventStepStart.Invoke(new TweenAnimEventInfo(currStep.index, 0));
                    playAnim(0, currStep.DelayStart);
                    // StartCoroutine(playStepWithDelay(currStep.DelayStart));
                }
                else
                {
                    onStepCompleted();
                }
            }

        }

        public void Stop()
        {
             LeanTween.cancel(gameObject);

             StopAllCoroutines();
        }




        /*-----------------------------------------| private |-----------------------------------------*/
 


        private void playAnim(int animIndex, float stepStartDelay = 0)
        {
            if (animIndex < currStep.Animations.Length)
            {
                currAnim = currStep.Animations[animIndex];
                moveTo(currAnim,stepStartDelay);
            }


        }

        private void moveTo(TweenAnim anim, float stepStartDelay = 0)
        {
            anim.isPlaying = true;
            LeanTween.cancel(gameObject, false);

            Vector3 pos = getAnimPos();
            Vector3 rot = getAnimRot();
            Vector3 scale = getAnimScale();
            if (currAnim.Position.Enabled)
            {
                LTDescr twPos = LeanTween.moveLocal(gameObject, pos, anim.AnimTime).setEase(anim.Ease).setIgnoreTimeScale(true).setOnComplete(moveToCompleted).setOnUpdate(onUpdate).setDelay(anim.AnimDelayStart + stepStartDelay);
                if (anim.IsPingPong)   twPos.setLoopPingPong(1);
            }

            if (currAnim.Rotation.Enabled)
            {
                LTDescr twRot = LeanTween.rotateLocal(gameObject, rot, anim.AnimTime).setEase(anim.Ease).setIgnoreTimeScale(true).setOnComplete(moveToCompleted).setDelay(anim.AnimDelayStart  + stepStartDelay);
                if (anim.IsPingPong)   twRot.setLoopPingPong(1);
            }

            if (currAnim.Scale.Enabled)
            {
                LTDescr twScale = LeanTween.scale(gameObject, scale, anim.AnimTime).setEase(anim.Ease).setIgnoreTimeScale(true).setDelay(anim.AnimDelayStart  + stepStartDelay);
                if (anim.IsPingPong)   twScale.setLoopPingPong(1);
            }

            



            EventAnimStarted.Invoke(new TweenAnimEventInfo(currStep.index, currAnim.index));

        }

        private Vector3 getAnimPos()
        {
            Vector3 def = currAnim.Position.IsLocalSpace ? transform.localPosition : transform.position;
            KeyValuePair<Vector3, Vector3> vectorValues = getAnimVector(currAnim.Position, def);
            if (currAnim.Position.IsLocalSpace)
                transform.localPosition = vectorValues.Key;
            else
                transform.position = vectorValues.Key;
            return vectorValues.Value;
        }
        private Vector3 getAnimRot()
        {
            Vector3 def = currAnim.Rotation.IsLocalSpace ? transform.localRotation.eulerAngles : transform.rotation.eulerAngles;
            
            KeyValuePair<Vector3, Vector3> vectorValues = getAnimVector(currAnim.Rotation, def);
            if (currAnim.Rotation.IsLocalSpace)
                transform.localEulerAngles = vectorValues.Key;
            else
                transform.eulerAngles = vectorValues.Key;
            return vectorValues.Value;
        }
        private Vector3 getAnimScale()
        {
            Vector3 def = transform.localScale;
            
            KeyValuePair<Vector3, Vector3> vectorValues = getAnimVector(currAnim.Scale, def);
            transform.localScale = vectorValues.Key;
            return vectorValues.Value;
        }


        private KeyValuePair<Vector3, Vector3> getAnimVector(AnimVector anim, Vector3 defaultValue)
        {
            KeyValuePair<Vector3, Vector3> pair;
            Vector3 value = defaultValue;
            if (anim.Enabled)
            {
                if (anim.X) value.x = anim.Value.x;
                if (anim.Y) value.y = anim.Value.y;
                if (anim.Z) value.z = anim.Value.z;
            }
            else
            {
                if (currStep.index == 0 && currAnim.index == 0)
                {
                    value = anim.IsLocalSpace ? anim.initValueLocal : anim.initValue;

                }

            }

            if (anim.IsTo)
            {
                pair = new KeyValuePair<Vector3, Vector3>(defaultValue, value);
            }
            else
            {
                pair = new KeyValuePair<Vector3, Vector3>(value, defaultValue);
            }
            return pair;
        }


        protected virtual void onUpdate(float value)
        {
            TweenAnimEventInfo info = new TweenAnimEventInfo(currStep.index, currAnim.index);
            info.AnimRatio = value;
            EventAnimUpdated.Invoke(info);
        }

        private void moveToCompleted()
        {


            if (currAnim.isPlaying)
            {
                currAnim.isPlaying = false;
                int next = currAnim.index + 1;
                if (next < currStep.Animations.Length)
                {
                      Debug.Log("playAnim next "+ next);
                    playAnim(next);
                }
                else
                {
                    onStepCompleted();
                }
            }



        }

        private void onStepCompleted()
        {
            EventStepEnd.Invoke(new TweenAnimEventInfo(currStep.index, currAnim.index));
            currStep.repeatCounter++;

            if (currStep.repeatCounter == currStep.StepRepeat)
            {
                if (currStep.IsAutoPlayNext)
                {
                    if (currStep.index == Steps.Length - 1 && Steps.Length > 1)
                    {
                        PlayStep(0);
                    }
                    else
                    {
                        PlayStep(currStep.index + 1);
                    }
                }



            }
            else
            {
                PlayStep(currStep.index);
            }

        }




    }
}
