using Ruckcat;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ruckcat
{
    [RequireComponent(typeof(HyperLevelCont))]
    public class HyperGameCont : CoreGameCont
    {

        [TitleGroup("Main"), PropertyOrder(0)] [Tooltip("Ilgili level baslatilir. DebugLevel == -1 ise playerprefs daki son kalinan leveldan baslar.")] public int DebugLevel = -1; 
        [FoldoutGroup("Core"), PropertyOrder(99)] [Tooltip("Farkli fpsleri simulate etmek icin kullanılır")] public int SimulateFPS = 0;

        [HideInInspector] public HyperLevelCont Level;


        public override void Init()
        {
#if UNITY_EDITOR
            if (SimulateFPS > 0)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = SimulateFPS;
            }

#endif
            Level = GetComponent<HyperLevelCont>();
            if (DebugLevel > -1)
            {
                Level.CurrLevel = DebugLevel;
            }
            else
            {
                Level.RestoreLevel();
            }

            if (HyperCameraCont.Instance) HyperCameraCont.Instance.Init();


            Level.Init();


            base.Init();
            Level.InitLevel();
        }


        /* CoreGameCont'de normalde burada oyunu baslatiyoruz. HyperGameCont ise burada pageGame'den start'in tetiklenmesini bekleyecek. */
        protected override void uiContOnHandler(GameStatus _status)
        {
            if (_status == GameStatus.LOADED)
            {
                ((HyperUICont)CoreUiCont.Instance).OpenPageGame().EventStart.AddListener(onStarCalledFromPageGame);
            }
        }

        /* pageGame'den gelen start, daha sonra levelCont'dan ayni isimli event'in donusunu bekliyoruz */
        protected virtual void onStarCalledFromPageGame()
        {
            HyperLevelCont.Instance.EventStart.AddListener(onStarCalledFromLevelCont);
            Level.startLevel();
        }

        protected virtual void onStarCalledFromLevelCont()
        {
            StartGame();
        }

        public override void StartGame()
        {
            base.StartGame();

        }

      


        public override void Update()
        {
            base.Update();

        }
    }
}