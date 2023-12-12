using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _focusObject;
    [SerializeField] private RectTransform _rect;
    private void Awake()
    {
        if (null != _focusObject)
        {
            _rect = GetComponent<RectTransform>();
        }   
    }
    private void Update()
    {
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
            _mainCamera.transform.rotation * Vector3.up);

        if (null != _focusObject)
        {
            _rect.localPosition = new Vector3(_focusObject.transform.localPosition.x, _focusObject.transform.localPosition.y+1, _focusObject.transform.localPosition.z +0.3f);
        }
    }
}
