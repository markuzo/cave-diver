using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameOverMenu : MonoBehaviour
{
    public float ScrollSpeed = 3;
    public bool Reload = false;

    bool _scrolling = true;
    float _lastAdded = 0;

    List<Tuple<Text, string>> _texts = new List<Tuple<Text, string>>();
    int _textsidx = 0;

    void Start()
    {
        foreach (var text in GetComponentsInChildren<Text>())
        {
            _texts.Add(new Tuple<Text, string>(text, text.text));
            text.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_scrolling)
        {
            if (Input.anyKeyDown)
                ScrollSpeed *= 2;

            if (Time.realtimeSinceStartup - _lastAdded > (1/ScrollSpeed))
            {
                var current = _texts[_textsidx];
                var currentText = current.Item1;
                var desiredText = current.Item2;

                currentText.text = desiredText.Substring(0, currentText.text.Length + 1);
                _lastAdded = Time.realtimeSinceStartup;

                if (currentText.text.Length == desiredText.Length)
                {
                    _textsidx++;
                    if (_textsidx == _texts.Count)
                    {
                        _scrolling = false;
                    }
                }
            }
        }   
        else if (Input.anyKeyDown)
        {
            if (Reload) GameManager.Instance.ReloadLevel();
            else 
            {
                GameManager.Instance.WriteNewScore();
                GameManager.Instance.LoadMainMenu(); 
            }
        }
    }
}
