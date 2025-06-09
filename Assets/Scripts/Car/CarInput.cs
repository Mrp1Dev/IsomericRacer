using UnityEngine;
using UnityEngine.InputSystem;

public class CarInput : MonoBehaviour
{
    private InputAction horizontalAxis;
    private InputAction verticalAxis;
    public float HorizontalAxis { get; private set; }
    public float VerticalAxis { get; private set; }
    private PlayerInput input;

    void Update()
    {
        if (input)
        {
            HorizontalAxis = horizontalAxis.ReadValue<float>();
            VerticalAxis = verticalAxis.ReadValue<float>();
        }
    }

    public void SetPlayerInputRef(PlayerInput inputRef)
    {
        input = inputRef;
        horizontalAxis = input.actions.FindAction("HorizontalAxis");
        verticalAxis = input.actions.FindAction("VerticalAxis");
    }
}
