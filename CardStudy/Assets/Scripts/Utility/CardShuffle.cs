using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public static class CardShuffle
{
    private static readonly Random random = new Random();
    
    public static void Shuffle<T>(this List<T> list)
    {
        var n = list.Count;
        while (n-- > 1)
        { 
            
            var index = random.Next( n + 1 );
            var value = list[index];
            list[index] = list[n];
            list[n] = value;
        }
    }
}
