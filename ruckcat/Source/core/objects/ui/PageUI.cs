using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ruckcat
{
    public class PageUI : CoreUI, IElement
    {
        private ElementCont.EventStatus eventElementStatus;
        protected ElementCont popupCont;
        public Image PopupDisabledScreen;


        public override void Init()
        {
            base.Init();
            eventElementStatus = new ElementCont.EventStatus();
            //if (PageId.Length < 1) Id = name;
            Id = name;
            foreach (CoreUI ui in GetComponentsInChildren<CoreUI>()) //child baseUı Id'lerini atiyoruz
            {
                if (ui != this)
                {
                    if (ui.gameObject == this.gameObject)
                        ui.Id = Id;
                    else
                        ui.Id = Id + "/" + ui.name;

                }

            }

            popupCont = gameObject.AddComponent<ElementCont>();
            popupCont.Init(ElementCont.ControllerActionType.GAMEOBJECT);
            popupCont.Event.AddListener(onPopupContStatus);
            foreach (Popup popup in gameObject.GetComponentsInChildren<Popup>(true))
            {




                AddPopup(popup);

            }

            CoreUiCont.Instance.EventTouch.AddListener(onUiTouchHandler);
            if (PopupDisabledScreen) PopupDisabledScreen.enabled = false;
        }

        private void onUiTouchHandler(Ruckcat.TouchUI info)
        {
            if (PopupDisabledScreen)
            {
                if (info.Target.name == PopupDisabledScreen.name)
                {
                    if (info.Order == 0)
                    {

                        ClosePopup();
                    }
                }
            }


        }

        public T OpenPopup<T>() where T : Popup
        {
            T popup = popupCont.Open<T>();
            return popup;
        }

        public void ClosePopup()
        {
            popupCont.CloseCurrent();
        }

        public T GetPopup<T>() where T : Popup
        {

            T popup = popupCont.GetByClass<T>();


            return popup;
        }

        private void onPopupContStatus(IElement _element, ElementCont.Status _status)
        {
            if (_status == ElementCont.Status.OPENED)
                if (PopupDisabledScreen) PopupDisabledScreen.enabled = true;
            if (_status == ElementCont.Status.CLOSED)
                if (PopupDisabledScreen) PopupDisabledScreen.enabled = false;

        }

        public void SetElementStatus(ElementCont.Status _newStatus)
        {
            switch (_newStatus)
            {
                case ElementCont.Status.LOAD:
                    Load();
                    break;
                case ElementCont.Status.OPEN:
                    Open();
                    break;
                case ElementCont.Status.CLOSE:
                    Close();
                    break;

            }

        }

        //public virtual void Load()
        //{
        //    LoadCompleted();
        //}
        //public virtual void Open()
        //{
        //    OpenCompleted();
        //}
        //public virtual void Close()
        //{
        //    CloseCompleted();
        //}
        protected override void Loaded()
        {
            eventElementStatus.Invoke(this, ElementCont.Status.LOADED);
        }
        protected override void Opened()
        {
            eventElementStatus.Invoke(this, ElementCont.Status.OPENED);
        }
        protected override void Closed()
        {
            eventElementStatus.Invoke(this, ElementCont.Status.CLOSED);
        }



        /* POPUP */
        public void AddPopup(Popup _popup)
        {
            popupCont.Add(_popup);
        }



        public string GetElementId()
        {
            return Id;
        }


        public ElementCont.EventStatus GetEventElementStatus()
        {
            return eventElementStatus;
        }


        //void OnValidate()
        //{
        //    if (PageId == null) PageId = name;
        //}


        

    }
}