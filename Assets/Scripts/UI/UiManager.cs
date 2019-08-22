
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Transform canvas;  // The main canvas
    [SerializeField]
    private Transform ammoUiContainer;
    private Camera cam;
    private Vector2 leftScreenPos;
    private Vector2 rightScreenPos;
    [SerializeField]
    private GameObject ammoDropPrefab;
    [SerializeField]
    private GameObject winScreen;
    [SerializeField]
    private GameObject gameOverScreen;
    [SerializeField]
    private Image winTreasureImage;
    [SerializeField]
    private Image loseTreasureImage;
    [SerializeField]
    private Transform tileMap;

    [Header("Dessert Select Animation")]
    [SerializeField]
    private GameObject dessertChoices;
    [SerializeField]
    private Image[] dessertImages;
    [SerializeField]
    private GameObject selectScreenBackground;
    [SerializeField]
    private GameObject guidanceText;
    [SerializeField]
    private Image playerImage;
    [SerializeField]
    private Sprite playerIdleSprite;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float animationDuration;
    [SerializeField]
    private GameObject gameBeginText;

    [Header("Panel UI")]
    [SerializeField]
    private GameObject panelUI;
    [SerializeField]
    private Image[] lifeIcons;
    [HideInInspector]
    public Sprite filledLifeSprite;
    [HideInInspector]
    public Sprite emptyLifeSprite;
    [SerializeField]
    private GameObject[] gunUiBorders;
    [SerializeField]
    private TextMeshProUGUI moneyDisplayValue;
    
    [Header("Wave End Texts")]
    [SerializeField]
    private TextMeshProUGUI waveCompleteText;
    private Vector3 originalCompletePos;
    [SerializeField]
    private TextMeshProUGUI gunUnlockText;
    [SerializeField]
    private TextMeshProUGUI treasureLostText;
    [SerializeField]
    private float textDuration;

    [Header("Shop")]
    [SerializeField]
    private GameObject shop;
    [HideInInspector]
    public bool shopActive;
    [SerializeField]
    private TextMeshProUGUI continueButtonText;
    private Animator shopAnimator;

    [Header("Fade Screen")]
    [SerializeField]
    private Image fadeScreen;
    [SerializeField]
    private Color inactiveColour;
    [SerializeField]
    private Color activeColour;
    [SerializeField]
    private float winFadeTime;
    [SerializeField]
    private float gameOverFadeTime;

    void Start() {
        cam = Camera.main;

        // Set screen edge positions
        Vector3[] corners = new Vector3[4];
        canvas.GetComponent<RectTransform>().GetWorldCorners(corners);
        leftScreenPos = Vector2.zero;
        rightScreenPos = corners[3];

        // Set waveCompleteText's original position to be used for ShowGunUnlocked()
        originalCompletePos = waveCompleteText.rectTransform.localPosition;

        ActivateBorder(0);
        shopAnimator = shop.GetComponent<Animator>();        
    }

    public IEnumerator PlaySelectDessertAnim(int dessert) {
        playerImage.sprite = playerIdleSprite;
        // Clone selected dessert and move it and the player image
        GameObject selectedDessert = Instantiate(dessertImages[dessert].gameObject, dessertImages[dessert].transform.position, Quaternion.identity, canvas);
        dessertChoices.SetActive(false);
        guidanceText.SetActive(false);
        StartCoroutine(MoveImageToSprite(playerImage, playerTransform.GetComponentInChildren<SpriteRenderer>(), Time.time, animationDuration));
        StartCoroutine(MoveImageToSprite(selectedDessert.GetComponent<Image>(), gameManager.treasure.GetComponent<SpriteRenderer>(), Time.time, animationDuration));
        yield return new WaitForSeconds(1.5f);

        // Erase images, show game area
        selectScreenBackground.SetActive(false);
        playerImage.gameObject.SetActive(false);
        Destroy(selectedDessert);
        yield return new WaitForSeconds(1.5f);

        panelUI.SetActive(true);
        gameManager.audioManager.Play(Constants.EMPHASIS);
        gameBeginText.SetActive(true);
        yield return new WaitForSeconds(3f);
        gameBeginText.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(gameManager.StartGame());
    }

    // Transform given image into given sprite
    private IEnumerator MoveImageToSprite(Image image, SpriteRenderer spriteRenderer, float startTime, float duration) {
        float interpolant = 0;
        Vector3 originPos = image.transform.position;
        Vector2 originSize = image.rectTransform.sizeDelta;
        
        // Determine desired image size based on sprite size, taking into account camera size
        // Each increase/decrease from a camera size of 3 results in a 33% decrease/increase of sprite size
        Vector2 destSize = new Vector2(spriteRenderer.sprite.rect.width, spriteRenderer.sprite.rect.height) / (cam.orthographicSize / 3);

        while (interpolant < 1) {
            interpolant = (Time.time - startTime) / duration;
            // Lerp image's position and size toward sprite's
            image.transform.position = cam.WorldToScreenPoint(Vector3.Lerp(cam.ScreenToWorldPoint(originPos), spriteRenderer.transform.position, interpolant));
            image.rectTransform.sizeDelta = Vector2.Lerp(originSize, destSize, interpolant);
            yield return null;
        }
    }

    public void UpdateHealth(int newValue) {
        if (0 <= newValue && newValue <= lifeIcons.Length) {
            for (int i = 0; i < newValue; i++) {
                lifeIcons[i].sprite = filledLifeSprite;
                lifeIcons[i].SetNativeSize();
            }
            for (int i = newValue; i < lifeIcons.Length; i++) {
                lifeIcons[i].sprite = emptyLifeSprite;
                lifeIcons[i].SetNativeSize();
            }
        }
    }

    public void UpdateMoneyDisplay(int value) {
        moneyDisplayValue.text = value.ToString();
    }
    
    public void DisableWaveEndTexts() {
        waveCompleteText.gameObject.SetActive(false);
        gunUnlockText.gameObject.SetActive(false);
        treasureLostText.gameObject.SetActive(false);
    }

    // Show wave complete text only, without gun unlock text
    public void ShowWaveComplete() {
        waveCompleteText.gameObject.SetActive(true);
        waveCompleteText.rectTransform.localPosition = treasureLostText.rectTransform.localPosition;
    }

    // Show gun unlock text (and wave complete text)
    public void ShowGunUnlocked() {
        waveCompleteText.gameObject.SetActive(true);
        waveCompleteText.rectTransform.localPosition = originalCompletePos;
        gunUnlockText.gameObject.SetActive(true);
    }

    // Show wave lost text
    public void ShowTreasureLost(string treasureName) {
        treasureLostText.gameObject.SetActive(true);
        treasureLostText.text = treasureName.ToUpper() + " LOST";
    }

    public void InitializeShop(Dictionary<UpgradeType, UpgradableStat> upgrades, int quantity) {
        foreach(UpgradeType key in upgrades.Keys) {
            upgrades[key].upgradeOption.InstantiateCells(quantity);
            upgrades[key].upgradeOption.SetUpgradeInfo(0, 0, upgrades[key].percentBuffPerLevel, upgrades[key].GetCost(0), false);
        }
        UpdateButtonInteractability();
    }

    public void UpdateButtonInteractability() {
        foreach(UpgradeType key in gameManager.upgrades.Keys) {
            // Upgrade must be affordable and not max level
            bool affordable = gameManager.playerMoney >= gameManager.upgrades[key].GetCost(gameManager.playerTurret.statLevels[key]);
            bool upgradeMaxed = gameManager.playerTurret.statLevels[key] >= gameManager.maxUpgradeLevel;
            bool upgradable = affordable && !upgradeMaxed;
            gameManager.upgrades[key].upgradeOption.SetInteractable(upgradable);
        }
    }

    private void UpdateContinueButtonText() {
        continueButtonText.text = "START WAVE " + (gameManager.waveManager.waveIndex + 1);
    }

    public void OpenShop() {
        UpdateButtonInteractability();
        UpdateContinueButtonText();
        shopActive = true;
        shopAnimator.SetTrigger("SlideIn");
    }

    public void CloseShop() {
        if (shopActive) {
            shopActive = false;
            shopAnimator.SetTrigger("SlideOut");
        }
    }

    public void UpdateUpgradeOption(UpgradeOption upgradeOption, int level, float totalPercent, float nextBuffAmount, int cost, bool maxed) {
        upgradeOption.SetUpgradeInfo(level, totalPercent, nextBuffAmount, cost, maxed);
    }

    public void ActivateBorder(int borderToActivate) {
        DeactivateGunBorders();
        gunUiBorders[borderToActivate].SetActive(true);
    }

    private void DeactivateGunBorders() {
        foreach(GameObject border in gunUiBorders) {
            border.SetActive(false);
        }
    }

    public void AmmoDropUI(Vector3 enemyPosition, Gun gun, int dropQuantity) {
        gun.gunUi.UpdateAmmo(gun.ammo);

        // Instantiate UI and set appropriate colour and quantity display
        AmmoDrop ammoDrop = Instantiate(ammoDropPrefab, cam.WorldToScreenPoint(enemyPosition), Quaternion.identity, ammoUiContainer).GetComponent<AmmoDrop>();
        ammoDrop.SetIconColour(gun.colour);
        ammoDrop.SetQuantityDisplay(dropQuantity);

        // Adjust position if popup is offscreen
        Vector3 ammoDropPos = ammoDrop.transform.position;
        Rect rect = ammoDropPrefab.GetComponent<RectTransform>().rect;
        float xCutoff = 0;
        if (ammoDropPos.x - rect.width / 2 < leftScreenPos.x) {
            xCutoff = leftScreenPos.x - (ammoDropPos.x - rect.width / 2);
        } else if (ammoDropPos.x + rect.width / 2 > rightScreenPos.x) {
            xCutoff = -((ammoDropPos.x + rect.width / 2) - rightScreenPos.x);
        }
        ammoDrop.transform.position = ammoDropPos + new Vector3(xCutoff, 0, 0);
    }

    public IEnumerator DisplayGameWon() {
        // Fade out screen
        yield return new WaitForSeconds(1.5f);
        FadeOut(winFadeTime);
        yield return new WaitForSeconds(winFadeTime);
        ClearScreen();
        winTreasureImage.sprite = gameManager.treasure.GetComponent<SpriteRenderer>().sprite;
        winTreasureImage.SetNativeSize();
        winScreen.SetActive(true);
        // Reveal screen again
        yield return new WaitForSeconds(2f);
        FadeIn(0f);
    }

    public IEnumerator DisplayGameOver() {
        yield return new WaitForSeconds(1.5f);
        FadeOut(gameOverFadeTime);
        yield return new WaitForSeconds(gameOverFadeTime);
        ClearScreen();
        gameManager.waveManager.ClearActiveEnemies();
        loseTreasureImage.sprite = gameManager.treasure.GetComponent<Treasure>().gameOverSprite;
        loseTreasureImage.SetNativeSize();
        gameOverScreen.SetActive(true);
        yield return new WaitForSeconds(2f);
        FadeIn(0f);
    }
    
    // Remove sprites and UI for the end game screen
    private void ClearScreen() {
        DisableWaveEndTexts();
        gameManager.player.gameObject.SetActive(false);
        gameManager.treasure.gameObject.SetActive(false);
        panelUI.SetActive(false);
        tileMap.localScale += new Vector3(1, 1, 0);
    }

    public void FadeOut(float fadeTime) {
        StartCoroutine(Fade(inactiveColour, activeColour, fadeTime));
    }

    public void FadeIn(float fadeTime) {
        StartCoroutine(Fade(activeColour, inactiveColour, fadeTime));
    }

    private IEnumerator Fade(Color startColor, Color fadeToColor, float fadeTime) {
        if (fadeTime <= 0) {
            fadeScreen.color = fadeToColor;
        } else {
            float timeElapsed = 0;
            while (timeElapsed < fadeTime) {
                float interpolant = timeElapsed / fadeTime;
                fadeScreen.color = Color.Lerp(startColor, fadeToColor, interpolant);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
