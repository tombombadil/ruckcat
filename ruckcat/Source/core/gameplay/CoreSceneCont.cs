using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Ruckcat
{
    [System.Serializable]
    public struct PoolPrefab 
    {
        //public string Id;
        public GameObject Prefab;
        public int CountOfSpawnOnStart;
      
    }

  
    public class CoreSceneCont : CoreCont<CoreSceneCont>
    {
        private List<CoreSceneObject> listItems =  new List<CoreSceneObject>();
        public PoolPrefab[] PoolPrefabs;
        private Dictionary<string, PoolCont> listPools = new Dictionary<string, PoolCont>();
        public EventTouch EventOnTouchItem = new EventTouch();


        

        public override void Init()
        {
            for (int i = 0; i < PoolPrefabs.Length; i++)
            {
                PoolPrefab poolPrefab = PoolPrefabs[i];
                CoreSceneObject prefabSceneObj = poolPrefab.Prefab.GetComponent(typeof(CoreSceneObject)) as CoreSceneObject;
                if (prefabSceneObj)
                {
                    string key = poolPrefab.Prefab.name;
                    if (!listPools.ContainsKey(key))
                    {
                        PoolCont pool = new PoolCont();
                        listPools.Add(key, pool);
                    }
                        
                }
                else
                {
                    Debug.LogError("CoreSceneCont : PoolPrefabs icinde CoreSceneObject olmayan bir prefab var!");
                }

            }

            foreach (CoreSceneObject obj in transform.root.GetComponentsInChildren<CoreSceneObject>(true))
            {
                Debug.Log("#obj " + obj.name);
            }
            //listSpawnCounter = new Dictionary<string, int>();
            // foreach (CoreSceneObject go in FindObjectsOfType<CoreSceneObject>() as CoreSceneObject[]) //sahnedeki inactive objeleri almiyor
            // foreach (CoreSceneObject go in GameObject.FindObjectsOfType<CoreSceneObject>(true) as CoreSceneObject[]) 
            // {
                // if (!listItems.Contains(go))
                // {


                    // listItems.Add(go);
                    
                // }
                // go.Init();
                // Debug.Log(go.name + " - init");
            // }
            
            foreach (CoreSceneObject obj in Utils.FindObjectsOfType<CoreSceneObject>() )
            {
                if (!listItems.Contains(obj))
                listItems.Add(obj);
                    

            }

            base.Init();
        }


       
        public override void StartGame()
        {
            base.StartGame();
            //foreach (BaseSceneObject sceneObj in listItems)
            // for (int i = 0; i < listItems.Count; i++)
            // {
            //
            //     CoreSceneObject sceneObj = listItems[i];
            //     if(sceneObj)
            //     {
            //     if (sceneObj.GameStatus != GameStatus.STARTGAME)
            //         sceneObj.StartGame();
            //     }
            //    
            // }
            
        }

        /* Update -> unity update function'i   */
        public override void Update()
        {
            base.Update();
        }

        /* Pool sisteminde spawn edilmis Item'in scene'den kaldirilmasidir. Burada Item Destroy edilmez. scene'de setActive(false) durumuna getirilir */
        public virtual bool DeSpawnItem(CoreSceneObject _item)
        {
            bool r = false;
            if (_item)
            {

                PoolCont pool = getPoolCont(_item.PrefabName);
                if (pool != null)
                {
                    pool.DeSpawn(_item);
                    r = true;
                }

            }

            return r;

        }


        /* Item kalici olarak destroy edilir */
        public virtual bool DestroyItem(CoreSceneObject _item, float _destroyDelay = 0)
        {
            bool r = false;
            if (_item)
            {

                PoolCont pool = getPoolCont(_item.PrefabName);
                if (pool != null)
                {
                    pool.Remove(_item);
                }


                _item.OnPreDestroy();
                if (listItems.Contains(_item)) listItems.Remove(_item);

                if (CoreGameCont.Instance.DebugLogLevel > 0) Debug.Log("Destroy and null ->  " + _item.name);
                Destroy(_item.gameObject, _destroyDelay);

                r = true;
            }
            return r;

        }




        /* Belirtilen class'in scene'deki tum instancelerini List<> halinde doner */
        public List<T> GetItems<T>() where T : CoreSceneObject
        {
            List<T> list = new List<T>();
            //foreach (CoreSceneObject item in listItems)
            //{
            //    //T t = (T)item;
            //    if (item is T)
            //        list.Add(item as T);
            //}
            foreach (T c in FindObjectsOfType<T>() as T[])
            {
                        list.Add(c);


            }
            return list;
        }

        /* 
         * Scene'e bir gameobject spawn eder
         * @_autoRegister :  obje scene sistemine register edilir (CoreSceneObject.Init(),  CoreSceneObject.StartGame() otomatik olarak cagrilir) 
         */
        public T SpawnItem<T>(GameObject Prefab, Vector3 _position, Transform _parent, bool _autoRegister = true) where T : CoreSceneObject
        {
            //if(GetStatus() != GameStatus.STARTGAME)
            //{
            //    Debug.LogError("CoreSceneCont -> oyun baslatilmadan (startGame) SpawnItem cagrilamaz!");
            //    return null;
            //}
            if (Prefab.GetComponent<T>() == null)
            {
                Debug.LogError("SpawnItem -> prefab icinde ilgili class yok.");
                return null;
            }


            GameObject go = null;


            PoolCont pool = getPoolCont(Prefab.name);
            if (pool != null)
            {
                go = pool.Spawn(Prefab).gameObject;
                go.transform.position = _position;
            }
            else
            {
                go = Instantiate(Prefab, _position, Quaternion.identity);
            }
            

            T item = go.GetComponent<T>();
            item.PrefabName = Prefab.name;
            if (_parent != null) item.transform.parent = _parent;

            if (_autoRegister) registerItem(item);
           
       

           
 
          
            OnItemSpawned(item);
            return item;
        }
        

        

        private void registerItem(CoreSceneObject _item)
        { 
            if (!listItems.Contains(_item))
            {
              
                _item.gameObject.name = _item.PrefabName + _item.gameObject.GetInstanceID().ToString();
                listItems.Add(_item);
               if (!_item.IsInited) _item.Init();
               


               CoreObject[] listChild = _item.GetComponentsInChildren<CoreObject>(true);
                   
                foreach (CoreObject childSceneObj in listChild)
                {
                    if (childSceneObj != _item)
                    {

                        if (!childSceneObj.IsInited)
                        {
                            
                            childSceneObj.Init();
                        }
                             
                        
                      
                        
                    }

                }

                if (GameStatus  == GameStatus.STARTGAME)
                {
                    _item.StartGame();
                }
                foreach (CoreObject childSceneObj in listChild)
                {
                    if (childSceneObj != _item)
                    {
                        if (GameStatus == GameStatus.STARTGAME)
                            childSceneObj.StartGame();

                    }

                }
            }
            else
            {
                _item.StartGame();
                CoreSceneObject[] listChild = _item.GetComponentsInChildren<CoreSceneObject>();
                foreach (CoreSceneObject childSceneObj in listChild)
                {
                    if (childSceneObj != _item)
                    {
                            childSceneObj.StartGame();

                    }

                }
            }

        }

        /* spawn sonrasi cagrilir */
        protected virtual void OnItemSpawned(CoreSceneObject _sceneObject)   {  }




        public bool UpdateTouch(Ruckcat.Touch _touch)
        {
            bool isAnyhitResult = false;
            RaycastHit[] hits = getSceneHitsFromScreen(_touch.GetScreenPoint());
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                //RaycastHit hit = hits[0];
                {
                    CoreSceneObject sceneObj = hit.collider.gameObject.GetComponent<CoreSceneObject>();
                    if (sceneObj)
                    {
                        if (sceneObj.IsListenTouchEvent)
                        {
                            _touch.TargetDispatcher = sceneObj;
                            sceneObj.OnTouch(_touch);
                            isAnyhitResult = true;
                            EventOnTouchItem.Invoke(_touch);
                        }

                    }
                }
            }
            return isAnyhitResult;
        }

        /* touch sonrasi world(scene)'de hit olan objeleri(BaseSceneObject) doner */
        private RaycastHit[] getSceneHitsFromScreen(Vector2 _screenPos)
        {
            RaycastHit[] hits = new RaycastHit[0];
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(_screenPos.x, _screenPos.y, 0));

            //List<RaycastResult> uiRayResult = CoreUiCont.Instance.GetRaycastResult(_screenPos);
            //if(uiRayResult.Count == 0)
            hits = Physics.RaycastAll(ray, 100.0F);
            return hits;
        }








        private PoolCont getPoolCont(string prefabName)
        {
            PoolCont r = null;
    
                if (listPools.ContainsKey(prefabName))
                {
                    r = listPools[prefabName];
                }
               
                else

                {
                    r = new PoolCont();
                    listPools.Add(prefabName, r);
                }
                  
          
            return r;
        }

    }





    public class PoolCont
    {
        public List<CoreSceneObject> listCache;
        public bool IsValid = false;

        public PoolCont()
        {
            listCache = new List<CoreSceneObject>();
            IsValid = true;
        }


        public CoreSceneObject Spawn(GameObject Prefab)
        {
            CoreSceneObject item = null;


            if (listCache.Count > 0)
            {
                item = listCache[0];
                listCache.RemoveAt(0);
            }
            else
            {
                GameObject go = CoreSceneCont.Instantiate(Prefab, Vector3.zero, Quaternion.identity);
                item = go.GetComponent<CoreSceneObject>();


            }

            item.gameObject.SetActive(true);


            return item;
        }

        public void DeSpawn(CoreSceneObject _item)
        {
            if (_item)
            {
                if (addToCacheList(_item))
                {
                    _item.OnDeSpawn();
                    _item.gameObject.SetActive(false);
                }

            }


        }
        public void Remove(CoreSceneObject _item)
        {
            if (_item)
            {
                if (listCache.Contains(_item))
                {
                    listCache.Remove(_item);
                }
            }


        }


        private bool addToCacheList(CoreSceneObject _item)
        {
            bool r = false;
            if (_item)
            {
                if (!listCache.Contains(_item))
                {
                    listCache.Add(_item);
                    r = true;
                }


            }

            return r;
        }



    }



}


