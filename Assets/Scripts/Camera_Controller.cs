using UnityEngine;

/// <summary>
/// Controls Camera position according to the generated map and the UI position
/// Camera adapts to Landscape if map is taller than wider
/// Camera adapts its ortographic size to map size
/// </summary>
public class Camera_Controller : MonoBehaviour
{
    const float X_OFFSET = 0.2f;
    const int Z_OFFSET = -1;
    const int YPOS = 30;
    const float ORTOGRAPHICSIZE_RELATIVE_CONSTANT = 4.2f;

    public void PositionCameraToMap(Vector2 mapSize)
    {
        // Move and rotate camera according to map being landscape or portrait
        if (mapSize.x < mapSize.y)
            transform.position = new(mapSize.x / 2 + X_OFFSET, YPOS, mapSize.y / 2 + Z_OFFSET);
        else
        {
            transform.position = new(mapSize.x / 2 + Z_OFFSET, YPOS, mapSize.y / 2 + Z_OFFSET);
            RotateCameraToLandscape(mapSize);
        }

        SetCamSize_AccordingToMapSize(mapSize);
    }

    /// <summary>
    /// Rotates counter-clockwise the camera
    /// </summary>
    void RotateCameraToLandscape(Vector2 mapSize)
    {
        if (mapSize.x > mapSize.y)
            transform.Rotate(new(0, 0, 1), -90);
    }

    void SetCamSize_AccordingToMapSize(Vector2 mapSize)
    {
        float newOrtographicSize;

        if (mapSize.x > mapSize.y)
        {
            newOrtographicSize = mapSize.x / ORTOGRAPHICSIZE_RELATIVE_CONSTANT;
        }
        else
            newOrtographicSize = mapSize.y / ORTOGRAPHICSIZE_RELATIVE_CONSTANT;

        Camera cam = GetComponent<Camera>();
        cam.orthographicSize = newOrtographicSize;
    }

}
