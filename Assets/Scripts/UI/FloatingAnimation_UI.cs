using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FloatingAnimation_UI : MonoBehaviour
{
    private void OnEnable()
    {
        RectTransform rt = gameObject.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        rt.localScale = Vector3.zero;
        
        rt.localPosition += Vector3.up * 10;
        LeanTween.scale(rt, Vector3.one, 0.75f).setEase(LeanTweenType.easeOutBounce).setOnComplete(() =>
        {
            LeanTween.alphaCanvas(canvasGroup, 0f, 0.5f).setDestroyOnComplete(true);
        });
    }
}
