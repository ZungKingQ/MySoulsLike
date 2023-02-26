using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZQ
{
    /// <summary>
    /// 该类主要用于判断哪个装备被选择
    /// </summary>
    public class EquitmentWindowUI : MonoBehaviour
    {
        public bool rightHandSlot01_Selected;
        public bool rightHandSlot02_Selected;
        public bool leftHandSlot01_Selected;
        public bool leftHandSlot02_Selected;

        public HandleEquitmentSlotUI[] handleEquitmentSlotUI;

        public void LoadWeaponsOnEquitmemtScreen(PlayerInventory playerInventory)
        {
            for (int i = 0; i < handleEquitmentSlotUI.Length; i++)
            {
                if (handleEquitmentSlotUI[i].rightHandSlot01)
                {
                    handleEquitmentSlotUI[i].AddItem(playerInventory.weaponInRightHandSlots[0]);
                }
                else if (handleEquitmentSlotUI[i].rightHandSlot02)
                {
                    handleEquitmentSlotUI[i].AddItem(playerInventory.weaponInRightHandSlots[1]);
                }
                else if (handleEquitmentSlotUI[i].leftHandSlot01)
                {
                    handleEquitmentSlotUI[i].AddItem(playerInventory.weaponInLeftHandSlots[0]);
                }
                else if (handleEquitmentSlotUI[i].leftHandSlot02)
                {
                    handleEquitmentSlotUI[i].AddItem(playerInventory.weaponInLeftHandSlots[1]);
                }
            }
        }
        public void SelectRightHandSlot01()
        {
            rightHandSlot01_Selected = true;
        }
        public void SelectRightHandSlot02()
        {
            rightHandSlot02_Selected = true;
        }
        public void SelectLeftHandSlot01()
        {
            leftHandSlot01_Selected = true;
        }
        public void SelectLeftHandSlot02()
        {
            leftHandSlot02_Selected = true;
        }
    }
}