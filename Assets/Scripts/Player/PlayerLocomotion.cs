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
        PlayerManager playerManager;

        Transform cameraObject;
        InputHandler inputHandler;
        public Vector3 moveDirection;
        
        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler; 
        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Ground & Air Detection Stats")]
        [SerializeField]
        float groundDetectionRayStartPoint = 0.5f;
        [SerializeField]
        float minimumDistranceNeededToBeginFall = 1f;
        [SerializeField]
        float groundDirectionRayDistance = 0.3f;
        LayerMask ignoreForGroundCheck;
        public float intAirTimer;

        [Header("Status")]
        [SerializeField]
        float rotationSpeed = 10;
        [SerializeField]
        float fallingSpeed = 70;


        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerManager = GetComponent<PlayerManager>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();

            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
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
            if (inputHandler.rollFlag)
                return;

            if (playerManager.isInteracting)
                return;

            //控制方向
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = (int)movementSpeed.runningSpeed;

            if(inputHandler.sprintFlag && inputHandler.moveAmout > 0.5f)
            {
                speed = (int)movementSpeed.sprintingSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
            }
            else
            {
                    moveDirection *= speed;
                    playerManager.isSprinting = false;
            }

            //提供速度
            Vector3 projectVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmout, 0, playerManager.isSprinting);
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

        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            // 如果前方0.4f处有碰撞体
            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }
            // 如果在空中，给个重力，以及原运动方向的力以模拟惯性
            if(playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallingSpeed);
                rigidbody.AddForce(moveDirection * fallingSpeed / 10f);
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin -= dir * groundDirectionRayDistance;

            targetPositon = myTransform.position;

            //Debug.DrawRay(origin, -Vector3.up * minimumDistranceNeededToBeginFall, Color.red, 0.1f, false);
            // 当未达到坠落的最小距离时
            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistranceNeededToBeginFall, ignoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPositon.y = tp.y;

                if(playerManager.isInAir)
                {
                    if(intAirTimer > 0.5f)
                    {
                        Debug.Log("int air for" + intAirTimer);
                        animatorHandler.PlayTargetAnimation("Standing_Jump_Running_Landing", true);
                        intAirTimer = 0;
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Empty", false);
                        intAirTimer = 0;
                    }

                    playerManager.isInAir = false;
                }
            }
            // 达到坠落的距离
            else
            {
                if(playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }
                if(playerManager.isInAir == false)
                {
                    if(playerManager.isInteracting == false)
                    {
                        // 坠落动画
                        animatorHandler.PlayTargetAnimation("Standing_Idle_To_Crouch", true);
                    }

                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * ((int)movementSpeed.runningSpeed / 2);
                    playerManager.isInAir = true;
                }
            }

            if(playerManager.isGrounded)
            {
                if (playerManager.isInteracting || inputHandler.moveAmout > 0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPositon, Time.deltaTime);
                }
                else
                {
                    myTransform.position = targetPositon;
                }
            }
        }
    }
    
}