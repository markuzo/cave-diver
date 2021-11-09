using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // assets - 
    /*
     *  Pixel font - https://assetstore.unity.com/packages/2d/fonts/free-pixel-font-thaleah-140059
     */
    public float Speed = 10f;
    public Text TimeLeftText;
    public Text LivesLeftText;
    public Text ScoreText;
    public GameObject AmmoText;
    public float TimeLeft = 30;

    public bool Invincible = false;

    public GameObject Pause;
    public GameObject GameOver;
    public GameObject LivesLeft;

    public SoundEffects SoundEffects;

    public GameObject ProjectilePrefab;

    public Sprite Sprite1;
    public Sprite Sprite2;

    Vector2 _move = Vector2.zero;
    float _lastMove = 0;
    Rigidbody2D _rb;
    SpriteRenderer _renderer;

    int _harpoonAmmo = 0;
    GameObject _harpoon;
    SpriteRenderer _harpoonRenderer;
    bool _hasHarpoon = false;

    bool _gotPearl = false;
    int _displayTime;
    int _score;

    bool _paused = false;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _displayTime = (int)TimeLeft;
        _score = GameManager.Score;
        _harpoon = transform.GetChild(0).gameObject;
        _harpoonRenderer = _harpoon.GetComponent<SpriteRenderer>();
        _harpoon.SetActive(false);
        TimeLeftText.text = _displayTime.ToString();
        LivesLeftText.text = "" + GameManager.Lives + "UP";

        UpdateScore(0);
        UpdateHarpoonText();
    }

    // Update is called once per frame
    void Update()
    {
        if (_paused) return;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            _move += Vector2.down;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q))
            _move += Vector2.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            _move += Vector2.right;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z))
            _move += Vector2.up;

        if (Input.GetKeyDown(KeyCode.RightBracket)) Invincible = !Invincible;
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            var hiddenstuff = GameObject.Find("/LevelHidden");
            if (hiddenstuff != null)
                hiddenstuff.SetActive(false);
        }

        var shoot = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
        if (shoot && _hasHarpoon && _harpoonAmmo > 0)
        {
            LaunchProjectile();

            _harpoonAmmo--;
            Debug.Log("FIRE!");

            if (_harpoonAmmo == 0)
                _harpoon.SetActive(false);

            UpdateHarpoonText();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseTheGame(true);
            return;
        }

        TimeLeft -= Time.deltaTime;
        if ((int)TimeLeft != _displayTime)
        {
            _displayTime = (int)TimeLeft;
            TimeLeftText.text = _displayTime.ToString();
        }
        if (TimeLeft <= 0) Death();
    }

    public void PauseTheGame(bool on)
    {
        Time.timeScale = on ? 0 : 1;
        Pause.SetActive(on);
        _paused = on;
    }

    void FixedUpdate()
    {
        if (_move != Vector2.zero)
        {
            if (_move.x != 0)
            {
                _renderer.flipX = _move.x > 0;
                _harpoonRenderer.flipX = _renderer.flipX;
                var localPos = _harpoon.transform.localPosition;
                float x = 0;
                if (_renderer.flipX)
                    x = Mathf.Abs(localPos.x);
                else
                    x = -Mathf.Abs(localPos.x);
                _harpoon.transform.localPosition = new Vector3(x, localPos.y, localPos.z);
            }
            _rb.AddForce(_move.normalized * Time.deltaTime * Speed);
            _move = Vector2.zero;

            if (Time.time - _lastMove > 1)
            {
                _lastMove = Time.time;
                _renderer.sprite = _renderer.sprite == Sprite1 ? Sprite2 : Sprite1;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("pearl"))
        {
            UpdateScore(_displayTime);
            collision.gameObject.SetActive(false);
            _gotPearl = true;
            Debug.Log("Got pearl!");
            var objs = GameObject.FindGameObjectsWithTag("fall");
            foreach (var obj in objs) obj.GetComponent<Fall>().EnableFalling();
            SoundEffects.PlayEffect(SoundEffects.EffectType.Score);
        }
        else if (collision.gameObject.CompareTag("surface") && _gotPearl)
        {
            collision.gameObject.SetActive(false);
            Win();
        }
        else if (collision.gameObject.CompareTag("hide"))
        {
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("death"))
        {
            if (!Invincible)
                Death();
        }
        else if (collision.gameObject.CompareTag("scuba"))
        {
            TimeLeft += 30f;
            collision.gameObject.SetActive(false);
            SoundEffects.PlayEffect(SoundEffects.EffectType.Score);
        }
        else if (collision.gameObject.CompareTag("harpoon"))
        {
            _harpoonAmmo += 2;
            _hasHarpoon = true;
            _harpoon.SetActive(true);
            collision.gameObject.SetActive(false);
            UpdateHarpoonText();
            SoundEffects.PlayEffect(SoundEffects.EffectType.Score);
        }
        else if (collision.gameObject.CompareTag("ammo"))
        {
            _harpoonAmmo += 3;
            if (_hasHarpoon) _harpoon.SetActive(true);
            collision.gameObject.SetActive(false);
            UpdateHarpoonText();
            SoundEffects.PlayEffect(SoundEffects.EffectType.Score);
        }
    }

    public void UpdateScore(int amount)
    {
        _score += amount;
        ScoreText.text = $"{_score:0000}";
    }

    void UpdateHarpoonText()
    {
        for (var i = 0; i < AmmoText.transform.childCount; i++)
        {
            var child = AmmoText.transform.GetChild(i);
            child.gameObject.SetActive(_harpoonAmmo - i > 0);
        }
    }

    void LaunchProjectile()
    {
        var obj = Instantiate(ProjectilePrefab);
        obj.transform.position = _harpoon.transform.position;
        obj.GetComponent<Projectile>().Direction = _harpoonRenderer.flipX ? Vector3.right : Vector3.left;
    } 

    void Win()
    {
        UpdateScore(100);
        Time.timeScale = 0;
        Debug.Log("WIN!");
        GameManager.Score += _score;
        GameManager.Instance.LoadNextLevel();
    }

    void Death() {
        // this means we're already dying
        if (_paused) return;

        Debug.Log("Dead.");
        Time.timeScale = 0;
        _paused = true;

        if (GameManager.Lives == 0)
        {
            SoundEffects.PlayEffect(SoundEffects.EffectType.Death, true);
            GameOver.SetActive(true);
        }
        else
        {
            GameManager.Lives--;

            var txt = GameManager.Lives == 0 ? "LIFE" : "LIVES";
            LivesLeft.SetActive(true);
            LivesLeft.GetComponentInChildren<Text>().text = $"{GameManager.Lives + 1} {txt} LEFT...";
            SoundEffects.PlayEffect(SoundEffects.EffectType.Hit);
        }
    }
}

