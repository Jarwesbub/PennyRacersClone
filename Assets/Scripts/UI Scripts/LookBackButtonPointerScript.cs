using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;// Required when using Event data.

public class LookBackButtonPointerScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject MainCamera;

    void Awake()
    {
        if(MainCamera == null)
            MainCamera = GameObject.Find("MainCamera");
    }

    //Do this when the mouse is clicked over the selectable object this script is attached to.
    public void OnPointerDown(PointerEventData eventData)
    {
        MainCamera.GetComponent<CameraController>().ButtonLookBack();
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        MainCamera.GetComponent<CameraController>().ButtonLookBack();
    }
}
