using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollText : MonoBehaviour
{
    public float ScrollSpeed = 10; // char/sec
    public GameObject Options;
    public GameObject HighScore;

    SoundEffects _soundEffects;
    List<Tuple<Text, string>> _texts = new List<Tuple<Text, string>>();
    int _textsidx = 0;
    bool _scrolling = true;
    float _lastAdded = 0;

    int _currentOption = 0;
    List<Tuple<Text, string>> _options = new List<Tuple<Text, string>>();

    int _credit = 0;
    int _music = 5;

    List<Tuple<string, int>> _scores = new List<Tuple<string, int>>();

    bool _inHighScore = false;

    IEnumerator _flashTextCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        _soundEffects = GameObject.Find("/SoundEffects").GetComponent<SoundEffects>();

        // options first as we zero the text later
        foreach (var option in Options.GetComponentsInChildren<Text>())
            _options.Add(new Tuple<Text, string>(option, option.text));

        foreach (var text in GetComponentsInChildren<Text>())
        {
            _texts.Add(new Tuple<Text,string>(text, text.text));
            text.text = "";
        }

        var highScoreFile = System.IO.Path.Combine(Application.persistentDataPath, "high-scores.txt");
        if (System.IO.File.Exists(highScoreFile))
        {
            var lines = System.IO.File.ReadAllLines(highScoreFile);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                _scores.Add(new Tuple<string, int>(parts[0], int.Parse(parts[1])));
            }
            _scores.Sort((x, y) => y.Item2.CompareTo(x.Item2));
        }
        else
        {
            // just write some text numbesr
            using (var swriter = new System.IO.StreamWriter(highScoreFile))
            {
                swriter.WriteLine("at0,15");
                swriter.WriteLine("mi,10");
                swriter.WriteLine("um,5");
            }
            _scores.Add(new Tuple<string, int>("at0", 15));
            _scores.Add(new Tuple<string, int>("mi", 10));
            _scores.Add(new Tuple<string, int>("um", 5));
            _scores.Sort((x, y) => y.Item2.CompareTo(x.Item2));
        }
        var scoreString = "";
        foreach (var score in _scores)
            scoreString += $"{score.Item1} - {score.Item2}\n";
        HighScore.GetComponentsInChildren<Text>().Last().text = scoreString;
    }

    // Update is called once per frame
    void Update()
    {
        if (_scrolling)
        {
            if (Input.anyKeyDown)
                ScrollSpeed *= 2;

            if (Time.time - _lastAdded > (1/ScrollSpeed))
            {
                var current = _texts[_textsidx];
                var currentText = current.Item1;
                var desiredText = current.Item2;

                currentText.text = desiredText.Substring(0, currentText.text.Length + 1);
                _lastAdded = Time.time;

                if (currentText.text.Length == desiredText.Length)
                {
                    _textsidx++;
                    if (_textsidx == _texts.Count)
                    {
                        SetFirstOption();
                        _scrolling = false;
                    }
                }
            }
        }
        else
        {
            if (_inHighScore && Input.anyKeyDown)
                NavigateHighScore(false);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                MoveOption(false);
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                MoveOption(true);
            else if (Input.GetKeyDown(KeyCode.Escape))
                GameManager.Instance.Quit();
            else if (Input.anyKeyDown)
                ChangeCurrentOption();
        }
    }

    void NavigateHighScore(bool enter)
    {
        Options.SetActive(!enter);
        HighScore.SetActive(enter);
        _inHighScore = enter;
    }

    void SetFirstOption()
    {
        _currentOption = 0;
        _options[_currentOption].Item1.text = "-- " + _options[_currentOption].Item2 + " --";
    }

    void MoveOption(bool forward)
    {
        _soundEffects.PlayEffect(SoundEffects.EffectType.GoodMenuChoice);

        var nextId = forward ? _currentOption - 1 : _currentOption + 1;
        if (nextId >= 0 && nextId < _options.Count)
        {
            _options[_currentOption].Item1.text = _options[_currentOption].Item2;
            _options[nextId].Item1.text = "-- " + _options[nextId].Item2 + " --";
            _currentOption = nextId;
        }
        if (_flashTextCoroutine != null)
        {
            StopCoroutine(_flashTextCoroutine);
            _flashTextCoroutine = null;
            _texts[_texts.Count - 1].Item1.gameObject.SetActive(true);
        }
    }

    void ChangeCurrentOption()
    {
        switch (_currentOption)
        {
            case 0: // insert coin
                _credit = (_credit + 1) % 4;
                UpdateCredit();
                _soundEffects.PlayEffect(SoundEffects.EffectType.GoodMenuChoice);
                break;
            case 1: // play
                if (_credit == 0)
                {
                    if (_flashTextCoroutine == null)
                    {
                        _soundEffects.PlayEffect(SoundEffects.EffectType.BadMenuChoice);
                        _flashTextCoroutine = FlashText();
                        StartCoroutine(_flashTextCoroutine);
                    }
                }
                else
                {
                    _soundEffects.PlayEffect(SoundEffects.EffectType.MenuStart);
                    GameManager.Score = 0;
                    GameManager.Lives = _credit;
                    GameManager.Instance.LoadNextLevel();
                }
                // else we start
                break;
            case 2: // high score
                NavigateHighScore(true);
                _inHighScore = true;
                _soundEffects.PlayEffect(SoundEffects.EffectType.GoodMenuChoice);
                break;
            case 3: // music
                var old = _music;
                _music = (_music + 1) % 6;
                UpdateMusic(old);
                _soundEffects.PlayEffect(SoundEffects.EffectType.GoodMenuChoice);
                break;
            case 4: // quit
                GameManager.Instance.Quit();
                break;
        }
    }

    IEnumerator FlashText()
    {
        var credit = _texts[_texts.Count - 1];
        var creditgo = credit.Item1.gameObject;

        while (_credit == 0)
        {
            creditgo.SetActive(!creditgo.activeInHierarchy);
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }

    void UpdateCredit()
    {
        var credit = _texts[_texts.Count - 1];
        credit.Item1.text = credit.Item2.Replace("0", _credit.ToString());
    }

    void UpdateMusic(int old)
    {
        var idx = _options.Count - 2;
        var music = _options[idx];
        var newText = music.Item2.Replace(old.ToString(), _music.ToString());
        _options[idx] = new Tuple<Text,string>(music.Item1, newText);
        music.Item1.text = "-- " + newText + " --";

        var audio = GameObject.Find("/Audio").GetComponent<AudioSource>();
        audio.volume = _music / 5f;

        _soundEffects.SetVolume(audio.volume);
    }
}
