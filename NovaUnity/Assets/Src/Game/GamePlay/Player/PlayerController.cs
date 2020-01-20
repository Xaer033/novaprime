﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PlayerController
{

//    struct RaycastOrigins
//    {
//        public Vector3 topLeft;
//        public Vector3 topRight;
//        public Vector3 bottomLeft;
//        public Vector3 bottomRight;
//    }

    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool right;
        public bool left;
        public bool climbingSlope;
        public bool decendingSlope;
        public float slopeAngle;
        public float slopeAngleOld;
        public Vector3 velocityOld;
        

        public void Reset(Vector3 oldVelocity)
        {
            above = below = right = left = climbingSlope = decendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
            velocityOld = oldVelocity;
        }
    }

    
    private PlayerAvatarView _view; 
    
    private Vector3 _velocity;
    private Vector3 _fixedPosition;
    private float _velocityXSmoothing;

//    private float _horizontalRaySpacing;
//    private float _verticalRaySpacing;

    private RaycastController _raycastController;
//    private RaycastOrigins _raycastOrigins;
    public CollisionInfo _collisionInfo;
    
    // Start is called before the first frame update
    public void Start(PlayerAvatarView view)
    {
        _view = view;
        
        _raycastController = new RaycastController(
            _view.horizontalRayCount, 
            _view.verticalRayCount,
            _view.collider, 
            _view.collisionMask);
    }

    public Vector3 position
    {
        get
        {
            return _view.transform.position;
        }
    }
    
    public Quaternion rotation
    {
        get
        {
            return _view.transform.rotation;
        }
    }
    
    // Update is called once per frame
    public void Step(float deltaTime, FrameInput input)
    {
        if (_view)
        {
            if (_collisionInfo.above || _collisionInfo.below)
            {
                _velocity.y = 0;
            }

            float timeToJumpApex = _view.timeToJumpApex;
            float gravity = -(2 * _view.jumpHeight) / (timeToJumpApex * timeToJumpApex);
            float jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            
            if (input.jump && _collisionInfo.below)
            {
                _velocity.y = jumpVelocity;
            }
            float targetVelocityX  = input.horizontalMovement * _view.speed;

            float accelerationTime = _collisionInfo.below ? _view.accelerationTimeGround : _view.accelerationTimeAir;
            
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, accelerationTime);
            _velocity.y += gravity * deltaTime;
            
            
//            if (_view.requestVelocity != Vector3.zero)
//            {
//                _move(deltaTime, _view.requestVelocity);
//                _view.requestVelocity = Vector3.zero;
//            }
//            
            _move(deltaTime, _velocity * deltaTime);

        }
