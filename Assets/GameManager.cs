using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }

    }
    // singleton

    public static int Score = 0;
    public static int Lives = 1;

    int _level = 0;
    Image _fade;

    void Start()
    {
        _level = SceneManager.GetActiveScene().buildIndex;
        _fade = GameObject.Find("/UI/Fade").GetComponent<Image>();
        StartCoroutine(FadeWhenLoad());
    }

    public void WriteNewScore()
    {
        // should call this at the very end of the game
        // or when real dead
        if (Score == 0)
            return;

        var highScoreFile = System.IO.Path.Combine(Application.persistentDataPath, "high-scores.txt");
        using (var sw = new System.IO.StreamWriter(highScoreFile, true))
            sw.WriteLine($"at0mium,{Score}");
    }

    public void LoadNextLevel()
    {
        _fade = GameObject.Find("/UI/Fade").GetComponent<Image>();

        var next = (_level + 1) % (SceneManager.sceneCountInBuildSettings+1);
        _level = next;

        StartCoroutine(FadeAndLoad());
    }

    public void LoadMainMenu()
    {
        _level = -1;
        LoadNextLevel();
    }

    public void ReloadLevel()
    {
        _level--;
        LoadNextLevel();
    }

    public void Quit()
    {
#if (UNITY_EDITOR)
    UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE) 
    Application.Quit();
#elif (UNITY_WEBGL)
    Application.OpenURL("about:blank");
#endif
    }

    IEnumerator FadeAndLoad()
    {
        Time.timeScale = 0;

        while (_fade.color.a < 1) { 
            _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, _fade.color.a + 0.025f);
            yield return new WaitForSecondsRealtime(0.01f);
        }

        SceneManager.LoadScene(_level);
    }

    IEnumerator FadeWhenLoad()
    {
        Time.timeScale = 0;

        _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, 1);
        yield return new WaitForSecondsRealtime(0.01f);

        while (_fade.color.a > 0)
        {
            _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, _fade.color.a - 0.025f);
            yield return new WaitForSecondsRealtime(0.01f);
        }

        Time.timeScale = 1;
    }
}
