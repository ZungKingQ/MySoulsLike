using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZQ
{
    public class UIManager : MonoBehaviour
    {
        public PlayerInventory playerInventory;
        EquitmentWindowUI equitmentWindowUI;

        [Header("UI Window")]
        public GameObject hudWindow;
        public GameObject selectWindow;
        public GameObject weaponInventoryWindow;
        public GameObject equitmentInventoryWindow;

        [Header("Weapon Inventory")]
        public GameObject weaponInventorySlotPrefab;
        public Transform weaponInventorySlotParent;

        WeaponInventorySlot[] weaponInventorySlots;

        private void Awake()
        {
            equitmentWindowUI = equitmentInventoryWindow.GetComponentInChildren<EquitmentWindowUI>();
        }
        private void Start()
        {
            weaponInventorySlots = weaponInventorySlotParent.GetComponentsInChildren<WeaponInventorySlot>();
            equitmentWindowUI.LoadWeaponsOnEquitmemtScreen(playerInventory);
        }
        private void OnEnable()
        {
            equitmentWindowUI.transform.parent.gameObject.SetActive(false);
            
        }
        public void UpdateUI()
        {
            #region Weapon Inventory Slots （保持角色武器库存与UI库存相同）
            for (int i = 0; i < weaponInventorySlots.Length; i++)
            {
                if(i < playerInventory.weaponInventory.Count)
                {
                    if(weaponInventorySlots.Length < playerInventory.weaponInventory.Count)
                    {
                        Instantiate(weaponInventorySlotPrefab, weaponInventorySlotParent);
                        weaponInventorySlots = weaponInventorySlotParent.GetComponentsInChildren<WeaponInventorySlot>();
                    }
                    weaponInventorySlots[i].AddItem(playerInventory.weaponInventory[i]);
                }
                else
                {
                    weaponInventorySlots[i].ClearInventorySlot();
                }
            }
            #endregion
        }
        /// <summary>
        /// 打开三个功能的选择UI
        /// </summary>
        public void OpenSelectWindow()
        {
            selectWindow.SetActive(true);
        }
        /// <summary>
        /// 关闭三个功能的选择UI
        /// </summary>
        public void CloseSelectWindow()
        {
            selectWindow.SetActive(false);
        }
        /// <summary>
        /// 关闭武器库存UI
        /// </summary>
        public void ClosAllInventoryWindow()
        {
            weaponInventoryWindow.SetActive(false);
            equitmentInventoryWindow.SetActive(false);
        }
    }
}

