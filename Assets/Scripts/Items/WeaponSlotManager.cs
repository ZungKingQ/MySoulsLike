using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class WeaponSlotManager : MonoBehaviour
    {
        WeaponHolderSlot leftHnadSlot;
        WeaponHolderSlot rightHnadSlot;

        private void Awake()
        {
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
            if(isLeft)
            {
                leftHnadSlot.LoadWeaponModel(weaponItem);
            }
            else
            {
                rightHnadSlot.LoadWeaponModel(weaponItem);
            }
        }
    }
}

