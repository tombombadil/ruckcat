using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Ruckcat
{
    [RequireComponent(typeof(Canvas))]
    public class CoreUiCont : CoreCont<CoreUiCont>
    {
        [HideInInspector] public CoreUI currentUi;
        [HideInInspector] private List<CoreUI> listUIs;
        [HideInInspector] private List<PageUI> listPages;
        [HideInInspector] private Canvas canvas;
        [HideInInspector] private GraphicRaycaster graphRaycast;
        private ElementCont pageCont;
        private Dictionary<Graphic, CoreUI> listTouchTargets;
        /* 
         * EventTouch :  tiklanan gameobject bir BaseUI'ye sahipse bu BaseUI invoke edilir, 
         * sahip degilse parent'inda BaseUI var mı diye bakilir ve varsa parentinda olan 
         * BaseUI invoke edilir. o da yoksa herhangi bir sey yapılmaz 
         */
        public EventTouchUI EventTouch;




        public override void Init()
        {
            if (!IsInited)
            {
                base.Init();
                EventTouch = new EventTouchUI();
                listUIs = new List<CoreUI>();
                pageCont = gameObject.AddComponent<ElementCont>();
                pageCont.Init(ElementCont.ControllerActionType.GAMEOBJECT);
                listTouchTargets = new Dictionary<Graphic, CoreUI>();
                canvas = GetComponent<Canvas>();
                graphRaycast = GetComponent<GraphicRaycaster>();
                currentUi = null;

                addChildToList(transform);


                foreach (CoreUI ui in listUIs)
                {
                    ui.Init();
                    if (ui.TouchTarget != null) listTouchTargets.Add(ui.TouchTarget, ui);

                    if (ui is PageUI) AddPage((PageUI)ui);
                }
            }

        }


        public void AddPage(PageUI _page)
        {
            pageCont.Add(_page);
        }

        public override void Load()
        {
            base.Load();
            //Loaded();
        }


        protected override void Loaded()
        {
            base.Loaded();
        }

   




        private void addChildToList(Transform _parent)
        {
            for (int i = 0; i < _parent.childCount; i++)
            {

                Transform child = _parent.GetChild(i);
                if (child != _parent) addChildToList(child);


            }
            CoreUI ui = _parent.GetComponent<CoreUI>();
            if (ui) listUIs.Add(ui);

        }

        public bool UpdateTouch(Ruckcat.Touch _tap)
        {
            bool isAnyhitResult = false;
            List<RaycastResult> list = GetRaycastResult(_tap.GetScreenPoint());
            List<CoreUI> listTargets = new List<CoreUI>();
            int order = 0;
            for (int i = 0; i < list.Count; i++)
            {
                RaycastResult ray = list[i];
                Graphic graphicComp = ray.gameObject.GetComponent<Graphic>();

                if (graphicComp)
                {


                    if (listTouchTargets.ContainsKey(graphicComp))
                    {
                        CoreUI ui = listTouchTargets[graphicComp];

                        if (ui)
                        {

                            if (!listTargets.Contains(ui))
                            {
                                if (ui.GetSelectable() == SelectableStatus.ENABLED)
                                {

                                    Ruckcat.TouchUI touch = new Ruckcat.TouchUI();
                                    touch.Target = ui;
                                    touch.Order = order;
                                    EventTouch.Invoke(touch);
                                    listTargets.Add(ui);
                                    ui.OnTouch(touch);
                                }


                            }




                        }
                    }
                    order++;
                    isAnyhitResult = true;


                }

                //if(ui==null)
                //{
                //    ui = ray.gameObject.transform.parent.gameObject.GetComponent<BaseUI>();
                //}


            }

            return isAnyhitResult; ;
        }



       

        public List<RaycastResult> GetRaycastResult(Vector2 _screenPos)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            if (graphRaycast)
            {
                PointerEventData ped = new PointerEventData(null);
                ped.position = _screenPos;

                graphRaycast.Raycast(ped, results);

            }

            return results;
        }





        public CoreUI GetCurrent()
        {
            return currentUi;
        }

        public T GetPageByClass<T>() where T : PageUI
        {
            return pageCont.GetByClass<T>();


        }

        public PageUI GetPage(string _id) 
        {
            return (PageUI) pageCont.GetById("");


        }

        public PageUI OpenPage(string _pageId)
        {
            PageUI page = (PageUI) pageCont.Open(_pageId);
            return page;

        }

        public T OpenPage<T>() where T : PageUI
        {
            T page = (T) pageCont.Open<T>();
            return page;
        }



    }
}