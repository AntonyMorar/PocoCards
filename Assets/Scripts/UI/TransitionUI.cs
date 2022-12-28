using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUI : MonoBehaviour
{
    // Private ****
    private Animator _animator;
    
    // MonoBehavior Callbacks ****
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        LevelsManager.Instance.OnSceneLoad += LevelsManager_OnSceneLoad;
        LevelsManager.Instance.OnSceneLoaded += LevelsManager_OnSceneLoaded;
    }

    private void OnDisable()
    {
        LevelsManager.Instance.OnSceneLoad -= LevelsManager_OnSceneLoad;
        LevelsManager.Instance.OnSceneLoaded -= LevelsManager_OnSceneLoaded;
    }
    
    // Private Methods ****
    private void LevelsManager_OnSceneLoad(object sender, EventArgs e)
    {
        _animator.SetTrigger("Start");
    }

    private void LevelsManager_OnSceneLoaded(object sender, EventArgs e)
    {
        _animator.SetTrigger("Close");
    }
}
