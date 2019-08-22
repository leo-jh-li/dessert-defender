using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private Wave[] waves;
    [SerializeField]
    private int[] unlockWaves;              // Array of waves at which new guns are unlocked
    [HideInInspector]
    public int waveIndex;
    private int enemyIndex;
    [SerializeField]
    private GameManager gameManager;
    [HideInInspector]
    public UiManager uiManager;
    private bool active;
    [SerializeField]
    private float spawnX;
    private RandomRange spawnY;
    [SerializeField]
    private float spawnYBufferW;            // Size of margin along the inner edge of the top and bottom of the screen where enemies won't spawn
    [SerializeField]
    private RectTransform uiPanel;
    private float camHeight;
    [SerializeField]
    private float maxExtraSpawnTime;        // Maximum of potential spawn time added after each spawn
    private float remainingSpawnCooldown;

    [Header("Music Fade Values")]
    [SerializeField]
    private float battleFadeOut;
    [SerializeField]
    private float battleFadeIn;
    [SerializeField]
    private float gameOverFadeOut;
    [SerializeField]
    private float shopFadeOut;
    [SerializeField]
    private float shopFadeIn;
    [SerializeField]
    private float transition;

    [Space(10)]
    [SerializeField]
    private GameObject[] enemies;
    private List<Enemy> activeEnemies;


    void Start() {
        Camera cam = Camera.main;
        camHeight = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, 0)).y;
        activeEnemies = new List<Enemy>();
        // Get height of UI panel by subtracting the bottom of the screen's y from the panel's top y
        float panelTopY = cam.ScreenToWorldPoint(uiPanel.position + new Vector3(0, uiPanel.rect.height/2), 0).y;
        float bottomOfScreen = cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        float uiPanelHeight = panelTopY - bottomOfScreen;

        // Set spawn point's y, factoring in the spawn buffer and the height of the UI
        spawnY = new RandomRange(cam.ScreenToWorldPoint(Vector3.zero).y + spawnYBufferW + uiPanelHeight, camHeight - spawnYBufferW);

        // Calculate each enemy group's height, based on their renderers
        foreach(GameObject enemyObj in enemies) {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds combinedBounds = renderers[0].bounds;
                foreach (Renderer render in renderers) {
                    combinedBounds.Encapsulate(render.bounds);
                }
                enemy.renderersHeight = combinedBounds.size.y;
            }
        }
        waveIndex = 0;
        enemyIndex = 0;
    }

    void Update() {
        if (active) {
            if (remainingSpawnCooldown <= 0) {
                GameObject nextEnemy = PopNextEnemy();
                if (nextEnemy != null) {
                    SpawnEnemy(nextEnemy);
                } else {
                    if (enemyIndex >= waves[waveIndex].spawnInfo.Length) {
                        if (activeEnemies.Count == 0) {
                            WinWave();
                        }
                    }
                    if (WavesComplete()) {
                        WinGame();
                    }
                }
            } else {
                remainingSpawnCooldown -= Time.deltaTime;
                // If there are no enemies, speed up the next spawn
                if (activeEnemies.Count == 0) {
                    remainingSpawnCooldown -= Time.deltaTime;
                }
            }
        }
    }

    private GameObject PopNextEnemy() {
        if (waveIndex >= waves.Length || enemyIndex >= waves[waveIndex].spawnInfo.Length) {
            return null;
        }
        EnemyType type = waves[waveIndex].spawnInfo[enemyIndex].enemyType;
        enemyIndex++;
        return enemies[(int)type];
    }

    private bool WavesComplete() {
        return waveIndex >= waves.Length;
    }

    private Enemy SpawnEnemy(GameObject enemyPrefab) {
        Enemy enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();

        // Randomize position
        spawnY.max = camHeight - enemy.renderersHeight / 2 - spawnYBufferW;
        enemy.transform.position = new Vector3(spawnX, spawnY.GetRandom(), 0);
        enemy.gameManager = gameManager;
        foreach(Enemy childEnemy in enemy.GetComponentsInChildren<Enemy>()) {
            childEnemy.gameManager = gameManager;
        }
        // Calculate lower and upper bounds of seconds it takes to kill this enemy
        float baseTimeToKill = enemy.GetGroupHealth() / gameManager.playerTurret.GetBasicDPS();
        float lowestTimeToKill = enemy.GetGroupHealth() / gameManager.playerTurret.GetTopDPS();
        RandomRange spawnCooldown = new RandomRange(lowestTimeToKill, baseTimeToKill + maxExtraSpawnTime);
        remainingSpawnCooldown += spawnCooldown.GetRandom();
        enemy.enemiesList = activeEnemies;
        activeEnemies.Add(enemy);
        return enemy;
    }

    public void ClearActiveEnemies() {
        foreach(Enemy enemy in activeEnemies) {
            Destroy(enemy.gameObject);
        }
        activeEnemies.Clear();
    }

    public void StartWave() {
        Debug.Log("Wave " + (waveIndex + 1) + " Start");
        active = true;
        ClearActiveEnemies();
        gameManager.player.CalmDown();
        gameManager.ResetTreasure();
        uiManager.DisableWaveEndTexts();
        gameManager.playerTurret.SetControl(true);
        // Play music at full volume if shop music is not playing; fade it in otherwise (for the first wave)
        if (!gameManager.audioManager.GetSound(Constants.SHOP_MUSIC).source.isPlaying) {
            gameManager.audioManager.Play(Constants.BATTLE_MUSIC);
        } else {
            StartCoroutine(gameManager.audioManager.FadeTransition(Constants.SHOP_MUSIC, shopFadeOut, true, transition, Constants.BATTLE_MUSIC, battleFadeIn, true));
        }
    }

    // Debugging function to skip current wave, granting money and ammo drops for all enemies
    public void SkipWave() {
        if (activeEnemies.Count == 0) {
            StartWave();
        }
        for(int i = activeEnemies.Count - 1; i >= 0; i--) {
            activeEnemies[i].Die();
        }
        activeEnemies.Clear();
        GameObject nextEnemy = PopNextEnemy();
        while (nextEnemy != null) {
            SpawnEnemy(nextEnemy).Die();
            nextEnemy = PopNextEnemy();
        }
        WinWave();
    }

    private void WinWave() {
        Debug.Log("Wave " + (waveIndex + 1) + " Complete");
        waveIndex++;
        // Unlock gun if incoming wave is in unlockWaves
        bool gunUnlocked = false;
        // Add expected money gained 
        gameManager.expectedMoney += CalculateWaveMoney(waves[waveIndex - 1]);
        if (Array.Exists(unlockWaves, element => element == waveIndex + 1)) {
            gunUnlocked = gameManager.playerTurret.UnlockNextGun();
        }
        if (gunUnlocked) {
            uiManager.ShowGunUnlocked();
        } else {
            uiManager.ShowWaveComplete();
        }
        StartCoroutine(EndWave());
    }

    // End the current wave and open the store
    public IEnumerator EndWave() {
        active = false;
        yield return new WaitForSeconds(1.25f);
        if (!WavesComplete()) {
            StartCoroutine(gameManager.audioManager.FadeTransition(Constants.BATTLE_MUSIC, battleFadeOut, true, transition, Constants.SHOP_MUSIC, shopFadeIn, true));
        }
        yield return new WaitForSeconds(0.25f);
        gameManager.playerTurret.SetControl(false);
        enemyIndex = 0;
        remainingSpawnCooldown = 0;
        if (!WavesComplete()) {
            uiManager.OpenShop();
        }
    }

    public void LoseWave() {
        if (active) {
            active = false;
            gameManager.playerLives -= 1;
            uiManager.UpdateHealth(gameManager.playerLives);
            Treasure treasure = gameManager.treasure.GetComponent<Treasure>();
            treasure.carried = false;
            treasure.transform.parent = null;
            Debug.Log("WAVE OVER - TREASURE LOST");
            if (gameManager.playerLives <= 0) {
                LoseGame();
                return;
            }
            uiManager.ShowTreasureLost(treasure.treasureName);
            StartCoroutine(EndWave());
        }
    }

    private void WinGame() {
        active = false;
        StartCoroutine(uiManager.DisplayGameWon());
        StartCoroutine(gameManager.audioManager.FadeOut(Constants.BATTLE_MUSIC, gameOverFadeOut, false));
        Debug.Log("Game Won");
    }

    private void LoseGame() {
        active = false;
        StartCoroutine(uiManager.DisplayGameOver());
        StartCoroutine(gameManager.audioManager.FadeOut(Constants.BATTLE_MUSIC, gameOverFadeOut, false));
        Debug.Log("GAME OVER");
    }

    // Returns money gained from defeating every enemy in a given wave
    public int CalculateWaveMoney(Wave wave) {
        int total = 0;
        foreach (SpawnInfo info in wave.spawnInfo) {
            total += enemies[(int)info.enemyType].GetComponent<Enemy>().moneyValue;
        }
        return total;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(new Vector3(spawnX, 0, 0), new Vector3(1, 10, 0));
    }
}
