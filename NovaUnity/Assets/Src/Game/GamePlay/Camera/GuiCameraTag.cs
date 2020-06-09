using UnityEngine;
using UnityEngine.EventSystems;

public class GuiCameraTag : MonoBehaviour
{
    [SerializeField]
    public Camera _camera;

    [SerializeField]
    public Canvas _canvas;

    [SerializeField]
    public Canvas _dynamicCanvas;

    [SerializeField]
    public EventSystem _eventSystem;
    
    public Camera cam
    {
        get { return _camera; }
    }

    public Canvas uiCanvas
    {
        get { return _canvas; }
    }

    public Canvas dynamicCanvas
    {
        get { return _dynamicCanvas; }
    }

    public EventSystem eventSystem
    {
        get { return _eventSystem; }
    }
}
