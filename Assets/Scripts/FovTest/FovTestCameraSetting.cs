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

        //ī�޶�� ť���� y ��ġ ������ �Ÿ��� ����մϴ�.
        float verticalDistance = cam.transform.position.y;

        // ī�޶��� ���Ⱑ �־����� ��, ������ ī�޶�� ť�� ������ ���� �Ÿ��� ����մϴ�.
        float actualDistance = verticalDistance / Mathf.Cos(cam.transform.rotation.eulerAngles.x * Mathf.Deg2Rad);

        // ī�޶� ���������� ���̸� ����մϴ�.
        float frustumHeight = 2.0f * actualDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        // ī�޶� ���������� �ʺ� ����մϴ�.
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
        // ī�޶� ���� ����.
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

            //ī�޶�� ť���� y ��ġ ������ �Ÿ��� ����մϴ�.
            float verticalDistance = cam.transform.position.y;

            // ī�޶��� ���Ⱑ �־����� ��, ������ ī�޶�� ť�� ������ ���� �Ÿ��� ����մϴ�.
            float actualDistance = verticalDistance / Mathf.Cos(cam.transform.rotation.eulerAngles.x * Mathf.Deg2Rad);

            // ī�޶� ���������� ���̸� ����մϴ�.
            float frustumHeight = 2.0f * actualDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

            // ī�޶� ���������� �ʺ� ����մϴ�.
            float frustumWidth = frustumHeight * cam.aspect;

            //float tiltRad = cam.transform.eulerAngles.x * Mathf.Deg2Rad;

            //// ī�޶��� y ��ġ�� ���� y ��ġ ���̸� ����մϴ�.
            //float verticalDistance = cam.transform.position.y - 0; // ���� y ��ġ�� 0�̹Ƿ�

            //// ī�޶� ���ߴ� ���� �߰����������� �Ÿ��� ����մϴ�.
            //float midPointDistance = verticalDistance / Mathf.Cos(tiltRad);

            //// �� ���������� ���̿� �ʺ� ����մϴ�.
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

                //// ī�޶�� xz ��� ������ �Ÿ��� ���
                ////float distance = cam.transform.position.y / Mathf.Sin(cam.transform.rotation.eulerAngles.x * Mathf.Deg2Rad);

                //// Cube�� z ũ�⸦ ���
                //float zSize = distance * 2;

                //// Cube�� y ũ�⸦ ���
                //float ySize = cam.transform.position.y * 2;

                //// Cube�� x ũ�⸦ ���
                //float xSize = ySize * aspect;

                //// Cube�� ũ�⸦ ����
                //go.transform.localScale = new Vector3(xSize, ySize, zSize);

                //// Cube�� ��ġ�� ����
                //go.transform.localPosition = new Vector3(0, 0, (far + near) * 0.5f);
                return;
            }
        }
    }
}
