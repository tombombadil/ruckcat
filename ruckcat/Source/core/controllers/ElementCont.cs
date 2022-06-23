using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ruckcat
{
    public interface IElement
    {
        string GetElementId();
        ElementCont.EventStatus GetEventElementStatus();
        void SetElementStatus(ElementCont.Status status);

        

    }

    public class ElementCont : MonoBehaviour
    {

        public enum ControllerActionType { COMPONENT, GAMEOBJECT }; //controller'in neyi enable/disable edecegi?
        public enum Status { LOAD, LOADED, OPEN, OPENED, CLOSE, CLOSED };

        //----------- EVENT -------------

        //[System.Serializable]
        public class EventStatus : UnityEvent<IElement, Status>
        {
        }

        //----------- PARAMS -------------
        private List<IElement> elementList;
        private List<IElement> currentList;
        //private IElement[] currentList;
        private IElement current;
        private ControllerActionType actionType;
        private bool isWaitPrevClose; //curr open olmadan once, prev closed beklensin mi?
        public EventStatus Event = new EventStatus();

        //----------- FUNCS -------------
        public void Add(IElement _element)
        {
            if (elementList == null) elementList = new List<IElement>();
            elementList.Add(_element);
            //_element.SetFuncCallbackToController(callbackFromElement);
            setEnableElement(_element, false);

            _element.GetEventElementStatus().AddListener(callbackFromElement);
        }

        public void Init(ControllerActionType _actionTarget, bool _isWaitPrevClose = true)
        {
            actionType = _actionTarget;
            isWaitPrevClose = _isWaitPrevClose;
            currentList = new List<IElement>();
        }
        public IElement Open(string _idElement)
        {
            IElement newElem = GetById(_idElement);
            if (newElem != null)
            {
                //IElement curr = GetCurrent();
                //if (curr != null)
                {
                    if (!currentList.Contains(newElem)) currentList.Insert(0, newElem);



                }


                setStatus(newElem, Status.LOAD);
            }
            return newElem;
        }
        public T Open<T>() where T : IElement
        {
            T newElem = GetByClass<T>();
            if (newElem != null)
            {
                Open(newElem.GetElementId());
            }
            return newElem;
        }

        public void CloseCurrent()
        {
            setStatus(current, Status.CLOSE);

        }

        private void setStatus(IElement _target, Status _newStatus)
        {
            //StartCoroutine(_target.CallbackElementController(_newStatus, callbackFromElement));
            _target.SetElementStatus(_newStatus);
        }
        private void callbackFromElement(IElement _elem, Status _status)
        {
            switch (_status)
            {
                case Status.LOADED:

                    if (currentList.Count == 1)
                    {
                        setStatus(_elem, Status.OPEN);
                        setEnableElement(_elem, true);
                    }
                    else if (currentList.Count > 1)
                    {
                        for (int i = 1; i < currentList.Count; i++)
                        {
                            if (isWaitPrevClose)
                                setStatus(currentList[i], Status.CLOSE);
                        }
                    }
                    break;
                case Status.OPENED:

                    current = _elem;
                    //if (!currentList.Contains(_elem)) currentList.Add(_elem);
                    setEnableElement(current, true);
                    break;

                case Status.CLOSED:

                    if (currentList.Count > 0)
                    {
                        if (currentList.Count > 1)
                        {
                            if (_elem == current) //bir onceki current!
                            {
                                setStatus(currentList[0], Status.OPEN); //open 'da ekledigiimizi acma vakti
                            }
                        }

                        setEnableElement(_elem, false);
                        currentList.Remove(_elem);
                    }

                    break;
            }

            Event.Invoke(_elem, _status);

        }

        private IElement GetCurrent()
        {
            IElement r = null;
            if (currentList.Count > 0)
            {
                r = currentList[currentList.Count - 1];
            }
            return r;
        }

        public IElement GetById(string _id)
        {
            IElement t = (IElement)elementList.Find(e => e.GetElementId() == _id);
            return t;

        }


        public T GetByClass<T>() where T : IElement
        {
            T t = (T)elementList.Find(e => e.GetType() == typeof(T));
            if(t == null) t = (T)elementList.Find(e => e.GetType().BaseType == typeof(T));
            return t;

        }

        private void setEnableElement(IElement _elem, bool _status)
        {
            CoreObject obj = (CoreObject)_elem;
            if (actionType == ControllerActionType.COMPONENT)
                obj.enabled = _status;
            else if (actionType == ControllerActionType.GAMEOBJECT)
                obj.gameObject.SetActive(_status);
        }
        private bool getEnableElement(IElement _elem)
        {
            bool r = false;
            CoreObject obj = (CoreObject)_elem;
            if (actionType == ControllerActionType.COMPONENT)
                r = obj.enabled;
            else if (actionType == ControllerActionType.GAMEOBJECT)
                r = obj.gameObject.activeInHierarchy;

            return r;
        }



    }
}