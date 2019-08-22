using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public UiManager uiManager;
    public WaveManager waveManager;
    public AudioManager audioManager;
    public Player player;
    public TurretController playerTurret;
    [HideInInspector]
    public int playerMoney;
    [HideInInspector]
    public int expectedMoney;                   // Current expected money, assuming player defeats every enemy
    public int playerLives;
    [HideInInspector]
    public Transform treasure;
    [SerializeField]
    private GameObject[] treasurePrefabs;
    public Vector3 treasureStartingLocation;
    public float treasureConvergenceDist;      // Distance at which enemies start moving vertically toward treasure
    [SerializeField]
    private int maxPlayerMoney = 99999;
    [Header("Upgrades")]
    [SerializeField]
    private UpgradableStat[] upgradableStatsInfo;
    public Dictionary<UpgradeType, UpgradableStat> upgrades;
    public int maxUpgradeLevel;

    void Start() {
        upgrades = new Dictionary<UpgradeType, UpgradableStat>();
        foreach(UpgradableStat stat in upgradableStatsInfo) {
            upgrades[stat.upgradeType] = stat;
        }
        uiManager.InitializeShop(upgrades, maxUpgradeLevel);
        waveManager.uiManager = uiManager;
    }

    public IEnumerator StartGame() {
        playerTurret.controlEnabled = true;
        yield return new WaitForSeconds(1.5f);
        waveManager.StartWave();
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SelectDessert(int dessert) {
        treasure = Instantiate(treasurePrefabs[dessert], treasureStartingLocation, Quaternion.identity).transform;
        uiManager.filledLifeSprite = treasure.GetComponent<Treasure>().filledLifeSprite;
        uiManager.emptyLifeSprite = treasure.GetComponent<Treasure>().emptyLifeSprite;
        uiManager.UpdateHealth(playerLives);
        StartCoroutine(uiManager.PlaySelectDessertAnim(dessert));
    }

    public void IncreaseMoney(int value){
        playerMoney = Mathf.Min(playerMoney + value, maxPlayerMoney);
        uiManager.UpdateMoneyDisplay(playerMoney);
    }

    public void DropAmmo(Vector3 enemyPosition) {
        GunType typeToDrop = playerTurret.GetRandomAmmoType();
        if (typeToDrop == GunType.BASIC) {
            return;
        }
        Gun gun = playerTurret.guns[(int)typeToDrop];
        float variance = gun.ammoDropQuantity * gun.dropQuantityVariance;
        RandomRange ammoRange = new RandomRange(gun.ammoDropQuantity - variance, gun.ammoDropQuantity + variance);
        int dropQuantity = Mathf.Max(1, (int)ammoRange.GetRandom());
        IncreaseAmmo(typeToDrop, dropQuantity);
        uiManager.AmmoDropUI(enemyPosition, gun, dropQuantity);
    }

    private void IncreaseAmmo(GunType gunType, int value) {
        playerTurret.IncreaseAmmo(gunType, value);
    }

    public void PurchaseUpgrade(UpgradeType upgradeType) {
        // Validate purchase attempt (upgrade is not max level, player can afford it, and shop is active)
        int statLevel = playerTurret.statLevels[upgradeType];
        if (statLevel < maxUpgradeLevel && playerMoney >= upgrades[upgradeType].GetCost(statLevel) && uiManager.shopActive) {
            UpgradeStat(upgradeType);
        }
    }

    public void PurchaseUpgrade(int upgradeType) {
        PurchaseUpgrade((UpgradeType)upgradeType);
    }

    private void UpgradeStat(UpgradeType upgradeType) {
        int prevStatLevel = playerTurret.statLevels[upgradeType];
        int newLevel = prevStatLevel + 1;
        playerTurret.SetStat(upgradeType, newLevel, newLevel * upgrades[upgradeType].percentBuffPerLevel);
        uiManager.UpdateUpgradeOption(upgrades[upgradeType].upgradeOption, newLevel,
            upgrades[upgradeType].percentBuffPerLevel * newLevel,
            upgrades[upgradeType].percentBuffPerLevel, upgrades[upgradeType].GetCost(newLevel),
            newLevel >= maxUpgradeLevel);
        playerMoney -= upgrades[upgradeType].GetCost(prevStatLevel);
        uiManager.UpdateMoneyDisplay(playerMoney);
        uiManager.UpdateButtonInteractability();
    }

    public void ResetTreasure() {
        treasure.transform.position = treasureStartingLocation;
    }
}
