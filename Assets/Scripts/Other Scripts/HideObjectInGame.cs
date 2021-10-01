using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjectInGame : MonoBehaviour
{
    private Renderer rend;

    void Awake()
    {
        rend = gameObject.GetComponent<Renderer>();
        rend.enabled = false;
    }

}
