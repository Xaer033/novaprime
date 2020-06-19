using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeWarpFieldController : MonoBehaviour
{
    [Serializable]
    public class LayerMaskScale
    {
        public LayerMask layerMask;
        public float timeScale;
    }
    
    public List<LayerMaskScale> affectedLayerList;
    
    public LayerMask affectedLayers;
    public Renderer _renderer;
    
    
    public float timeScale = 0.2f;
    public float radius = 2;
    public float pulseScale = 1.0f;
    
    private int _radiusShaderId;
    private Material _timeWarpMat;

    private Vector3 _initialRendererScale;
    private Collider2D[] _colliderList;
    
    private Dictionary<Collider2D, ITimeWarpTarget> _timeWarpMap = new Dictionary<Collider2D, ITimeWarpTarget>();
    
    private HashSet<ITimeWarpTarget> _insideSet = new HashSet<ITimeWarpTarget>();
    private List<ITimeWarpTarget> _insideList = new List<ITimeWarpTarget>();
    private List<ITimeWarpTarget> _currentSphereTargets = new List<ITimeWarpTarget>();

    void Awake()
    {
        _colliderList = new Collider2D[50];
        _radiusShaderId = Shader.PropertyToID("_radius");
        if (_renderer)
        {
            _timeWarpMat = _renderer.material;
            _initialRendererScale = _renderer.transform.localScale;
        }
        
    }
    void FixedUpdate()
    {
        if (_timeWarpMat)
        {
            _timeWarpMat.SetFloat(_radiusShaderId, radius);
        }

        _animateRenderer();
        
        _currentSphereTargets.Clear();
        
        int colliderCount = Physics2D.OverlapCircleNonAlloc(transform.position, radius, _colliderList, affectedLayers);
        _generateCurrentTargetList(ref _currentSphereTargets, _colliderList, colliderCount);
        
        for (int i = 0; i < _currentSphereTargets.Count; ++i)
        {
            ITimeWarpTarget target = _currentSphereTargets[i];
            if (!_insideSet.Contains(target))
            {
                _addTarget(target);
            }
        }

        for (int i = _insideList.Count -1; i >= 0; --i)
        {
            ITimeWarpTarget target = _insideList[i];
            if (!_currentSphereTargets.Contains(target))
            {
                _removeTarget(target);
            }
        }
        
    }

    private void _animateRenderer()
    {
        if (_renderer)
        {
            _renderer.transform.localScale = _initialRendererScale + (Vector3.one * Mathf.Abs(Mathf.Sin(Time.time) * pulseScale));
        }
    }
    
    private void _generateCurrentTargetList(ref List<ITimeWarpTarget> targetList, Collider2D[] colliderList, int colliderCount)
    {
        for (int i = 0; i < colliderCount; ++i)
        {
            Collider2D collider = colliderList[i];
            ITimeWarpTarget target;
            if (!_timeWarpMap.TryGetValue(collider, out target))
            {
                target = collider.gameObject.GetComponent<ITimeWarpTarget>();
                if (target != null)
                {
                    _timeWarpMap[collider] = target;
                }
            }

            if (target == null)
            {
                continue;
            }

            targetList.Add(target);
        }
    }
    
    private void _addTarget(ITimeWarpTarget target)
    {
        _insideSet.Add(target);
        _insideList.Add(target);
        target.OnTimeWarpEnter(timeScale);
    }
    
    private void _removeTarget(ITimeWarpTarget target)
    {
        target.OnTimeWarpExit();
        _insideSet.Remove(target);
        _insideList.Remove(target);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
        transform.localScale = Vector3.one * radius;
    }
}
