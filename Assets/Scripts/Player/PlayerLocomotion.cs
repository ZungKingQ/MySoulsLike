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
        CameraHandler cameraHandler;
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
        float fallingSpeed = 300;

        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
        }
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
            // ����ʱ��������ӽ��ܿ���
            if(inputHandler.lockOn_Flag)
            {
                if(inputHandler.sprintFlag || inputHandler.rollFlag)
                {
                    // ����ʱ��������ӽ��ܿ��ƣ����ʹ��cameraHandler.cameraTransform�ķ��򣬳�Ŀ�귽��������뷭��
                    Vector3 targetDirection = Vector3.zero;
                    targetDirection += cameraHandler.cameraTransform.forward * inputHandler.vertical;
                    targetDirection += cameraHandler.cameraTransform.right * inputHandler.horizontal;
                    targetDirection.Normalize();
                    targetDirection.y = 0;

                    if (targetDirection == Vector3.zero)
                    {
                        targetDirection = transform.forward;
                    }

                    Quaternion tr = Quaternion.LookRotation(targetDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }
                else
                {
                    Vector3 rotationDirection = moveDirection;
                    rotationDirection = cameraHandler.currentLockOnTarget.position - transform.position;
                    rotationDirection.Normalize();
                    rotationDirection.y = 0;
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }
            }
            // ������ʱ����������ת����
            else
            {
                Vector3 targetDir = Vector3.zero;
                float moveOverride = inputHandler.moveAmout;

                //�õ���תλ�õĵ�λ����ʸ��
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
        }
        public void HandleMovement(float delta)
        {
            if (inputHandler.rollFlag)
                return;

            if (playerManager.isInteracting)
                return;

            //���Ʒ���
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

            //�ṩ�ٶ�
            Vector3 projectVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectVelocity;

            if(inputHandler.lockOn_Flag && !inputHandler.sprintFlag)
            {
                // ����״̬�£�������ת���ƣ���˸�horizontal��ֵ�������ҵ�ƽ���ƶ�
                animatorHandler.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal, playerManager.isSprinting);
            }
            else
            {
                // ��HandleRotation������ת���ƣ��������horizontal����Ϊ0
                animatorHandler.UpdateAnimatorValues(inputHandler.moveAmout, 0, playerManager.isSprinting);
            }
            if (animatorHandler.canRotate)
            {
                HandleRotation(Time.deltaTime);
            }
        }
        public void HandleRollingAndSprinting(float delta)
        {
            if (animatorHandler.animator.GetBool("isInteracting"))
                return;
            if(inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;
                if(inputHandler.moveAmout > 0)
                {
                    animatorHandler.PlayTargetAnimation("Stand To Roll", true);
                    moveDirection.y = 0;
                    myTransform.rotation = Quaternion.LookRotation(moveDirection);
                    animatorHandler.animator.SetBool("isInteracting", false);
                }
                else 
                {
                    animatorHandler.PlayTargetAnimation("Crouch_Walk_Back", true);
                }
            }
        }
        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            // ���ǰ��0.4f������ײ��
            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }
            // ����ڿ��У������������Լ�ԭ�˶����������ģ�����
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
            // ��δ�ﵽ׹�����С����ʱ
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
            // �ﵽ׹��ľ���
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
                        // ׹�䶯��
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
            // ��һ�ν���������ƶ�ʱģ�͵Ľ��������
            if (playerManager.isInteracting || inputHandler.moveAmout > 0)
                myTransform.position = Vector3.Lerp(myTransform.position, targetPositon, delta / 0.1f);
            else
                myTransform.position = targetPositon;
        }
        public void HandleJumping()
        {
            if (playerManager.isInteracting)
                return;

            if(inputHandler.jump_Input)
            {
                if(inputHandler.moveAmout >= 0)
                {
                    moveDirection = cameraObject.forward * inputHandler.vertical;
                    moveDirection += cameraObject.right * inputHandler.horizontal;

                    animatorHandler.PlayTargetAnimation("Jump", true);
                    //animatorHandler.PlayTargetAnimation("Standing_Jump", true);
                    moveDirection.y = 0;
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = jumpRotation;
                }
            }
        }



        #endregion
    }

}