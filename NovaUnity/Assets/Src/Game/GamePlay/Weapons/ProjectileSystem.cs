using GhostGen;
using UnityEngine;

public class ProjectileSystem : NotificationDispatcher
{
    private ProjectileState[] _projectilePool;
    private BulletView[] _projectileViewPool;
    
    private int _poolSize;
    
    public ProjectileSystem(int poolSize)
    {
        _poolSize = poolSize;
        _projectilePool = new ProjectileState[poolSize];
        _projectileViewPool = new BulletView[poolSize];
    }
    
    public void Start()
    {
        BulletView projectileTemplate = Singleton.instance.gameplayResources.bulletView;
        for (int i = 0; i < _poolSize; ++i)
        {
            _projectilePool[i] = new ProjectileState();
            _projectileViewPool[i] = GameObject.Instantiate<BulletView>(projectileTemplate);
            _projectileViewPool[i].state = _projectilePool[i];
            _projectileViewPool[i].Recycle();
        }
    }

    public void Step(float deltaTime)
    {
        
    }
    
    public void FixedStep(float deltaTime)
    {
        if (_projectilePool == null || _projectileViewPool == null)
        {
            return;
        }

        RaycastHit hit;
        for (int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = _projectilePool[i];
            BulletView view = _projectileViewPool[i];
            
            if (state.isActive)
            {
                float scaledDeltaTime = state.timeScale * deltaTime;
                
                state.deathTimer -= deltaTime;

                float lookAhead = state.velocity.magnitude * scaledDeltaTime;
                Vector3 bulletDir = state.velocity.normalized;
                Vector3 rayStart = state.position;
                Debug.DrawRay(rayStart, bulletDir * lookAhead, Color.green);
                bool isHit = Physics.Raycast(rayStart, bulletDir, out hit, lookAhead, view.collisionMask);
                if (isHit)
                {
                    
                    Debug.DrawRay(hit.point, hit.normal, Color.red, 1.0f);
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

    public ProjectileState Spawn(ProjectileData data, Vector3 position, Vector3 direction)
    {
        for (int i = 0; i < _poolSize; ++i)
        {
            ProjectileState state = _projectilePool[i];
            BulletView view = _projectileViewPool[i];
            
            if (!state.isActive)
            {
                state.isActive = true;
                state.type = data.type;
                state.speed = data.speed;
                state.damage = data.damage;
                state.deathTimer = data.deathTimer;
                state.timeScale = 1.0f;
                state.position = position;
                state.velocity = direction * state.speed;
                
                view.Reset(position);
                return state;
            }
        }

        return null;
    }
}
