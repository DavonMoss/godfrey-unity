using UnityEngine;
using UnityEngine.InputSystem;

public abstract class GodfreyAbstractState
{
    public abstract void ReceiveInput(InputAction.CallbackContext value);
    public abstract void EnterState(GodfreyStateManager godfrey);
    public abstract void UpdateState(GodfreyStateManager godfrey);
}
