using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    menuName = "CardGame/Effects/IntegerEffect",
    fileName = "IntegerEffect",
    order = 4)]
public class DealDamageEffect : IntegerEffect, IEntityEffect
{
    public override string GetName()
    {
        return $"Deal {Value.ToString()} damage";
    }

    public override void Resolve(RuntimeCharacter source, RuntimeCharacter target)
    {
        var damage = Value;
        Debug.Log("Deal Damage" + damage);
    }
}
