using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    /// <summary>
    /// WeaponItem的对象中存储：
    /// 武器，是否装备，轻/重击动画的名称
    /// </summary>
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject weaponPrefab;
        public bool isUnarmed;

        [Header("One Hnaded Attack Animations")]
        public string OneHnad_Light_Attack;
        public string OneHnad_Heavy_Attack;
    }
}

