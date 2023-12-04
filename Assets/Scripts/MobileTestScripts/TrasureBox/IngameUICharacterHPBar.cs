using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUICharacterHPBar : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main; // 메인 카메라를 찾아서 저장합니다.
    }

    private void Update()
    {
        // UI 객체가 항상 카메라를 바라보도록 회전합니다.
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
            _mainCamera.transform.rotation * Vector3.up);
    }
}
