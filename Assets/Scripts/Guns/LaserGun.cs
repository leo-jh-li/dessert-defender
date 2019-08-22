using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : Gun
{
    
    [SerializeField]
    private GameObject laser;
    [SerializeField]
    private LaserBeam laserBeam;

    void Awake() {
        fireSfxName = Constants.LASER_FIRE;
    }

    public override bool Fire(TurretController turret, Vector3 origin, Vector3 targetPoint) {
        if (!CheckCost()) {
            return false;
        }
        Vector3 dirVector = GetDirVector(origin, targetPoint);
        if (!ValidateDirVect(dirVector)) {
            return false;
        }
        SpendAmmo();
        // Set laser beam's damage and rotate it to proper position
        laser.SetActive(true);
        laserBeam.FireBeam(bulletDamage * turret.damageMultiplier, Gun.DirVectToAngle(dirVector));
        turret.audioManager.Play(fireSfxName);
        turret.camBehaviour.TriggerWeakShake();
        return true;
    }
}
