using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType {
    BASIC,
    MACHINE,
    SHOTGUN,
    ROCKET,
    LASER
}

public abstract class Gun : MonoBehaviour
{
    [Header("Base Values")]
    public float shotCooldown;
    public float bulletSpeed;
    public float bulletDamage;

    [Space(10)]
    public float ammo;
    public float ammoDropQuantity;
    public float dropQuantityVariance;

    [SerializeField]
    protected float fireCost;
    [SerializeField]
    public Color colour;
    [SerializeField]
    public GunUi gunUi;
    [SerializeField]
    protected GameObject bulletPrefab;
    protected string fireSfxName;

    // Returns a direction vector converted to an angle in degrees
    public static float DirVectToAngle(Vector3 directionVector) {
        return Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg - 90;
    }

    // Returns the direction vector of a shot given the origin and the target
    protected Vector3 GetDirVector(Vector3 origin, Vector3 targetPoint) {
        Vector3 dirVector = targetPoint - origin;
        dirVector.z = 0;
        dirVector.Normalize();
        return dirVector;
    }

    // Return true iff direction points in front of turret (i.e., allow only a 180 degree arc in front of the turret)
    protected bool ValidateDirVect(Vector3 dirVector) {
        return dirVector.x >= 0;
    }

    protected bool CheckCost() {
        return ammo >= fireCost;
    }

    protected void SpendAmmo() {
        ammo -= fireCost;
        gunUi.UpdateAmmo(ammo);
    }

    // Fires toward given targetPoint and returns true iff shot was successful
    public abstract bool Fire(TurretController turret, Vector3 origin, Vector3 targetPoint);

    // Fires a single bullet toward given targetPoint
    protected bool FireAtPoint(TurretController turret, Vector3 origin, Vector3 targetPoint) {
        return FireInDirection(turret, origin, GetDirVector(origin, targetPoint));
    }

    // Fires a single bullet with given direction vector
    protected bool FireInDirection(TurretController turret, Vector3 origin, Vector3 dirVector) {
        if (!ValidateDirVect(dirVector)) {
            return false;
        }
        Bullet bullet = Instantiate(bulletPrefab, origin, Quaternion.identity).GetComponent<Bullet>();
        // Rotate bullet toward direction
        bullet.transform.Rotate(0, 0, DirVectToAngle(dirVector));
        bullet.Initialize(dirVector, bulletSpeed, bulletDamage * turret.damageMultiplier, turret.audioManager, turret.camBehaviour);
        turret.audioManager.Play(fireSfxName);
        return true;
    }

    // Fires a single bullet with given direction vector at given angle (in degrees)
    protected bool FireInDirectionAngled(TurretController turret, Vector3 origin, Vector3 dirVector, float angle) {
        angle *= Mathf.Deg2Rad;
        Vector3 angledDir = new Vector3(dirVector.x * Mathf.Cos(angle) - dirVector.y * Mathf.Sin(angle), dirVector.x * Mathf.Sin(angle) + dirVector.y * Mathf.Cos(angle), dirVector.z);
        return FireInDirection(turret, origin, angledDir);
    }
}
