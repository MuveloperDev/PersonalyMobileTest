using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IngameUIAimComboActionButton
{
    
    [SerializeField] private int _maxComboCnt;
    [SerializeField] private int _curComboCnt;
    [SerializeField] private int _comboStepCooldown;
    [SerializeField] private int _interComboCooldown;
    [SerializeField] private IngameUICoolTime _coolTIme;

    public IngameUIAimComboActionButton(IngameUICoolTime argCoolTIme, int argMaxComboCnt, int argComboStepCoolTime, int argInterComboCooldown)
    {
        _coolTIme = argCoolTIme;
        _maxComboCnt = argMaxComboCnt;
        _comboStepCooldown = argComboStepCoolTime;
        _interComboCooldown = argInterComboCooldown;
        Initialize();
    }

    public void Initialize()
    {
        _curComboCnt = _maxComboCnt;
        _coolTIme.SetMaxCoolTimeValue(_comboStepCooldown);
    }

    public void Reset()
    {
        _coolTIme.Reset();
        Initialize();
    }

    public void RefreshData(int argMaxComboCnt, int argComboStepCoolTime, int argInterComboCooldown)
    {
        _maxComboCnt = argMaxComboCnt;
        _comboStepCooldown = argComboStepCoolTime;
        _interComboCooldown = argInterComboCooldown;
        _coolTIme.RefreshData(_comboStepCooldown);
    }

#pragma warning disable CS1998
    public async UniTask DownProcessCombo()
    {
    }

    // �޺�Ÿ�Կ��� �ΰ����� ������� �Ѵ�.
    // ��¡(Ƽ��, �ٿ�)(�� ��), ���(��),
    // ��Ÿ�� ����Ÿ�Ե� �ʿ���. (���� ������ ������ ǥ���ϴ��� or offsetCoolTime�� �����ϴ���.)
    public async UniTask UpProcessCombo()
    {
        NextCombo();
    }

    private async void NextCombo()
    {
        if (0 != _curComboCnt - 1)
        {
            await _coolTIme.Interaction();
            Debug.Log($"_curComboCnt : {_curComboCnt}");
            --_curComboCnt;
        }
        else
        {
            // (��ġ��) ���࿡ �޺��� �پ� ��� ������ 1�� �߰��ɶ������� ���� �ð����� �����ش�.
            // ��ġ���� �ƴϰ� �Ϲ� �޺��� ��쿡�� ������ ��Ÿ������ �����ش�.
            _coolTIme.SetMaxCoolTimeValue(_interComboCooldown);
            await _coolTIme.Interaction();
            Initialize();
        }
    }
}
