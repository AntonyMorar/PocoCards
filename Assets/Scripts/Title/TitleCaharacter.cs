using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCaharacter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float randomDelay = Random.Range(0.1f, 2f);
        LeanTween.scaleY(gameObject, 0.95f, Random.Range(2.1f,2.9f)).setDelay(randomDelay).setLoopPingPong();
    }
}
