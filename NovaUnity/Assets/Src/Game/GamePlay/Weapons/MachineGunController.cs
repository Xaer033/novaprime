using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunController
{
    private GameSystems _gameSystems;
    private Vector3 _aimPosition;

    private MachineGunView _view;
    private MachineGunState _state;

    public MachineGunView view
    {
        get { return _view; }
    }
    

    public MachineGunController(GameSystems gameSystems, MachineGunState state)
    {
        _gameSystems = gameSystems;
        _state = state;
        
        // Move this later
        _view = GameObject.Instantiate<MachineGunView>(Singleton.instance.gameplayResources.machineGunView);
    }
    
    public void OnTimeWarpEnter(float timeScale)
    {
        _view.OnTimeWarpEnter(timeScale);
    }

    public void OnTimeWarpExit()
    {
        _view.OnTimeWarpExit();
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
            return;
        
        Vector3 visualWeaponPos = _view.barrelHook.position;
        Vector3 viewDir = getFireDirection(_view.barrelHook.forward, 0.05f);//(targetPos - visualWeaponPos).normalized;
        viewDir.z = 0;

        Vector3 adjustedPos = visualWeaponPos + (viewDir * 20.0f);
        
        RaycastHit hit;
        bool isHit = Physics.Raycast(visualWeaponPos, (adjustedPos - visualWeaponPos).normalized, out hit, 20.0f, _view.targetLayerMask);
        if (isHit)
        {
            adjustedPos = hit.point;
        }

//        Debug.DrawRay(adjustedPos, Vector3.up, Color.red, 1.0f);
        _view.Fire(adjustedPos, 10.0f);

        ProjectileData projectileData = Singleton.instance.gameplayResources.bulletData;
        _gameSystems.projectileSystem.Spawn(projectileData, visualWeaponPos, viewDir);
        
        _state.fireTimer = _view.fireCooldown;
    }
  
    private Vector3 getFireDirection(Vector3 forward, float range)
    {
        Vector3 perp = Vector3.Cross(forward, _view.barrelHook.right);
        Vector3 offset = perp.normalized * UnityEngine.Random.Range(-range, range); // * Replace this with predicable seeded random function
        Vector3 result = forward + offset;
        return result.normalized;
    }
}
