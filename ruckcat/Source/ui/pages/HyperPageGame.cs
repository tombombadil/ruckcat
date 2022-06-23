using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Ruckcat
{
    public class HyperPageGame : HyperPage
    {
   
        [HideInInspector] public EventStartAction EventStart = new EventStartAction();


        public RectTransform Bar;
        public TextMeshProUGUI TxtScore;
        public TextMeshProUGUI TxtLevel;
        public HyperButton BtnStart;
        public HyperButton BtnRestart;
        public HyperButton BtnDebugPanel;
        public CoreUI ButtonSettings;

        public override void Init()
        {
            base.Init();
            if(Bar) Bar.transform.localScale = new Vector3(0,1,1);
            //SetBarScale(0);
            ViewScore(0);

            SetText("TxtLevel", "Level: " + (HyperLevelCont.Instance.CurrLevel));
            SetText("TxtScore", HyperConfig.Instance.Score.ToString());

            if (BtnStart) BtnStart.EventTouch.AddListener(onBtnStartClicked);
           
            if (BtnRestart) BtnRestart.EventTouch.AddListener(onBtnRestartClicked);

            if (ButtonSettings) ButtonSettings.EventTouch.AddListener(onClickBtnSettings);




        }

        public override void Open()
        {
            base.Open();
        }

        public override void StartGame()
        {
            base.StartGame();
            if (BtnRestart) BtnStart.gameObject.SetActive(false);
        }


        public override void Close()
        {
            base.Close();
        }
        protected virtual void onBtnStartClicked(TouchUI _touch)
        {
            EventStart.Invoke();
        }
        private void onBtnRestartClicked(TouchUI _touch)
        {
            CoreGameCont.Instance.RestartGame();
        }
        private void onClickBtnSettings(TouchUI info)
        {
            OpenPopup<HyperPopupSettings>();
        }

        public void ViewScore(float _score)
        {
            if(TxtScore) TxtScore.SetText(_score.ToString());
        }

       


   


    }
}
