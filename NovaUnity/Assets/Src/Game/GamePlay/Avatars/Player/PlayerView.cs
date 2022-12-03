using UnityEngine;

public class PlayerView : AvatarView
{
    // [BoxGroup("Hooks")]
    public Transform deadRoot;
    
    // [BoxGroup("Hooks")]
    public Transform _leftFootHook;
    // [BoxGroup("Hooks")]
    public Transform _rightFootHook;
    
    private ParticleSystem _leftFootPuffFx;
    private ParticleSystem _rightFootPuffFx;
    
    // public PhotonView netView;
    
    private void Awake()
    {
        ParticleSystem jumpPrefab = Singleton.instance.gameplayResources.jumpLandFX;
        if(jumpPrefab != null && _viewRoot != null)
        {
            if(_leftFootHook != null)
            {
                _leftFootPuffFx = Instantiate<ParticleSystem>(jumpPrefab, _viewRoot);
                _leftFootPuffFx.transform.localPosition = Vector3.zero;
                _leftFootPuffFx.transform.localRotation = Quaternion.identity;
            }

            if(_rightFootHook != null)
            {
                _rightFootPuffFx = Instantiate<ParticleSystem>(jumpPrefab, _viewRoot);
                _rightFootPuffFx.transform.localPosition = Vector3.zero;
                _rightFootPuffFx.transform.localRotation = Quaternion.identity;
            }
        }
    }
    
    public void PlayFootPuffFx()
    {
        if(!_leftFootPuffFx.isPlaying)
        {
            _leftFootPuffFx.Clear();
            _leftFootPuffFx.Play();
        }
        else
        {
            _rightFootPuffFx.Clear();
            _rightFootPuffFx.Play();
        }
    }
}
