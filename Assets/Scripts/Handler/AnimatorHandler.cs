using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class AnimatorHandler : MonoBehaviour
    {
        public PlayerManager playerManager;
        public Animator animator;
        public InputHandler inputHandler;
        public PlayerLocomotion playerLocomotion;
        private int vertical;
        private int horizontal;
        public bool canRotate;
        private void Start()
        {
        }
        public void Initialize()
        {
            animator = GetComponent<Animator>();        
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            playerManager = GetComponentInParent<PlayerManager>();
            vertical = Animator.StringToHash("vertical");
            horizontal = Animator.StringToHash("horizontal");
            canRotate = animator.GetBool("canRotate");
        }

        public void UpdateAnimatorValues(float verticalMovement,float horizontalMovement,bool IsSprinting)
        {
            #region 限制vertical
            float v = 0;
            if(verticalMovement > 0 && verticalMovement < 0.55f)
                v = 0.5f;
            else if (verticalMovement > 0.55f)
                v = 1;
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
                v = -0.5f;
            else if (verticalMovement < -0.55f)
                v = -1;
            else 
                v = 0;
            #endregion
            #region 限制horizontal
            float h = 0;
            if(horizontalMovement > 0 && horizontalMovement < 0.55f)
                h = 0.5f;
            else if (horizontalMovement > 0.55f)
                h = 1;
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
                h = -0.5f;
            else if (horizontalMovement < -0.55f)
                h = -1;
            else
                h = 0;
            #endregion

            if(IsSprinting)
            {
                v = 2;
                h = horizontalMovement;
            }

            animator.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            animator.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
            this.CanRotate();
        }
        public void CanRotate()
        {
            canRotate = true;
        }
        public void StopRotate()
        {
            canRotate = false;
        }
        public void CanCombo()
        {
            animator.SetBool("canDoCombo", true);
        }
        public void CantCombo()
        {
            animator.SetBool("canDoCombo", false);
        }    
        public void PlayTargetAnimation(string targetAnim,bool IsInteracting)
        {
            animator.applyRootMotion = IsInteracting;
            animator.SetBool("IsInteracting", IsInteracting);
            //在0.2s内完成动作的衔接
            animator.CrossFade(targetAnim, 0.2f);
        }
        private void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false)
                return;

            playerLocomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / Time.deltaTime;
            playerLocomotion.rigidbody.velocity = velocity;
        }
    }
}