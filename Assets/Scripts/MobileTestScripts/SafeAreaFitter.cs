using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform panel;
    private Rect lastSafeArea = new Rect(0, 0, 0, 0);
    private Vector2 lastScreenSize = Vector2.zero;
    private ScreenOrientation lastOrientation;

    private void Awake()
    {
        panel = GetComponent<RectTransform>();
    }
    private async void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        await UniTask.WaitForEndOfFrame(this);
        await UniTask.Delay(1000);
        ApplySafeArea(Screen.safeArea);
        lastOrientation = Screen.orientation;
        //Refresh();
    }
    private void Update()
    {
        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y || Screen.orientation != lastOrientation)
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            lastOrientation = Screen.orientation;
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
