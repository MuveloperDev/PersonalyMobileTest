using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIStatusEffectsManager : MonoBehaviour
{
    [Header("DEPENDENCY")]
    [SerializeField] private HorizontalLayoutGroup _layoutGroup;
    [SerializeField] private GameObject _disableGroup;

    [Header("DATA INFORMATION")]
    [SerializeField] private List<StatusEffectData> _serverEffectList;
    [SerializeField] private List<IngameUIStatusEffect> _vaildEffects;
    [SerializeField] private List<IngameUIStatusEffect> _inVaildEffects;
    [SerializeField] private List<StatusEffectData> _waitingList;

    [SerializeField] private const int MAX_PRINT_STATUS_EFFECT_CNT = 10;


    private void Awake()
    {
        _serverEffectList = new List<StatusEffectData>();
        _waitingList = new List<StatusEffectData>();
        _vaildEffects = new List<IngameUIStatusEffect>();

        var list = _disableGroup.GetComponentsInChildren<IngameUIStatusEffect>(true);
        _inVaildEffects = list.ToList();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //int Id = Random.Range(1, 15);
            int Id = RandomId();
            StatusEffectData data = new StatusEffectData()
            {
                groupId = Id,
                duration = Random.Range(10, 30),
            };
            _serverEffectList.Add(data);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            _serverEffectList.RemoveAt(Random.Range(1, _serverEffectList.Count()));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            _serverEffectList.Clear();
        }
        RefreshData();
    }
    int RandomId()
    {
        HashSet<int> validGroupIds = new HashSet<int>(_serverEffectList.Select(effect => effect.groupId));
        int Id = Random.Range(1, 50);

        if (true == validGroupIds.Contains(Id))
        {
            return RandomId();
        }
        else
        {
            return Id;
        }
    }

    // ������ ������ �ݹ����� ��� ����.
    public void GetCurrentStatusEffect()
    {
        // �������� �޾ƿ´�.
        // StatusEffectList ���� �׷��� ���̵� ��ĥ���� �ִ°�?
        // ��, �׷��� ���̵� ��ü�� �� ����Ʈ�� �����ִ°� �ƴѰ�
        // StatusEffectList ������Ʈ
        RefreshData();
    }
    // _serverEffectList�� ������Ʈ �� ���� ȣ��
    void RefreshData()
    {
        HashSet<int> validGroupIds = new HashSet<int>(_vaildEffects.Select(effect => effect.GetGroupId()));
        HashSet<int> serverGroupIds = new HashSet<int>(_serverEffectList.Select(effect => effect.groupId));

        // ���� ���� �����Ϳ� ���°� ��� ����Ʈ�� �ִٸ� �ٷ� ���̵�.
        // _validEffects�� �ִ°� ���������Ϳ� ���ٸ� ����.
        // WatingList�� ���⼭ �����°� �´�.
        _waitingList.Clear();
        List<IngameUIStatusEffect> RemoveList = new();
        foreach (var effect in _vaildEffects)
        {
            // ���⸦ ��ġ�� ���� �׷���̵��� ������ ������ _vaildEffects���� ��������.
            // _vaildEffects�� ������ ���� �����Ͱ� ������ �ȵȴ�.
            if (false == serverGroupIds.Contains(effect.GetGroupId()))
            {
                // ����
                RemoveList.Add(effect);
            }
        }
        foreach (var effect in RemoveList)
        {
            effect.OnHide();
        }

        // �̹� ������� ����Ʈ
        List<StatusEffectData> DisplayList = new List<StatusEffectData>();
        // ������� �ƴ� ����Ʈ
        List<StatusEffectData> isNotDisplay = new List<StatusEffectData>();

        // �и� �۾�
        foreach (var sel in _serverEffectList)
        {
            if (false == validGroupIds.Contains(sel.groupId))
            {
                // ������� �ƴ�
                isNotDisplay.Add(sel);
            }
            else
            {
                // �����
                DisplayList.Add(sel);
            }
        }

        // �и��� �Ǿ��ٸ� ������ΰ� 10���ΰ�?
        
        // 10����� �̹� �ƽ� �̋��� Waiting�� ��������.

        if (MAX_PRINT_STATUS_EFFECT_CNT > DisplayList.Count)
        {
            // 10�� ���� �۴ٸ� 10������ ä���.

            // 10�� ���� �ʿ��� ����
            int fillCnt = 10 - DisplayList.Count;

            // ���� isNotDisplay�� fillCnt���� �۴ٸ�?
            if (fillCnt > isNotDisplay.Count)
            {
                // isNotDisplay.Count ��ŭ ä���.
                foreach (var data in isNotDisplay)
                {
                    DisplayList.Add(data);
                }
                isNotDisplay.Clear();
            }
            else
            {
                // ���� isNotDisplay�� fillCnt���� ū���
                for (int i = 0; i < fillCnt; i++)
                {
                    DisplayList.Add(isNotDisplay[i]);
                }
                for (int i = fillCnt - 1; i >= 0; i--)
                {
                    isNotDisplay.Remove(isNotDisplay[i]);
                }
            }

        }
        // WaitingList���� �и��۾��� �Ϸ�
        _waitingList = isNotDisplay;

        // ���÷��� ����Ʈ�� �����ø���Ʈ�� �����������Ƿ�
        // ���÷��� ����Ʈ�� �̹� ������� ����Ʈ�� ���ԵǾ��ٸ� type�� ���ؼ� ��ü�۾�.
        HashSet<int> displayGroupIds = new HashSet<int>(DisplayList.Select(effect => effect.groupId));
        foreach (var data in DisplayList)
        {
            if (false == validGroupIds.Contains(data.groupId))
            {
                // �űԷ� ���� �����̻�
                // �κ����� ����Ʈ���� ������ �����͸� �������ش�.
                AddNewSatatusEffect(_inVaildEffects[0], data);
                continue;
            }


            // type�� ���ؼ� ��ü�۾�.
            // �ٵ� �����ʿ��� List�� �ߺ��� �׷�ID�� ���ü��� �ִ��� Ȯ���ؾ���.
            // ��ü type�� groupID ���� ���������� üũ�ؾ���()
            foreach (var statusEffect in _vaildEffects)
            {
                // ���⼭ Ÿ�� ���� ��ü�� �� ��ü�ϰ� ����� ����� �ȴ�.
                //statusEffect.ReplaceData(data);
            }
        }
    }

    public void AddNewSatatusEffect(IngameUIStatusEffect argEffect, StatusEffectData argData)
    {
        bool result = _inVaildEffects.Remove(argEffect);
        if (false == result)
        {
            Debug.LogError("argEffect is not exist in _inVaildEffectsList");
            return;
        }

        _vaildEffects.Add(argEffect);
        argEffect.transform.SetParent(_layoutGroup.transform);
        argEffect.transform.SetAsFirstSibling();
        argEffect.endDurationCallback = MoveInValidEffectsList;
        argEffect.OnShow(argData);
    }

    // �̰� ������ duration�� �����ٸ� �ݹ�
    public void MoveInValidEffectsList(IngameUIStatusEffect argEffect)
    {
        // �׽�Ʈ �ڵ�
        for (int i = 0; i < _serverEffectList.Count(); i++)
        {
            if (_serverEffectList[i].groupId == argEffect.GetGroupId())
            {
                _serverEffectList.RemoveAt(i);
            }
        }

        // ����� ��ü�۾�
        if (0 != _waitingList.Count())
        {
            argEffect.transform.SetAsFirstSibling();
            argEffect.OnShow(_waitingList[0]);
            _waitingList.RemoveAt(0);
            return;
        }
        // WatingList�� ���ٸ�.
        bool result = _vaildEffects.Remove(argEffect);
        if (false == result)
        {
            Debug.LogError("argEffect is not exist in _inVaildEffectsList");
            return;
        }
        _inVaildEffects.Add(argEffect);
        argEffect.transform.SetParent(_disableGroup.transform);
    }
}
