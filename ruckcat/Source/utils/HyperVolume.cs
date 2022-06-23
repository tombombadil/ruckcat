using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using static UnityEngine.ParticleSystem;

namespace Ruckcat {

/*
Bu class bir event (trigger/collision/ray) gerceklestiginde, roketyper gameplayinde zaten var olan actionlarin baslatilmasini (trigger edilmesini) saglar
@Layers -> bir layer filtresi olusturmak icin arraye hangi objelerin trigger olacagini ekleyin. array bos ise filtre uygulanmaztum physics setttingsde tanimli tum objeler trigger olur.

@ChangeCamera -> aktif olan camerayi degistirir
@PlayTutorial -> iligli idye sahip tutorail oynatilir. ilgili tutorail HyperTutorialCont classinda tanimlanmis olmalidir.
*/

[RequireComponent(typeof(BoxCollider))]
public class HyperVolume : HyperSceneObj
{
    [BoxGroup("NoTitle", false)] public ContactType TypeOfContact = ContactType.TRIGGER_ENTER;

    [ShowIfGroup("NoTitle/Curve/TypeOfContact", Value = ContactType.RAY)]
    [BoxGroup("NoTitle/Curve", false)]
    [Tooltip("RayDirectionAndLength hem direction hem uzunlugun ayni vectorle tanimlanmasidir. orn : 0,0,5 vectoru z yonunde ileri dogru 5 birimlik ray gonderecektir")]
    public Vector3 RayLengthAndDirection = new Vector3(0, 0, 1);

    /* base */
    public class BaseAction
    {
        public bool Enabled = false;
         [Tooltip("action'in start delay suresi")]
        public int DelayStart = 0;
         [Tooltip("action'in (eger varsa) end action'inin kac sn sonra baslatilacagi. -1(hic bir zaman end cagrilmaz)")]
        public float Duration = -1;
        [HideInInspector] public bool isStarted;

    }
    /* CHANGE CAMERA */
    [System.Serializable, Toggle("Enabled")]
    public class ActionChangeCam : BaseAction
    {

        public string CamId;
        public float AnimTime = 1;

    }
    public ActionChangeCam ChangeCamera = new ActionChangeCam();

    /* PLAY TUTORIAL */

    [System.Serializable, Toggle("Enabled")]
    public class ActionPlayTutorial : BaseAction
    {
        [Tooltip("HyperTutorialCont icinde daha once tanimlanmis step id")]
        public string StepId;

    }
    public ActionPlayTutorial PlayTutorial = new ActionPlayTutorial();


    /* PLAY PARTICLE */
    [System.Serializable, Toggle("Enabled")]
    public class ActionPlayParticle : BaseAction
    {
        public ParticleSystem Particle;
         [Tooltip("play oldugunda, particle emission degeri")]
        public float EmissionRate;
         [HideInInspector] public float defaultEmission;


    }
    public ActionPlayParticle PlayParticle = new ActionPlayParticle();

    /* END LEVEL */
    [System.Serializable, Toggle("Enabled")]
    public class ActionEndLevel : BaseAction
    {
          [Tooltip("NONE:sadece endlevel cagrilir, result'a levelCont icinde karar verilecek")]
         public GameResult Result;
    }
    public ActionEndLevel EndLevel = new ActionEndLevel();





    public string[] Layers = new string[] { };
    public string[] Tags = new string[] { };
    private BaseAction[] listActions;


    public override void Init()
    {
        base.Init();

         listActions = new BaseAction[] { ChangeCamera,PlayTutorial,PlayParticle,EndLevel  };
    }


    public override void OnContact(ContactInfo contact)
    {
        base.OnContact(contact);
      
        if (!getHasLayerOther(contact.Other.gameObject)) return;
        if (!getOtherHasTag(contact.Other.gameObject)) return;
        Debug.Log("OnContact " + this.name + " - " + contact.Other.name);
        if (TypeOfContact == contact.Type)
        {
            foreach (BaseAction action in listActions)
            {
                if (action.Enabled && !action.isStarted)
                {
                    startAction(action);
                }
            }
        }
    }



    public override void Update()
    {
        base.Update();

        if (TypeOfContact == ContactType.RAY)
        {
            List<RaycastHit> hits = Utils.RAYCAST(transform.position, RayLengthAndDirection, Layers, true);
            if (hits.Count > 0)
            {
                foreach (BaseAction action in listActions)
                {
                    if (action.Enabled && !action.isStarted)
                    {
                        startAction(action);
                    }
                }
            }
        }


    }


    protected virtual void startAction(BaseAction action)
    {
        if (action.Enabled && !action.isStarted)
        {
         
            if (action == ChangeCamera)
            {
                HyperCameraCont.Instance.ChangeCamTo(ChangeCamera.CamId, ChangeCamera.AnimTime, 0);
            }
             if (action == PlayTutorial)
            {
                   Debug.Log("startAction " + PlayTutorial.StepId);
                HyperTutorial.Instance.PlayById(PlayTutorial.StepId);
            }
            if (action == PlayParticle)
            {
               if(PlayParticle.Particle) 
               {
                  EmissionModule emiss = PlayParticle.Particle.emission;
                  PlayParticle.defaultEmission = emiss.rateOverTime.constant;
                  setParticleEmission(PlayParticle.EmissionRate);
                  PlayParticle.Particle.Play();
               }
            }
              if (action == EndLevel)
            {
                HyperLevelCont.Instance.EndLevel(EndLevel.Result);
            }
 

            action.isStarted = true;
            if(action.Duration >= 0)  StartCoroutine(callEndAction(action.Duration));
       

        }
    }

    protected virtual void stopAction(BaseAction action)
    {
        if (action == ChangeCamera)
            {
            }

             if (action == PlayTutorial)
            {
                HyperTutorial.Instance.Stop();
            }
            if (action == PlayParticle)
            {
               if(PlayParticle.Particle) 
               {
                  setParticleEmission(PlayParticle.defaultEmission);
                  PlayParticle.Particle.Stop();
               }
            }
              if (action == EndLevel)
            {
            }
 
    }


      IEnumerator callEndAction(float time)
        {
             yield return new WaitForSeconds(time);
              foreach (BaseAction action in listActions)
                {
                    if (action.Enabled && action.isStarted)
                    {
                        stopAction(action);
                    }
                }
        }


    private bool getHasLayerOther(GameObject _other)
    {
        bool r = true;
        if (Layers.Length > 0)
            r = Layers.Contains(LayerMask.LayerToName(_other.layer));
        return r;

    }
    private bool getOtherHasTag(GameObject _other)
    {
        bool r = true;
        if (Tags.Length > 0)
            r = Tags.Contains(_other.tag);
        return r;

    }
    

    private void setParticleEmission(float value)
    {
        // EmissionModule emiss = PlayParticle.Particle.emission;
        // PlayParticle.defaultEmission = emiss.rateOverTime.constant;
    }



}

}