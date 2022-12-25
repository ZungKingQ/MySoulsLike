using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    /// <summary>
    /// ScriptableObject
    /// 可用于存储数据的对象
    /// </summary>
    public class Item : ScriptableObject
    {
        [Header("Item Information")]
        public Sprite itemIcon;
        public string itemName;
    }
}

