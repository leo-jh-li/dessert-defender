using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Titan : Walker
{
    [SerializeField]
    private string defeatSfxName;

    protected override void Start() {
        base.Start();
        gameManager.audioManager.Play(Constants.TITAN_SPAWN);
    }

    // Set boss's spawn cooldown to 0
    public override float GetGroupHealth() {
        return 0;
    }

    public override void Die(){
        base.Die();
        gameManager.audioManager.Play(defeatSfxName);
    }
}
