using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : Gun
{
    void Awake() {
        fireSfxName = Constants.BASIC_FIRE;
    }

    public override bool Fire(TurretController turret, Vector3 origin, Vector3 targetPoint) {
        return FireAtPoint(turret, origin, targetPoint);
    }
}
