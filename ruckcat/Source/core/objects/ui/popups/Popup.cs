using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ruckcat
{
    public class Popup : CoreUI, IElement
    {
        private ElementCont.EventStatus eventElementStatus;
        public CoreUI BtnClose;
        public override void Init()
        {
            base.Init();
            if (BtnClose)
                BtnClose.EventTouch.AddListener(onTouchCloseBtn);
        }

        public override void StartGame()
        {
            base.StartGame();

        }

        public void SetElementStatus(ElementCont.Status _newStatus)
        {
            switch (_newStatus)
            {
                case ElementCont.Status.LOAD:
                    eventElementStatus.Invoke(this, ElementCont.Status.LOADED);
                    break;
                case ElementCont.Status.OPEN:
                    Open();
                    break;
                case ElementCont.Status.CLOSE:
                    Close();
                    break;

            }

        }


        public override void Open()
        {
            eventElementStatus.Invoke(this, ElementCont.Status.OPENED);
            base.Open();
        }
        public override void Close()
        {
            eventElementStatus.Invoke(this, ElementCont.Status.CLOSED);
            base.Close();
        }


        public ElementCont.EventStatus GetEventElementStatus()
        {
            if (eventElementStatus == null) eventElementStatus = new ElementCont.EventStatus();
            return eventElementStatus;
        }

        public string GetElementId()
        {
            return name;
        }

        protected virtual void onTouchCloseBtn(Ruckcat.TouchUI _tap)
        {
            Close();
        }

    }
}