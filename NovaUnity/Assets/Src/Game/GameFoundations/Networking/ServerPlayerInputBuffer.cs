using CircularBuffer;
using UnityEngine;

public class ServerPlayerInputBuffer
{
    private RingBuffer<PlayerInputTickPair> _inputBuffer;
    
    public ServerPlayerInputBuffer(int size)
    {
        if(size <= 0)
        {
            Debug.LogError("Buffer size cannot be 0 or less :/ Do better");
            return;
        }

        _inputBuffer = new RingBuffer<PlayerInputTickPair>(size);
    }


    public void Push(SendPlayerInput inputMessage)
    {
        // Input is old, we have received newer already, throw out
        // if(inputMessage.header.sequence < lastAck)
        // {
        //     return;
        // }

        for(int i = 0; i < inputMessage.inputCount; ++i)
        {
            PlayerInputTickPair tickPair = inputMessage.inputList[i];

            if(_inputBuffer.IsEmpty)
            {
                _inputBuffer.PushFront(tickPair);
                continue;
            }

            if(tickPair.tick > _inputBuffer.Front().tick)
            {
                _inputBuffer.PushFront(tickPair);
            }   
        }
    }

    public PlayerInputTickPair Pop()
    {
        return !_inputBuffer.IsEmpty ? _inputBuffer.PopBack() : default(PlayerInputTickPair);
    }

    public bool isEmpty
    {
        get { return _inputBuffer.IsEmpty; }
    }
}
