using System.Collections.Generic;
using GhostGen;
using UnityEngine;

public class ProjectileSystem : NotificationDispatcher, IGameSystem
{
    private GameSystems _gameSystems;
    private GameState _gameState;
    
//    private ProjectileState[] _projectilePool;
    private List<BulletView> _projectileViewPool;
    
    private int _poolSize;
    private RaycastHit2D[] _raycastHitList;
    
    public ProjectileSystem(int poolSize)
    {
        _poolSize = poolSize;
        _projectileViewPool = new List<BulletView>(poolSize);
        _raycastHitList = new RaycastHit2D[3];
    }
    
    public void Start(GameSystems gameSystems, GameState gameState)
    {
        _gameSystems = gameSystems;
        _gameState = gameState;
        
        BulletView projectileTemplate = Singleton.instance.gameplayResources.bulletView;
        for (int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = new ProjectileState();
            _gameState.projectileStateList.Add(state);
            
            BulletView view = GameObject.Instantiate<BulletView>(projectileTemplate);
            view.state = state;
            view.Recycle();
            _projectileViewPool.Add(view);
        }
    }

    public void Step(float deltaTime)
    {
        
    }
    
    public void FixedStep(float deltaTime)
    {
        if (_gameState.projectileStateList == null || _projectileViewPool == null)
        {
            return;
        }

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

                    if (target != null)
                    {
                        AttackData damageData = new AttackData(state.data.damageType, state.damage, state.velocity.normalized);
                        AttackResult result = target.TakeDamage(damageData);

                        _gameSystems.DispatchEvent(GamePlayEventType.AVATAR_DAMAGED, false, result);
                    }
                    //Do damage
                    state.isActive = false;
                }
                
                state.position += (state.velocity * scaledDeltaTime);
                view.transform.position = state.position;

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

        _gameState.projectileStateList.Clear();
    }

    public ProjectileState Spawn(ProjectileData data, Vector3 position, Vector3 direction)
    {
        for (int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = _gameState.projectileStateList[i];
            BulletView view = _projectileViewPool[i];
            
            if (!state.isActive)
            {
                state.isActive = true;
                state.data = data;
                state.speed = data.speed;
                state.damage = data.damage;
                state.deathTimer = data.deathTimer;
                state.timeScale = 1.0f;
                state.position = position;
                state.velocity = direction * state.speed;
                
                view.Reset(position);
                
                _gameSystems.DispatchEvent(GamePlayEventType.PROJECTILE_SPAWNED, false, state);
                return state;
            }
        }

        return null;
    }
}
