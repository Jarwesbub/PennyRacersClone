using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjectInGame : MonoBehaviour
{
    private Renderer rend;
    public bool hideObject;
    void Awake()
    {
        if (hideObject)
        {
            rend = gameObject.GetComponent<Renderer>();
            rend.enabled = false;
        }
    }

}
