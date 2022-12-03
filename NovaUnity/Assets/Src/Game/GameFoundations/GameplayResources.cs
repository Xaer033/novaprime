using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(menuName ="Nova/Gameplay Resources")]
public class GameplayResources : ScriptableObject
{
    public NetSimulator   netSimulator;
    public GameplayCamera gameplayCamera;
    public BulletView     bulletView;
    public ParticleSystem bulletImpactFX;
    public ParticleSystem avatarImpactFX;
    public ParticleSystem jumpLandFX;

    public HealthUIView healthUIPrefab;
    public UnitMap unitMap;
    
    
    
    private Dictionary<string, object> _resourceMap = new Dictionary<string, object>();

    private void OnEnable()
    {
        _resourceMap["gameplayCamera"] = gameplayCamera;
        _resourceMap["bulletView"] = bulletView;
        _resourceMap["bulletImpactFX"] = bulletImpactFX;
        _resourceMap["avatarImpactFX"] = avatarImpactFX;
        _resourceMap["jumpLandFX"] = jumpLandFX;
        _resourceMap["healthUIPrefab"] = healthUIPrefab;
        _resourceMap["unitMap"] = unitMap;
    }

    public T GetResource<T>(string resourceType)  where T : Object
    {
        return (T)_resourceMap[resourceType];
    }

}
