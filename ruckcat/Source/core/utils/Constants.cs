using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;







namespace Ruckcat
{

    #region ContactStatus / ContactInfo / ContactEvent
    public enum ContactType { TRIGGER_ENTER, TRIGGER_STAY, TRIGGER_EXIT, COLLISION_ENTER, COLLISION_STAY, COLLISION_EXIT, RAY };

    public struct ContactInfo
    {
        public CoreSceneObject Target;
        public ContactType Type;
        public Collider Other;
     
        public static ContactInfo Create(CoreSceneObject _from, ContactType _status, Collider _collider)
        {
            ContactInfo info;
            info.Target = _from;
            info.Type = _status;
            info.Other = _collider;
            return info;
        }
    }
    [System.Serializable]
    public class ContactEvent : UnityEvent<ContactInfo>
    {
    }

    #endregion

    #region GameStatus / GameResult / GameStatusEvent

    public enum GameStatus { NONE, INIT, STARTGAME,  GAMEOVER, GAMEOVER_SUCCEED, GAMEOVER_FAILED, LOAD, LOADED, OPEN, OPENED, CLOSE, CLOSED };
    public enum GameResult { NONE, FAILED, SUCCEED };
        [System.Serializable]
        public class GameStatusEvent : UnityEvent<GameStatus>
        {
        }



    #endregion



    public struct Touch
    {
        public Vector2[] Points; //screen viewport points (0~1)
        public Vector3[] ScenePoints;  //world points
        public int TouchIndex; //finger index for multitouch
        public CoreSceneObject TargetDispatcher;
        public float DeltaTime; //time delta from touch start to touch end
        public TouchPhase Phase;
        public bool IsHitUIElement; //herhangi bir ui elemente touch hit olmussa true doner
        public bool IsHitSceneElement; 
        public void Reset()
        {
            Points = new Vector2[2];
            ScenePoints = new Vector3[2];
        }

      
        /* convert scenepoint(0-1920) to viewport(0-1) point */
        public Vector2 GetPoint(int _index=0)
        {
            Vector2 p = Vector2.zero;
            if (_index < Points.Length)
                p = Points[_index];
            return p;
        }
        public Vector2 GetScreenPoint(int _index = 0)
        {

            return Camera.main.ViewportToScreenPoint(GetPoint(_index));
        }


        /* normal vector (end-start) */
        public Vector2 GetNormal()
        {
            Vector2 normal = Vector2.zero;
            if(Points.Length > 1)
            {
                normal = (GetPoint(1) - GetPoint(0)).normalized;
            }
            return normal;
        }

        /* speed of (endViewport-startViewport).  */
        public float GetSpeed()
        {
            float speed = 0;
            if (Points.Length > 1)
            {
                speed = (GetPoint(1) - GetPoint(0)).magnitude * (1f / DeltaTime );
            }
            return speed;
        }
        public override string ToString() { return "@Points: " + Points.ToString() + ",@ScenePoints: " + ScenePoints.ToString(); }
    }

    public struct TouchUI
    {
        public int Order;  //click depth
        public CoreUI Target;

        public override string ToString() { return "@Target: " + Target.name + ",@Order: " + Order; }
    }

    public class EventTouch : UnityEvent<Touch> {  };
    public class EventTouchUI : UnityEvent<TouchUI> {  };
    public class EventSwipe : UnityEvent<Touch> {  };


}




