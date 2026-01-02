using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TileClickHandler : MonoBehaviour
{
    private InputSystem_Actions _inputSystem;
    
    [SerializeField] private float cameraScopeMultiplier;
    [SerializeField] private int maxCameraSize;
    [SerializeField] private int minCameraSize;

    [SerializeField] private int cameraBoundX;
    [SerializeField] private int cameraBoundY;
    [SerializeField] private float cameraMoveThreshold;

    [SerializeField] private float flagPlaceTime;
    [SerializeField] private float currentFlagPlaceTime;

    [SerializeField] private float pinchSensitivity = 0.01f;
    
    private bool _primaryInputInProgress;
    private bool _inputCanceledByTimer;
    private Vector2 _primaryInputStartPos;
    private Vector3 _cameraStartPos;
    private float _lastPinchDistance;
    
    private Camera _camera;

    private void Awake()
    {
        _inputSystem = new InputSystem_Actions();
        _camera = Camera.main;
    }
    
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        
        _inputSystem.Enable();
        _inputSystem.Player.PrimaryAction.started += OnPrimaryInputStarted;
        _inputSystem.Player.PrimaryAction.canceled += OnPrimaryInputCanceled;
        
        _inputSystem.Player.ScrollAction.performed += OnScrollAction;
    }
    
    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        
        _inputSystem.Disable();      
        _inputSystem.Player.PrimaryAction.started -= OnPrimaryInputStarted;
        _inputSystem.Player.PrimaryAction.canceled -= OnPrimaryInputCanceled;
        
        _inputSystem.Player.ScrollAction.performed -= OnScrollAction;
    }
    
    private void OnPrimaryInputStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Primary Input Started");
        
        _primaryInputInProgress = true;
        _inputCanceledByTimer = false;
        
        currentFlagPlaceTime = flagPlaceTime;
        
        _cameraStartPos = _camera.transform.position;
        
        
        if (Touchscreen.current != null)
        {
            _primaryInputStartPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            _primaryInputStartPos = Mouse.current.position.ReadValue();
        }
    }
    
    private void OnPrimaryInputCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Primary Input Canceled");
        
        if (_inputCanceledByTimer) return;
        
        if (Vector3.Distance(_camera.transform.position, _cameraStartPos) <= cameraMoveThreshold)
        {
            OnPrimaryInputClick(false);
        }
        
        _primaryInputInProgress = false;
        _primaryInputStartPos = Vector2.zero;
    }
    
    private void OnScrollAction(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        float yValue = value.y;
        
        if (_camera == null) return;

        if (yValue < 0)
        {
            _camera.orthographicSize += cameraScopeMultiplier;
        }
        else if (yValue > 0)
        {
            _camera.orthographicSize -= cameraScopeMultiplier;
        }
        
        _camera.orthographicSize = Mathf.Clamp(
            _camera.orthographicSize, 
            minCameraSize, 
            maxCameraSize);
    }
    
    private void HandlePinchZoomMobile()
    {
        if (_camera == null) return;

        var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
        if (touches.Count < 2)
        {
            _lastPinchDistance = 0f;
            return;
        }

        // Use the first two touches
        var t0 = touches[0].screenPosition;
        var t1 = touches[1].screenPosition;

        float currentDistance = Vector2.Distance(t0, t1);

        if (_lastPinchDistance <= 0f)
        {
            _lastPinchDistance = currentDistance;
            return;
        }

        float pinchDelta = currentDistance - _lastPinchDistance; // >0 fingers moving apart, <0 moving together
        _lastPinchDistance = currentDistance;

        // Convert pixel delta to orthographic size change
        float deltaSize = -pinchDelta * pinchSensitivity; // invert so pinch-out zooms in
        _camera.orthographicSize += deltaSize;

        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minCameraSize, maxCameraSize);
    }

    private void CancelPrimaryInput()
    {
        _primaryInputInProgress = false;
        _inputCanceledByTimer = true;
        _primaryInputStartPos = Vector2.zero;
    }
    
    private void Update()
    {
        HandlePinchZoomMobile();
        
        if (!_primaryInputInProgress || _camera == null) return;
        
        currentFlagPlaceTime -= Time.deltaTime;
        if (currentFlagPlaceTime <= 0 && Vector3.Distance(_camera.transform.position, _cameraStartPos) <= cameraMoveThreshold)
        {
            CancelPrimaryInput();
            OnPrimaryInputClick(true);
            return;
        }

        Vector2 currentScreenPos = Vector2.zero;
        
        if (Touchscreen.current != null)
        {
            currentScreenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            currentScreenPos = Mouse.current.position.ReadValue();
        }
        
        Vector2 delta = _primaryInputStartPos - currentScreenPos;
        
        float pixelsPerUnit =
            Screen.height / (_camera.orthographicSize * 2f);

        Vector3 worldDelta = new Vector3(
            delta.x / pixelsPerUnit,
            delta.y / pixelsPerUnit,
            0f
        );

        _camera.transform.position = _cameraStartPos + worldDelta;
        _camera.transform.position = new Vector3(
                Mathf.Clamp(_camera.transform.position.x, -cameraBoundX, cameraBoundX),
                Mathf.Clamp(_camera.transform.position.y, -cameraBoundY, cameraBoundY),
                _camera.transform.position.z
            );
    }
    
    private void OnPrimaryInputClick(bool placeFlag)
    {
        Vector2 screenPos = Vector2.zero;

        if (Touchscreen.current != null)
        {
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        } 
        else if (Mouse.current != null)
        {
            screenPos = Mouse.current.position.ReadValue();
        }

        if (_camera == null)
        {
            Debug.LogError("No Camera main found");
            return;
        }
        
        Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);
        
        HandleClick(worldPos, placeFlag);
    }
    
    private void HandleClick(Vector2 worldPos, bool placeFlag)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            TileScript tile = hit.collider.GetComponent<TileScript>();
            if (tile != null && !placeFlag)
            {
                tile.MouseLeftClick();
            }
            else if (tile != null && placeFlag)
            {
                tile.MouseRightClick();
            }
        }
    }
}