//        if (_view)
//        {
//            _acceleration.x = input.horizontalMovement;
//            if(_view.transform != null)
//            {
//                _view.transform.localPosition = Vector3.Lerp(_view.transform.localPosition, _fixedPosition, deltaTime);
//            }
//        }
    }
    
    public void FixedStep(float fixedDeltaTime)
    {
//        if (_view)
//        {
//            _velocity.y += _view.gravity * fixedDeltaTime;
//            Vector3 deltaVel = _velocity * fixedDeltaTime;
//            
//            _move(fixedDeltaTime, deltaVel);
//        }
    }

    private Vector3 _verticalCollisions(Vector3 currentVelocity)
    {
        Vector3 newVelocity = currentVelocity;
        float directionY = Mathf.Sign(currentVelocity.y);
        float rayLength = Mathf.Abs(currentVelocity.y) + _raycastController.skinWidth;
        RaycastHit hit;
        
        for (int i = 0; i < _view.verticalRayCount; ++i)
        {
            Vector3 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector3.right * (_raycastController.verticalRaySpacing * i + currentVelocity.x);
            if (Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, _view.collisionMask))
            {
                newVelocity.y = (hit.distance - _raycastController.skinWidth) * directionY;
                rayLength = hit.distance;

                if (_collisionInfo.climbingSlope)
                {
                    newVelocity.x = newVelocity.y / Mathf.Tan(_collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(newVelocity.x);
                }
                _collisionInfo.below = directionY == -1;
                _collisionInfo.above = directionY == 1;
            }
            
            Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength, Color.red);
        }

        if (_collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(newVelocity.x);
            rayLength = Mathf.Abs(newVelocity.x) + _raycastController.skinWidth;
            Vector3 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) + Vector3.up * newVelocity.y;
            
            if(Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, _view.collisionMask))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle != _collisionInfo.slopeAngle)
                {
                    _velocity.x = (hit.distance - _raycastController.skinWidth) * directionX;
                    _collisionInfo.slopeAngle = slopeAngle;
                }
            }
        }
        return newVelocity;
    }
    
    private Vector3 _horizontalCollisions(Vector3 currentVelocity)
    {
        Vector3 newVelocity = currentVelocity;
        float directionX = Mathf.Sign(currentVelocity.x);
        float rayLength = Mathf.Abs(currentVelocity.x) + _raycastController.skinWidth;
        RaycastHit hit;
        
        for (int i = 0; i < _view.horizontalRayCount; ++i)
        {
            Vector3 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
            rayOrigin += Vector3.up * (_raycastController.horizontalRaySpacing * i);
            if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, _view.collisionMask))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (i == 0 && slopeAngle <= _view.maxClimbAngle)
                {
                    if (_collisionInfo.decendingSlope)
                    {
                        _collisionInfo.decendingSlope = false;
                        newVelocity = _collisionInfo.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != _collisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - _raycastController.skinWidth;
                        newVelocity.x -= distanceToSlopeStart * directionX;
                    }

                    newVelocity = _climbSlope(currentVelocity, slopeAngle);
                    newVelocity.x += distanceToSlopeStart * directionX;
                }

                if (!_collisionInfo.climbingSlope || slopeAngle > _view.maxClimbAngle)
                {
                    newVelocity.x = (hit.distance - _raycastController.skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (_collisionInfo.climbingSlope)
                    {
                        newVelocity.y = Mathf.Tan(_collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(newVelocity.x);
                    }
                    _collisionInfo.left = directionX == -1;
                    _collisionInfo.right = directionX == 1;
                }
            }
            Debug.DrawRay(rayOrigin, Vector3.right * directionX * rayLength, Color.red);

        }

        return newVelocity;
    }

    private Vector3 _climbSlope(Vector3 velocity, float slopeAngle)
    {
        Vector3 newVelocity = velocity;
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        
        newVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
        
        if (velocity.y <= climbVelocityY)
        {
            newVelocity.y = climbVelocityY;
            _collisionInfo.below = true;
            _collisionInfo.climbingSlope = true;
            _collisionInfo.slopeAngle = slopeAngle;
        }
        
        return newVelocity;

    }

    private RaycastController.RaycastOrigins _raycastOrigins
    {
        get { return _raycastController.origins; }
    }

    private Vector3 _decendSlope(Vector3 velocity)
    {
        Vector3 newVelocity = velocity;
        float directionX = Mathf.Sign(velocity.x);
        RaycastHit hit;
        Vector3 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight :  _raycastOrigins.bottomLeft;
        
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, _view.collisionMask))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (Mathf.Abs(slopeAngle) > 0.0001f && slopeAngle <= _view.maxDecendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - _raycastController.skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float decendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        newVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);;
                        newVelocity.y -= decendVelocityY;

                        _collisionInfo.slopeAngle = slopeAngle;
                        _collisionInfo.decendingSlope = true;
                        _collisionInfo.below = true;
                    }
                }
            }
        }

        return newVelocity;
    }
    private void _move(float deltaTime, Vector3 velocity)
    {
        _raycastController.Step();
        _collisionInfo.Reset(velocity);

        if (velocity.y < 0)
        {
            velocity = _decendSlope(velocity);
        }
        if (Mathf.Abs(velocity.x) > 0.0001f)
        {
            velocity = _horizontalCollisions(velocity);
        }

        if (Mathf.Abs(velocity.y) > 0.0001f)
        {
            velocity = _verticalCollisions(velocity);
        }
        
        _view.transform.Translate(velocity);
        
    }
//    private void _updateRaycastOrigins()
//    {
//        Bounds bounds = _view.collider.bounds;
//        bounds.Expand(skinWidth * -2);
//        
//        _raycastOrigins.bottomLeft = new Vector3(bounds.min.x, bounds.min.y);
//        _raycastOrigins.bottomRight = new Vector3(bounds.max.x, bounds.min.y);
//        _raycastOrigins.topLeft = new Vector3(bounds.min.x, bounds.max.y);
//        _raycastOrigins.topRight = new Vector3(bounds.max.x, bounds.max.y);
//
//    }
//    private void _calculateRaySpacing()
//    {
//        Bounds bounds = _view.collider.bounds;
//        bounds.Expand(skinWidth * -2);
//
//        int horizontalRayCount = Mathf.Clamp(_view.horizontalRayCount, 2, int.MaxValue);
//        int verticalRayCount = Mathf.Clamp(_view.verticalRayCount, 2, int.MaxValue);
//        
//        _horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
//        _verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
//    }
}
