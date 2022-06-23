using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ruckcat;

public class TutorialPreset : CoreUI
{

    
    public enum  RuleOfHide
    {
        NONE,
        HIDE_ON_STARTGAME
    }
    public int ShowUntilTheLevel = 1; //ilgili level dahil, o levela kadar gosterilir.
    public RuleOfHide HideRule;

    public override void Init()
    {
      base.Init();

      gameObject.SetActive((LevelCont.Instance.CurrLevel <= ShowUntilTheLevel));

    }
  public override void StartGame()
  {
      base.StartGame();

      if(HideRule == RuleOfHide.HIDE_ON_STARTGAME)
        gameObject.SetActive(false);
  }
}
