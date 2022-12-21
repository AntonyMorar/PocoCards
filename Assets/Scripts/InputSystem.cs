using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    // Public *****
    public static InputSystem Instance { get; private set; }

    public event EventHandler OnEscapeDown;
    
    // MonoBehavior Callbacks
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeDown?.Invoke(this,EventArgs.Empty);
        }
    }

}
