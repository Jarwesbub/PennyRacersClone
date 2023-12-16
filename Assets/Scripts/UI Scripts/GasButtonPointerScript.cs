using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;// Required when using Event data.

public class GasButtonPointerScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler// required interface when using the OnPointerDown method.
{
    private GameObject PlayerControl;

    void Awake()
    {
        PlayerControl = GameObject.Find("PlayerController");
    }

    //Do this when the mouse is clicked over the selectable object this script is attached to.
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerControl.GetComponent<CarController>().SetUIButtonVerticalValue(1);
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        PlayerControl.GetComponent<CarController>().SetUIButtonVerticalValue(0);
    }

}