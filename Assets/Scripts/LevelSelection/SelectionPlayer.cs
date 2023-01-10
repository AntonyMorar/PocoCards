using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectionPlayer : MonoBehaviour
{
    [SerializeField] private GameObject mainSprite;
    void Start()
    {
        LeanTween.rotateZ(mainSprite, -20, 0.5f).setLoopPingPong();
    }
}
