using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed = 2;
    public Vector3 Direction = Vector3.left;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;

        var update = Direction * Speed * Time.deltaTime;
        transform.position = transform.position + update;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.gameObject;
        if (other.CompareTag("level") ||
            (other.CompareTag("death") && other.GetComponent<Killable>() != null))
        {
            Destroy(gameObject);
        }
    }
}
