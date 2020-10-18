using System.Collections.Generic;
using UnityEngine;

public class AbstractAudioStore : ScriptableObject, IAudioStore
{
    public List<AudioClip> _audioList;

    public virtual AudioClip RetrieveAudioClip()
    {
        return null;
    }
}
