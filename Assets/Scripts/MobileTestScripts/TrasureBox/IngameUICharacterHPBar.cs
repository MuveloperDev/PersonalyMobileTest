using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUICharacterHPBar : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main; // ���� ī�޶� ã�Ƽ� �����մϴ�.
    }

    private void Update()
    {
        // UI ��ü�� �׻� ī�޶� �ٶ󺸵��� ȸ���մϴ�.
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
            _mainCamera.transform.rotation * Vector3.up);
    }
}
