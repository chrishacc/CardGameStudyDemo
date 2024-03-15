using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CardSelectionHasArrow : CardSelectionBase
{
    private Vector3 previousClickPosition;//ǰһ���������λ��

    private const float CardDetectionOffset = 2.2f;//ѡ�п��ƺ������ק�������λ�ƾͻἤ����ƶ��Ķ���
    private const float CardAnimationTime = 0.2f;//�����ƶ��Ķ���ʱ��

    private const float SelectionCardYOffset = -0.92f;//ѡ�еĿ��ƻ������ƶ��������
    private const float AttackCardInMiddlePositionX = -0.15f;//�����������м��λ�� ������ʵ���������

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
            //�������Ƿ�ѡ����һ������(���߼�⣩
            var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hitInfo = Physics2D.Raycast(mousePosition, Vector3.forward, Mathf.Infinity, enemyLayer);
            if(hitInfo.collider != null)
            {
                _selectedEnemy = hitInfo.collider.gameObject;
                PlaySelectedCard();

                selectedCard = null;

                //��ʱ���������£���ʱ��Ҫȡ��������ʾ������ͷ��Ϊ��������Ч������ʾ��׼��
                isArrowCreated = false;
                _attackArrow.EnableArrow(false);
            }

        }
    }

    private void DetectCardSelection()
    {
        //�������Ƿ��ڿ��Ƶ��Ϸ����˵������
        var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hitInfo = Physics2D.Raycast(mousePosition, Vector3.forward,Mathf.Infinity,cardLayer);

        if(hitInfo.collider != null)
        {
            //�����ҵ���˿��ƣ���ѡ�����ſ���
            selectedCard = hitInfo.collider.gameObject;
            selectedCard.GetComponent<SortingGroup>().sortingOrder += 10;
            previousClickPosition = mousePosition;
        }
    }

    //���¿��ƺͼ�ͷ��λ��
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
