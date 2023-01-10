using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    void Start()
    {
        LeanTween.moveY(gameObject, transform.position.y + 0.25f, 2).setLoopPingPong();
    }
}
