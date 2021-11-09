using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetween : MonoBehaviour
{
    public Transform From;
    public Transform To;

    public float Speed = 1f;

    SpriteRenderer _renderer;
    Transform _target;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _target = To;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target.position, Speed * Time.deltaTime);

        if (At(_target)) { 
            _target = _target == From ? To : From;
            _renderer.flipX = _target == From;
        }
        
    }

    bool At(Transform target) =>
        Vector3.Distance(transform.position, target.position) < 0.01f;
}
