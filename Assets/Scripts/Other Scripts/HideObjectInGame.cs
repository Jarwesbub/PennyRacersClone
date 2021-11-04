using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjectInGame : MonoBehaviour
{
    private Renderer rend;
    public bool HideObject;
    void Awake()
    {
        if (HideObject)
        {
            rend = gameObject.GetComponent<Renderer>();
            rend.enabled = false;
        }
    }

}
