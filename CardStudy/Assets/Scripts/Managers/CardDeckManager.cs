using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeckManager : MonoBehaviour
{
    private List<RuntimeCard> _deck;
    private List<RuntimeCard> _discardPile;
    private List<RuntimeCard> _hand;

    private const int DeckCapacity = 30;
    private const int HandCapacity = 30;
    private const int DiscardPileCapacity = 30;

    public CardDisplayManager cardDisplayManager;

    private void Awake()
    {
        _deck = new List<RuntimeCard>(DeckCapacity);
        _discardPile = new List<RuntimeCard>(DiscardPileCapacity);
        _hand = new List<RuntimeCard>(HandCapacity);
    }

    public int LoadDeck(List<CardTemplate> deck)
    {
        var deckSize = 0;
        foreach (var template in deck)
        {
            if(template == null)
                continue;

            var card = new RuntimeCard
            {
                Template = template
            };

            _deck.Add(card);
            ++deckSize;
        }

        return deckSize;
    }

    public void ShuffleDeck()
    {
        _deck.Shuffle();
    }

    public void DrawCardFromDeck(int amount)
    {
        var deckSize = _deck.Count;

        if(deckSize >= amount )
        {
            var previousDeckSize = deckSize;

            var drawnCards = new List<RuntimeCard>(amount);

            for(var i = 0; i < amount; i++)
            {
                var card = _deck[0];
                _deck.RemoveAt(0);
                _hand.Add(card);
                drawnCards.Add(card);
            }

            cardDisplayManager.CreateHandCards(drawnCards);
        }
        //���deck��û���㹻���ƣ���Է����ƶ��е���ϴһ���ƣ�Ȼ��ϴ�õ��Ʒ���deck�У�Ȼ��
        //�ٴ�dek��ȡ�Ʒŵ�����
        else
        {
            for(var i = 0;i < _discardPile.Count; i++)
            {
                _deck.Add(_discardPile[i]);
            }

            _discardPile.Clear();

            cardDisplayManager.UpdateDiscardPileSize(_discardPile.Count);//����UI(Ŀǰû�ã�

            //��ֹ����Ƶ���������deck������
            if(amount > _deck.Count + _discardPile.Count)
            {
                amount = _deck.Count + _discardPile.Count;
            }

            DrawCardFromDeck(amount);
        }

    }

    public void MoveCardToDiscardPile(RuntimeCard card)
    {
        _hand.Remove(card);
        _discardPile.Add(card);
    }
}
