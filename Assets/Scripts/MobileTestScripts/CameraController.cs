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
        _offsetPos = new Vector3(0, 2.7f, -4.3f);
        _offsetRot = new Quaternion(22.53f, 0, 0,0);
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
