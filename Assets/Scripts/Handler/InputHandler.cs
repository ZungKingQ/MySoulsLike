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

        private float rollSprintTimer;
        public bool ifrollSprintInput;
        private int Oppsite = -1;

        PlayerInputController playerInputController;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;

        public bool rb_input;
        public bool rt_input;

        
        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
        }
        public void OnEnable()
        {
            if (playerInputController == null)
            {
                playerInputController = new PlayerInputController();
                playerInputController.PlayerMovement.Move.performed += playerInputController => movementInput = playerInputController.ReadValue<Vector2>();
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
        float InputL_AttackTimer;
        float InputH_AttackTimer;
        public void HandleAttackInput(float delta)
        {
            playerInputController.PlayerActions.RB.performed += i => rb_input = true;
            playerInputController.PlayerActions.RT.performed += i => rt_input = true;
            
            if (rb_input)
            {
                InputL_AttackTimer += delta;
                if (InputL_AttackTimer > 1f)
                {
                    playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
                    InputL_AttackTimer = 0;
                }
            }
            if(rt_input)
            {
                InputH_AttackTimer += delta;
                if (InputH_AttackTimer > 0.02f)
                {
                    playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
                    InputH_AttackTimer = 0;
                }
            }
        }
    }
}