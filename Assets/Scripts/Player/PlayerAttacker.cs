using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;
        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }
        public void HandleLightAttack(WeaponItem weapon)
        {
            animatorHandler.PlayTargetAnimation(weapon.OneHnad_Light_Attack, true);
            //animatorHandler.PlayTargetAnimation(weapon.standing_melee_attack_horizontal, true);
        }
        public void HandleHeavyAttack(WeaponItem weapon)
        {
            animatorHandler.PlayTargetAnimation(weapon.OneHnad_Heavy_Attack, true);
            //animatorHandler.PlayTargetAnimation(weapon.standing_melee_combo_attack_ver_2, true);
            //animatorHandler.PlayTargetAnimation(weapon.standing_melee_combo_attack_ver_3, true);
        }
    }
}

