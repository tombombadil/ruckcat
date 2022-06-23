using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ruckcat;
using TMPro;
using System;
using UnityEngine.UI;

namespace Ruckcat
{


    [Serializable]
    public class NotifyElement
    {
        public string Id;
        public List<GameObject> Prefabs;

        public List<GameObject> listGameobjects = new List<GameObject>();
    }

    public class Notify : CoreCont<Notify>
    {
        public List<NotifyElement> ListElements = new List<NotifyElement>();

        private NotifyElement getElement(string elementId, int prefabIndex)
        {
            NotifyElement element = null;
            NotifyElement elem = ListElements.Find(e => e.Id == elementId);
            if (elem != null)
            {
                if (prefabIndex < elem.Prefabs.Count)
                    element = elem;
            }
            return element;
        }

        // TextPrefabs listesine eklenen tüm prefabları oluşturur (StartGame)
        private GameObject addNewElem(Vector3 startPos, string idElement, int indexPrefab)
        {
            GameObject itemGo = null;
            RectTransform _parent = CoreUiCont.Instance.GetPageByClass<HyperPageGame>().GetComponent<RectTransform>();
            RectTransform PrefabRect = null;
            NotifyElement element = getElement(idElement, indexPrefab);
            if (element != null)
            {
                CoreSceneObject item = CoreSceneCont.Instance.SpawnItem<CoreSceneObject>(element.Prefabs[indexPrefab], Vector3.zero, _parent);
                itemGo = item.gameObject;
                item.StartGame();
                element.listGameobjects.Add(item.gameObject);
                item.gameObject.SetActive(false);
                PrefabRect = item.GetComponent<RectTransform>();
            }

            PrefabRect.SetParent(_parent);
            /*PrefabRect.anchoredPosition = _parent.position;
            PrefabRect.anchorMin = new Vector2(0, 0);
            PrefabRect.anchorMax = new Vector2(0, 0);
            PrefabRect.pivot = new Vector2(0.5f, 0.5f);*/
            PrefabRect.localScale = new Vector3(1, 1, 1);
            /*PrefabRect.sizeDelta = _parent.rect.size;
            PrefabRect.localPosition = startPos;
            PrefabRect.localRotation = Quaternion.identity;*/
            return itemGo;
        }

        // scene (3d) pointe gore ui'a notify text cikarir
        public void NotifyTextFromWorldPoint(string id, int index, string text, Vector3 startPosition, Vector3 endPosition, float animTime)
        {
            // System.Func<float, float> _tweenScaleFunc = tweenScaleFunc ?? TweenScaleFunctions.SineEaseIn;

            NotifyTextFromScreenPoint(id, index, text, Camera.main.WorldToScreenPoint(startPosition), Camera.main.WorldToScreenPoint(endPosition), animTime);
        }





        // viewport (ui) pointe gore ui'a notify cikarir

        /// <summary>
        /// *** DESCRIPTION *** 
        /// <para>Default Index : 0 </para>
        /// <para>Default Tween Scale Func : SineEaseIn </para>
        /// </summary>
        /// 
        public void NotifyTextFromScreenPoint(string id, int index, string text, Vector3 startPosition, Vector3 endPosition, float animTime, System.Func<float, float> tweenScaleFunc = null)
        {
            // System.Func<float, float> _tweenScaleFunc = tweenScaleFunc ?? TweenScaleFunctions.SineEaseIn;
            TextMeshProUGUI _text;
            int _index = index;

            GameObject newItem = addNewElem(Vector3.zero, id, index);
            if (newItem)
            {
                _text = newItem.GetComponentInChildren<TextMeshProUGUI>();
                _text.text = text/* + " $"*/;
                TweenMove(newItem, startPosition, endPosition, animTime);
            }

        }

        private void TweenMove(GameObject _gameObject, Vector3 startPosition, Vector3 endPosition, float animTime)
        {


            LeanTween.value(0, 1, animTime).setDelay(0).setEase(LeanTweenType.easeInExpo).setOnUpdate((float value) =>
             {
                 {//update transform

                     Vector3 startPos = startPosition;
                    Vector3 endPos = endPosition;
                    startPos.z = endPos.z = 0.0f;
                     _gameObject.GetComponent<RectTransform>().position = startPos + ((endPos - startPos) * value);


                }
                 {//update color

                     _gameObject.SetActive(true);
                     TextMeshProUGUI _text = _gameObject.GetComponentInChildren<TextMeshProUGUI>();
                     _text.color = new Color(
                    _text.color.r,
                    _text.color.g,
                    _text.color.b,
                    (1 - 1 * value)
                    );
                     if (_gameObject.GetComponentInChildren<Image>())
                     {
                         _gameObject.GetComponentInChildren<Image>().color = new Color(
                        _gameObject.GetComponentInChildren<Image>().color.r,
                        _gameObject.GetComponentInChildren<Image>().color.g,
                        _gameObject.GetComponentInChildren<Image>().color.b,
                        (1 - 1 *value)
                        );
                     }
                 }
                

             }).setOnComplete(() =>
             {
                 _gameObject.GetComponent<RectTransform>().position = startPosition;
                 _gameObject.SetActive(false);

             });
 
        }


    }

}
