using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "CardGame/Templates/Card",order = 0)]
public class CardTemplate : ScriptableObject
{
    public int id;
    public string cardName;
    public int cost;
    public CardType type;
    public Sprite picture;

    public List<Effect> Effects = new List<Effect>();
}
