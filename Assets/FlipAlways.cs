using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipAlways : MonoBehaviour
{
    SpriteRenderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Flip());
    }

    IEnumerator Flip()
    {
        while (true) {
            _renderer.flipX = !_renderer.flipX;
            yield return new WaitForSeconds(1f);
        }
    }

}
