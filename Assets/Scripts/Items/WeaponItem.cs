using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    /// <summary>
    /// WeaponItem�Ķ����д洢��
    /// �������Ƿ�װ������/�ػ�����������
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

