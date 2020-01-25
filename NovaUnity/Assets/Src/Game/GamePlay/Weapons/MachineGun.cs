using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun
{
    
    private Vector3 _aimPosition;
    private float _fireTimer;

    private MachineGunView _view;

    public MachineGunView view
    {
        get { return _view; }
    }
    

    public MachineGun()
    {
        // Move this later
        _view = GameObject.Instantiate<MachineGunView>(Singleton.instance.gameplayResources.machineGunView);
    }

    public void FixedStep(float deltaTime)
    {
        if (_fireTimer > 0)
        {
            _fireTimer -= deltaTime;
        }
    }
    
    public void Fire(Vector3 targetPos)
    {
        if(_fireTimer > 0)
            return;
        
        Vector3 visualWeaponPos = _view.barrelHook.position;
        Vector3 viewDir = getFireDirection(_view.barrelHook.forward, 0.08f);//(targetPos - visualWeaponPos).normalized;
        viewDir.z = 0;

        Vector3 adjustedPos = visualWeaponPos + (viewDir * 20.0f);
        RaycastHit hit;
        bool isHit = Physics.Raycast(visualWeaponPos, (adjustedPos - visualWeaponPos).normalized, out hit, 20.0f, _view.targetLayerMask);
        if (isHit)
        {
            adjustedPos = hit.point;
        }
        
        
        Debug.DrawRay(adjustedPos, Vector3.up, Color.red);
        _view.Fire(adjustedPos, 10.0f);
        _fireTimer = _view.fireCooldown;
    }
  
    private Vector3 getFireDirection(Vector3 forward, float range)
    {
        Vector3 result;

        Vector3 perp = Vector3.Cross(forward, _view.barrelHook.right);
        Vector3 offset = perp.normalized * UnityEngine.Random.Range(-range, range);
        result = forward + offset;
        return result.normalized;
    }
}
