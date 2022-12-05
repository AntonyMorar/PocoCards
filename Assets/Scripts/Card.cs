using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card : MonoBehaviour
{
    // Serialized **********
    [SerializeField] private LayerMask layer;
    // Private **********
    private CardData _cardData;

    private Camera _camera;
    private bool _isSelectable = true;
    private Player _owner;
    private bool _isMine;
    private bool _inBoard;


    // MonoBehaviour Callbacks **********
    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(Input.mousePosition),50,layer);

            if (hit.collider != null && hit.collider.transform == transform)
            {
                Debug.Log("Down: " + _cardData.cardName);
                if (!_isMine || !_isSelectable) return;

                if (_inBoard) AddToHand();
                else AddToBoard();
            }
            

        }
    }

    // Public Methods **********
    public void SetCard(CardData cardData, Player owner)
    {
        _cardData = cardData;
        _owner = owner;
        _isMine = GameManager.Instance.GetMyPlayer() == owner;
    }
    
    // Private Methods **********
    private void OnMouseDown()
    {

    }

    private void AddToBoard()
    {
        transform.position += Vector3.up * 2.5f;
        _inBoard = true;
        
        _owner.RemoveFromHand(this);
        Board.Instance.AddToBoardHand(this);
    }

    private void AddToHand()
    {
        transform.position += Vector3.down * 2.5f;
        _inBoard = false;
        
        _owner.AddToHand(this);
        Board.Instance.RemoveFromBoardHand(this);
    }
}
