using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(
    menuName = "CardGame/Patterns/Repeat Pattern",
    fileName = "RepeatPattern",
    order = 0)]
public class RepeatPattern : Pattern
{
    public int Times;//所要重复的次数

    public override string GetName()
    {
        return $"Repeat x {Times.ToString()}";
    }
}
