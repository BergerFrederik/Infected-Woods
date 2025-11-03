using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameInput : MonoBehaviour
{

    private PlayerInput playerInput;

    public object Player { get; internal set; }

    private void Awake()
    {               
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();
        if (inputVector.sqrMagnitude > 1)
        {
            inputVector = inputVector.normalized;
        }
        return inputVector;
    }

    internal void Enable()
    {
        throw new NotImplementedException();
    }

    internal void Disable()
    {
        throw new NotImplementedException();
    }
}
