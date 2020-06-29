using System.Data;
using UnityEngine;
using UnityEngine.Animations;

public class MachineGunView : MonoBehaviour, IWeaponView
{
    [SerializeField]
    public ParticleSystem _muzzleFlashFX;
    
    [SerializeField] 
    public Transform _leftHandHook;
    
    [SerializeField] 
    public Transform _rightHandHook;
    
    [SerializeField]
    public Transform _barrelHook;

    [SerializeField]
    public ParentConstraint _parentConstraint;

    [SerializeField]
    public Animator _animator;
    
    private int attachParentIndex;
    private ParticleSystem[] _muzzleFireList;
    private int _muzzleIndex;
    
    public MachineGunController controller { get; set; }
    
    public void OnTimeWarpEnter(float timeScale)
    {
        if (controller != null)
        {
            controller.OnTimeWarpEnter(timeScale);
        }
    }

    public void OnTimeWarpExit()
    {
        if (controller != null)
        {
            controller.OnTimeWarpExit();
        }
    }
    
    public Transform barrelHook
    {
        get { return _barrelHook; }
    }

    public Transform leftHandHook
    {
        get { return _leftHandHook; }
    }

    public Transform rightHandHook
    {
        get { return _rightHandHook; }
    }

    private void Awake()
    {
        _muzzleFireList = new ParticleSystem[3];
        for(int i = 0; i < _muzzleFireList.Length; ++i)
        {
            _muzzleFireList[i] = GameObject.Instantiate<ParticleSystem>(_muzzleFlashFX, 
                                                                        _barrelHook.transform.position, 
                                                                        _barrelHook.transform.rotation, 
                                                                        _barrelHook.transform);
            
        }
        _muzzleIndex = 0;
    }
    
    public void Attach(Transform bodyParentHook, ParentConstraint leftHandConstraint, ParentConstraint rightHandConstraint)
    {
        if(_parentConstraint != null && bodyParentHook != null)
        {
            transform.SetParent(bodyParentHook.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        
        if(leftHandConstraint != null )
        {
            if(leftHandHook != null)
            {
                leftHandConstraint.transform.position = leftHandHook.position;
                leftHandConstraint.transform.rotation = leftHandHook.rotation;
                
                ConstraintSource leftHandSource = new ConstraintSource();
                leftHandSource.weight = 1.0f;
                leftHandSource.sourceTransform = leftHandHook;
                leftHandConstraint.AddSource(leftHandSource);
                leftHandConstraint.weight = 1.0f;
            }
            else
            {
                leftHandConstraint.weight = 0;
            }
        }

        if(rightHandConstraint != null)
        {
            if(rightHandHook != null)
            {
                rightHandConstraint.transform.position = rightHandHook.position;
                rightHandConstraint.transform.rotation = rightHandHook.rotation;
                    
                ConstraintSource rightHandSource = new ConstraintSource();
                rightHandSource.weight = 1.0f;
                rightHandSource.sourceTransform = rightHandHook;
                rightHandConstraint.AddSource(rightHandSource);
                rightHandConstraint.weight = 1.0f;
            }
            else
            {
                rightHandConstraint.weight = 0;
            }
        } 
    }
    
    public void Fire(Vector3 target, float speedx)
    {
        if(_animator != null)
        {
            int fireIndex = Random.Range(0, 2);
            _animator.SetInteger("primaryFireIndex", fireIndex);
            _animator.SetTrigger("primaryFire");
        }

        ParticleSystem muzzleFX = _retrieveMuzzleFX();
        if(muzzleFX != null)
        {
            muzzleFX.Clear();
            muzzleFX.Play();
        }
    }

    private ParticleSystem _retrieveMuzzleFX()
    {
        return _muzzleFireList[_muzzleIndex++ % _muzzleFireList.Length];
    }
}
