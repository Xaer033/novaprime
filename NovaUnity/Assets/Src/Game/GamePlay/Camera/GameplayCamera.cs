using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameplayCamera : MonoBehaviour
{
    public Vector2 focusAreaSize;
    public Vector3 targetOffset;
    public float lookAheadDistanceX;
    public float lookSmoothTimeX;
    public float lookSmoothTimeY;

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
    
    private FocusArea _focusArea;
    private Vector3 _velocity;
    private PlayerAvatarView _target;
    private bool _lookAheadStopped;
    private Camera _camera;
    
    public Camera camera
    {
        get { return _camera; }
    }

    public PlayerAvatarView target
    {
        set
        {
            _target = value;
            if (_target != null && _target.constrainer.collisionCollider != null)
            {
                _focusArea = new FocusArea(target.constrainer.collisionCollider.bounds, focusAreaSize);
            }
            else
            {
                Debug.LogError("Target or target's collider is null!");
            }
            
            targetList.Add(target.transform);
        }
        get { return _target; }
    }

    public void Awake()
    {
        _camera = GetComponent<Camera>();
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null)
        {
            _focusArea.Step(target.constrainer.collisionCollider.bounds, focusAreaSize);
            Vector3 focusPosition = _focusArea.center + targetOffset;

            if (_focusArea.velocity.x != 0)
            {
                _lookAheadDirX = Mathf.Sign(_focusArea.velocity.x);
                float targetInputX = _target.controller.lastInput.horizontalMovement;
                if (Mathf.Sign(targetInputX) == _lookAheadDirX && Mathf.Abs(targetInputX) > 0)
                {
                    _lookAheadStopped = false;
                    _targetlookAheadX = _lookAheadDirX * lookAheadDistanceX;
                }
                else
                {
                    if (!_lookAheadStopped)
                    {
                        _lookAheadStopped = true;
                        _targetlookAheadX = _currentLookAheadX + (_lookAheadDirX * lookAheadDistanceX - _currentLookAheadX)/4.0f;
                    }
                }
            }

//            _currentLookAheadX = Mathf.SmoothDamp(_currentLookAheadX, _targetlookAheadX, ref _smoothLookVelocityX, lookSmoothTimeX);
//
//            focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref _smoothVelocityY, lookSmoothTimeY);
//            focusPosition += Vector3.right * _currentLookAheadX;
//            transform.position = focusPosition;
        }

        Bounds boundObj = _getBounds();

        float distance = boundObj.size.x;
        float newZoom = Mathf.Lerp(maxZoom, minZoom, distance / limit);
        _camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, newZoom, Time.deltaTime);
        
        Vector3 center = boundObj.center + targetOffset;
        transform.position = Vector3.SmoothDamp(transform.position, center, ref _smoothVelocity, lookSmoothTimeX);
    }

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

    private void OnDrawGizmos()
    { 
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        
        Gizmos.DrawCube(_focusArea.center, focusAreaSize);
    }
    
    
    struct FocusArea
    {
        public Vector3 center;
        public Vector3 velocity;
        public Vector2 size;
        
        public float left;
        public float right;
        public float top;
        public float bottom;

        public FocusArea(Bounds targetBounds, Vector2 targetSize)
        {
            size = targetSize;
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;
            center = new Vector3((left + right) / 2, (top + bottom) / 2);
            velocity = Vector3.zero;
        }

        public void Resize(Bounds targetBounds, Vector2 targetSize)
        {
            size = targetSize;
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;
            center = new Vector3((left + right) / 2, (top + bottom) / 2);
            velocity = Vector3.zero;
        }
        public void Step(Bounds targetBounds, Vector2 targetSize)
        {
            if (size != targetSize)
            {
                Resize(targetBounds, targetSize);
            }
            
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }

            left += shiftX;
            right += shiftX;
            
            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }

            top += shiftY;
            bottom += shiftY;
            
            center = new Vector3((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector3(shiftX, shiftY);
        }
    }
}
