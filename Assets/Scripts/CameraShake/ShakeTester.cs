using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class ShakeTester : MonoBehaviour
{
    [SerializeField] private ShakeData _data;

    // �ε����� ����ϰ� ����.

    // �����Ҷ� �����Ͱ� �ߺ��Ȱ� �ִ��� üũ�ϴ� ��� �߰��Ұ�.
    // 
    void Start()
    {
        CameraShake(_data);
    }

    // �̷������� Api ���ָ� �ɵ�?
    public void CameraShake(ShakeData argData)
    {
        CameraShakerHandler.Shake(argData);
    }

}
