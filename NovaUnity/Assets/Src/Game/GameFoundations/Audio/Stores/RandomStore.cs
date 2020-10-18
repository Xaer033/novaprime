using UnityEngine;

[CreateAssetMenu(menuName ="Nova/Audio/RandomStore")]
public class RandomStore : AbstractAudioStore
{
    private int _currentIndex;

    public void OnEnable()
    {
        _currentIndex = 0;
    }

    // Start is called before the first frame update
    override public AudioClip RetrieveAudioClip()
    {
        if(_audioList == null || _audioList.Count <= 0)
        {
            return null;
        }

        _currentIndex = Random.Range(0, _audioList.Count);
        return _audioList[_currentIndex];
    }
}
