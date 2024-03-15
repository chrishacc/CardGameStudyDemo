using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectionBase : BaseManager
{
    protected Camera mainCamera;
    public LayerMask cardLayer;

    public CardDisplayManager cardDisplayManager;
    public EffectResolutionManager effectResolutionManager;
    public CardDeckManager deckManager;

    protected GameObject selectedCard;

    public LayerMask enemyLayer;

    protected virtual void Start()
    {
        mainCamera = Camera.main;
    }

    protected virtual void PlaySelectedCard()
    {
        var cardObject = selectedCard.GetComponent<CardObject>();
        var cardTemplate = cardObject.template;


        cardDisplayManager.ReOrganizeHandCards(selectedCard);

        //�����ƴ���󣬽�����ѡ�еĿ����Ƴ�
        cardDisplayManager.MoveCardToDiscardPile(selectedCard);
        deckManager.MoveCardToDiscardPile(cardObject.runtimeCard);
    }
}
