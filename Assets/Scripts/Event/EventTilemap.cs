using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(EventTrigger))]
public class EventTilemap : MonoBehaviour
{
    [SerializeField] readonly Tilemap _tileMap;
    private Vector3 Origin;
    private Vector3 Difference;
    private Vector3 ResetCamera;
    public float zoomMin = 5f;
    public float zoomMax = 8f;
    private Camera _camera;

    private int SizeEmptyEdge = 3;
    [SerializeField] private Tilemap tileMap;

    private void Awake()
    {
        _camera = Camera.main;

    }

    void Start()
    {
        ResetCamera = _camera.transform.position;



        EventTrigger trigger = this.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { ClickTilemap((PointerEventData)data); });
        trigger.triggers.Add(entry);

        EventTrigger.Entry startDrag = new EventTrigger.Entry();
        startDrag.eventID = EventTriggerType.BeginDrag;
        startDrag.callback.AddListener((data) => { OnStartDragTilemap((PointerEventData)data); });
        trigger.triggers.Add(startDrag);

        EventTrigger.Entry entryDrag = new EventTrigger.Entry();
        entryDrag.eventID = EventTriggerType.Drag;
        entryDrag.callback.AddListener((data) => { OnDragTilemap((PointerEventData)data); });
        trigger.triggers.Add(entryDrag);

    }

    public Bounds GetTilemapBounds()
    {
        return tileMap.localBounds;
    }
    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        Vector3 newTransformPosition = new Vector3(0, 0, -10);

        var yDelta = targetPosition.y;

        yDelta = Mathf.Clamp(yDelta, GetBottomMapBoundary(), GetTopMapBoundary());

        newTransformPosition.y = yDelta;

        var xDelta = targetPosition.x;

        xDelta = Mathf.Clamp(xDelta, GetLeftMapBoundary(), GetRightMapBoundary());

        newTransformPosition.x = xDelta;

        return newTransformPosition;
    }

    private float GetEmptySpace()
    {
        return _camera.aspect * SizeEmptyEdge;
    }
    public float GetLeftMapBoundary()
    {
        return GetTilemapBounds().min.x + GetCameraHorizontalExtent() - GetEmptySpace();
    }
    public float GetRightMapBoundary()
    {
        return GetTilemapBounds().max.x - GetCameraHorizontalExtent() + GetEmptySpace();
    }
    public float GetBottomMapBoundary()
    {
        return GetTilemapBounds().min.y + GetCameraVerticalExtent() - GetEmptySpace();
    }
    public float GetTopMapBoundary()
    {
        return GetTilemapBounds().max.y - GetCameraVerticalExtent() + GetEmptySpace();
    }
    private float GetCameraVerticalExtent()
    {

        return _camera.orthographicSize;
    }
    private float GetCameraHorizontalExtent()
    {
        return _camera.aspect * GetCameraVerticalExtent();
    }
    private void OnStartDragTilemap(PointerEventData data)
    {
        Origin = _camera.ScreenToWorldPoint(Input.mousePosition);

    }
    private void OnDragTilemap(PointerEventData data)
    {
        Difference = Origin - _camera.ScreenToWorldPoint(Input.mousePosition);

        // _camera.transform.position += Difference;

        _camera.transform.position = ClampCamera(_camera.transform.position + Difference);
    }


    public void ClickTilemap(PointerEventData data)
    {
        if (data.dragging)
        {
            //Debug.Log($"Dragging {Input.touchCount}");
            return;
        }
        else
        {
            GameManager.Instance.MapManager.ChangePath();
        }

        zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    void zoom(float increment)
    {
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - increment, zoomMin, zoomMax);
    }

    private void LateUpdate()
    {

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 0.01f);

        }
        else if (Input.mouseScrollDelta != Vector2.zero)
        {
            zoom(Input.GetAxis("Mouse ScrollWheel"));

            var newPosCamera = ClampCamera(_camera.transform.position);
            if (_camera.transform.position != newPosCamera)
            {
                _camera.transform.position = newPosCamera;
            }
        }
    }

}
