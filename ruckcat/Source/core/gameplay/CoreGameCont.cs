using System.Collections;
using HighlightPlus;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ruckcat
{

    [RequireComponent(typeof(CoreSceneCont))]
    public class CoreGameCont : CoreCont<CoreGameCont>
    {
        [FoldoutGroup("Core",expanded: false), PropertyOrder(99)] [Tooltip("DebugMode > 1 durumunda debug panel aktif olur")] public int DebugMode = 0;
        [FoldoutGroup("Core",expanded: false), PropertyOrder(99)]  [Tooltip("Ruckcat core debug loglarin gösterilmesi icin DebugLogLevel > 0 olmalı ")]public int DebugLogLevel = 0;
        [FoldoutGroup("Core",expanded: false), PropertyOrder(99)] [Tooltip(" CoreSceneCont.SpawnItem() 'in pooling ozelligini kullanip kullanmayacagi ")] public bool UsingPoolSpawning = true;
        
        [FoldoutGroup("Core"), PropertyOrder(99)] public string Version = "beta v1.0";
        [HideInInspector] public bool IsStartedGame;
        [HideInInspector] public CoreSceneCont sceneCont;
        [HideInInspector] public CoreUiCont uiCont;
        private CoreInputCont inputCont;

        void Awake()
        {
            Init();
        }


        public override void Init()
        {
            base.Init();


            inputCont = GetComponent<CoreInputCont>();
            if (inputCont == null) inputCont = gameObject.AddComponent<CoreInputCont>();
            if (GetComponent<DebugScreen>() == null) gameObject.AddComponent<DebugScreen>();
            uiCont = FindObjectOfType<CoreUiCont>();
            sceneCont = FindObjectOfType<CoreSceneCont>();



            if (!uiCont) Debug.LogWarning("UiCont not found!");
            if (!sceneCont) Debug.LogError("SceneCont not found!");
            if (!sceneCont) Debug.LogError("InputCont not found!");

            // inputCont.Init();



            uiCont.EventGameStatus.AddListener(uiContOnHandler);
            uiCont.Init();

            sceneCont.Init();


            uiCont.Load(); //trigger!

            //if (Application.platform = RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            if (DebugMode < 2)
                FindObjectOfType<DebugScreen>().enabled = false;
           

            // foreach (CoreObject obj in FindObjectsOfType<CoreObject>() as CoreObject[])
            // {
            //     if (obj.GameStatus == GameStatus.NONE)
            //     {
            //         obj.Init();
            //     }
            //
            // }
            
            foreach (CoreObject obj in Utils.FindObjectsOfType<CoreObject>() )
            {
                if (!obj.IsInited)
                {
                    obj.Init();
                }

            }
        }


        protected virtual void uiContOnHandler(GameStatus _status)
        {
            if (_status == GameStatus.LOADED)
                StartGame();
        }

        public override void StartGame()
        {


            IsStartedGame = true;
            base.StartGame();
            // sceneCont.StartGame();
            // uiCont.StartGame();
            
            foreach (CoreObject obj in Utils.FindObjectsOfType<CoreObject>() )
            {
                if (!obj.IsGameStarted )
                {
                    obj.StartGame();
                }

            }

        }

        private bool isRestartingGame = false;
        public virtual void RestartGame()
        {
            if (!isRestartingGame)
            {
                isRestartingGame = true;
                LeanTween.cancelAll();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
                
        }




    }
}