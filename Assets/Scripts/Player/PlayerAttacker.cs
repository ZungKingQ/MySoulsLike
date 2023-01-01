using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;
        InputHandler inputHandler;
        public string lastAttack;
        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            inputHandler = GetComponent<InputHandler>();
        }
        public void HandleLightAttack(WeaponItem weapon)
        {
            animatorHandler.PlayTargetAnimation(weapon.OneHnad_Light_Attack[1], true);
            lastAttack = weapon.OneHnad_Light_Attack[1];
        }
        public void HandleHeavyAttack(WeaponItem weapon)
        {
            animatorHandler.PlayTargetAnimation(weapon.OneHnad_Heavy_Attack[1], true);
            lastAttack = weapon.OneHnad_Heavy_Attack[1];
        }
        public void HandleWeaponCombo(WeaponItem weapon)
        {
            if(inputHandler.comboFlag)
            {
                animatorHandler.animator.SetBool("canDoCombo", false);
                if (lastAttack == weapon.OneHnad_Light_Attack[1])
                {
                    animatorHandler.PlayTargetAnimation(weapon.OneHnad_Light_Attack[2], true);
                }
            }
        }
    }
}

