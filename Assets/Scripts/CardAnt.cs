using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnt : Card
{
    private State _state;
    private float _stateTimer;
    
    private bool _canMakeAction;
    private bool _canAttack;
    protected override void Update()
    {
        base.Update();

        if (!_isActive) return;
        
        _stateTimer -= Time.deltaTime;
        switch (_state)
        {
            case State.Flipping:
                break;
            case State.Action:
                if (_canMakeAction)
                {
                    Debug.Log("Ant action: add shield");
                    _canMakeAction = false;
                }
                break;
            case State.Attacking:
                if (_canAttack)
                {
                    MakeAttack();
                    _canAttack = false;
                }
                break;
            case State.CoolOff:
                break;
        }
        
        if (_stateTimer <= 0)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (_state)
        {
            case State.Flipping:
                _state = State.Action;
                _stateTimer = 2f;
                break;
            case State.Action:
                _state = State.Attacking;
                _stateTimer = 0.5f;
                break;
            case State.Attacking:
                _state = State.CoolOff;
                _stateTimer = 0.5f;
                break;
            case State.CoolOff:
                ActionComplete();
                break;
        }
    }
    
    // Public Methods *****
    public override void TakeAction(Action onRevealComplete)
    {
        ActionStart(onRevealComplete);

        _state = State.Flipping;
        _stateTimer = 1f;

        _canMakeAction = true;
        _canAttack = true;
    }
}
