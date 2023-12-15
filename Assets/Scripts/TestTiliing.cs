using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTiliing : MonoBehaviour
{
    public Material material;
    public float tileCount = 1;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Image>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            material.mainTextureScale = new Vector2(tileCount, 1);
            tileCount += 0.1f;
        }
    }
}
