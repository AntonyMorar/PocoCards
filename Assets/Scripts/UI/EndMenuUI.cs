using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMenuUI : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.75f).setEase(LeanTweenType.easeOutBounce);
    }
}
