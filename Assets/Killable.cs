using UnityEngine;

public class Killable : MonoBehaviour
{
    SpriteRenderer _renderer;
    Collider2D _collider;
    MoveBetween _moveBetween;

    private void Start()
    {
        _renderer = gameObject.GetComponent<SpriteRenderer>();
        _collider = gameObject.GetComponent<Collider2D>();
        _moveBetween = gameObject.GetComponent<MoveBetween>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("projectile"))
        {
            transform.rotation = Quaternion.identity;
            _renderer.flipY = true;
            _renderer.color = new Color(0.5f,0,0);
            _collider.enabled = false;
            if (_moveBetween != null) _moveBetween.enabled = false;
            GameObject.Find("/Player").GetComponent<Player>().UpdateScore(10);
        }
    }
}
