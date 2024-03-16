using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

public class GameDriver : MonoBehaviour
{
    public CardBank startingDeck;

    private GameObject player;
    private List<GameObject> enemies = new List<GameObject>();
    
    [Header("Managers")]
    [SerializeField]private CardManager cardManager;

    [SerializeField]private CardDeckManager cardDeckManager;

    [SerializeField]private CardDisplayManager cardDisplayManager;

    [SerializeField]private EffectResolutionManager effectResolutionManager;

    [SerializeField] private CardSelectionHasArrow cardSelectionHasArrow;

    private List<CardTemplate> playerDeck = new List<CardTemplate>();

    [Header("Character Pivots")]
    [SerializeField] public Transform playerPivot;
    [SerializeField] public Transform enemyPivot;

    [SerializeField] private AssetReference enemyTemplate;
    [SerializeField] private AssetReference playerTemplate;

    private void Start()
    {
        cardManager.Initialize();

        CreatePlayer(playerTemplate);
        CreateEnemy(enemyTemplate);
        
    }

    private void CreatePlayer(AssetReference playerTemplateReference)
    {
        var handle = Addressables.LoadAssetAsync<HeroTemplate>(playerTemplateReference);
        handle.Completed += operationResult =>
        {
            var template = operationResult.Result;
            player = Instantiate(template.Prefab, playerPivot);
            Assert.IsNotNull(player);

            foreach (var item in template.StartingDeck.Items)
            {
                for (int i = 0; i < item.Amount; i++)
                {
                    playerDeck.Add(item.Card);
                }
            }

            var obj = player.GetComponent<CharacterObject>();
            obj.Template = template;
            obj.Character = new RuntimeCharacter()//临时初始化
            {
                Hp = 100,
                Shield = 100,
                Mana = 100,
                MaxHp = 100
            };


            Initialize();
        };



        
    }

    private void CreateEnemy(AssetReference templateReference)
    {
        var handle = Addressables.LoadAssetAsync<EnemyTemplate>(templateReference);
        handle.Completed += operationResult =>
        {
            var pivot = enemyPivot;
            var template = operationResult.Result;
            var enemy = Instantiate(template.Prefab, pivot);

            Assert.IsNotNull(enemy);

            var obj = enemy.GetComponent<CharacterObject>();
            obj.Template = template;
            obj.Character = new RuntimeCharacter()//临时初始化
            {
                Hp = 100,
                Shield = 100,
                Mana = 100,
                MaxHp = 100
            };

            enemies.Add(enemy);
        };
    }

    public void Initialize()
    {
        cardDeckManager.LoadDeck(playerDeck);
        cardDeckManager.ShuffleDeck();

        cardDisplayManager.Initialize(cardManager);

        cardDeckManager.DrawCardFromDeck(5);

        var playerCharacter = player.GetComponent<CharacterObject>();
        var enemyCharacters = new List<CharacterObject>(enemies.Count);

        foreach (var enemy in enemies)
        {
            enemyCharacters.Add(enemy.GetComponent<CharacterObject>());
        }

        cardSelectionHasArrow.Initialize(playerCharacter, enemyCharacters);
        effectResolutionManager.Initialize(playerCharacter, enemyCharacters);
    }
}
