using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ruckcat
{
    public class CoreObject : MonoBehaviour
    {
        [HideInInspector] public bool IsGameStarted;
        [HideInInspector] public GameStatusEvent EventGameStatus = new GameStatusEvent();
        private bool _IsInited;
        [HideInInspector] public bool IsInited { get { return _IsInited; } }


        public virtual void Init()
        {
            GameStatus = GameStatus.INIT;
           
            if (!IsInited)
            {
                _IsInited = true;
                HyperLevelCont.Instance.EventGameStatus.AddListener(onLeveltContHandler);
            }
        }

        public virtual void Load()
        {
            Loaded();
        }
      
        protected virtual void Loaded()
        {
            GameStatus = GameStatus.LOADED;
        }

        public virtual void StartGame()
        {
            GameStatus = GameStatus.STARTGAME;
            IsGameStarted = true;
        }
  
        protected virtual void GameOver(GameResult result)
        {
            GameStatus = GameStatus.GAMEOVER;
            IsGameStarted = false;
        }

        public virtual void Update()
        {
        }


  
        /* GameStatus : oyunun genel akisinin belirlendigi statelerdir. INIT, STARtGAme gibi */
        private GameStatus gameStatus = GameStatus.NONE;
        public GameStatus GameStatus
        {
            get { return gameStatus; }
            set { 
                gameStatus = value;
                EventGameStatus.Invoke(gameStatus);
                if (CoreGameCont.Instance.DebugLogLevel > 1) Debug.Log("BaseObject(" + name + ") gameStatus : " + gameStatus.ToString());

            }
        }



        /* levelcont dinleniyor..*/
        private void onLeveltContHandler(GameStatus _status)
        {
            if(_status == GameStatus.GAMEOVER_FAILED)
            {
                GameOver(GameResult.FAILED);
            }
             if(_status == GameStatus.GAMEOVER_SUCCEED)
            {
                 GameOver(GameResult.SUCCEED);
            }
        }


    }
}
