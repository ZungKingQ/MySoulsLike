using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class PlayerManager : MonoBehaviour
    {
        InputHandler inputHandler;
        Animator animator;
        CameraHandler cameraHandler;
        PlayerLocomotion playerLocomotion;
        public bool isSprinting;
        public bool isInteracting;
        public bool isInAir;
        public bool isGrounded;
        private void Awake()
        {
            cameraHandler = CameraHandler.instance;
        }
        void Start()
        {
            inputHandler = GetComponent<InputHandler>();
            animator = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
        }

        void Update()
        {
            isInteracting = animator.GetBool("IsInteracting");
            
            inputHandler.TickInput(Time.deltaTime);
            this.isSprinting = inputHandler.ifrollSprintInput;
            playerLocomotion.HandleMovement(Time.deltaTime);
            playerLocomotion.HandleRollingAndSprinting(Time.deltaTime);
            playerLocomotion.HandleFalling(Time.deltaTime, playerLocomotion.moveDirection);
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
        }
        private void LateUpdate()
        {
            inputHandler.rollFlag = false;
            inputHandler.sprintFlag = false;
            inputHandler.rb_input = false;
            inputHandler.rt_input = false;
              
            if(isInAir)
            {
                playerLocomotion.intAirTimer += Time.deltaTime;
            }
        }
    }
}