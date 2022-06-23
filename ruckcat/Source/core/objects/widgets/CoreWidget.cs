using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ruckcat
{

    /* widget sahnede bir SceneObject olarak davranir. Bu haliyle bir SceneObjecttir ama icindeki canvas ve elementleri ui elementi gibi davranir */
    [RequireComponent(typeof(Canvas))]
    public class CoreWidget : CoreSceneObject
    {
        private Canvas canvas;
        // private Vector3 defalultRot;

        public override void Init()
        {
            base.Init();

            canvas = GetComponentInChildren<Canvas>();
            // defalultRot = transform.localRotation.eulerAngles;

            foreach (CoreUI coreui in GetComponentsInChildren<CoreUI>(true))
            {
                if (coreui.GameStatus == GameStatus.NONE)
                {
                    coreui.Init();
                }
            }

        }

        public override void Update()
        {
            base.Update();
            
            this.transform.LookAt(Camera.main.transform);
            
        }


        public override void OnTouch(Ruckcat.Touch _info)
        {
            base.OnTouch(_info);
        }
    }
}