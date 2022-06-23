using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ruckcat
{
    public class Utils
    {
        private static Dictionary<ParticleSystem, float> storedParticleEmission = new Dictionary<ParticleSystem, float>();

        public static void SetParticle(ParticleSystem particle, bool enable)
        {
            if (particle)
            {
                if (enable)
                    particle.Play();
                else
                    particle.Stop();

            }

        }

        public static List<T> FindObjectsOfType<T>()
        {
            List<T> results = new List<T>();
            for(int i = 0; i< SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                // if (s.isLoaded)
                {
                    var allGameObjects = s.GetRootGameObjects();
                    for (int j = 0; j < allGameObjects.Length; j++)
                    {
                        var go = allGameObjects[j];
                        results.AddRange(go.GetComponentsInChildren<T>(true));
                    }
                }
            }
            return results;
        }

        public static void SetParticleEmissionRate(ParticleSystem particle, float rate)
        {
            if (particle)
            {
                ParticleSystem.EmissionModule emission = particle.emission;
                if (!storedParticleEmission.ContainsKey(particle))
                {
                    storedParticleEmission[particle] = emission.rateOverTime.constant;
                }

                ParticleSystem.MinMaxCurve curve = emission.rateOverTime;
                curve.constant = rate;
                Debug.Log("rate " + rate);
                emission.rateOverTime = curve;

            }

        }

        public static void ResetParticleEmissionRate(ParticleSystem particle)
        {
            if (particle)
            {

                if (storedParticleEmission.ContainsKey(particle))
                {
                    ParticleSystem.EmissionModule emission = particle.emission;
                    ParticleSystem.MinMaxCurve curve = emission.rateOverTime;
                    curve.constant = storedParticleEmission[particle];
                }



            }

        }


        public static List<T> FindChildrenComponentsByTag<T>(GameObject _parent, string _tag) where T : Component
        {
            List<T> result = new List<T>();

            T[] childrenComp = _parent.GetComponentsInChildren<T>();
            foreach (T comp in childrenComp)
            {
                if (comp.transform.tag == _tag)
                {
                    result.Add(comp);
                }
            }
            return result;
        }

        public static T GetGameObjectClass<T>(string _gameObjectName)
        {
            GameObject go = (GameObject)GameObject.Find(_gameObjectName);
            T _comp = default(T);
            if (go)
            {
                _comp = (T)go.GetComponent<T>();
            }
            return _comp;
        }

        public static Vector3 TO_V3(Vector2 _vec)
        {
            return new Vector3(_vec.x, _vec.y, 0);
        }
        public static Vector2 TO_V2(Vector3 _vec)
        {
            return new Vector2(_vec.x, _vec.y);

        }
        public static float RANDOM_FLOAT_CLAMP(float _from, float _to, float _min, float _max)
        {
            float min = Mathf.Max(_from, _min);
            float max = Mathf.Min(_to, _max);
            return UnityEngine.Random.Range(min, max);

        }
        public static List<T> TO_LIST<T>(T[] arr)
        {
            List<T> list = new List<T>();
            foreach (T e in arr)
            {
                list.Add(e);
            }
            return list;
        }

        public static List<RaycastHit> RAYCAST(Vector3 _start, Vector3 _length, string[] layerNames, bool _debugDraw = true)
        {
            RaycastHit[] hits;
            if (layerNames.Length > 0)
                hits = Physics.RaycastAll(_start, _length.normalized, _length.magnitude, LayerMask.GetMask(layerNames), QueryTriggerInteraction.UseGlobal);
            else
                hits = Physics.RaycastAll(_start, _length.normalized, _length.magnitude);
            Debug.DrawLine(_start, _start + _length, Color.gray);

            Vector3 lastHit = _start;
            Color[] colors = new Color[] { Color.red, Color.blue, Color.yellow, Color.cyan };
            int indexColor = 0;
            foreach (RaycastHit r in hits)
            {
                Debug.DrawLine(lastHit, r.point, colors[indexColor]);
                indexColor++;
                if (indexColor >= colors.Length) indexColor = 0;
            }
            return TO_LIST<RaycastHit>(hits);
        }


        public static void DebugDrawRect(Vector3 position, float _size, Color color)
        {
            Debug.DrawLine(position, new Vector3(position.x + _size, position.y, position.z), color);
            Debug.DrawLine(position, new Vector3(position.x, position.y - _size, position.z), color);
            Debug.DrawLine(new Vector3(position.x, position.y - _size, position.z), new Vector3(position.x + _size, position.y - _size, position.z), color);
            Debug.DrawLine(new Vector3(position.x + _size, position.y - _size, position.z), new Vector3(position.x + _size, position.y, position.z), color);
        }


        public static Vector3 GetRandomPositionInBoxCollider(BoxCollider _box, bool isLocal=false)
        {
            Bounds bounds = _box.bounds;
            Vector3 min = isLocal?-bounds.extents : bounds.center - bounds.extents;
            Vector3 max = isLocal?bounds.extents : bounds.center + bounds.extents;
            return GetRandomPositionInVector(new Vector3[] { min, max });
        }



        public static Vector3 GetRandomPositionInVector(Vector3[] _ranges)
        {
            Vector3 min = Vector3.zero;
            Vector3 max = Vector3.zero;
            if (_ranges.Length > 0) min = _ranges[0];
            if (_ranges.Length > 1) max = _ranges[1];
            float x = UnityEngine.Random.Range(min.x, max.x);
            float y = UnityEngine.Random.Range(min.y, max.y);
            float z = UnityEngine.Random.Range(min.z, max.z);
            return new Vector3(x, y, z);
        }



        public static string ValueRounding(int value, string suffix=" k")
        {
            float roundedValue;
            string result;
        
            if (value >= 1000)
            {
                roundedValue = Mathf.Floor(value / 100) / 10;
                result = roundedValue.ToString() + suffix;
            }
            else if (value <= -1000)
            {
                roundedValue = Mathf.Floor(value / 100) / 10;
                result = roundedValue.ToString() + suffix;
            }
            else
            {
                result = value.ToString();
            }
        
            return result;
        }

    }

}