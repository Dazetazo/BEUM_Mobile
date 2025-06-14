using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllers : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void Update()
    {
        // Movimiento simple
        transform.Translate(moveInput * Time.deltaTime * 5f);
    }

    private void OnEnable() => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();
}
