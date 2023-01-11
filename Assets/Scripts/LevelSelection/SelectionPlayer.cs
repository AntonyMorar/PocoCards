using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectionPlayer : MonoBehaviour
{
    [SerializeField] private GameObject mainSprite;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    void Start()
    {
        LeanTween.scale(gameObject, Vector3.one, 0.66f).setEase(LeanTweenType.easeOutBack);
        LeanTween.rotateZ(mainSprite, -20, 0.5f).setLoopPingPong();
    }

    public void Hide()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.66f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
