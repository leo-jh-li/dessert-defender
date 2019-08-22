using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Gun
{
    [SerializeField]
    private float maxSpreadAngle;    // Maximum value angle a shot can be off by

    void Awake() {
        fireSfxName = Constants.MACH_FIRE;
    }

    public override bool Fire(TurretController turret, Vector3 origin, Vector3 targetPoint) {
        if (!CheckCost()) {
            return false;
        }
        Vector3 dirVector = GetDirVector(origin, targetPoint);
        // Fire with some spread based on maxSpreadAngle
        bool fired = FireInDirectionAngled(turret, origin, dirVector, Random.Range(-maxSpreadAngle, maxSpreadAngle));
        if (fired) {
            SpendAmmo();
        }
        return fired;
    }
}
