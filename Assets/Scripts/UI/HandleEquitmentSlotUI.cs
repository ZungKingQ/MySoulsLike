using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ZQ
{
    /// <summary>
    /// 这是装备窗口的物品显示类
    /// </summary>
    public class HandleEquitmentSlotUI : MonoBehaviour
    {
        public Image icon;
        WeaponItem weaponItem;

        public bool rightHandSlot01;
        public bool rightHandSlot02;
        public bool leftHandSlot01;
        public bool leftHandSlot02;

        public void AddItem(WeaponItem newWeaponItem)
        {
            weaponItem = newWeaponItem;
            icon.sprite = weaponItem.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }
        public void ClearItem(WeaponItem newWeaponItem)
        {
            weaponItem = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }
    }
}