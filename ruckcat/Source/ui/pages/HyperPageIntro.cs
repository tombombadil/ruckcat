using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Ruckcat
{

    public class HyperPageIntro : HyperPage
    {
        public class EventLoaded : UnityEvent { };
        public EventLoaded EventIntroLoaded = new EventLoaded();
        public HyperButton BtnStart;
        public TextMeshProUGUI TxtLevel;
        public override void Init()
        {
            base.Init();
            BtnStart.EventTouch.AddListener(onStartClicked);


            SetText("TxtLevel", "Level: " + (HyperLevelCont.Instance.CurrLevel));
        }


        public override void Open()
        {
            base.Open();
        }

  
        public override void Close()
        {
            base.Close();
        }


        private void onStartClicked(TouchUI _info)
        {
            //EventStart.Invoke();
            //CoreUiCont.Instance.Loaded();
        }

        public override void OnTouch(TouchUI info)
        {
            base.OnTouch(info);
            EventIntroLoaded.Invoke();

        }


    }
}
