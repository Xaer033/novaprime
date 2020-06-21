using System;
using System.Collections.Generic;
using GhostGen;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileSystem : NotificationDispatcher, IGameSystem
{
    private GameSystems _gameSystems;
    private GameState _gameState;
    private GameplayResources _gameplayResources;

    private GameObject _bulletParent;
    
//    private ProjectileState[] _projectilePool;
    private List<BulletView> _projectileViewPool;
    private List<ParticleSystem> _projectileImpactViewList;
    private List<ParticleSystem> _bloodImpactViewList;
    
    private int _poolSize;
    private RaycastHit2D[] _raycastHitList;
    
    
    public int priority { get; set; }
    
    public ProjectileSystem(GameplayResources gameplayResources, int poolSize)
    {
        _gameplayResources = gameplayResources;
        _poolSize = poolSize;
        _projectileViewPool = new List<BulletView>(poolSize);
        _projectileImpactViewList = new List<ParticleSystem>(poolSize);
        _bloodImpactViewList = new List<ParticleSystem>(40);
        
        _raycastHitList = new RaycastHit2D[3];
    }
    
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState = gameState;
        
        _bulletParent = new GameObject("BulletParent");
        BulletView projectileTemplate = _gameplayResources.bulletView;
        ParticleSystem bulletImpactTemplate = _gameplayResources.bulletImpactFX;
        ParticleSystem bloodImpactTemplate = _gameplayResources.avatarImpactFX;
        
        for (int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = new ProjectileState();
            _gameState.projectileStateList.Add(state);
            
            BulletView view = GameObject.Instantiate<BulletView>(projectileTemplate, _bulletParent.transform);
            view.state = state;
            view.Recycle();
            _projectileViewPool.Add(view);


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

    public void Step(float deltaTime)
    {
        float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        
        for(int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = _gameState.projectileStateList[i];
            BulletView view = _projectileViewPool[i];
        
            if(state.isActive)
            {
                view.transform.position = Vector3.Lerp(state.prevPosition, state.position, alpha);
            }
        }
    }
    
    public void FixedStep(float deltaTime)
    {
        if (_gameState.projectileStateList == null || _projectileViewPool == null)
        {
            return;
        }

        _projectileImpactViewList.Sort(_impactFXSort);
        _bloodImpactViewList.Sort(_impactFXSort);
        
        for (int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = _gameState.projectileStateList[i];
            BulletView view = _projectileViewPool[i];
            
            if (state.isActive)
            {
                float scaledDeltaTime = state.timeScale * deltaTime;
                
                state.deathTimer -= deltaTime;

                float lookAhead = state.velocity.magnitude * scaledDeltaTime;
                Vector3 bulletDir = state.velocity.normalized;
                Vector3 rayStart = state.position;
                
                Debug.DrawRay(rayStart, bulletDir * lookAhead, Color.green);
                
                int hitCount = Physics2D.RaycastNonAlloc(rayStart, bulletDir, _raycastHitList, lookAhead, state.data.targetMask);
                if(hitCount > 0)
                {
                    RaycastHit2D hit = _raycastHitList[0];
                    IAttackTarget target = hit.collider.GetComponent<IAttackTarget>();
                    var impactList = _projectileImpactViewList;
                    
                    //Do damage
                    if (target != null)
                    {
                        int layer = hit.transform.gameObject.layer;
                        
                        if(layer == LayerMask.NameToLayer("player") ||
                           layer == LayerMask.NameToLayer("enemies"))
                        {
                            impactList = _bloodImpactViewList;
                        }
                        
                        AttackData damageData = new AttackData(state.ownerUUID, view.gameObject.layer, state.data.damageType, state.damage, state.velocity.normalized);
                        AttackResult result = target.TakeDamage(damageData);

                        _gameSystems.DispatchEvent(GamePlayEventType.AVATAR_DAMAGED, false, result);
                    }

                    switch(hit.transform.gameObject.layer)
                    {
                    
                        
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
                // view.transform.position = state.position;
                

                if (state.deathTimer <= 0.0f)
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
        }
    }


    public void LateStep(float deltaTime)
    {
        
    }
    
    public void CleanUp()
    {
        for (int i = 0; i < _projectileViewPool.Count; ++i)
        {
            GameObject.Destroy(_projectileViewPool[i].gameObject);
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
    }

    public ProjectileState Spawn(string ownerUUID, ProjectileData data, Vector3 position, Vector3 direction)
    {
        for (int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = _gameState.projectileStateList[i];
            BulletView view = _projectileViewPool[i];
            
            if (!state.isActive)
            {
                state.isActive = true;
                state.ownerUUID = ownerUUID;
                state.data = data;
                state.speed = data.speed;
                state.damage = data.damage;
                state.deathTimer = data.deathTimer;
                state.timeScale = 1.0f;
                state.position = position;
                state.prevPosition = position;
                state.velocity = direction * state.speed;
                
                view.Reset(position);
                Vector3 eulerAngles = view.transform.eulerAngles;
                eulerAngles.z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                view.transform.rotation = Quaternion.Euler(eulerAngles);
                
                _gameSystems.DispatchEvent(GamePlayEventType.PROJECTILE_SPAWNED, false, state);
                return state;
            }
        }

        return null;
    }

    private int _impactFXSort(ParticleSystem a, ParticleSystem b)
    {
        return b.isPlaying.CompareTo(a.isPlaying);
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
            Vector3 faceDir = Vector3.Reflect(bulletDir, hit.normal);
            Vector3 calculatedRot = Quaternion.LookRotation(faceDir).eulerAngles;
            //Randomize it a bit
            calculatedRot.x += Random.Range(-35, 35);
            
            impactFX.Clear();
            impactFX.transform.position = hit.point;
            impactFX.transform.rotation = Quaternion.LookRotation(faceDir);
            impactFX.transform.localRotation = Quaternion.Euler(calculatedRot);
            impactFX.Play();
        }

        return impactFX;

    }
}
