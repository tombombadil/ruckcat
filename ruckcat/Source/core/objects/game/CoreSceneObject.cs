using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;




namespace Ruckcat
{
    public class CoreSceneObject : CoreObject
    {

        [FoldoutGroup("Core", expanded: false), PropertyOrder(99)] [Tooltip("bir collider component varsa, touch eventlerini yakalar")] public bool IsListenTouchEvent = false;
        [FoldoutGroup("Core", expanded: false), PropertyOrder(99)] [Tooltip("collision olmus obje listesini saklar")] public bool IsStoreCollisions = false;
        [FoldoutGroup("Core", expanded: false), PropertyOrder(99)] [Tooltip("collision olmus obje listesini saklar")] public bool IsStoreTriggers = false;
        [HideInInspector] public Rigidbody rb;
        protected CoreSceneObject parent;
        protected GameObject gMesh;
        [HideInInspector] public ContactEvent EventCollision = new ContactEvent();
        [HideInInspector] public EventTouch EventTouch = new EventTouch();
        [HideInInspector] public string PrefabName;
        private List<Collider> listTriggers;
        private List<Collider> listCollisions;
       


        #region RoketGame - GamePlay Functions [Init / StartGame / StartLevel / Update /  OnDeSpawn / ]
        public override void Init()
        {
            if (!IsInited)
            {

                enabled = false;

                if (transform.parent != null) parent = transform.parent.gameObject.GetComponent < CoreSceneObject > ();
                rb = GetComponent<Rigidbody>();

                listTriggers = new List<Collider>();
                listCollisions = new List<Collider>();

                base.Init();
            }


        }


        public override void StartGame()
        {
            base.StartGame();
            enabled = true;
            if (CoreGameCont.Instance.DebugLogLevel > 1) Debug.Log("BaseGameObject(" + name + ").StartGame");
        }



        public override void Update()
        {
            base.Update();
        }






        #endregion

        /*--------------------------------------------------------------------------------------------*/
        /*-----------------------------------------| PUBLIC |-----------------------------------------*/
        /*--------------------------------------------------------------------------------------------*/


        /* destroy oncesi cagrilir */
        public virtual void OnPreDestroy()
        {
            listTriggers.Clear();
            EventGameStatus.RemoveAllListeners();
        }

        /* CoreSceneCont pool sisteminde, obje deSpawn (hala sahnede ama deactive) oldugunda */
        public virtual void OnDeSpawn()
        {

        }

        /* TOUCH EVENTS (call from InputCont) */
        public virtual void OnTouch(Ruckcat.Touch _info)
        {
            EventTouch.Invoke(_info);
        }

        public List<Collider> GetCollisionList()
        {
            return listCollisions;
        }

        public List<Collider> GetTriggerList()
        {
            return listTriggers;
        }


        /*--------------------------------------------------------------------------------------------*/
        /*-----------------------------------------| private |-----------------------------------------*/
        /*--------------------------------------------------------------------------------------------*/

        public virtual void OnCollisionEnter(Collision collision)
        {
            if (IsStoreCollisions)
            {

                Collider foundInList = FindColliderInList(listCollisions, collision.collider);
                if (foundInList == null)
                {
                    listCollisions.Add(collision.collider);
                    //Debug.Log(name + ".collision(+) to : " + collision.collider.gameObject.ToString());
                    if (CoreGameCont.Instance.DebugLogLevel > 0) Debug.Log("+listCollisions.count " + listCollisions.Count);
                }

            }
            EventCollision.Invoke(ContactInfo.Create(this, ContactType.COLLISION_ENTER, collision.collider));
        }
        public virtual void OnCollisionExit(Collision collision)
        {
            if (IsStoreCollisions)
            {

                Collider foundInList = FindColliderInList(listCollisions, collision.collider);
                if (foundInList != null)
                {
                    listCollisions.Remove(collision.collider);
                    if (CoreGameCont.Instance.DebugLogLevel > 0) Debug.Log("-listCollisions.count " + listCollisions.Count);
                    //Debug.Log(name + "-collision(-) from : " + collision.collider.gameObject.ToString());
                }
            }

            EventCollision.Invoke(ContactInfo.Create(this, ContactType.COLLISION_EXIT, collision.collider));
        }

        public virtual void OnTriggerEnter(Collider other)
        {


            if (IsStoreTriggers)//to-do : destroy edilmis obje buraya takilmasin diye ekledim. ama destroy edilmis go'ler hala listede tutuluyor!
            {

                Collider foundInList = FindColliderInList(listTriggers, other);
                if (foundInList == null)
                {
                    listTriggers.Add(other);
                    if (CoreGameCont.Instance.DebugLogLevel > 0) Debug.Log(name + ".trigger(+) to : " + other.gameObject.ToString());
                }

            }

            EventCollision.Invoke(ContactInfo.Create(this, ContactType.TRIGGER_ENTER, other));
        }

        public virtual void OnTriggerExit(Collider other)
        {
            if (IsStoreTriggers)
            {

                //Collider foundInList = listTriggers.Find(c => c == other);
                Collider foundInList = FindColliderInList(listTriggers, other);
                if (foundInList != null)
                {
                    listTriggers.Remove(foundInList);
                    if (CoreGameCont.Instance.DebugLogLevel > 0) Debug.Log(name + "-trigger(-) from : " + other.gameObject.ToString());
                }
            }
            EventCollision.Invoke(ContactInfo.Create(this, ContactType.TRIGGER_EXIT, other));
        }

        /* hem objeyi bulmak hem de destroy olmus null objeleri listeden temizlemek icin */
        private Collider FindColliderInList(List<Collider> _list, Collider _collider)
        {
            Collider r = null;

            //int countList = _list.Count;
            for (int i = 0; i < _list.Count; i++)
            {
                Collider collider = _list[i];
                if (collider != null)
                {
                    if (collider.gameObject != null)
                    {
                        if (collider.gameObject == _collider.gameObject)
                            r = collider;
                    }
                    else
                    {
                        _list.RemoveAt(i);
                    }
                }
                else
                {
                    _list.RemoveAt(i);
                }

            }
            return r;

        }









        public void SetLayerName(string _layer)
        {
            gameObject.layer = LayerMask.NameToLayer(_layer);
        }
        public string GetLayerName()
        {
            return LayerMask.LayerToName(gameObject.layer);
        }


    }
}