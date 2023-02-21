using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class EnemyStats : MonoBehaviour
    {
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        Animator animator;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }
        private void Start()
        {
            this.maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }

        private int SetMaxHealthFromHealthLevel()
        {
            return this.healthLevel * 10;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            animator.Play("Standing_React_Small_From_Front");

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animator.Play("Standing_React_Death_Backward");
            }
        }
    }
}

