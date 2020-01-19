using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{
    private PlayerAvatarView _view; 
    
    private Vector3 _velocity;
    private Vector3 _acceleration;
    private Vector3 _fixedPosition;
    // Start is called before the first frame update
    public void Start(PlayerAvatarView view)
    {
        _view = view;
    }

    // Update is called once per frame
    public void Step(float deltaTime)
    {
        if (_view)
        {
            _acceleration.x = Input.GetAxis("playerHorizontal");
            if(_view.transform != null)
            {
                _view.transform.localPosition = Vector3.Lerp(_view.transform.localPosition, _fixedPosition, Time.deltaTime);
            }
        }
    }
    
    public void FixedStep(float fixedDeltaTime)
    {
        if (_view)
        {
            float deltaTime = Time.fixedDeltaTime;
            _acceleration = _acceleration.normalized * _view.speed;
            _fixedPosition = _view.transform.localPosition + _velocity * deltaTime;

            float dragForce = 1.0f - (_view.drag * deltaTime);
            _velocity = (_velocity + _acceleration * deltaTime) * dragForce;
    //        _velocity.y = 0;
            
            _acceleration = Vector3.zero;
        }
    }
}
