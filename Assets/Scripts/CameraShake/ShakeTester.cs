using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class ShakeTester : MonoBehaviour
{
    [SerializeField] private ShakeData _data;

    // 인덱스로 사용하게 설정.

    // 시작할때 데이터가 중복된게 있는지 체크하는 기능 추가할것.
    // 
    void Start()
    {
        CameraShake(_data);
    }

    // 이런식으로 Api 빼주면 될듯?
    public void CameraShake(ShakeData argData)
    {
        CameraShakerHandler.Shake(argData);
    }

}
