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
        InteractableUI interactableUI;

        public GameObject interactionPopUp;
        public GameObject itemPopUp;

        public bool isSprinting;
        public bool isInteracting;
        public bool isInAir;
        public bool isGrounded;
        public bool canDoCombo;
        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
            interactableUI = FindObjectOfType<InteractableUI>();
        }
        void Start()
        {
            inputHandler = GetComponent<InputHandler>();
            animator = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
        }

        void Update()
        {
            float delta = Time.fixedDeltaTime;

            isInteracting = animator.GetBool("isInteracting");
            canDoCombo = animator.GetBool("canDoCombo");
            animator.SetBool("isInAir", isInAir);
            inputHandler.TickInput(delta);
            this.isSprinting = inputHandler.ifrollSprintInput;

            playerLocomotion.HandleRollingAndSprinting(delta);
            playerLocomotion.HandleJumping();

            CheckForInteractableObject();
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
        }
        private void LateUpdate()
        {
            float delta = Time.fixedDeltaTime;

            inputHandler.rollFlag = false;
            inputHandler.a_Input = false;
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
            inputHandler.jump_Input = false;
            inputHandler.inventoryUI_Input = false;
            inputHandler.equitmentUI_Input = false;
            inputHandler.d_Pad_Up = false;
            inputHandler.d_Pad_Down = false;
            inputHandler.d_Pad_Left = false;
            inputHandler.d_Pad_Right = false;
            
            if(isInAir)
            {
                playerLocomotion.intAirTimer += Time.deltaTime;
            }

            
        }
        public void CheckForInteractableObject()
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers))
            {
                if(hit.collider.tag == "Interactable")
                {
                    Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                    if(interactableObject != null)
                    {
                        string interactableText = interactableObject.interactableText;
                        interactableUI.interactableText.text = interactableText;
                        interactionPopUp.SetActive(true);

                        if(inputHandler.a_Input)
                        {
                            hit.collider.GetComponent<Interactable>().Interact(this);
                            StartCoroutine(InteractFading());
                        }
                    }
                }
            }
            else
            {
                if(interactionPopUp != null)
                {
                    interactionPopUp.SetActive(false);
                }
                //if (itemPopUp != null)
                //{
                //    itemPopUp.SetActive(false);
                //}
            }
        }
        IEnumerator InteractFading()
        {
            yield return new WaitForSeconds(1.5f);
            this.itemPopUp.SetActive(false);
        }
    }
}