using UnityEngine;

public interface IMovable
{
    Vector3 CurrentPosition { get; }
    Vector3 CurrentVelocity { get; }

    void MoveTo(Vector3 position);   
    void StopMovement();              
    bool IsMoving();                
}
