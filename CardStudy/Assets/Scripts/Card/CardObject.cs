using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Text;
using DG.Tweening;
using UnityEngine.Rendering;

public class CardObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro typeText;
    [SerializeField] private TextMeshPro descriptionText;

    [SerializeField] private SpriteRenderer picture;

    public CardTemplate template;
    public RuntimeCard runtimeCard;

    private Vector3 _savedPosition;
    private Quaternion _savedRotation;
    private int _savedSortingOrder;

    private SortingGroup _sortingGroup;
    

    private void Awake()
    {
        _sortingGroup = GetComponent<SortingGroup>();
    }

    private void Start()
    {
        var testCard = new RuntimeCard
        {
            Template = template
        };

        SetCard(testCard);
    
    }

    public void SetCard(RuntimeCard card)
    {
        runtimeCard = card;
        template = card.Template;

        //picture.sprite = template.picture;

        //costText.text = template.cost.ToString();
        //nameText.text = template.name;
        //typeText.text = template.type.typeName;
        //var builder = new StringBuilder();
        //descriptionText.text = builder.ToString();
    }

    public void SaveTransform(Vector3 position, Quaternion rotation)
    {
        _savedPosition = position;
        _savedRotation = rotation;
        _savedSortingOrder = _sortingGroup.sortingOrder;
    }

    public void Reset(Action onComplete)
    {
        transform.DOMove(_savedPosition,0.2f);
        transform.DORotateQuaternion(_savedRotation, 0.2f);
        _sortingGroup.sortingOrder = _savedSortingOrder;
        onComplete();
    }
}
