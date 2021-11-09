using System.Collections;
using UnityEngine;

public class Bobble : MonoBehaviour
{
    public float Amount = 0.1f;
    bool _up = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Bob());
    }

    // Update is called once per frame
    IEnumerator Bob()
    {
        while (true)
        {
            var y = _up ? transform.position.y + Amount : transform.position.y - Amount;
            _up = !_up;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
            yield return new WaitForSeconds(1f);
        }
    }
}
