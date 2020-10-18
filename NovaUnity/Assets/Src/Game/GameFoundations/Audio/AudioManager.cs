using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour, IPostInit
{

    private AudioMixer _mixer;
    private AudioDictionary _dictionary;
    private AudioSource _mainSource;
    
    public AudioManager(AudioMixer mixer, AudioSource mainSource, AudioDictionary dictionary)
    {
        _mixer = mixer;
        _mainSource = mainSource;
        _dictionary = dictionary;
        
    }

    public void PostInit()
    {
        
    }

    public bool PlayOneShot(AudioType type, float volumeScale = 1.0f)
    {
        bool result = false;
        
        IAudioStore store = _dictionary.GetAudio(type);
        if(store != null)
        {
            AudioClip clip = store.RetrieveAudioClip();
            if(clip != null)
            {
                _mainSource.PlayOneShot(clip, volumeScale);
                result = true;
            }
        }

        return result;
    }
}
