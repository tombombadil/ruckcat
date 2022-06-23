using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ruckcat
{


    public class HyperSceneObj : CoreSceneObject
    {


        public override void Init()
        {
            base.Init();
        }

        public override void StartGame()
        {
            base.StartGame();

            foreach (CoreSceneObject child in transform.GetComponentsInChildren<CoreSceneObject>())
            {
                if (child != this)
                {
                    child.EventCollision.AddListener(OnContactChild);
                    child.EventTouch.AddListener(OnTouchChild);
                }
            }
        }


        public override void Update()
        {
            base.Update();
        }


        /*--------------------------------------------------------------------------------------------*/
        /*-----------------------------------------| PUBLIC |-----------------------------------------*/
        /*--------------------------------------------------------------------------------------------*/

        public virtual void OnContact(ContactInfo contact)
        {

        }
        public virtual void OnContactChild(ContactInfo contact)
        {

        }

        public override void OnTouch(Ruckcat.Touch touch)
        {
            base.OnTouch(touch);
        }

        public virtual void OnTouchChild(Ruckcat.Touch touch)
        {

        }
        public override void OnDeSpawn()
        {
            base.OnDeSpawn();
        }





        /*--------------------------------------------------------------------------------------------*/
        /*-----------------------------------------| private |-----------------------------------------*/
        /*--------------------------------------------------------------------------------------------*/

        public override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);

            ContactInfo contact = ContactInfo.Create(this, ContactType.COLLISION_ENTER, collision.collider);
            OnContact(contact);
        }


        public override void OnCollisionExit(Collision collision)
        {
            base.OnCollisionExit(collision);
            ContactInfo contact = ContactInfo.Create(this, ContactType.COLLISION_EXIT, collision.collider);
            OnContact(contact);
        }

        public override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);
            ContactInfo contact = ContactInfo.Create(this, ContactType.TRIGGER_ENTER, collider);
            OnContact(contact);
        }

        public override void OnTriggerExit(Collider collider)
        {
            base.OnTriggerExit(collider);
            ContactInfo contact = ContactInfo.Create(this, ContactType.TRIGGER_EXIT, collider);
            OnContact(contact);
        }


        public void OnCollisionStay(Collision collision)
        {
            ContactInfo contact = ContactInfo.Create(this, ContactType.COLLISION_STAY, collision.collider);
            OnContact(contact);
        }

        public void OnTriggerStay(Collider collider)
        {
            ContactInfo contact = ContactInfo.Create(this, ContactType.TRIGGER_STAY, collider);
            OnContact(contact);
        }
    }

}