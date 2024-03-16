using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class CardSelectionNoArrow : CardSelectionBase
{
    private Vector3 originalCardPosition;
    private Quaternion originalCardRotation;
    private int originalCardSortingOrder;

    private const float cardCancelAnimationTime = 0.2f;
    private const Ease cardAnimationEase = Ease.OutBack;

    private const float CardAboutToBePlayedOffsetY = 1.5f;//选中卡牌后超过该缓冲位移才会被认为是选中
    private const float CardAnimationTime = 0.4f;//卡牌移动的动画时间
    [SerializeField] private BoxCollider2D cardArea;

    private bool isCardAboutToBePlayed;//选中卡牌准备打出的状态

    private void Update()
    {
        if(cardDisplayManager.isMoving())
            return;

        if(isCardAboutToBePlayed)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            DetectCardSelection();
        }
        else if(Input.GetMouseButtonDown(1))
        {
            DetectCardUnselection();
        }

        if(selectedCard != null)
        {
            UpdataSelectedCard();
        }

    }

    

    private void DetectCardSelection()
    {
        if(selectedCard != null)
        {
            return;
        }

        //判断玩家是否在卡牌上方点击鼠标
        var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hitInfo = Physics2D.Raycast(mousePosition, Vector3.forward, Mathf.Infinity,cardLayer);

        if(hitInfo.collider != null)
        {
            var card = hitInfo.collider.GetComponent<CardObject>();
            var cardTemplate = card.template;

            if(!CardUtils.CardHasTargetableEffect(cardTemplate))
            {
                selectedCard = hitInfo.collider.gameObject;
                originalCardPosition = selectedCard.transform.position;
                originalCardRotation = selectedCard.transform.rotation;
                originalCardSortingOrder = selectedCard.GetComponent<SortingGroup>().sortingOrder;
                //selectedCard.transform.position = new Vector3(selectedCard.transform.position.x, selectedCard.transform.position.y, -1.0f);
                //selectedCard.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                //selectedCard.GetComponent<SpriteRenderer>().sortingOrder = 100;
            }

        }
    }

    private void UpdataSelectedCard()
    {
        //处理鼠标左键已经松开时的逻辑
        if(Input.GetMouseButtonUp(0))
        {
            var card = selectedCard.GetComponent<CardObject>();

            //如果选中卡牌后，鼠标左键已经松开且选中的卡牌是准备打出的状态
            if(card.State == CardObject.CardState.AboutToBePlayed)
            {
                //设置此状态用于屏蔽掉卡牌的拖拽效果
                isCardAboutToBePlayed = true;

                //移动非攻击卡牌到效果施放区域
                var sequence = DOTween.Sequence();

                sequence.Append(selectedCard.transform.DOMove(cardArea.bounds.center, CardAnimationTime)
                    .SetEase(cardAnimationEase));
                sequence.AppendInterval(CardAnimationTime + 0.1f);
                sequence.AppendCallback(() =>
                {
                    //开始施放效果
                    PlaySelectedCard();
                    selectedCard = null;
                    isCardAboutToBePlayed = false;
                });

                selectedCard.transform.DORotate(Vector3.zero, CardAnimationTime);

            }
            //如果选中卡牌后，发现不想打这张牌，且卡牌移动的距离还不足以触发让非攻击卡牌进入
            //效果施放区的动画，这时重置卡牌状态，并把它放置回起始位置
            else
            {
                card.SetState(CardObject.CardState.InHand);
                selectedCard.GetComponent<CardObject>().Reset(() =>
                {
                    selectedCard = null;
                });
            }

        }

        if(selectedCard != null)
        {
            var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0.0f;
            selectedCard.transform.position = mousePosition;
            Debug.Log("selectedCard != null");

            var card = selectedCard.GetComponent<CardObject>();

            //检测非攻击卡牌再选中后的距离是否足够大，可以改变它的出牌状态
            //如果足够大，则脱离手握牌状态，进入待出牌状态
            if(mousePosition.y > originalCardPosition.y + CardAboutToBePlayedOffsetY)
            {
                card.SetState(CardObject.CardState.AboutToBePlayed);
            }
            else
            {
                card.SetState(CardObject.CardState.InHand);
            }

        }
    }

    private void DetectCardUnselection()
    {
        if(selectedCard != null)
        {
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                selectedCard.transform.DOMove(originalCardPosition, cardCancelAnimationTime).SetEase(cardAnimationEase);
                selectedCard.transform.DORotate(originalCardRotation.eulerAngles, cardCancelAnimationTime);
                //selectedCard.GetComponent<SortingGroup>().sortingOrder = originalCardSortingOrder;
            });
            sequence.OnComplete(() =>
            {
                selectedCard.GetComponent<SortingGroup>().sortingOrder = originalCardSortingOrder;
                selectedCard = null;
            });
        }
    }
}
