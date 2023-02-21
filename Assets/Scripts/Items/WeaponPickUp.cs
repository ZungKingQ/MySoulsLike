using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace ZQ
{
    public class WeaponPickUp : Interactable
    {
        public WeaponItem weapon;
        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);

            PickUpItem(playerManager);
        }
        private void PickUpItem(PlayerManager playerManager)
        {
            PlayerInventory playerInventory;
            PlayerLocomotion playerLocomotion;
            AnimatorHandler animatorHandler;
            playerInventory = playerManager.GetComponent<PlayerInventory>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
            animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();

            playerLocomotion.rigidbody.velocity = Vector3.zero;
            
            animatorHandler.PlayTargetAnimation("Pick Up Item", true);
            playerInventory.weaponInventory.Add(weapon);
            playerManager.itemPopUp.gameObject.GetComponentInChildren<Text>().text = weapon.itemName;
            playerManager.itemPopUp.GetComponentInChildren<RawImage>().texture = weapon.itemIcon.texture;
            playerManager.itemPopUp.SetActive(true);
            Destroy(gameObject);
        }
        
    }
}

