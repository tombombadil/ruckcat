using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Ruckcat
{
    public enum SelectableStatus { ENABLED, HIGHLIGHT, SELECTED, DISABLED };
    public enum TouchEventTarget { SELF, PARENT };
    public class CoreUI : CoreObject
    {


         [FoldoutGroup("Core",expanded: false), PropertyOrder(99)]
        public Graphic TouchTarget;
        [HideInInspector] public string Id;
        [HideInInspector] public Ruckcat.EventTouchUI EventTouch = new Ruckcat.EventTouchUI();
        private SelectableStatus selectableStatus;
        public override void Init()
        {
            base.Init();
            
        }


        public override void Load()
        {
            Loaded();
        }
        public virtual void Open()
        {
            Opened();
        }
        public virtual void Close()
        {
            Closed();
        }
        protected override void Loaded()
        {
            GameStatus = GameStatus.LOADED;
        }
        protected virtual void Opened()
        {
            GameStatus = GameStatus.OPENED;
        }
        protected virtual void Closed()
        {
            GameStatus = GameStatus.CLOSED;
        }


        public virtual void SetSelectable(SelectableStatus _status)
        {

            selectableStatus = _status;
        }
        public virtual SelectableStatus GetSelectable()
        {

            return selectableStatus;
        }



        /* TOUCH EVENT */
        public virtual void OnTouch(Ruckcat.TouchUI info)
        {
            EventTouch.Invoke(info);
        }

    }

}