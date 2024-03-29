using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Canvas : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

    private Vector2 iniPoint;
    private Vector2 endPoint;

    private Vector3 direction;
    public Vector3 Direction { get { return direction; } }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // GameObject go = EventSystem.current.currentSelectedGameObject;
        // if (go != null && go.tag != "GameController")
        // {
        //     GameObject joystick = GameObject.Find("VirtualJoystick");
        //     joystick.transform.position = new PointerEventData(EventSystem.current).position;
        // }
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        endPoint = ped.position;
        Vector2 diff = endPoint - iniPoint;
        direction = new Vector3(diff.x, 0, diff.y).normalized;
        Debug.Log($"OnDrag:d,i,e({direction},{iniPoint},{endPoint})");
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        // GameObject joystick = GameObject.Find("/Canvas/Panel/VirtualJoystick");
        // joystick.transform.position = ped.position;
        // joystick.GetComponent<VirtualJoystick>().OnPointerDown(ped);
        iniPoint = ped.position;
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        direction = Vector3.zero;
    }
}
