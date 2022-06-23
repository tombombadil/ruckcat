using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ruckcat
{

[System.Serializable]
public class HyperConfig
{
    private static HyperConfig _ins;
    public static HyperConfig Instance { get { if (_ins == null) _ins = new HyperConfig(); return _ins; } }


    private bool _IsVibration;
    public bool IsVibration
    {
        get {
            if (!PlayerPrefs.HasKey("Vibration"))
            {
                IsVibration = true;
            }
           
                return (PlayerPrefs.GetInt("Vibration") != 0); }
        set { 
            _IsVibration = value;
            
            PlayerPrefs.SetInt("Vibration", _IsVibration ? 1:0);

        }
    }


    private float _Score;
    public float Score
    {
        get
        {
            if (PlayerPrefs.HasKey("Score"))
            {
                _Score = PlayerPrefs.GetFloat("Score");
            }

            return _Score;
        }
        set
        {

            _Score = value;
            PlayerPrefs.SetFloat("Score", _Score);
            HyperPageGame pageGame = ((HyperUICont)CoreUiCont.Instance).GetPageGame();
            if (pageGame != null)
                pageGame.ViewScore(_Score);


        }
    }


    private bool _flyTutorial;
    public bool flyTutorial
    {
        get
        {
            if (!PlayerPrefs.HasKey("ShotEnemyTutorial"))
            {
                flyTutorial = true;
            }

            return (PlayerPrefs.GetInt("ShotEnemyTutorial") != 0);
        }
        set
        {
            _flyTutorial = value;

            PlayerPrefs.SetInt("ShotEnemyTutorial", _flyTutorial ? 1 : 0);

        }
    }

    private bool _ShotWallTutorial;
    public bool ShotWallTutorial
    {
        get
        {
            if (!PlayerPrefs.HasKey("ShotWallTutorial"))
            {
                ShotWallTutorial = true;
            }

            return (PlayerPrefs.GetInt("ShotWallTutorial") != 0);
        }
        set
        {
            _ShotWallTutorial = value;

            PlayerPrefs.SetInt("ShotWallTutorial", _ShotWallTutorial ? 1 : 0);

        }
    }
    
    private bool _ShotEnemyTutorial;
    public bool ShotEnemyTutorial
    {
        get
        {
            if (!PlayerPrefs.HasKey("ShotEnemyTutorial"))
            {
                ShotEnemyTutorial = true;
            }

            return (PlayerPrefs.GetInt("ShotEnemyTutorial") != 0);
        }
        set
        {
            _ShotEnemyTutorial = value;

            PlayerPrefs.SetInt("ShotEnemyTutorial", _ShotEnemyTutorial ? 1 : 0);

        }
    }
    
    

    private bool _ShotPlaneTutorial;
    public bool ShotPlaneTutorial
    {
        get
        {
            if (!PlayerPrefs.HasKey("ShotPlaneTutorial"))
            {
                ShotPlaneTutorial = true;
            }

            return (PlayerPrefs.GetInt("ShotPlaneTutorial") != 0);
        }
        set
        {
            _ShotPlaneTutorial = value;

            PlayerPrefs.SetInt("ShotPlaneTutorial", _ShotPlaneTutorial ? 1 : 0);

        }
    }
        private bool _ShotTrapTutorial;
    public bool ShotTrapTutorial
    {
        get
        {
            if (!PlayerPrefs.HasKey("ShotTrapTutorial"))
            {
                ShotTrapTutorial = true;
            }

            return (PlayerPrefs.GetInt("ShotTrapTutorial") != 0);
        }
        set
        {
            _ShotTrapTutorial = value;

            PlayerPrefs.SetInt("ShotTrapTutorial", _ShotTrapTutorial ? 1 : 0);

        }
    }

}

}