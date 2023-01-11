using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    // Serialized ****
    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private SelectionPlayer selectionPlayer;
    [SerializeField] private List<LevelNode> levelNodeList;
    [SerializeField] private float speed = 2f;

    // Private ****
    private SelectionPlayer _selectionPlayer;
    private int _selectedLevel;
    // Movement
    private bool _moving;
    private Vector3 _targetPosition;

    // MonoBehavior Callbacks
    private void OnEnable()
    {
        mainMenuUI.OnTraveling += MainMenuUI_OnTraveling;
    }

    private void OnDisable()
    {
        mainMenuUI.OnTraveling -= MainMenuUI_OnTraveling;
    }

    private void Update()
    {
        UpdateInput();

        if (!mainMenuUI.IsTraveling()) return;
        _selectionPlayer.transform.position = Vector3.MoveTowards(_selectionPlayer.transform.position, _targetPosition, speed * Time.deltaTime);
        if (Vector3.Distance(_selectionPlayer.transform.position, _targetPosition) < 0.05f) _moving = false;
    }

    private void UpdateInput()
    {
        if (_moving || !mainMenuUI.IsTraveling()) return;
        
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            GameManager.Instance.SelectLevel(_selectedLevel);
            LevelsManager.Instance.ChangeScene(GameManager.SceneState.InGame);
        }
        
        // Move
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            TryMove(Direction.Left);
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            TryMove(Direction.Down);
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            TryMove(Direction.Right);
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            TryMove(Direction.Up);
        }
    }
    
    // Private Methods ****
    private bool TryMove(Direction inputDirection)
    {
        LevelNode levelNode = levelNodeList[_selectedLevel];

        // Last Level
        foreach (Direction direction in levelNode.lastLevel)
        {
            if (direction == inputDirection)
            {
                _selectedLevel--;
                
                SoundManager.PlaySound(SoundManager.Sound.UiSelect);
                _moving = true;
                _targetPosition = levelNodeList[_selectedLevel].spawnTransform.position;
                return true;
            }
        }
        
        // Last Level
        foreach (Direction direction in levelNode.nextLevel)
        {
            if (direction == inputDirection)
            {
                _selectedLevel++;
                
                SoundManager.PlaySound(SoundManager.Sound.UiSelect);
                _moving = true;
                _targetPosition = levelNodeList[_selectedLevel].spawnTransform.position;
                return true;
            }
        }

        return false;
    }

    private void MainMenuUI_OnTraveling(object sender, bool traveling)
    {
        if (traveling)
        {
            _selectionPlayer = Instantiate(selectionPlayer, levelNodeList[_selectedLevel].spawnTransform.position, Quaternion.identity, transform);
            _targetPosition = levelNodeList[_selectedLevel].spawnTransform.position;
        }
        else
        {
            _selectionPlayer.Hide();
        }
    }

    // Public Data ****
    public enum Direction
    {
        Left,
        Down,
        Right,
        Up
    }

    [Serializable]
    public struct LevelNode
    {
        public Transform spawnTransform;
        public int level;
        public Direction[] lastLevel;
        public Direction[] nextLevel;
    }
}
