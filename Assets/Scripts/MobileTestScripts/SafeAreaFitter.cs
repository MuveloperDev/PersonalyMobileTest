using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform panel;
    private Rect lastSafeArea = new Rect(0, 0, 0, 0);
    private Vector2 lastScreenSize = Vector2.zero;
    private void Awake()
    {
        panel = GetComponent<RectTransform>();

    }
    private async void Start()
    {
        var originalResolution = new Vector2(Screen.width, Screen.height);

        await UniTask.WaitForEndOfFrame(this);
        // Temporarily change the resolution
        Screen.SetResolution(1, 1, Screen.fullScreen);
        ApplySafeArea(Screen.safeArea);
        //// Revert the resolution
        Screen.SetResolution((int)originalResolution.x, (int)originalResolution.y, Screen.fullScreen);
    }
    private void Update()
    {
        // Check if the screen size or orientation has changed
        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            Refresh();
        }
    }

    private void Refresh()
    {
        Rect safeArea = GetSafeArea();

        if (safeArea != lastSafeArea)
            ApplySafeArea(safeArea);
    }

    private Rect GetSafeArea()
    {
        return Screen.safeArea;
    }

    private void ApplySafeArea(Rect r)
    {
        lastSafeArea = r;

        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;
    }
}
