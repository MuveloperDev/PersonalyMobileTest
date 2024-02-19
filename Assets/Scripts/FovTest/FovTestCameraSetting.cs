using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovTestCameraSetting : MonoBehaviour
{
    [SerializeField] private float _fov;
    [SerializeField] private float _distance;
    [SerializeField] private float _offset;
    [SerializeField] private float _yaw;
    [SerializeField] private float _pitch;

    [SerializeField] Vector3 targetPos;
    GameObject go;
    // Start is called before the first frame update
    void Start()
    {
        _fov = 27;
        _distance = 17.5f;
        _offset = 1.3f;
        _yaw = 180;
        _pitch = -46;
        targetPos = new Vector3(3,0.0199f,5);

        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.SetParent(transform);
        go.transform.localPosition = new Vector3(0, 0, 17.5f);
        go.transform.localEulerAngles = new Vector3(135, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = CalculateCameraTransform();
        Vector3 vCamDir = ((targetPos + Vector3.up * _offset) - transform.position).normalized;
        transform.forward = vCamDir;

        Camera cam = Camera.main;

        //카메라와 큐브의 y 위치 사이의 거리를 계산합니다.
        float verticalDistance = cam.transform.position.y;

        // 카메라의 기울기가 주어졌을 때, 기울어진 카메라와 큐브 사이의 실제 거리를 계산합니다.
        float actualDistance = verticalDistance / Mathf.Cos(cam.transform.rotation.eulerAngles.x * Mathf.Deg2Rad);

        // 카메라 프러스텀의 높이를 계산합니다.
        float frustumHeight = 2.0f * actualDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        // 카메라 프러스텀의 너비를 계산합니다.
        float frustumWidth = frustumHeight * cam.aspect;

        float seta = cam.fieldOfView / 2;
        var a = Mathf.Tan(seta) * frustumHeight;
        a = a * a;

        var c = a + (frustumHeight * frustumHeight);
        c = Mathf.Sqrt(c);

        go.transform.localScale = new Vector3(frustumWidth, 1, c);
    }

    private Vector3 CalculateCameraTransform()
    {
        // 카메라 방향 결정.
        Quaternion axisAngle = Quaternion.Euler(_pitch, _yaw, 0.0f);
        Vector3 cookieViewDir = axisAngle * Vector3.forward;

        Vector3 vCamPosExpect = targetPos + cookieViewDir * _distance;
        //transform.position = vCamPosExpect;

        return vCamPosExpect;
    }

    async UniTask CheckFrustomField()
    {
        await UniTask.WaitForSeconds(3);


        Camera cam = Camera.main;


        while (true)
        {
            await UniTask.Yield();

            //float distance = cam.transform.position.y / Mathf.Sin(cam.transform.rotation.eulerAngles.x * Mathf.Deg2Rad);

            //float frustumHeight = 2.0f * (distance) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

            //float frustumWidth = frustumHeight * cam.aspect;

            //Debug.Log("frustumHeight: " + frustumHeight);
            //Debug.Log("frustumWidth: " + frustumWidth);

            //카메라와 큐브의 y 위치 사이의 거리를 계산합니다.
            float verticalDistance = cam.transform.position.y;

            // 카메라의 기울기가 주어졌을 때, 기울어진 카메라와 큐브 사이의 실제 거리를 계산합니다.
            float actualDistance = verticalDistance / Mathf.Cos(cam.transform.rotation.eulerAngles.x * Mathf.Deg2Rad);

            // 카메라 프러스텀의 높이를 계산합니다.
            float frustumHeight = 2.0f * actualDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

            // 카메라 프러스텀의 너비를 계산합니다.
            float frustumWidth = frustumHeight * cam.aspect;

            //float tiltRad = cam.transform.eulerAngles.x * Mathf.Deg2Rad;

            //// 카메라의 y 위치와 맵의 y 위치 차이를 계산합니다.
            //float verticalDistance = cam.transform.position.y - 0; // 맵의 y 위치가 0이므로

            //// 카메라가 비추는 맵의 중간지점까지의 거리를 계산합니다.
            //float midPointDistance = verticalDistance / Mathf.Cos(tiltRad);

            //// 뷰 프러스텀의 높이와 너비를 계산합니다.
            //float frustumHeight = 2.0f * midPointDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            //float frustumWidth = frustumHeight * cam.aspect;
            //Vector3 midPoint = cam.transform.position - cam.transform.forward * midPointDistance;

            float seta = cam.fieldOfView / 2;
            var a = Mathf.Tan(seta) * frustumHeight;
            a = a * a;

            var c = a + (frustumHeight * frustumHeight);
            c = Mathf.Sqrt(c);

            if (!float.IsNaN(frustumWidth) && !float.IsNaN(frustumHeight))
            {
                //go.transform.position = new Vector3(midPoint.x, 0, midPoint.z);
                //go.transform.localScale = new Vector3(frustumWidth, 1, c);

                //float near = cam.nearClipPlane;
                //float far = cam.farClipPlane;
                //float fov = cam.fieldOfView;
                //float aspect = cam.aspect;

                //// 카메라와 xz 평면 사이의 거리를 계산
                ////float distance = cam.transform.position.y / Mathf.Sin(cam.transform.rotation.eulerAngles.x * Mathf.Deg2Rad);

                //// Cube의 z 크기를 계산
                //float zSize = distance * 2;

                //// Cube의 y 크기를 계산
                //float ySize = cam.transform.position.y * 2;

                //// Cube의 x 크기를 계산
                //float xSize = ySize * aspect;

                //// Cube의 크기를 설정
                //go.transform.localScale = new Vector3(xSize, ySize, zSize);

                //// Cube의 위치를 설정
                //go.transform.localPosition = new Vector3(0, 0, (far + near) * 0.5f);
                return;
            }
        }
    }
}
