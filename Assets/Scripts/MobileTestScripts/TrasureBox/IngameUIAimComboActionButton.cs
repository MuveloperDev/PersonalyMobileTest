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

    // 콤보타입에도 두가지가 나뉘어야 한다.
    // 차징(티모, 다운)(업 발), 즉발(업),
    // 쿨타임 리셋타입도 필요함. (남은 개수가 찰때를 표기하던가 or offsetCoolTime을 적용하던가.)
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
            // (설치형) 만약에 콤보를 다쓴 경우 개수가 1더 추가될때까지의 남은 시간으로 셋해준다.
            // 설치형이 아니고 일반 콤보인 경우에는 설정된 쿨타임으로 셋해준다.
            _coolTIme.SetMaxCoolTimeValue(_interComboCooldown);
            await _coolTIme.Interaction();
            Initialize();
        }
    }
}
