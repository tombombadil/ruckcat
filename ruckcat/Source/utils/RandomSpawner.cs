using System;
using System.Collections;
using System.Collections.Generic;
using Ruckcat;
using Shapes2D;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;




[System.Serializable, Toggle("Enabled")]
public class PositionRuleFixed
{
    public bool Enabled;
    [Tooltip("Fixed value")] public float Offset;
    [Tooltip("true-> Prefab icindeki position baz alinir ve offset eklenir, false -> local position olarak direkt offset uygulanir")] public bool UsePrefabValue;
}

[System.Serializable, Toggle("Enabled")]
public class PositionRuleRandom
{
    public bool Enabled;
    [Tooltip("BoxCollider ilgili axis icinde random deger, (BoxCollider gerekli)")] public float Offset;
}

[System.Serializable]
public class PositionRule
{
    public PositionRuleFixed Fixed;
    public PositionRuleRandom Random;
}


public class RandomSpawner : HyperSceneObj
{
    public List<GameObject> Prefabs;
    public int Count = 1;
    public PositionRule X;
    public PositionRule Y;
    public PositionRule Z;
    public GameObject Parent;
    public BoxCollider BoxColliderAsRandom;
  
    public override void Init()
    {
        base.Init();
       
        
        if(!BoxColliderAsRandom)  BoxColliderAsRandom = GetComponentInChildren<BoxCollider>();
        

        for (int i = 0; i < Count; i++)
        {
            int rand = Random.Range(0, Prefabs.Count);
            GameObject prefab = Prefabs[rand];
            spawn(prefab);
        }


      
      
        
     
        
        

    }

    private float getPosition(PositionRule rule, string axis, GameObject prefab)
    {
        float v = 0;
        if (rule.Random.Enabled && BoxColliderAsRandom)
        {
            Vector3 rand =    Utils.GetRandomPositionInBoxCollider(BoxColliderAsRandom, true);
            switch (axis)
            {
                case "x":
                    v = rand.x + rule.Random.Offset;
                    break;
                case "y":
                    v = rand.y + rule.Random.Offset;
                    break;
                case "z":
                    v = rand.z + rule.Random.Offset;
                    break;
            }
        }
        else
        {
            switch (axis)
            {
                case "x":
                    v = rule.Fixed.UsePrefabValue ? prefab.transform.position.x + rule.Fixed.Offset : rule.Fixed.Offset;
                    break;
                case "y":
                    v = rule.Fixed.UsePrefabValue ? prefab.transform.position.y + rule.Fixed.Offset : rule.Fixed.Offset;
                    break;
                case "z":
                    v = rule.Fixed.UsePrefabValue ? prefab.transform.position.z + rule.Fixed.Offset : rule.Fixed.Offset;
                    break;
            }
        }

        return v;
    }

    private void spawn(GameObject prefab)
    {
        if (prefab)
        {
            Vector3 pos = prefab.transform.position;
            pos.x = getPosition(X, "x", prefab);
            pos.y = getPosition(Y, "y", prefab);
            pos.z = getPosition(Z, "z", prefab);

            Transform parentTrans = Parent ? Parent.transform : this.transform;
                HyperSceneObj obj = CoreSceneCont.Instance.SpawnItem<HyperSceneObj>(prefab, parentTrans.transform.position, parentTrans, true);
                Debug.Log("pos " + pos);
                obj.transform.localPosition = pos;
                // obj.transform.localRotation = prefab.transform.rotation;;



        }
    }
}
