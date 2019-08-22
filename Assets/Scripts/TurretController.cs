using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [HideInInspector]
    public bool controlEnabled;
    public Gun[] guns;
    private Gun currGun;
    private int unlockedGuns;
    [SerializeField]
    private int maxAmmo = 9999;
    private float remainingShotCooldown;

    // Upgrade levels and values
    [SerializeField]
    public Dictionary<UpgradeType, int> statLevels;
    [HideInInspector]
    public float damageMultiplier;
    [HideInInspector]
    public float fireRateMultiplier;
    [HideInInspector]
    public float dropBonus;     // Bonus % added to ammo drop rate
    private float baseDps;      // DPS of basic gun, without upgrades
    private float machineDps;   // DPS of machine gun, without upgrades

    [Space(10)]
    [HideInInspector]
    public AudioManager audioManager;
    [SerializeField]
    private Transform bulletSpawnpoint;
    public ParticleSystem rocketFireParticleSys;
    [SerializeField]
    private LayerMask whatIsPowerup;
    [SerializeField]
    private RectTransform uiPanel;
    private bool firing;
    private Camera cam;
    [HideInInspector]
    public CameraBehaviour camBehaviour;

    void Awake() {
        statLevels = new Dictionary<UpgradeType, int>();
        foreach(UpgradeType upgradeType in (UpgradeType[])System.Enum.GetValues(typeof(UpgradeType))) {
            statLevels[upgradeType] = 0;
        }
        audioManager = gameManager.audioManager;
    }

    void Start()
    {
        cam = Camera.main;
        camBehaviour = cam.GetComponent<CameraBehaviour>();
        currGun = guns[0];
        damageMultiplier = 1;
        fireRateMultiplier = 1;
        unlockedGuns = 1;
        for(int i = unlockedGuns; i < guns.Length; i++) {
            guns[i].gunUi.gameObject.SetActive(false);
        }
        controlEnabled = false;
        baseDps = guns[0].bulletDamage / guns[0].shotCooldown;
        machineDps = guns[1].bulletDamage / guns[1].shotCooldown;
    }
    
    void Update()
    {
        if (controlEnabled) {
            remainingShotCooldown -= Time.deltaTime;

            Vector3 screenInteractPos = Vector3.zero;    // Position of touch/mouse this update in screen space
            Vector3 interactPosition = Vector3.zero;
            bool interactBegan = false;     // Whether this touch/click began this update

            // Tap controls
            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);
                interactBegan = touch.phase == TouchPhase.Began;
                screenInteractPos = touch.position;
                interactPosition = cam.ScreenToWorldPoint(screenInteractPos);
                // If this touch began this frame, and the touch is not on the UI panel, start firing
                if (interactBegan && !RectTransformUtility.RectangleContainsScreenPoint(uiPanel, (Vector2)screenInteractPos) ) {
                    firing = true;
                }
            }
            // Mouse controls
            else if (Input.GetButton("Fire1")) {
                interactBegan = Input.GetButtonDown("Fire1");
                screenInteractPos = Input.mousePosition;
                interactPosition = cam.ScreenToWorldPoint(screenInteractPos);
                if (interactBegan && !RectTransformUtility.RectangleContainsScreenPoint(uiPanel, (Vector2)screenInteractPos) ) {
                    firing = true;
                }
            } else {
                // Stop firing if there is no touch nor click
                firing = false;
            }
            if (firing && remainingShotCooldown <= 0) {
                if (currGun.Fire(this, bulletSpawnpoint.position, interactPosition)) {
                    remainingShotCooldown = Mathf.Max(0, currGun.shotCooldown * fireRateMultiplier);
                }
            }
        }

        // Change weapons with 1-5
        if (Input.GetKey(KeyCode.Alpha1)) {
            AttemptChangeGun(GunType.BASIC);
        } else if (Input.GetKey(KeyCode.Alpha2)) {
            AttemptChangeGun(GunType.MACHINE);
        } else if (Input.GetKey(KeyCode.Alpha3)) {
            AttemptChangeGun(GunType.SHOTGUN);
        } else if (Input.GetKey(KeyCode.Alpha4)) {
            AttemptChangeGun(GunType.ROCKET);
        } else if (Input.GetKey(KeyCode.Alpha5)) {
            AttemptChangeGun(GunType.LASER);
        }
    }

    public void SetControl(bool enable) {
        if (enable) {
            if (!controlEnabled) {
                controlEnabled = true;
                remainingShotCooldown = 0;
            }
        } else {
            controlEnabled = false;
            firing = false;
        }
    }

    // Gets money's effect on DPS, roughly calculated, as a multiplier
    private float GetMoneyMult() {
        // Add a 1% increase in DPS for each unit of money earned
        return 1 + gameManager.expectedMoney / 100f * 2f;
    }

    // Returns roughly calculated DPS of the basic gun, taking into account money earned
    public float GetBasicDPS() {
        return baseDps * GetMoneyMult();
    }

    // Returns roughly calculated machine gun DPS, taking into account money earned
    private float GetMachineGunDPS() {
        return machineDps * GetMoneyMult();
    }

    // Returns machine gun DPS if the gun is unlocked; basic gun DPS togetherwise
    public float GetTopDPS() {
        return unlockedGuns > 1 ? GetMachineGunDPS() : GetBasicDPS();
    }

    // Returns a random type of (unlocked) gun that has ammo (i.e. a GunType that's not the basic gun); BASIC type if none are available
    public GunType GetRandomAmmoType() {
        if (unlockedGuns <= 1) {
            Debug.LogWarning("GetRandomAmmoType(): No available GunType.");
            return GunType.BASIC;
        }
        return (GunType)Random.Range(1, unlockedGuns);
    }

    // Unlocks the next gun if there is one and returns true iff unlock was successful
    public bool UnlockNextGun() {
        if (unlockedGuns < guns.Length) {
            guns[unlockedGuns].gunUi.gameObject.SetActive(true);
            guns[unlockedGuns].gunUi.UpdateAmmo(guns[unlockedGuns].ammo);
            unlockedGuns++;
            return true;
        } else {
            Debug.LogWarning("No more guns to unlock.");
            return false;
        }
    }

    public void AttemptChangeGun(GunType gunType) {
        if ((int)gunType < unlockedGuns && guns[(int)gunType] != currGun) {
            ChangeGun(gunType);
        }
    }

    public void AttemptChangeGun(int gunType) {
        AttemptChangeGun((GunType)gunType);
    }

    private void ChangeGun(GunType gunType) {
        audioManager.Play(Constants.SELECT_GUN);
        currGun = guns[(int)gunType];
        gameManager.uiManager.ActivateBorder((int)gunType);
    }

    public void IncreaseAmmo(GunType gunType, int value) {
        guns[(int)gunType].ammo = Mathf.Clamp(guns[(int)gunType].ammo + value, 0, maxAmmo);
    }

    public void SetStat(UpgradeType upgradeType, int newLevel, float newValue) {
        statLevels[upgradeType] = newLevel;
        if (upgradeType == UpgradeType.DAMAGE) {
            damageMultiplier = 1 + newValue;
        } else if (upgradeType == UpgradeType.FIRE_RATE) {
            fireRateMultiplier = 1 - newValue;
        } else if (upgradeType == UpgradeType.DROP_CHANCE) {
            dropBonus = newValue;
        }
    }
}
