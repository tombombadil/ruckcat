using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Ruckcat
{
    public class CoreInputCont : CoreCont<CoreInputCont>
    {

        private Touch touchStart;
        private float touchStartTime;
        /* EventTap : Ekranin herhangi bir yerine bir kez dokunulmasi */
        public EventTouch EventTouch =  new EventTouch();
        /* EventSwipe : Ekranin herhangi bir yerinde bir swipe yapıldiginda */
        public EventSwipe EventSwipe = new EventSwipe();
        private Plane virtualPlane;
        public override void Init()
        {
            virtualPlane = new Plane(Vector3.up, Vector3.zero);
            base.Init();
        }


        public override void Update()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                if (UnityEngine.Input.GetMouseButtonDown(0))
                {
                    startTouch(UnityEngine.Input.mousePosition);
                    //endTouch(Input.mousePosition);
                }
                if (UnityEngine.Input.GetMouseButtonUp(0))
                {
                    //startTouch(Input.mousePosition);
                    endTouch(UnityEngine.Input.mousePosition);
                }
                if (UnityEngine.Input.GetMouseButton(0))
                {
                    //startTouch(Input.mousePosition);
                    updateTouch(UnityEngine.Input.mousePosition);
                }
            }
            else
            {
                if (UnityEngine.Input.touchCount > 0)
                {
                    for (int i = 0; i < UnityEngine.Input.touchCount; i++)
                    {
                        UnityEngine.Touch touch = UnityEngine.Input.GetTouch(i);
                        Vector2 pos;
                        if (touch.phase == TouchPhase.Began)
                        {
                             pos = touch.position;
                            startTouch(pos, i);

                        }
                        if (touch.phase == TouchPhase.Ended)
                        {
                             pos = touch.position;
                            endTouch(pos, i);

                        }

                        if (touch.phase == TouchPhase.Moved)
                        {
                             pos = touch.position;
                            updateTouch(pos, i);

                        }
                    }

                }
            }
        }



        protected Ruckcat.Touch startTouch(Vector2 _screenPos, int _touchIndex=0)
        {
            touchStart = new Ruckcat.Touch();
            touchStart.Reset();
            touchStart.Points[0] = Camera.main.ScreenToViewportPoint(_screenPos);
            touchStart.ScenePoints[0] = getOnPlaneHit(_screenPos);
            touchStart.Phase = TouchPhase.Began;
            touchStart.TouchIndex = _touchIndex;
            touchStart.IsHitUIElement = CoreUiCont.Instance.UpdateTouch(touchStart);
            touchStart.IsHitSceneElement =CoreSceneCont.Instance.UpdateTouch(touchStart);
            EventTouch.Invoke(touchStart);
            updateUiAndScene(touchStart);
            touchStartTime = Time.time;
            return touchStart;

        }

        protected Ruckcat.Touch updateTouch(Vector2 _screenPos, int _touchIndex=0)
        {
          
            touchStart.Points[1] = Camera.main.ScreenToViewportPoint(_screenPos);
            touchStart.ScenePoints[1] = getOnPlaneHit(_screenPos);
            touchStart.DeltaTime = (Time.time - touchStartTime);
            touchStart.Phase = TouchPhase.Moved;
            touchStart.TouchIndex = _touchIndex;
            EventTouch.Invoke(touchStart);
            updateUiAndScene(touchStart);
            return touchStart;
        }

        protected Ruckcat.Touch endTouch(Vector2 _screenPos, int _touchIndex=0)
        {
            touchStart.IsHitUIElement = CoreUiCont.Instance.UpdateTouch(touchStart);
            touchStart.Points[1] = Camera.main.ScreenToViewportPoint(_screenPos);
            touchStart.ScenePoints[1] = getOnPlaneHit(_screenPos);
            touchStart.DeltaTime = (Time.time - touchStartTime);
            touchStart.Phase = TouchPhase.Ended;
            touchStart.TouchIndex = _touchIndex;
            EventTouch.Invoke(touchStart);
            EventSwipe.Invoke(touchStart);
            updateUiAndScene(touchStart);
            return touchStart;
        }

        private void updateUiAndScene(Ruckcat.Touch _tap)
        {
            bool callbackFromUI = false;//ui'de bir touch olmus mu?
            if (CoreUiCont.Instance)
            {
                if(_tap.Phase == TouchPhase.Began)
                  callbackFromUI = CoreUiCont.Instance.UpdateTouch(_tap);
            }

            if (!callbackFromUI)
            {
                CoreSceneCont.Instance.UpdateTouch(_tap);
            }
        }



        private Vector3 getOnPlaneHit(Vector2 _screenPos)
        {
            Vector3 r = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(_screenPos.x, _screenPos.y, 0));
            float enter = 0.0f;
            if (virtualPlane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                r = hitPoint;

            }
            return r;
        }


    }
}