using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(EventTrigger))]
public class EventTilemap : MonoBehaviour
{
    [SerializeField] Tilemap _tileMap;
    private Vector3 Origin;
    private Vector3 Difference;
    private Vector3 ResetCamera;

    public float zoomMin = 5f;
    public float zoomMax = 8f;

    //Assign this Camera in the Inspector
    public Camera m_OrthographicCamera;
    //These are the positions and dimensions of the Camera view in the Game view
    float m_ViewPositionX, m_ViewPositionY, m_ViewWidth, m_ViewHeight;

    void Start()
    {

        ResetCamera = Camera.main.transform.position;

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

    private void OnStartDragTilemap(PointerEventData data)
    {
        Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }
    private void OnDragTilemap(PointerEventData data)
    {

        Difference = Origin - Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Camera.main.transform.position += Difference;
    }


    public void ClickTilemap(PointerEventData data)
    {
        if (data.dragging)
        {
            //Debug.Log($"Dragging {Input.touchCount}");
            return;
        } else
        {
            GameManager.Instance.mapManager.ChangePath();
        }

        zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    //public void OnZoomTilemap(PointerEventData data)
    //{
    //    if (Input.touchCount == 2)
    //    {
    //        Touch touchZero = Input.GetTouch(0);
    //        Touch touchOne = Input.GetTouch(1);

    //        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
    //        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

    //        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
    //        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

    //        float difference = currentMagnitude - prevMagnitude;

    //        zoom(difference * 0.01f);

    //    }
    //    else if (Input.GetMouseButton(0))
    //    {
    //        Difference = Origin - Camera.main.ScreenToWorldPoint(Input.mousePosition);

    //        Camera.main.transform.position += Difference;

    //    }

    //    zoom(Input.GetAxis("Mouse ScrollWheel"));
    //}

    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
    }

    void Update()
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

        zoom(Input.GetAxis("Mouse ScrollWheel"));


    }

}
