using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Ruckcat
{
    [Serializable]
    public struct CamProperty
    {
        public string Id;
        public Vector3 LocalPos;
        public Vector3 LocalRot;
    }

    public class HyperCameraCont : CoreCont<HyperCameraCont>
    {
        public GameObject Target;
        public Vector3 FollowAxis = new Vector3(1, 1, 1); //axislerin takip edilip edilmeyecegi. 0:false, 1:true 
        public float SmoothSpeed = 3f;
        private Vector3 velocity = Vector3.zero;
        private PlayerCont _playercont;
        private bool isCamLocked;


        protected Camera Camera;
        private List<CamProperty> listItems;
        private string currState;
        [HideInInspector] public UnityEvent EventAnimCompleted = new UnityEvent();

        public override void Init()
        {
            base.Init();

            listItems = new List<CamProperty>();

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform item = transform.GetChild(i);
                if (item)
                {
                    CamProperty p;
                    p.Id = item.name.ToLower();
                    p.LocalPos = item.transform.localPosition;
                    p.LocalRot = item.transform.localEulerAngles;

                    listItems.Add(p);

                    if (i > 0) item.gameObject.SetActive(false);
                }
            }

            if (GetComponentsInChildren<Camera>().Length > 0)
            {
                Camera = GetComponentsInChildren<Camera>()[0];
                {
                    ChangeCamTo(Camera.name, 0);
                }
            }

            _playercont = FindObjectOfType<PlayerCont>();

            isCamLocked = true;
        }


        public override void StartGame()
        {
            base.StartGame();
            isCamLocked = false;
        }


        private void FixedUpdate()
        {
            if (!isCamLocked && Target) cameraMovement();
        }

        /*--------------------------------------------------------------------------------------------*/
        /*-----------------------------------------| PUBLIC |-----------------------------------------*/
        /*--------------------------------------------------------------------------------------------*/


        public void SetCameraRotateAroundObject(float _speed)
        {
            isCamLocked = true;
            transform.LookAt(Target.transform.position);
            transform.position = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);
            transform.eulerAngles = new Vector3(13, transform.eulerAngles.y, transform.eulerAngles.z);
            StartCoroutine(rotateAround(_speed));
        }

        public void CameraShake(float duration, float magnitude)
        {
            StartCoroutine(shakeCam(duration, magnitude));
        }

        private Vector3 lastCameraPose = new Vector3();
        private IEnumerator shakeCam(float duration, float magnitude)
        {
            float elapsed = 0.0f;
            lastCameraPose = new Vector3(Camera.transform.localPosition.x, Camera.transform.localPosition.y, Camera.transform.localPosition.z);
            while (elapsed < duration)
            {
                float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                Camera.transform.localPosition = new Vector3(x + Camera.transform.localPosition.x, y + Camera.transform.localPosition.y, Camera.transform.localPosition.z);
                elapsed += Time.deltaTime;
                yield return null;
            }
            Camera.transform.localPosition = lastCameraPose;
        }

        /* ChangeCamTo : cameranin su an ki pozisyonundan -> ilgili id'deki (child camera gameobject ismi) camera transformuna animasyon */
        public void ChangeCamTo(string camID, float animTime, float _delay = 0,
            LeanTweenType tweenType = LeanTweenType.easeOutSine)
        {
            if (getItem(camID.ToLower()).Id != null)
            {
                currState = camID.ToLower();

                setTransition(getItem(camID.ToLower()), animTime, _delay, tweenType);
            }
        }

        /* AnimTo :  ilgili id'deki (child camera gameobject ismi) camera transformundan -> cameranin su an ki pozisyonuna Animasyon */
        public void ChangeCamFrom(string camID, float animTime, float _delay = 0,
            LeanTweenType tweenType = LeanTweenType.easeOutSine)
        {
            if (getItem(camID.ToLower()).Id != null)
            {
                CamProperty from = getItem(camID.ToLower());
                Camera.transform.localPosition = from.LocalPos;
                Camera.transform.eulerAngles = from.LocalRot;


                setTransition(getItem(currState.ToLower()), animTime, _delay, tweenType);
            }
        }

        /* cameranin local pozisyonunu baz alarak merkez etrafinda donmesi. _repat=-1 ise infinite. */
        public void PlayRotateAround(float _time, int _repeat = -1)
        {
            LeanTween.rotateAround(gameObject, Vector3.up, 360, _time).setRepeat(_repeat);
        }


        /*-----------------------------------------| private |-----------------------------------------*/
        private void cameraMovement()
        {
            Vector3 temptarget = Target.transform.position;
            Vector3 desiredPosition = transform.position;
            if (FollowAxis.x == 1) desiredPosition.x = temptarget.x;
            if (FollowAxis.y == 1) desiredPosition.y = temptarget.y;
            if (FollowAxis.z == 1) desiredPosition.z = temptarget.z;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed * Time.deltaTime);
        }

        private IEnumerator rotateAround(float _speed)
        {
            while (true)
            {
                transform.RotateAround(Target.transform.position, new Vector3(0.0f, 1.0f, 0.0f),
                    20 * Time.deltaTime * _speed);
                yield return null;
            }
        }


        private void setTransition(CamProperty property, float _time, float _delay, LeanTweenType _tweenType)
        {
            LeanTween.cancel(Camera.gameObject);

            if (_time > 0)
            {
                LeanTween.moveLocal(Camera.gameObject, property.LocalPos, _time).setEase(_tweenType).setDelay(_delay);
                LTDescr d = LeanTween.rotateLocal(Camera.gameObject, property.LocalRot, _time).setEase(_tweenType)
                    .setDelay(_delay);
                if (d != null)
                {
                    d.setOnComplete(transitionOnCompleted);
                }
            }
            else
            {
                Camera.gameObject.transform.localPosition = property.LocalPos;
                Camera.gameObject.transform.localEulerAngles = property.LocalRot;
            }
        }

        private void transitionOnCompleted()
        {
            EventAnimCompleted.Invoke();
        }

        private CamProperty getItem(string id)
        {
            CamProperty p = listItems.Find(e => e.Id == id);
            return p;
        }
    }
}