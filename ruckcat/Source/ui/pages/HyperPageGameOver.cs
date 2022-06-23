using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Ruckcat
{
    public class HyperPageGameOver : HyperPage
    {
        public TextMeshProUGUI TxtResult;
        public HyperButton BtnRestart;
        public HyperButton BtnNextLevel;
        public GameObject StateSucc;
        public GameObject StateFail;
        public string TextfieldLevelScore = "TxtLevelScore";

        public override void Init()
        {
            base.Init();



           
            if (BtnRestart)
            {
                BtnRestart.EventTouch.AddListener(onBtnRestartClicked);
                //BtnRestart.gameObject.SetActive(false);
            }
            if (BtnNextLevel)
            {
                BtnNextLevel.EventTouch.AddListener(onBtnNextLevelClicked);
                //BtnNextLevel.gameObject.SetActive(false);
            }

            if (StateSucc) StateSucc.gameObject.SetActive(false);
            if (StateFail) StateFail.gameObject.SetActive(false);
        }


        public override void Open()
        {
            base.Open();
        }

    

        public override void Close()
        {
            base.Close();
        }


        public virtual void SetGameResult(GameResult _result)
        {
            string textResult = "";
            if (_result == GameResult.FAILED)
            {
                textResult = "you lose :(";
            }
            if (_result == GameResult.SUCCEED)
            {
                textResult = "you win!";
            }
            if (TxtResult) TxtResult.SetText(textResult);
            if (StateSucc) StateSucc.gameObject.SetActive(_result == GameResult.SUCCEED);
            if (StateFail) StateFail.gameObject.SetActive(_result == GameResult.FAILED);
            SetText("TxTLevel", "Level " + HyperLevelCont.Instance.CurrLevel.ToString());
        }

        private void onBtnRestartClicked(TouchUI _touch)
        {
            CoreGameCont.Instance.RestartGame();
        }

        private void onBtnNextLevelClicked(TouchUI _touch)
        {
            HyperLevelCont.Instance.NextLevel();
        }



        public void ViewLevelScore(string _value)
        {
            SetText(TextfieldLevelScore, _value);
        }


    }
}
