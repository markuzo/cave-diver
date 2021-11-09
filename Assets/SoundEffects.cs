using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioClip[] AudioClips;

    public enum EffectType
    {
        Score, 
        Death,
        Hit,
        MenuStart,
        GoodMenuChoice,
        BadMenuChoice,
    }

    AudioSource _mainAudio;
    AudioSource _audio;
    Dictionary<EffectType, AudioClip> _clips = new Dictionary<EffectType, AudioClip>();

    // Start is called before the first frame update
    void Start()
    {
        _mainAudio = GameObject.Find("/Audio").GetComponent<AudioSource>();
        _audio = GetComponent<AudioSource>();
        _audio.volume = _mainAudio.volume;

        foreach (var clip in AudioClips)
        {
            switch (clip.name)
            {
                case "score":
                    _clips.Add(EffectType.Score, clip);
                    break;
                case "death":
                    _clips.Add(EffectType.Death, clip);
                    break;
                case "hit":
                    _clips.Add(EffectType.Hit, clip);
                    break;
                case "menustart":
                    _clips.Add(EffectType.MenuStart, clip);
                    break;
                case "goodmenuchoice":
                    _clips.Add(EffectType.GoodMenuChoice, clip);
                    break;
                case "badmenuchoice":
                    _clips.Add(EffectType.BadMenuChoice, clip);
                    break;
            }
        }
    }

    public void SetVolume(float volume)
    {
        _audio.volume = volume;
    }

    public void PlayEffect(EffectType type, bool muteOthers = false)
    {
        AudioClip clip;
        if (_clips.TryGetValue(type, out clip))
        {
            _audio.clip = clip;

            if (muteOthers)
                StartCoroutine(PlayEffectUnique(_audio, _mainAudio));
            else 
                _audio.Play();
        }
    }

    IEnumerator PlayEffectUnique(AudioSource main, AudioSource other)
    {
        other.Pause();
        main.Play();
        while (main.isPlaying)
            yield return new WaitForSecondsRealtime(0.5f);
        yield return new WaitForSecondsRealtime(0.5f);
        other.Play();
    }
}
