using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private PlayerControls controls;
    private bool IsDragging;
    private Vector3 PrevMouseScreenPos;
    private Mouse mouse;
    private Camera _camera;
    private float maxCameraX = 24f; //TODO Dependería del tamaño del mapa
    private float maxCameraY = 12f;

    public void OnEnable()
    {
        controls = new PlayerControls();
        controls.Enable();
        IsDragging = false;
        mouse = Mouse.current;
        _camera = Camera.main;
    }

    public void Update()
    {
        if (controls.UI.Drag.WasPerformedThisFrame())
        {
            Debug.Log("Drag start");
            IsDragging = true;
            PrevMouseScreenPos = mouse.position.ReadValue();
        }
        if (controls.UI.Drag.WasReleasedThisFrame() && IsDragging)
        {
            Debug.Log("Drag end");
            IsDragging = false;
        }
    }

    public void OnZoom(InputValue value)
    {
        Debug.Log("Scroll");
        float zoom = Mathf.Sign(value.Get<float>());
        float cameraSize = _camera.orthographicSize;
        cameraSize *= (1 - zoom * 0.1f);
        _camera.orthographicSize = Mathf.Clamp(cameraSize, 1f, 10f);
    }

    public void OnDrag(InputValue value)
    {
        Debug.Log("Dragging");
    }

    /* Referencias:
     *      https://github.com/chonkgames/Drag-Move-a-Camera-in-Unity-2D/blob/main/CameraDrag.cs
     *      https://www.youtube.com/watch?v=H7pjj1K91HE
     */
    public void LateUpdate()
    {
        if (IsDragging && controls.UI.Drag.IsInProgress())
        {
            Vector3 mouseScreenPosition = mouse.position.ReadValue();
            Vector3 newCameraPosition = _camera.transform.position + _camera.ScreenToWorldPoint(PrevMouseScreenPos) - _camera.ScreenToWorldPoint(mouseScreenPosition);
            newCameraPosition.x = Mathf.Clamp(newCameraPosition.x, -maxCameraX, maxCameraX);
            newCameraPosition.y = Mathf.Clamp(newCameraPosition.y, -maxCameraY, maxCameraY);
            Camera.main.transform.position = newCameraPosition;
            PrevMouseScreenPos = mouseScreenPosition;
        }
    }
}
