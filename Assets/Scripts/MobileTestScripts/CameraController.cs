using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private Vector3 _offsetPos;
    [SerializeField] private Quaternion _offsetRot;

    private void Awake()
    {
        _offsetPos = new Vector3(5.61f, 6.29f, -3.74f);
        _offsetRot = new Quaternion(39.357f, -57.05f, 0,0);
    }
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(_offsetRot.x, _offsetRot.y, _offsetRot.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _target.transform.position + _offsetPos;
    }
}
