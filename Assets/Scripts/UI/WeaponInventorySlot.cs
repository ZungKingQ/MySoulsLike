using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZQ
{
    /// <summary>
    /// ���Ǳ������ڵ���Ʒ��ʾ��
    /// </summary>
    public class WeaponInventorySlot : MonoBehaviour
    {
        public Image icon;
        WeaponItem weaponItem;

        public void AddItem(WeaponItem newWeaponItem)
        {
            weaponItem = newWeaponItem;
            icon.sprite = weaponItem.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }
        public void ClearInventorySlot()
        {
            weaponItem = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }
    }
}