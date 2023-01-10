using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingRadiusController : MonoBehaviour
{
    // Serialized ****
    [SerializeField] private float speed = 1;
    // Private ****
    private Material _material;
    private float _time;
    private float _radius;
    private float _randomValue;
    void Start()
    {
        _material = this.gameObject.GetComponent<MeshRenderer>().material;
        _radius = 0.2f;
        _material.SetFloat("_RadiusA", _radius);

        float random = Random.Range(0.1f, 0.04f);
        _material.SetFloat("_RadiusB", random);
    }

    void Update()
    {
        _time += Time.deltaTime * speed;
        if (_time < 1)
        {
            _radius = Mathf.Lerp(0.2f, 1, _time);
            _material.SetFloat("_RadiusA", _radius);
        }

        if (_radius >= 0.65f)
        {
            Destroy(gameObject);
        }
    }

}
