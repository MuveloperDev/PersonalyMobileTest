using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTiliing : MonoBehaviour
{
    [SerializeField] private Material _originalMaterial;
    [SerializeField] private Material _material;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Image _image;
    //[SerializeField] private RawImage _image;

    [SerializeField] private const float _defalutValue = 1000;
    [SerializeField] private float originalAspectRatio;
    [SerializeField] private float _hp;
    public float tileCount = 1;
    public int x = 50;
    public int y = 50;
    // Start is called before the first frame update
    void Start()
    {
        _material = new Material(_originalMaterial);
        _image = GetComponent<Image>();
        _image.material = _material;
        _material.mainTexture = _image.mainTexture;
        _hp = 1000;
        //_material.mainTexture = _image.mate.texture;
    }

    // Update is called once per frame
    void Update()
    {
        SetHp(_hp);
        if (Input.GetKeyDown(KeyCode.F))
        {

        }
    }

    private void SetHp(float argHp)
    {
        // // 추후 여기서 Sheild Value 등등 로직 계산.
        ///var tileCnt = CalculateTileCnt(argHp);
        // //originalSize = _image.rectTransform.sizeDelta;
        // //_material.SetFloat("_TileX", tileCnt);
        ////_image.rectTransform.sizeDelta = originalSize / tileCnt;
        ///// 원래 텍스처
        float tileCnt = CalculateTileCnt(argHp);
        _material.mainTextureScale = new Vector2(tileCnt, 1f);
        //_material.texture.SetPixel(x, y, Color.black);
        //_material.mainTexture.width = x;
    }

    private float CalculateTileCnt(float argHp)
    {
        float value = argHp / _defalutValue;
        tileCount = value;
        return value;
    }
}
