using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameplayCamera : MonoBehaviour
{
    public CinemachineVirtualCamera cineCamera;
    
    public Vector3 targetOffset;
    public float lookSmoothTimeX;
    public float minZoom;
    public float maxZoom;
    public float limit;
    
    public List<Transform> targetList;
    
    private float _currentLookAheadX;
    private float _targetlookAheadX;
    private float _lookAheadDirX;
    private float _smoothLookVelocityX;
    private float _smoothVelocityY;
    private Vector3 _smoothVelocity;
    
    private Vector3 _velocity;
    private Camera _camera;
    
    public Camera gameCamera
    {
        get { return _camera; }
    }


    public void AddTarget(Transform t)
    {
        cineCamera.Follow = t;
        cineCamera.transform.position = t.position;
        if (targetList != null)
        {
            targetList.Add(t);
        }
    }

    public void RemoveTarget(Transform t)
    {
        cineCamera.Follow = null;
        
        if (targetList != null)
        {
            targetList.Remove(t);
        }
    }

    public void ClearTargets()
    {
        cineCamera.Follow = null;

        if (targetList != null)
        {
            targetList.Clear();
        }
    }
    public void Awake()
    {
        _camera = GetComponent<Camera>();
    }
    
    // Update is called once per frame
    // void LateUpdate()
    // {
    //     Bounds boundObj = _getBounds();
    //
    //     float distance = boundObj.size.x;
    //     float newZoom = Mathf.Lerp(maxZoom, minZoom, distance / limit);
    //     _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, newZoom, Time.deltaTime);
    //     
    //     Vector3 center = boundObj.center + targetOffset;
    //     transform.position = Vector3.SmoothDamp(transform.position, center, ref _smoothVelocity, lookSmoothTimeX);
    // }

    private Bounds _getBounds()
    {

        Vector3 startPos = targetList.Count > 0 ? targetList[0].position : Vector3.zero;
        Bounds b = new Bounds(startPos, Vector3.zero);//targetList[0].position, Vector3.zero);

        for (int i = 0; i < targetList.Count; ++i)
        {
            b.Encapsulate(targetList[i].position);    
        }

        return b;
    }
}
