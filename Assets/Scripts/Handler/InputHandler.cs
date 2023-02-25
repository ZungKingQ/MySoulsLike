using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class InputHandler : MonoBehaviour
    {
        PlayerInputController playerInputController;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        UIManager uiManager;
        CameraHandler cameraHandler;

        [Header("Input Information")]
        public float horizontal;
        public float vertical;
        public float moveAmout;
        public float mouseX;
        public float mouseY;
        public bool IsOpposite;

        private float rollSprintTimer;
        public bool ifrollSprintInput;
        private int Oppsite = -1;

        public bool rollFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool inventoryUI_Flag;
        public bool lockOn_Flag;

        public bool interact_Input;
        public bool lightAttack_Input;
        public bool heavyAttack_Input;
        public bool jump_Input;
        public bool inventoryUI_Input;
        public bool equitmentUI_Input;
        public bool lockOn_Input;
        public bool lockOnLeft_Input;
        public bool lockOnRight_Input;
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
            uiManager = FindObjectOfType<UIManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
        }
        public void OnEnable()
        {
            if (playerInputController == null)
            {
                playerInputController = new PlayerInputController();
                playerInputController.PlayerMovement.Move.performed += i => movementInput = i.ReadValue<Vector2>();
                playerInputController.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                playerInputController.PlayerActions.LightAttack.performed += i => lightAttack_Input = true;
                playerInputController.PlayerActions.HeavyAttack.performed += i => heavyAttack_Input = true;
                playerInputController.PlayerQuickSlots.DpadRight.performed += i => d_Pad_Right = true;
                playerInputController.PlayerQuickSlots.DpadLeft.performed += i => d_Pad_Left = true;
                playerInputController.PlayerActions.Interact.performed += i => interact_Input = true;
                playerInputController.PlayerActions.Jump.performed += i => jump_Input = true;
                playerInputController.PlayerActions.InventouryUI.performed += i => inventoryUI_Input = true;
                playerInputController.PlayerActions.LockOn.performed += i => lockOn_Input = true;
                playerInputController.PlayerActions.LockOnLeft.performed += i => lockOnLeft_Input = true;
                playerInputController.PlayerActions.LockOnRight.performed += i => lockOnRight_Input = true;
            }
            playerInputController.Enable();
        }
        public void OnDisable()
        {
            playerInputController.Disable();
        }
        public void TickInput(float delta)
        {
            HandleMoveInput(delta);
            HandleRollInput(delta);
            HandleAttackInput(delta);
            HandleQuickSlotsInput();
            HandleInventoryUIInput();
            HandleLockOnInput();
        }
        public void HandleMoveInput(float dalta)
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
            sprintFlag = ifrollSprintInput;

            if (ifrollSprintInput)
            {
                rollSprintTimer += delta;
                print(rollSprintTimer);
            }
            else
            {
                if(0 < rollSprintTimer && rollSprintTimer < 0.9f)
                {
                    rollFlag = true;
                    sprintFlag = false;
                }
                rollSprintTimer = 0;
            }
        }
        public void HandleAttackInput(float delta)
        {
            if (lightAttack_Input)
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
            if(heavyAttack_Input)
            {
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
            }
        }
        private void HandleQuickSlotsInput()
        {
            if(d_Pad_Right)
            {
                playerInventory.ChangeRightWeapon();
            }
            else if(d_Pad_Left)
            {
                playerInventory.ChangeLeftWeapon();
            }
        }
        private void HandleInventoryUIInput()
        {
            playerInputController.PlayerActions.InventouryUI.performed += i => inventoryUI_Input = true;

            if(inventoryUI_Input)
            {
                inventoryUI_Flag = !inventoryUI_Flag;

                if(inventoryUI_Flag)
                {
                    uiManager.OpenSelectWindow();
                    uiManager.UpdateUI();
                    uiManager.hudWindow.SetActive(false);
                }
                else
                {
                    uiManager.CloseSelectWindow();
                    uiManager.ClosAllInventoryWindow();
                    uiManager.hudWindow.SetActive(true);
                }
            }
            if (equitmentUI_Input)
            {
                equitmentUI_Input = !equitmentUI_Input;

                if (equitmentUI_Input)
                {
                    uiManager.OpenSelectWindow();
                    uiManager.UpdateUI();
                    uiManager.hudWindow.SetActive(false);
                }
                else
                {
                    uiManager.CloseSelectWindow();
                    uiManager.ClosAllInventoryWindow();
                    uiManager.hudWindow.SetActive(true);
                }
            }
        }
        private void HandleLockOnInput()
        {
            if(lockOn_Input && !lockOn_Flag)
            {
                cameraHandler.ClearLockOnTargets();
                lockOn_Input = false;
                // 将最近的锁定目标设置为当前锁定目标
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                CameraHandler.instance.HandleLockOn();
                if(cameraHandler.nearestLockOnTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                    lockOn_Flag = true;
                }
            }
            // 退出锁定
            else if(lockOn_Input && lockOn_Flag)
            {
                lockOn_Input = false;
                lockOn_Flag = false;
                cameraHandler.ClearLockOnTargets();
            }

            if(lockOn_Flag && lockOnLeft_Input)
            {
                lockOnLeft_Input = false;
                cameraHandler.HandleLockOn();
                if(cameraHandler.leftLockOnTarget != null)
                {
                    // 将左侧锁定
                    cameraHandler.currentLockOnTarget = cameraHandler.leftLockOnTarget;
                }
            }

            if (lockOn_Flag && lockOnRight_Input)
            {
                lockOnRight_Input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.rightLockOnTarget != null)
                {
                    // 将右侧锁定
                    cameraHandler.currentLockOnTarget = cameraHandler.rightLockOnTarget;
                }
            }

            cameraHandler.SetCameraHeight();
        }
    }
}