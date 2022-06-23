using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ruckcat;
using Sirenix.OdinInspector;


namespace Ruckcat
{

    public class Char : HyperSceneObj
    {
        protected Animator animator;
       
        private Vector3 lastPos;
        private float health = 1;
      

        public override void Init()
        {
           base.Init();
           lastPos = transform.position;
           animator = GetComponentInChildren<Animator>();
          
        }

        public override void StartGame()
        {
            base.StartGame();

        }

        public virtual void FixedUpdate()
        {
            float speed = GetVelocity().magnitude;
            animator.SetFloat("Speed", speed);
            animator.SetFloat("Health", Health);
        }

       
        public virtual Vector3 GetVelocity()
        {
            Vector3 vel = transform.position - lastPos;
            lastPos = transform.position;
            return vel;
        }
        
        public float Health
        {
            get { return health;}
            set { health = value; }
        }

  
        public override void Update()
        {
            base.Update();


        }
       

    }
}