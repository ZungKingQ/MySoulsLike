using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class WeaponSlotManager : MonoBehaviour
    {
        WeaponHolderSlot leftHnadSlot;
        WeaponHolderSlot rightHnadSlot;

        DamageCollider leftDamageCollider;
        DamageCollider rightDamageCollider;

        Animator animator;
        QuickSlotUI quickSlotUI;

        PlayerStats playerStats;

        public WeaponItem attackingWeapon;

        private void Awake()
        {
            quickSlotUI = FindObjectOfType<QuickSlotUI>();
            animator = GetComponent<Animator>();
            playerStats = GetComponentInParent<PlayerStats>();

            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot weaponHolderSlot in weaponHolderSlots)
            {
                if(weaponHolderSlot.isLeftHandSlot)
                {
                    leftHnadSlot = weaponHolderSlot;
                }
                else if(weaponHolderSlot.isRightHandSlot)
                {
                    rightHnadSlot = weaponHolderSlot;
                }
            }
        }
        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            quickSlotUI.UpdateWeaponQuickSlotUI(weaponItem, isLeft);

            if (isLeft)
            {
                leftHnadSlot.LoadWeaponModel(weaponItem);
                LoadLeftWeaponDamageCollider();
                
                #region Handle Arm Idle Animation
                if (weaponItem != null)
                {
                    animator.CrossFade(weaponItem.left_hand_idle, 0.2f);
                }
                else
                {
                    animator.CrossFade("Left Arm Empty", 0.2f);
                }
                #endregion
            }
            else
            {
                rightHnadSlot.LoadWeaponModel(weaponItem);
                LoadRightWeaponDamageCollider();

                #region Handle Arm Idle Animation
                if (weaponItem != null)
                {
                    animator.CrossFade(weaponItem.right_hand_idle, 0.2f);
                }
                else
                {
                    animator.CrossFade("Right Arm Empty", 0.2f);
                }
                #endregion
            }
        }

        #region Handle Weapon Damage Collider
        private void LoadLeftWeaponDamageCollider()
        {
            leftDamageCollider = leftHnadSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }
        private void LoadRightWeaponDamageCollider()
        {
            rightDamageCollider = rightHnadSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }
        public void OpenLeftDamageCollider()
        {
            leftDamageCollider.EnableDamageCollider();
        }
        public void OpenRightDamageCollider()
        {
            rightDamageCollider.EnableDamageCollider();
        }
        public void CloseLeftDamageCollider()
        {
            leftDamageCollider.DisableDamageCollider();
        }
        public void CloseRightDamageCollider()
        {
            rightDamageCollider.DisableDamageCollider();
        }
        #endregion

        #region Handle Weapon Stamina Drain
        public void DrainStaminaLightAttack()
        {
            playerStats.TakeStaminaDrain(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }
        public void DrainStaminaHeavyAttack()
        {
            playerStats.TakeStaminaDrain(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }
        #endregion
    }
}

