using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvatarView : MonoBehaviour
{
    public float speed;
    public float drag;
    
    private Vector3 _inputVector;

    private Vector3 _velocity;
    private Vector3 _acceleration;
    private Vector3 _fixedPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        _acceleration.x = Input.GetAxis("playerHorizontal");
//        if (Input.GetKey(KeyCode.D))
//        {
//            _inputVector.x = 1;
//        }
//        else if (Input.GetKey(KeyCode.A))
//        {
//            _inputVector.x = -1;
//        }
//        else
//        {
//            _inputVector.x = 0;
//        }
        if(transform != null)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _fixedPosition, Time.deltaTime);
        }
    }

    public void FixedUpdate()
    {
                    
        float deltaTime = Time.fixedDeltaTime;
        _acceleration = _acceleration.normalized * speed;
        _fixedPosition = transform.localPosition + _velocity * deltaTime;

        float dragForce = 1.0f - (drag * deltaTime);
        _velocity = (_velocity + _acceleration * deltaTime) * dragForce;
//        _velocity.y = 0;
        
        _acceleration = Vector3.zero;
        
    }
}
