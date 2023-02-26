using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZQ
{
    /// <summary>
    /// 这是背包窗口的物品显示类
    /// </summary>
    public class WeaponInventorySlot : MonoBehaviour
    {
        public Image icon;
        WeaponItem weaponItem;
        UIManager uiManager;
        PlayerInventory playerInventory;
        WeaponSlotManager weaponSlotManager;

        private void Awake()
        {
            uiManager = FindObjectOfType<UIManager>();
            playerInventory = FindObjectOfType<PlayerInventory>();
            weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
        }
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
        public void EquitThisItem()
        {
            if(uiManager.rightHandSlot01Selected)
            {
                // 把此时右手上的装备放进库
                playerInventory.weaponInventory.Add(playerInventory.weaponInRightHandSlots[0]);
                // 装备选择的武器
                playerInventory.weaponInRightHandSlots[0] = weaponItem;
                // 将选择的武器从库中移除
                playerInventory.weaponInventory.Remove(weaponItem);
            }
            else if(uiManager.rightHandSlot02Selected)
            {
                playerInventory.weaponInventory.Add(playerInventory.weaponInRightHandSlots[1]);
                playerInventory.weaponInRightHandSlots[1] = weaponItem;
                playerInventory.weaponInventory.Remove(weaponItem);
            }
            else if(uiManager.leftHandSlot01Selected)
            {
                playerInventory.weaponInventory.Add(playerInventory.weaponInLeftHandSlots[0]);
                playerInventory.weaponInLeftHandSlots[0] = weaponItem;
                playerInventory.weaponInventory.Remove(weaponItem);
            }
            else if(uiManager.leftHandSlot02Selected)
            {
                playerInventory.weaponInventory.Add(playerInventory.weaponInLeftHandSlots[1]);
                playerInventory.weaponInLeftHandSlots[1] = weaponItem;
                playerInventory.weaponInventory.Remove(weaponItem);
            }
            else
            {
                return;
            }

            playerInventory.currentRightWeaponIndex = playerInventory.currentRightWeaponIndex > 0 ? playerInventory.currentRightWeaponIndex : 0;
            playerInventory.currentLeftWeaponIndex = playerInventory.currentLeftWeaponIndex > 0 ? playerInventory.currentLeftWeaponIndex : 0;
            playerInventory.rightWeapon = playerInventory.weaponInRightHandSlots[playerInventory.currentRightWeaponIndex];
            playerInventory.leftWeapon = playerInventory.weaponInLeftHandSlots[playerInventory.currentLeftWeaponIndex];

            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);

            uiManager.equitmentWindowUI.LoadWeaponsOnEquitmemtScreen(playerInventory);
            uiManager.ResetAllSelectedSlots();
        }
    }
}