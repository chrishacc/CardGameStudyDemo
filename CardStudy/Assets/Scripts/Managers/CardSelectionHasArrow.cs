using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CardSelectionHasArrow : CardSelectionBase
{
    private Vector3 previousClickPosition;//前一次鼠标点击的位置

    private const float CardDetectionOffset = 2.2f;//选中卡牌后，鼠标拖拽超过这个位移就会激活卡牌移动的动画
    private const float CardAnimationTime = 0.2f;//卡牌移动的动画时间

    private const float SelectionCardYOffset = -0.92f;//选中的卡牌会向上移动这个距离
    private const float AttackCardInMiddlePositionX = -0.15f;//攻击卡牌在中间的位置 ，根据实际情况调整

    private AttackArrow _attackArrow;

    private bool isArrowCreated;
    private GameObject _selectedEnemy;

    protected override void Start()
    {
        base.Start();
        _attackArrow = FindFirstObjectByType<AttackArrow>();
    }

    private void Update()
    {
        if(cardDisplayManager.isMoving())
        {
            return;
        }
        if(Input.GetMouseButtonDown(0))
        {
            DetectCardSelection();
            DetectEnemySelection();
        }
        else if(Input.GetMouseButtonDown(0))
        {
            DetectEnemySelection();
        }
        else if(Input.GetMouseButtonDown(1))
        {
            DetectCardUnselection();
        }

        if(selectedCard != null)
        {
            UpdataCardAndTargetingArrow();
        }
    }

    
    private void DetectEnemySelection()
    {
        if(selectedCard != null)
        {
            //检查鼠标是否选择了一个敌人(射线检测）
            var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hitInfo = Physics2D.Raycast(mousePosition, Vector3.forward, Mathf.Infinity, enemyLayer);
            if(hitInfo.collider != null)
            {
                _selectedEnemy = hitInfo.collider.gameObject;
                PlaySelectedCard();

                selectedCard = null;

                //这时鼠标左键按下，这时需要取消攻击显示攻击箭头，为后续攻击效果的显示做准备
                isArrowCreated = false;
                _attackArrow.EnableArrow(false);
            }

        }
    }

    private void DetectCardSelection()
    {
        //检查玩家是否在卡牌的上方做了点击操作
        var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hitInfo = Physics2D.Raycast(mousePosition, Vector3.forward,Mathf.Infinity,cardLayer);

        if(hitInfo.collider != null)
        {
            //如果玩家点击了卡牌，就选中这张卡牌
            selectedCard = hitInfo.collider.gameObject;
            selectedCard.GetComponent<SortingGroup>().sortingOrder += 10;
            previousClickPosition = mousePosition;
        }
    }

    //更新卡牌和箭头的位置
    private void UpdataCardAndTargetingArrow()
    {
        var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var diffY = mousePosition.y - previousClickPosition.y;

        if(!isArrowCreated && diffY > CardDetectionOffset)
        {
            isArrowCreated = true;

            var position = selectedCard.transform.position;

            selectedCard.transform.DOKill();

            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                selectedCard.transform.DOMove(new Vector3(AttackCardInMiddlePositionX, SelectionCardYOffset,position.y),
                    CardAnimationTime);

                selectedCard.transform.DORotate(Vector3.zero, CardAnimationTime);
            });

            sequence.AppendInterval(0.15f);
            sequence.OnComplete(() =>
            {
                _attackArrow.EnableArrow(true);
            });

        }
    }

    private void DetectCardUnselection()
    {
        if(selectedCard != null)
        {
            var card = selectedCard.GetComponent<CardObject>();
            selectedCard.transform.DOKill();
            card.Reset(() =>
            {
                isArrowCreated = false;
                selectedCard = null;
            });

            _attackArrow.EnableArrow(false);
        }
    }

    protected override void PlaySelectedCard()
    {
        base.PlaySelectedCard();
        var card = selectedCard.GetComponent<CardObject>().runtimeCard;
        effectResolutionManager.ResolutionCardEffects(card, _selectedEnemy.GetComponent<CharacterObject>());
    }
}
