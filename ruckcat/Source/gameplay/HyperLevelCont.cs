using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Ruckcat
{

    public class HyperLevelCont : CoreCont<HyperLevelCont>
    {
        [HideInInspector] public EventStartAction EventStart = new EventStartAction();
        [HideInInspector] public int CurrLevel;
        [FoldoutGroup("Core", expanded: false), PropertyOrder(99)] [Tooltip("Time bazli level yapmak icin bu deger 'dan büyük olmalı.  ")] public float LevelTotalTime = 0;
        [FoldoutGroup("Core", expanded: false), PropertyOrder(99)] [Tooltip("Time bazlı levelin aktif edilip edilmeyeceği ")] public bool GameOverWhenTimeExceed = true;
        private float levelTime;
        private float _levelScore;
        private float _currentGameScore;






        public override void Init()
        {
            base.Init();
            _levelScore = 0;
            _currentGameScore =  HyperConfig.Instance.Score ;
        }

        public virtual void InitLevel()
        {
        }

        public override void StartGame()
        {
            base.StartGame();

        }



        /*--------------------------------------------------------------------------------------------*/
        /*-----------------------------------------| PUBLIC |-----------------------------------------*/
        /*--------------------------------------------------------------------------------------------*/
        public void TryEndLevel()
        {
            EndLevel(GetLevelResult());

        }

        public virtual void EndLevel(GameResult levelResult)
        {
            if (levelResult == GameResult.SUCCEED)
            {
                EventGameStatus.Invoke(GameStatus.GAMEOVER_SUCCEED);
            }
            if (levelResult == GameResult.FAILED)
            {
                EventGameStatus.Invoke(GameStatus.GAMEOVER_FAILED);
                TextMeshProUGUI txtLevelScore =  ((HyperUICont)CoreUiCont.Instance).GetPageGameOver().GetTextfield(((HyperUICont)CoreUiCont.Instance).GetPageGameOver().TextfieldLevelScore);
              if(txtLevelScore) txtLevelScore.gameObject.SetActive(false);
            }
           ((HyperUICont)CoreUiCont.Instance).OpenPageGameOver(levelResult);


            // TinySauce.OnGameFinished(CurrLevel.ToString(), levelResult == 2, HyperConfig.Instance.Score);


        }

        public float LevelScore
        {
            get => _levelScore; 
            set
            {

                _levelScore = value;
                ((HyperUICont)CoreUiCont.Instance).GetPageGameOver().ViewLevelScore(LevelScore.ToString());
                HyperConfig.Instance.Score = _currentGameScore + LevelScore;

            }
        }

        public void SetLevelScore(float _levelScore)
        {
            LevelScore += _levelScore;

        }


        public virtual GameResult GetLevelResult()
        {
            return GameResult.NONE;
        }


        /*--------------------------------------------------------------------------------------------*/
        /*-----------------------------------------| private |----------------------------------------*/
        /*--------------------------------------------------------------------------------------------*/
        public virtual void startLevel()
        {
            EventStart.Invoke();
        }




        public void FixedUpdate()
        {
            if (IsGameStarted)
            {

                if (LevelTotalTime > 0)
                {
                    if (LevelTime > LevelTotalTime)
                    {
                        timeExceeded();
                    }

                    //if(CoreUiCont.Instance.GetPageByClass<HyperPageGame>())
                    if (((HyperUICont)CoreUiCont.Instance).GetPageGame())
                    {
                        float ratio = (float)LevelTime / (float)LevelTotalTime;
                        ((HyperUICont)CoreUiCont.Instance).GetPageGame().SetBarScale("LevelTime", ratio);
                    }
                }

                LevelTime += Time.fixedDeltaTime;
            }

        }









        public float LevelTime
        {
            get { return levelTime; }
            set { levelTime = value; }
        }



        private void timeExceeded()
        {
            if (GameOverWhenTimeExceed)
                TryEndLevel();

        }
        public virtual  void NextLevel()
        {
            SaveLevel();
            HyperGameCont.Instance.RestartGame();
        }
        public virtual void RestoreLevel()
        {
            int level = 1;
            if (PlayerPrefs.HasKey("Level"))
            {
                level = PlayerPrefs.GetInt("Level");



            }
            CurrLevel = level;
        }


        public void SaveLevel()
        {
            PlayerPrefs.SetInt("Level", CurrLevel + 1);
            PlayerPrefs.Save();
        }


        public void SetLevel(int newLevel)
        {
            PlayerPrefs.SetInt("Level", newLevel);
            PlayerPrefs.Save();

            Invoke("restartAfterSetLevel", 0.2f);
        }
        private void restartAfterSetLevel()
        {
            HyperGameCont.Instance.RestartGame();

        }





    }
}
