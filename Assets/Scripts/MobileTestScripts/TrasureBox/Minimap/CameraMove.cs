using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCamera(Vector2 argPos)
    {
        // (transform.rotation.eulerAngles.x / 2) 카메라가 비추는 각도에 대한 보정값.
        Vector3 pos = new Vector3(argPos.x, transform.position.y,  argPos.y - (transform.rotation.eulerAngles.x / 2));
        transform.position = pos;
    }
}
