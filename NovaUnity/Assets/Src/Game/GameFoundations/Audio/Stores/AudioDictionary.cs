using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum AudioType
{
    NONE = 0,
    PLAYER_STEP,
    MACHINE_GUN,
    MENU_MOVE,
    MENU_SELECT
}


[CreateAssetMenu(menuName="Nova/Audio/AudioDictionary")]
public class AudioDictionary : ScriptableObject
{
    [SerializeField]
    private List<AudioPair> _audioList = new List<AudioPair>();
    private Dictionary<AudioType, IAudioStore> _audioDictionary = new Dictionary<AudioType, IAudioStore>();
    
    public void OnEnable()
    {
        if(_audioList == null)
        {
            Debug.LogError("Audio List is null!");
            return;
        }
        
        for(int i = 0; i < _audioList.Count; ++i)
        {
            AudioPair pair = _audioList[i];
            if(pair != null)
            {
                _audioDictionary[pair.type] = pair.audioStore;
            }
        }
    }
    
    public IAudioStore GetAudio(AudioType type)
    {
        IAudioStore store;
        if(!_audioDictionary.TryGetValue(type, out store))
        {
            Debug.LogError("No Audio Store for audio type: " + type.ToString());
        }

        return store;
    }
    
    [Serializable]
    public class AudioPair
    {
        public AudioType type;
        public AbstractAudioStore audioStore;
    }
}
