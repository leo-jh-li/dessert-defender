using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : Gun
{
    void Awake() {
        fireSfxName = Constants.ROCKET_FIRE;
    }

    public override bool Fire(TurretController turret, Vector3 origin, Vector3 targetPoint) {
        if (!CheckCost()) {
            return false;
        }
        bool fired = FireAtPoint(turret, origin, targetPoint);
        if (fired) {
            SpendAmmo();
            turret.rocketFireParticleSys.Play();
            turret.camBehaviour.TriggerShake();
        }
        return fired;
    }
}
