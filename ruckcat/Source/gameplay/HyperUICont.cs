using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Ruckcat
{
    [Serializable]
    public class CanvasElement
    {
        public string Id;
        public RectTransform UIObject;
        public Transform FollowTarget;
        public Vector3 FollowTargetOffset;
    }

    public class HyperUICont : CoreUiCont
    {
        public List<CanvasElement> ListCanvasElements = new List<CanvasElement>();
        public bool IsSkipIntro = false;
        public override void Init()
        {
            base.Init();

            initCanvasElements();


        }
        public override void Load()
        {
            if (!IsSkipIntro)
            {
                OpenPageInit();


            }
            else
            {
                onIntroPageStart();
            }

        }

        private void onIntroPageStart()
        {
            Loaded();
        }
       

        public override void Update()
        {
            base.Update();

            updateCanvasElements();

        }

        public HyperPageIntro OpenPageInit()
        {
            HyperPageIntro intro = transform.GetComponentInChildren<HyperPageIntro>(true);
            if (intro)
            {
                CoreUiCont.Instance.OpenPage(intro.GetElementId());
                intro.EventIntroLoaded.AddListener(onIntroPageStart);
            }
            return intro;
        }

        public HyperPageGame OpenPageGame()
        {
            HyperPageGame page = GetPageGame();
            if (page)
            {
                CoreUiCont.Instance.OpenPage(page.GetElementId());
            }


            
            return page;
        }
        public void OpenPageGameOver(GameResult gameOverResult)
        {
            HyperPageGameOver page = transform.GetComponentInChildren<HyperPageGameOver>(true);
            if (page)
            {
                CoreUiCont.Instance.OpenPage(page.GetElementId());
                page.SetGameResult(gameOverResult);
            }
        }


        public HyperPageGame GetPageGame()
        {
            return transform.GetComponentInChildren<HyperPageGame>(true);
        }
        public HyperPageGameOver GetPageGameOver()
        {
            return transform.GetComponentInChildren<HyperPageGameOver>(true);
        }

        /*------------ CANVAS ELEMENTS -------------*/
        protected virtual void initCanvasElements()
        {
            foreach (CanvasElement e in ListCanvasElements)
            {
                if (e.UIObject)
                {
                    ShowElement(e.Id, false);
                }
            }
        }
        public void ShowElement(string id, bool isShow)
        {
            CanvasElement e = GetCanvasElement(id);
            if (e.UIObject)
            {
                e.UIObject.gameObject.SetActive(isShow);
            }
        }

        public void SetElementFollowTarget(string id, GameObject target)
        {
            CanvasElement e = GetCanvasElement(id);
            if (e.UIObject)
            {
                e.FollowTarget = target.transform;
            }
        }

        private CanvasElement GetCanvasElement(string id)
        {
            CanvasElement item = ListCanvasElements.Find(e => e.Id == id);
            return item;
        }

        private void updateCanvasElements()
        {
            foreach (CanvasElement e in ListCanvasElements)
            {
                if (e.UIObject)
                {
                    if (e.FollowTarget)
                    {
                        Vector2 pos = convertWorlToScreen(e.FollowTarget.transform, e.FollowTargetOffset);
                        // e.UIObject.anchoredPosition = pos;
                        e.UIObject.transform.position = pos;
                    }
                }
            }
        }

        public Vector2 convertWorlToScreen(Transform worldTrans, Vector3 offset)
        {
            Vector3 v = Camera.main.WorldToScreenPoint(worldTrans.position + offset);
            return new Vector2(v.x, v.y);
        }
        

    }

}