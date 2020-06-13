using UnityEngine;
using UnityEngine.Animations;

public class MachineGunController : IWeaponController
{
    private GameSystems _gameSystems;
    private Vector3 _aimPosition;

    private IWeaponView _view;
    private MachineGunState _state;
    private MachineGunData _machineGunData;
    private ProjectileSystem _projectileSystem;
    

    public IWeaponView view
    {
        get { return _view; }
    }
    

    public MachineGunController(ProjectileSystem projectileSystem, 
                                MachineGunState state, 
                                MachineGunData data)
    {
        _state = state;
        _machineGunData = data;
        _projectileSystem = projectileSystem;
        
        // Move this later
        _view = GameObject.Instantiate<MachineGunView>(_machineGunData.view);
    }
    
    public void OnTimeWarpEnter(float timeScale)
    {
        _view.OnTimeWarpEnter(timeScale);
    }

    public void OnTimeWarpExit()
    {
        _view.OnTimeWarpExit();
    }

    public void Attach(Transform bodyParentHook, 
                       ParentConstraint leftHandConstraint, 
                       ParentConstraint rightHandConstraint)
    {
        view.Attach(bodyParentHook, leftHandConstraint, rightHandConstraint);
    }
    
    public void FixedStep(float deltaTime)
    {
        if (_state.fireTimer > 0)
        {
            _state.fireTimer -= deltaTime;
        }
    }
    
    public void Fire(Vector3 targetPos)
    {
        if(_state.fireTimer > 0)
        {
            return;
        }
        
        Vector3 visualWeaponPos = _view.barrelHook.position;
        Vector3 viewDir = getFireDirection(_view.barrelHook.forward, 0.05f);//(targetPos - visualWeaponPos).normalized;
        viewDir.z = 0;

        Vector3 adjustedPos = visualWeaponPos + (viewDir * 20.0f);
        
        RaycastHit2D[] raycastHits = new RaycastHit2D[1];
        int hitCount = Physics2D.RaycastNonAlloc(visualWeaponPos, 
                                                 (adjustedPos - visualWeaponPos).normalized, 
                                                 raycastHits, 
                                                 20.0f, 
                                                 _machineGunData.targetLayerMask);
        
        if (hitCount > 0)
        {
            adjustedPos = raycastHits[0].point;
        }

//        Debug.DrawRay(adjustedPos, Vector3.up, Color.red, 1.0f);
        _view.Fire(adjustedPos, 10.0f);

        _projectileSystem.Spawn(_machineGunData.projectileData, visualWeaponPos, viewDir);
        
        _state.fireTimer = _machineGunData.fireCooldownSeconds;
    }
  
    private Vector3 getFireDirection(Vector3 forward, float range)
    {
        Vector3 perp = Vector3.Cross(forward, _view.barrelHook.right);
        Vector3 offset = perp.normalized * Random.Range(-range, range); // * Replace this with predicable seeded random function
        Vector3 result = forward + offset;
        return result.normalized;
    }
}
