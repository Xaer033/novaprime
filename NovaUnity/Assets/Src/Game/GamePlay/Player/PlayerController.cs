using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{
    private PlayerAvatarView _view;

    private Vector3 _inputVector;
    // Start is called before the first frame update
    void Start(PlayerAvatarView view)
    {
        _view = view;
    }

    // Update is called once per frame
    public void Step(float deltaTime)
    {
        if (Input.GetKey(KeyCode.D))
        {
            _inputVector.x = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _inputVector.x = -1;
        }
        
        
    }
    
    public void FixedStep(float fixedDeltaTime)
    {
        
    }
}
