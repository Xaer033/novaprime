using System;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileSystem : NotificationDispatcher, IGameSystem
{
    private GameSystems       _gameSystems;
    private GameState         _gameState;
    private GameplayResources _gameplayResources;
    private GameObject        _bulletParent;

    private NetworkManager _networkManager;
    
    private Dictionary<Guid, BulletView> _netPrefabMap;
    private List<BulletView>             _projectileViewPool;
    private List<ParticleSystem>         _projectileImpactViewList;
    private List<ParticleSystem>         _bloodImpactViewList;
    
    private RaycastHit2D[] _raycastHitList;
    private int _poolSize;
    
    
    public int priority { get; set; }
    
    public ProjectileSystem(GameplayResources gameplayResources, int poolSize)
    {
        _gameplayResources        = gameplayResources;
        _poolSize                 = poolSize;
        _projectileViewPool       = new List<BulletView>(poolSize);
        _projectileImpactViewList = new List<ParticleSystem>(poolSize);
        _bloodImpactViewList      = new List<ParticleSystem>(40);
        _netPrefabMap             = new Dictionary<Guid, BulletView>();
        _raycastHitList           = new RaycastHit2D[3];
        
        _networkManager           = Singleton.instance.networkManager;
     
        BulletView projectileTemplate = _gameplayResources.bulletView;
        Guid bulletGuid = Guid.NewGuid();// projectileTemplate.netIdentity.assetId;
        _netPrefabMap[bulletGuid] = projectileTemplate;
    }
    
    public void Start(bool hasAuthority, GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState   = gameState;

        _gameSystems.onStep      += onStep;
        _gameSystems.onFixedStep += onFixedStep;

        // ClientScene.RegisterPrefab( _gameplayResources.bulletView.gameObject, 
        //                             onClientProjectileSpawnHandler, 
        //                             onClientProjectileUnspawnHandler);
        
        _bulletParent = new GameObject("BulletParent");
        BulletView     projectileTemplate   = _gameplayResources.bulletView;
        ParticleSystem bulletImpactTemplate = _gameplayResources.bulletImpactFX;
        ParticleSystem bloodImpactTemplate  = _gameplayResources.avatarImpactFX;
        
        for (int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = new ProjectileState();
            _gameState.projectileStateList.Add(state);

            if (hasAuthority)
            {
                BulletView view = GameObject.Instantiate<BulletView>(projectileTemplate, _bulletParent.transform);
                view.Recycle();
                _projectileViewPool.Add(view);
                
                // NetworkServer.Spawn(view.gameObject);
            }


            ParticleSystem impactFX =
                GameObject.Instantiate<ParticleSystem>(bulletImpactTemplate, _bulletParent.transform);
            impactFX.Stop();
            _projectileImpactViewList.Add(impactFX);
            
            ParticleSystem bloodImpactFX =
                GameObject.Instantiate<ParticleSystem>(bloodImpactTemplate, _bulletParent.transform);
            bloodImpactFX.Stop();
            _bloodImpactViewList.Add(bloodImpactFX);
        }
    }

    private void onStep(float deltaTime)
    {
        // Gross, figure out a better way to do this
        if(Singleton.instance.networkManager.isPureClient)
        {
            return;
        }
        
        
        float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        for(int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = _gameState.projectileStateList[i];
            BulletView      view  = _projectileViewPool[i];
        
            if(state.isActive)
            {
                view.transform.position = Vector2.Lerp(state.prevPosition, state.position, alpha);
            }
        }
    }
    
    private void onFixedStep(float deltaTime)
    {
        // Gross, figure out a better way to do this
        if(Singleton.instance.networkManager.isPureClient)
        {
            return;
        }
        
        if (_gameState.projectileStateList == null || _projectileViewPool == null)
        {
            return;
        }

        _projectileImpactViewList.Sort(_impactFXSort);
        _bloodImpactViewList.Sort(_impactFXSort);

        double now = TimeUtil.TimeSinceGameStart();
        
        for (int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = _gameState.projectileStateList[i];
            BulletView      view  = _projectileViewPool[i];
            
            if (state.isActive)
            {
                float   scaledDeltaTime = state.timeScale * deltaTime;
                
                float   lookAhead       = state.velocity.magnitude * scaledDeltaTime;
                Vector2 bulletDir       = state.velocity.normalized;
                Vector2 rayStart        = state.position;
                
                Debug.DrawRay(rayStart, bulletDir * lookAhead, Color.green);
                
                int hitCount = Physics2D.RaycastNonAlloc(rayStart, bulletDir, _raycastHitList, lookAhead, state.data.targetMask);
                if(hitCount > 0)
                {
                    RaycastHit2D hit = _raycastHitList[0];
                    
                    // TODO: Cache these GetComponents so they can be turned into a look up table
                    IAttackTarget target = hit.collider.GetComponentInParent<IAttackTarget>();
                    
                    var impactList = _projectileImpactViewList;
                    
                    //Do damage
                    if (target != null)
                    {
                        HitModifier hitModifier = hit.collider.GetComponent<HitModifier>();
                        float damageModAmount = (hitModifier != null) ? hitModifier.modifier : 1.0f;
                        
                        int layer = hit.transform.gameObject.layer;
                        
                        if(layer == LayerMask.NameToLayer("playerHurtbox") ||
                           layer == LayerMask.NameToLayer("enemyHurtbox"))
                        {
                            impactList = _bloodImpactViewList;
                        }

                        float damageAmount = state.damage * damageModAmount;

                        AttackData damageData = new AttackData(state.ownerUUID,
                                                               view.gameObject.layer,
                                                               state.data.damageType,
                                                               damageAmount,
                                                               state.velocity.normalized,
                                                               hit);
                        
                        AttackResult result = target.TakeDamage(damageData);

                        _gameSystems.DispatchEvent(GamePlayEventType.AVATAR_DAMAGED, false, result);
                    }

                    ParticleSystem impactFX = _activateImpactFX(impactList, bulletDir, hit);
                    if(impactFX == null)
                    {
                        Debug.LogWarning("Could not create impactFX!");
                    }
                    
                    state.isActive = false;
                }

                state.prevPosition = state.position;
                state.position += (state.velocity * scaledDeltaTime);
                

                if (state.deathTime < now)
                {
                    state.isActive = false;
                }
            }
            else
            {
                if (view.gameObject.activeSelf)
                {
                    view.Recycle();
                }
            }
            
            // Make sure this happens if state was written to!
            _gameState.projectileStateList[i] = state;
        }
    }

    public void CleanUp()
    {
        for (int i = 0; i < _projectileViewPool.Count; ++i)
        {
            if (NetworkServer.active)
            {
                // NetworkServer.Destroy(_projectileViewPool[i].gameObject);                
            }
            else
            {
                GameObject.Destroy(_projectileViewPool[i].gameObject);
            }
        }
        
        for (int i = 0; i < _projectileImpactViewList.Count; ++i)
        {
            GameObject.Destroy(_projectileImpactViewList[i].gameObject);
        }

        for(int i = 0; i < _bloodImpactViewList.Count; ++i)
        {
            GameObject.Destroy(_bloodImpactViewList[i].gameObject);
        }

        GameObject.Destroy(_bulletParent);
        
        _gameState.projectileStateList.Clear();
        
        
        _gameSystems.onStep      -= onStep;
        _gameSystems.onFixedStep -= onFixedStep;
        
        // ClientScene.UnregisterPrefab( _gameplayResources.bulletView.gameObject);
    }

    public ProjectileState Spawn(string ownerUUID, ProjectileData data, Vector3 position, Vector3 direction)
    {
        double now = TimeUtil.TimeSinceGameStart();
        
        for (int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = _gameState.projectileStateList[i];
            BulletView      view  = _projectileViewPool[i];
            
            if (!state.isActive)
            {
                state.isActive     = true;
                state.ownerUUID    = ownerUUID;
                state.data         = data;
                state.speed        = data.speed;
                state.damage       = data.damage;
                state.deathTime    = now + data.deathTimer;
                state.timeScale    = 1.0f;
                state.position     = position;
                state.prevPosition = position;
                state.velocity     = direction * state.speed;
                state.angle        = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                _gameState.projectileStateList[i] = state;
                
                view.Reset(position);
                Vector3 eulerAngles     = view.transform.eulerAngles;
                eulerAngles.z           = state.angle;
                view.transform.rotation = Quaternion.Euler(eulerAngles);
                
                _gameSystems.DispatchEvent(GamePlayEventType.PROJECTILE_SPAWNED, false, state);
                return state;
            }
        }

        return default(ProjectileState);
    }

    private int _impactFXSort(ParticleSystem a, ParticleSystem b)
    {
        return b.isPlaying.CompareTo(a.isPlaying);
    }
    
    // private GameObject onClientProjectileSpawnHandler(SpawnMessage msg)
    // {
    //     GameObject result = null;
    //     
    //     if (_netPrefabMap.ContainsKey(msg.assetId))
    //     {
    //         for (int i = 0; i < _poolSize; ++i)
    //         {
    //             ProjectileState state = _gameState.projectileStateList[i];
    //             BulletView      view  = _projectileViewPool[i];
    //             
    //             if (state.netId == 0)
    //             {
    //                 state.netId    = msg.netId;
    //                 state.isActive = false;
    //                 result         = view.gameObject;
    //                 break;
    //             }
    //         }
    //     }
    //     return result;
    // }
    
    private void onClientProjectileUnspawnHandler(GameObject obj)
    {
        
    }
    
    private ParticleSystem _getAvailableImpact(List<ParticleSystem> impactList)
    {
        for(int i = 0; i < impactList.Count; ++i)
        {
            if(!impactList[i].isPlaying)
            {
                return impactList[i];
            }
        }

        return null;
    }

    private ParticleSystem _activateImpactFX(List<ParticleSystem> impactList, Vector3 bulletDir, RaycastHit2D hit)
    {
        //Handle impact fx
        ParticleSystem impactFX = _getAvailableImpact(impactList);
        if(impactFX != null)
        {
            Vector3 faceDir       = Vector3.Reflect(bulletDir, hit.normal);
            Vector3 calculatedRot = Quaternion.LookRotation(faceDir).eulerAngles;
            //Randomize it a bit
            calculatedRot.x += Random.Range(-35, 35);
            
            impactFX.Clear();
            impactFX.transform.position      = hit.point;
            impactFX.transform.localRotation = Quaternion.Euler(calculatedRot);
            impactFX.Play();
        }

        return impactFX;
    }
}
