using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;




public enum LevelState { START, CHAR_JUMP,   START_PLAY_STAGE, END_PLAY_STAGE, START_FINISH_STATE, END_FINISH_STATE  }

public class EventStartAction : UnityEvent { };
