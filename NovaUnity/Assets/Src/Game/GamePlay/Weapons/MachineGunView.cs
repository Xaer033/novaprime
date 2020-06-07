using System.Data;
using UnityEngine;
using UnityEngine.Animations;

public class MachineGunView : MonoBehaviour, IWeaponView
{
    private const int kFXPoolSize = 50;
    
    // (This should all be in a weapon class)
    public GameObject _bulletFXPrefab;

    [SerializeField] 
    private Transform _leftHandHook;
    
    [SerializeField] 
    private Transform _rightHandHook;
    
    [SerializeField]
    private Transform _barrelHook;

    [SerializeField]
    private ParentConstraint _parentConstraint;

    [SerializeField]
    private Animator _animator;
    
    private int attachParentIndex;
    
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
    
    public void Attach(Transform bodyParentHook, ParentConstraint leftHandConstraint, ParentConstraint rightHandConstraint)
    {
        if(_parentConstraint != null && bodyParentHook != null)
        {
            transform.SetParent(bodyParentHook.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            
            //  _parentConstraint.transform.position = bodyParentHook.position;
            //  _parentConstraint.transform.rotation = bodyParentHook.rotation;
            //
            // ConstraintSource source = new ConstraintSource();
            // source.weight = 1.0f;
            // source.sourceTransform = bodyParentHook;
            // attachParentIndex = _parentConstraint.AddSource(source);
            // _parentConstraint.weight = 1.0f;
            // _parentConstraint.constraintActive = true;
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
//        float speed = bulletSpeed;
//        Vector3 startPos = barrelHook.position;
//
//        TrailRenderer fx = getNextFX();
//        Tween t = _fxTweens[_fxIndex];
//
//        //fx.Clear();
//
//        fx.transform.position = startPos;
//        
//        t = fx.transform.DOMove(target, speed);
//        t.SetEase(Ease.Linear);
//        t.SetSpeedBased(true);
//
//        t.OnStart(() =>
//        {
//            // fx.time = time;
//            fx.transform.position = startPos;
//            fx.transform.LookAt(target, Vector3.up);
//            fx.gameObject.SetActive(true);
//            //fx.emitting = (true);
//            fx.Clear();
//        });
//
//        t.OnComplete(()=>
//        {
//            //fx.Clear();
//            //fx.emitting = (false);
//            fx.gameObject.SetActive(false);
//            t = null;
//        });
    }
}
