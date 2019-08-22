using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriedObject : Enemy
{
    [SerializeField]
    private List<ItemCarrier> carriers;     // The enemy/enemies carrying this object

    protected override void Update() {
        
    }
    public override void Escape() {

    }

    public override void Die() {
        base.Die();
        foreach(ItemCarrier carrier in carriers) {
            carrier.OnItemDestroyed(gameObject);
        }
    }

    public override void BuffSpeed(float duration) {
        
    }
}
