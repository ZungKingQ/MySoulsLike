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


        #region Idle Animations
        public string right_hand_idle;
        public string left_hand_idle;
        //public Dictionary<int, string> OneHnad_Light_Attack = new Dictionary<int, string>
        //{
        //    {1, "standing_melee_attack_horizontal"},
        //    {2, "standing_melee_attack_downward"}
        //};
        #endregion
        #region One Hnaded Attack Animations
        public Dictionary<int, string> OneHnad_Light_Attack = new Dictionary<int, string>
        {
            {1, "standing_melee_attack_downward"},
            {2, "standing_melee_attack_horizontal"}
        };
        public Dictionary<int, string> OneHnad_Heavy_Attack = new Dictionary<int, string>
        {
            {1, "standing_melee_combo_attack_ver_1"},
            {2, "standing_melee_combo_attack_ver_2"},
            {3, "standing_melee_combo_attack_ver_3"}
        };
        #endregion
    }
}

