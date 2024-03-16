using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(
    menuName = "CardGame/Patterns/Random Pattern",
    fileName = "RandomPattern",
    order = 0)]
public class RandomPattern : Pattern
{
    public List<Probability> Probabilities = new List<Probability>(4);

    public override string GetName()
    {
        return "Random Patterm";
    }
}
