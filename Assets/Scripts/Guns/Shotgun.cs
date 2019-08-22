using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    [SerializeField]
    private float[] angledShots;    // Array of angles (in degrees) at which pairs of shots will be fired

    void Awake() {
        fireSfxName = Constants.SHOTGUN_FIRE;
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
        FireInDirection(turret, origin, dirVector);
        foreach(float angle in angledShots) {
            // Fire at angle and -angle
            FireInDirectionAngled(turret, origin, dirVector, angle);
            FireInDirectionAngled(turret, origin, dirVector, -angle);
        }
        turret.camBehaviour.TriggerMildShake();
        return true;
    }
}
