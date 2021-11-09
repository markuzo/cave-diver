using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Player Player;

    SoundEffects _soundEffects;
    int _currentSelection = 0;
    List<Tuple<Text, string>> _options = new List<Tuple<Text, string>>();

    void Start()
    {
        _soundEffects = GameObject.Find("/SoundEffects").GetComponent<SoundEffects>();

        foreach (var text in GetComponentsInChildren<Text>())
            _options.Add(new Tuple<Text, string>(text, text.text));

        _options[_currentSelection].Item1.text = "-- " + _options[_currentSelection].Item2 + " --";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.S)  || Input.GetKeyDown(KeyCode.DownArrow))
            MoveMenu();
        else if (Input.GetKeyDown(KeyCode.Escape))
            Player.PauseTheGame(false);  
        else if (Input.anyKeyDown)
            SelectOption();
    }

    void MoveMenu()
    {
        var next = _currentSelection == 0 ? 1 : 0;
        _options[_currentSelection].Item1.text = _options[_currentSelection].Item2;
        _options[next].Item1.text = "-- " + _options[next].Item2 + " --";
        _currentSelection = next;

        _soundEffects.PlayEffect(SoundEffects.EffectType.GoodMenuChoice);
    }

    void SelectOption()
    {
        _soundEffects.PlayEffect(SoundEffects.EffectType.GoodMenuChoice);

        if (_currentSelection == 0) // resume
        {
            Player.PauseTheGame(false);
        }
        else // quit
        {
            GameManager.Instance.LoadMainMenu();
        }
    }
}
