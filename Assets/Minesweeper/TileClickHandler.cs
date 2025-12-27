using UnityEngine;
using UnityEngine.InputSystem;

public class TileClickHandler : MonoBehaviour
{
    private InputSystem_Actions _inputSystem;

    private void Awake()
    {
        _inputSystem = new InputSystem_Actions();
    }
    
    private void OnEnable()
    {
        _inputSystem.Enable();
        _inputSystem.Player.PrimaryAction.performed += OnPrimaryInputPerformed;
    }

    private void OnDisable()
    {
        _inputSystem.Disable();      
        _inputSystem.Player.PrimaryAction.performed -= OnPrimaryInputPerformed;
    }

    private void OnPrimaryInputPerformed(InputAction.CallbackContext context)
    {
        Vector2 screenPos = Vector2.zero;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        } 
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            screenPos = Mouse.current.position.ReadValue();
        }

        if (Camera.main == null)
        {
            Debug.LogError("No Camera main found");
            return;
        }
        
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        
        HandleClick(worldPos);
    }
    
    private void HandleClick(Vector2 worldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            TileScript tile = hit.collider.GetComponent<TileScript>();
            if (tile != null)
            {
                tile.MouseLeftClick();
            }
        }
    }
}



