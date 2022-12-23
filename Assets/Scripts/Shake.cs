using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Camera))]
public class Shake : MonoBehaviour
{
    // Serialized ****
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private float multiplier = 0.1f;
    [SerializeField] private AnimationCurve curve;
    // Private ****
    private Camera _mainCamera;
    
    // MonoBehavior Callbacks
    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        Card.OnMakeHit += Card_OnMakeHit;
    }

    private void OnDisable()
    {
        Card.OnMakeHit -= Card_OnMakeHit;
    }
    

    // Private Methods
    private void Card_OnMakeHit(object sender, EventArgs e)
    {
        StartCoroutine(ShakingCorrutine());
    }

    private IEnumerator ShakingCorrutine()
    {
        yield return new WaitForSeconds(0.25f);
        
        Vector3 startPosition = _mainCamera.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            Vector3 randomPos = Random.insideUnitSphere;
            randomPos.z = 0;
            _mainCamera.transform.position = startPosition + randomPos * (strength * multiplier);
            
            yield return null;
        }

        _mainCamera.transform.position = startPosition;
    }

}
