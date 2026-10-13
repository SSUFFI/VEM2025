
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectRatio : MonoBehaviour
{
    private Camera cam;

    private const float TargetAspect = 16f / 9f;

    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        UpdateViewport();
    }

    private void Update()
    {
        if (Screen.width == lastScreenWidth &&
            Screen.height == lastScreenHeight)
        {
            return;
        }

        UpdateViewport();
    }

    private void UpdateViewport()
    {
        if (Screen.width <= 0 || Screen.height <= 0)
            return;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        float screenAspect = (float)Screen.width / Screen.height;

        if (screenAspect > TargetAspect)
        {
            float width = TargetAspect / screenAspect;
            float x = (1f - width) / 2f;

            cam.rect = new Rect(x, 0f, width, 1f);
        }
        else
        {
            float height = screenAspect / TargetAspect;
            float y = (1f - height) / 2f;

            cam.rect = new Rect(0f, y, 1f, height);
        }
    }
}
