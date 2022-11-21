using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{   
    public class PlayerLocomotion : MonoBehaviour
    {
        enum movementSpeed
        {
            none,
            runningSpeed = 5,
            sprintingSpeed = 7
        };

        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 moveDirection;
        
        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler; 
        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Status")]
        [SerializeField]
        float rotationSpeed = 10;
        public bool IsSprinting;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();
        }

        public void Update()
        {
            inputHandler.TickInput(Time.deltaTime);
            IsSprinting = inputHandler.ifrollSprintInput;
            this.HandleMovement(Time.deltaTime);
            HandleRollingAndSprinting(Time.deltaTime);
        }

        #region Movement
        Vector3 normalVector;
        Vector3 targetPositon;
        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmout;

            //得到旋转的单位方向矢量
            targetDir += cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;
            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = myTransform.forward;
            }
            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }
        #endregion
        public void HandleMovement(float delta)
        {
            //控制方向
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;
            
            if(inputHandler.sprintFlag)
            {
                IsSprinting = true;
                moveDirection *= (int)movementSpeed.sprintingSpeed;
            }
            else
            {
                moveDirection *= (int)movementSpeed.runningSpeed;
            }

            //提供速度
            Vector3 projectVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmout, 0, IsSprinting);
            if (animatorHandler.canRotate)
            {
                HandleRotation(Time.deltaTime);
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (animatorHandler.animator.GetBool("IsInteracting"))
                return;

            if(inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if(inputHandler.moveAmout > 0)
                {
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0;
                    myTransform.rotation = Quaternion.LookRotation(moveDirection);
                    animatorHandler.animator.SetBool("IsInteracting", false);
                }
                else 
                {
                    animatorHandler.PlayTargetAnimation("Crouch_Walk_Back", true);
                }
            }
            if (inputHandler.jumpFlag)
            {
                animatorHandler.PlayTargetAnimation("Jump", true);
            }
        }    
    }
    
}