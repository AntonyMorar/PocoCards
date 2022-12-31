using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckEditorUI : MonoBehaviour
{
    // Constants ****
    private const int MAX_DECK = 8;
    
    // Serialized ****
    [SerializeField] private CardUI cardUiPrefab;
    [Header("Reference")]
    [SerializeField] private RectTransform playerHandTransform;
    [SerializeField] private RectTransform collectionHandTransform;
    [Header("Info Reference")]
    [SerializeField] private TMP_Text infoName;
    [SerializeField] private Image infoImage;
    [SerializeField] private TMP_Text infoDescription;
    [SerializeField] private TMP_Text infoCost;
    [SerializeField] private Button useButton;
    [SerializeField] private Button removeButton;
    
    // Private ****
    private GraphicRaycaster _raycaster;
    
    private List<CardUI> _playerHand = new List<CardUI>();
    private List<CardUI> _collectionHand = new List<CardUI>();
    private CardUI _selectedCard;
    
    // MonoBehavior Callbacks ****
    private void Awake()
    {
        _raycaster = GetComponent<GraphicRaycaster>();
    }

    private void Start()
    {
        GameManager.Instance.SetState(GameManager.SceneState.DeckEditor);

        foreach (PlayerProfile.PlayerCard playerCard in GameManager.Instance.GetPlayerProfile().allDeck)
        {
            if(!playerCard.unlocked) continue;

            switch (playerCard.inDeck)
            {
                case true:
                    CardUI handCard = Instantiate(cardUiPrefab, playerHandTransform);
                    handCard.SetCard(playerCard.cardData);
                    _playerHand.Add(handCard);
                    break;
                case false:
                    CardUI collectionCard = Instantiate(cardUiPrefab, collectionHandTransform);
                    collectionCard.SetCard(playerCard.cardData);
                    _collectionHand.Add(collectionCard);
                    break;
            }
        }
    }

    private void Update()
    {
        //Check if the left Mouse button is clicked
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();
 
            //Raycast using the Graphics Raycaster and mouse click position
            pointerData.position = Input.mousePosition;
            _raycaster.Raycast(pointerData, results);
 
            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.TryGetComponent(out CardUI cardUI) && cardUI != _selectedCard)
                {
                    SoundManager.PlaySound(SoundManager.Sound.UiChange);
                    SetSelectedCard(cardUI);
                }
                
            }
        }
    }

    private void OnEnable()
    {
        InputSystem.OnEscDeck += InputSystem_OnEscDeck;
        useButton.onClick.AddListener(() => AddToHand(_selectedCard));
        removeButton.onClick.AddListener(() => AddToCollection(_selectedCard));
    }

    private void OnDisable()
    {
        InputSystem.OnEscDeck -= InputSystem_OnEscDeck;
        useButton.onClick.RemoveListener(() => AddToHand(_selectedCard));
        removeButton.onClick.RemoveListener(() => AddToCollection(_selectedCard));
    }
    
    // Private Methods
    private void InputSystem_OnCardClick(object sender, CardUI baseCard)
    {
        if (_playerHand.Contains(baseCard))
        {
            AddToCollection(baseCard);
        }
        else if (_collectionHand.Contains(baseCard))
        {
            AddToHand(baseCard);
        }
    }
    
    // Use when deck is incomplete
    private void InputSystem_OnEscDeck(object sender, EventArgs e)
    {
        if (_playerHand.Count < MAX_DECK)
        {
            DeckWarning();
        }
        else
        {
            GoBack();
        }
    }
    
    private void AddToHand(CardUI baseCard)
    {
        if (_playerHand.Count >= MAX_DECK)
        {
            Debug.Log("Max reached");
            SoundManager.PlaySound(SoundManager.Sound.UiError);
            return;
        }
        
        SoundManager.PlaySound(SoundManager.Sound.UiChange);
        _playerHand.Add(baseCard);
        _collectionHand.Remove(baseCard);
        baseCard.transform.SetParent(playerHandTransform);
        
        // Update GameManager data
        GameManager.Instance.ChangeDeck(baseCard.GetCardData(), true);
        
        // Update Info
        SetSelectedCard(baseCard);
    }
    
    private void AddToCollection(CardUI baseCard){
        
        SoundManager.PlaySound(SoundManager.Sound.UiChange);
        _playerHand.Remove(baseCard);
        _collectionHand.Add(baseCard);
        baseCard.transform.SetParent(collectionHandTransform);
        
        // Update GameManager data
        GameManager.Instance.ChangeDeck(baseCard.GetCardData(), false);
        
        // Update Info
        SetSelectedCard(baseCard);
    }

    private void SetSelectedCard(CardUI cardData)
    {
        _selectedCard = cardData;
        CardData cardDataTemp = _selectedCard.GetCardData();

        infoName.text = cardDataTemp.cardName;
        infoImage.sprite = cardDataTemp.cardIcon;
        infoDescription.text = cardDataTemp.description;
        infoCost.text = "Cost: " + cardDataTemp.cost;

        if (IsInPlayerHand())
        {
            useButton.gameObject.SetActive(false);
            removeButton.gameObject.SetActive(true);
        }
        else
        {
            useButton.gameObject.SetActive(true);
            removeButton.gameObject.SetActive(false);
        }

    }

    private bool IsInPlayerHand()
    {
        foreach (CardUI cardUI in _playerHand)
        {
            if (cardUI == _selectedCard) return true;
        }
        
        return false;
    }
    
    private void GoBack()
    {
        //Audio
        SoundManager.PlaySound(SoundManager.Sound.UiSelect);
        GameManager.Instance.Save();
        LevelsManager.Instance.ChangeScene(GameManager.SceneState.MainMenu);
    }
    
    private void DeckWarning()
    {
        Debug.Log("the deck must contain " + MAX_DECK + "Cards");
    }
}
