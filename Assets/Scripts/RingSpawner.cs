using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSpawner : MonoBehaviour
{
    // Serialized ****
    [SerializeField] private GameObject ringPrefab;
    [SerializeField] private float timeS;
    void Start()
    {
        InvokeRepeating("InstantiateRings", 1, timeS);
    }

    private void InstantiateRings()
    {
        GameObject ring = Instantiate(ringPrefab, transform);
    }

}
