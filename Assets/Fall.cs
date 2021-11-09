using UnityEngine;

public class Fall : MonoBehaviour
{
    public GameObject FallingObject;

    bool _canFall = false;
    bool _falling = false;

    Player _player;

    public void EnableFalling() => _canFall = true;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_canFall || _falling)
            return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("projectile"))
        {
            _falling = true;
            FallingObject.GetComponent<Rigidbody2D>().simulated = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == FallingObject)
        {
            FallingObject.GetComponent<Rigidbody2D>().simulated = false;
            FallingObject.SetActive(false);

            // dodged, now add points
            _player.UpdateScore(2);
        }
    }
}
