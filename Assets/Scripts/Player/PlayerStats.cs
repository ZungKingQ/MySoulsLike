using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class PlayerStats : MonoBehaviour
    {
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        public int staminaLevel = 10;
        public int maxStamina;
        public int currentStamina;

        public HealthBarUI healthBar;
        public StaminaBarUI staminaBar;
        AnimatorHandler animHandler;

        private void Awake()
        {
            animHandler = GetComponentInChildren<AnimatorHandler>();
        }
        private void Start()
        {
            this.maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);

            this.maxStamina = SetMaxStaminaFromHealthLevel();
            currentStamina = maxStamina;
            staminaBar.SetMaxStamina(maxStamina);
            staminaBar.SetCurrentStamina(currentStamina);
        }

        private int SetMaxHealthFromHealthLevel()
        {
            return this.healthLevel * 10;
        }

        private int SetMaxStaminaFromHealthLevel()
        {
            return this.staminaLevel * 10;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            healthBar.SetCurrentHealth(currentHealth);
            
            animHandler.PlayTargetAnimation("Standing_React_Small_From_Front", true);
            
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animHandler.PlayTargetAnimation("Standing_React_Death_Backward", true);
            }
        }
        public void TakeStaminaDrain(int cost)
        {
            currentStamina -= cost;
            staminaBar.SetCurrentStamina(currentStamina);
        }
    }
}

