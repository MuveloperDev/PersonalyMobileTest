using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTiliing : MonoBehaviour
{
    [SerializeField] private Material _originalMaterial;
    [SerializeField] private Material _material;
    [SerializeField] private Image _image;
    //[SerializeField] private RawImage _image;

    [SerializeField] private const float _defalutValue = 1000;
    [SerializeField] private float originalAspectRatio;
    [SerializeField] private float _hp;
    public float tileCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        _material = new Material(_originalMaterial);
        _image = GetComponent<Image>();
        _image.material = _material;
        _hp = 1500;
        //_material.mainTexture = _image.mate.texture;
    }

    // Update is called once per frame
    void Update()
    {
        SetHp(_hp);
    }

    private void SetHp(float argHp)
    {
        // ���� ���⼭ Sheild Value ��� ���� ���.
        var tileCnt = CalculateTileCnt(argHp);

        //spriteRenderer.material.mainTextureScale = new Vector2(tileCnt, 1);
        //_material.mainTextureOffset = new Vector2(tileCnt, 1f);


        // mainTextureScale�� x ���� tileCnt��, y ���� 1�� ����
        _material.mainTextureScale = new Vector2(tileCnt, 1f);

        //_material.SetFloat("_TileX", tileCnt);
    }

    private float CalculateTileCnt(float argHp)
    {
        float value = argHp / _defalutValue;
        tileCount = value;
        return value;
    }
}
