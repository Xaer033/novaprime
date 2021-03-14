using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayerInputGenerator : IInputGenerator
{  
    private ServerPlayerInputBuffer _inputBuffer;


    public PlayerInputTickPair lastTickInput { get; private set; }

    public ServerPlayerInputGenerator()
    {
        _inputBuffer = new ServerPlayerInputBuffer(PlayerState.MAX_INPUTS);
    }

    public void AddInput(SendPlayerInput inputMsg)
    {
        _inputBuffer?.Push(inputMsg);
    }
    
    public FrameInput GetInput()
    {
        FrameInput result;

        if (!_inputBuffer.isEmpty)
        {
            lastTickInput = _inputBuffer.Pop();
            result        = lastTickInput.input;
            
        }
        else
        {
            result = lastTickInput.input;
        }
        
        return result;
    }

    public void Clear()
    {
        
    }
}
