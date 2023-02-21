using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZQ
{
    public class QuickSlotUI : MonoBehaviour
    {
        public Image leftWeaponIcon; 
        public Image rightWeaponIcon;

        /// <summary>
        /// 显示左右手的装备UI
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="isLeft"></param>
        public void UpdateWeaponQuickSlotUI(WeaponItem weapon, bool isLeft)
        {
            if (isLeft)
            {
                if (weapon != null)
                {
                    leftWeaponIcon.sprite = weapon.itemIcon;
                    leftWeaponIcon.enabled = true;
                }
                else
                {
                    leftWeaponIcon.sprite = null;
                    leftWeaponIcon.enabled = false;
                }
            }
            else
            {
                if (weapon != null)
                {
                    rightWeaponIcon.sprite = weapon.itemIcon;
                    rightWeaponIcon.enabled = true;
                }
                else
                {
                    rightWeaponIcon.sprite = null;
                    rightWeaponIcon.enabled = false;
                }
                
            }
        }
    }
}

