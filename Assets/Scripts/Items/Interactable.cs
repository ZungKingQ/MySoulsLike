using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class Interactable : MonoBehaviour
    {
        public float radius = 0.6f;
        public string interactableText;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
        /// <summary>
        /// 交互时调用
        /// </summary>
        /// <param name="playerManager"></param>
        public virtual void Interact(PlayerManager playerManager)
        {
            print("You Pick Up An Item!");
        }
    }
}

