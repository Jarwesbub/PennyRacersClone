using UnityEngine;

public class VSyncScript : MonoBehaviour
{
    public int vSyncValue; //0-4
    public int maxFrameRate;

    void Start()
    {
        // Sync framerate to monitors refresh rate
        QualitySettings.vSyncCount = vSyncValue;
        Application.targetFrameRate = maxFrameRate;
    }
}
