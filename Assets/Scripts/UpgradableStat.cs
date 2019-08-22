using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType {
    DAMAGE,
    FIRE_RATE,
    DROP_CHANCE
}

[System.Serializable]
public class UpgradableStat
{
    public UpgradeType upgradeType;
    public float percentBuffPerLevel;
    public int baseCost;
    public int costPerLevel;
    public UpgradeOption upgradeOption;

    public int GetCost(int level) {
        return baseCost + costPerLevel * level;
    }
}
