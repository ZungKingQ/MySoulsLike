using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;
        public bool isLeftHandSlot;
        public bool isRightHandSlot;

        public GameObject currentWeaponModel;

        public void UnLoadWeapon()
        {
            if (currentWeaponModel != null)
            {
                currentWeaponModel.SetActive(false);
            }
        }
        public void DestroyWeapon()
        {
            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }
        public void LoadWeaponModel(WeaponItem weaponItem)
        {
            // destroy the current instantiation and get new one
            DestroyWeapon();

            if (weaponItem == null)
            {
                // unload weapon
                return;
            }

            GameObject model = Instantiate(weaponItem.weaponPrefab) as GameObject;
            if(model != null)
            {
                if(parentOverride != null)
                {
                    model.transform.parent = parentOverride;
                }
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }
            currentWeaponModel = model;
        }
    }
}

