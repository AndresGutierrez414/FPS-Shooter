using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopData : MonoBehaviour
{
    [System.Serializable]
    public class ShopItems
    {
        public ShopItem[] shopList;
    }

    [System.Serializable]
    public class ShopItem
    {
        public string upgradeName;
        public bool isUnlocked;
        public int upgradeCost;
        public int upgradeLevel = 1;
        public upgradeLevelData[] upgradeLevelData;
    }

    [System.Serializable]
    public class upgradeLevelData
    {
        public int upgradeCost;
        public int damage;
        public int speed;
    }

}
