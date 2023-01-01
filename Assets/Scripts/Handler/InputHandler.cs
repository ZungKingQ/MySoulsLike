using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class InputHandler : MonoBehaviour
    {
        [HideInInspector]
        public float horizontal;
        [HideInInspector]
        public float vertical;
        [HideInInspector]
        public float moveAmout;
        public float mouseX;
        public float mouseY;
        public bool IsOpposite;
        [HideInInspector]
        public bool rollFlag;
        [HideInInspector]
        public bool jumpFlag;
        [HideInInspector]
        public bool sprintFlag;
        [HideInInspector]
        public bool comboFlag;

        private float rollSprintTimer;
        public bool ifrollSprintInput;
        private int Oppsite = -1;

        PlayerInputController playerInputController;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;

        public bool rb_input;
        public bool rt_input;
        public bool d_Pad_Up;
        public bool d_Pad_Down;
        public bool d_Pad_Left;
        public bool d_Pad_Right;

        private Vector2 movementInput;
        private Vector2 cameraInput;

        private void Awake()
        {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
        }
        public void OnEnable()
        {
            if (playerInputController == null)
            {
                playerInputController = new PlayerInputController();
                playerInputController.PlayerMovement.Move.performed += i => movementInput = i.ReadValue<Vector2>();
                playerInputController.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            }
            playerInputController.Enable();
        }
        public void OnDisable()
        {
            playerInputController.Disable();
        }
        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
            HandleAttackInput(delta);
            HandleQuickSlotsInput();
        }
        public void MoveInput(float dalta)
        {
            Oppsite = IsOpposite == true ? Oppsite : 1;
            this.horizontal = movementInput.x;
            this.vertical = movementInput.y;
            //作为Animator中移动Action的参数
            this.moveAmout = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            this.mouseX = cameraInput.x * Oppsite;
            this.mouseY = cameraInput.y * Oppsite;
        }
        public void HandleRollInput(float delta)
        {
            ifrollSprintInput = playerInputController.PlayerActions.Rolling.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            if (ifrollSprintInput)
            {
                rollSprintTimer += delta;
                rollFlag = false;
                sprintFlag = true;
            }
            else
            {
                if(0 < rollSprintTimer && rollSprintTimer < 0.5f)
                {
                    rollFlag = true;
                    sprintFlag = false;
                }
                rollSprintTimer = 0;
            }
        }
        public void HandleJumoInput(float delta)
        {
            if(playerInputController.PlayerActions.Jump.phase == UnityEngine.InputSystem.InputActionPhase.Started)
                jumpFlag = true;
        }
        public void HandleAttackInput(float delta)
        {
            playerInputController.PlayerActions.RB.performed += i => rb_input = true;
            playerInputController.PlayerActions.RT.performed += i => rt_input = true;
            
            if (rb_input)
            {
                if(playerManager.canDoCombo)
                {
                    comboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                }
                else
                {
                    if (playerManager.isInteracting)
                        return;
                    if (playerManager.canDoCombo)
                        return;

                    playerAttacker.HandleLightAttack(playerInventory.rightWeapon);

                }
                comboFlag = false;
            }
            if(rt_input)
            {
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
            }
        }
        private void HandleQuickSlotsInput()
        {
            playerInputController.PlayerQuickSlots.DpadRight.performed += i => d_Pad_Right = true;
            playerInputController.PlayerQuickSlots.DpadLeft.performed += i => d_Pad_Left = true;

            if(d_Pad_Right)
            {
                playerInventory.ChangeRightWeapon();
            }
            else if(d_Pad_Left)
            {
                playerInventory.ChangeLeftWeapon();
            }
        }
    }
}