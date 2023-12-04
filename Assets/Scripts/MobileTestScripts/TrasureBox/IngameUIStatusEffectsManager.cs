using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIStatusEffectsManager : MonoBehaviour
{
    [Header("DEPENDENCY")]
    [SerializeField] private HorizontalLayoutGroup _layoutGroup;
    [SerializeField] private GameObject _disableGroup;
    [SerializeField] private GameObject DescriptionBox;
    [SerializeField] private IngameUIToolTipBox _toolTipBox;

    [Header("DATA INFORMATION")]
    [SerializeField] private List<StatusEffectData> _serverAllocatedStatusEffectList = new();
    [SerializeField] private List<IngameUIStatusEffect> _activeTaskStatusEffectsList = new();
    [SerializeField] private List<IngameUIStatusEffect> _inactiveTaskStatusEffectsList = new();
    [SerializeField] private List<StatusEffectData> _pendingStatusEffectList = new();

    [SerializeField] private const int MAX_DISPLAY_STATUS_EFFECT_CNT = 10;

    // ���� �߰� ��������� ������ 
    private void Awake()
    {
        var IngameUIStatusEffectArray = _disableGroup.GetComponentsInChildren<IngameUIStatusEffect>(true);
        _inactiveTaskStatusEffectsList = IngameUIStatusEffectArray.ToList();
        foreach (var statusEffect in _inactiveTaskStatusEffectsList)
        {
            statusEffect.Initialize(_toolTipBox);
            statusEffect.onDurationEnd = MoveInValidEffectsList;
        }
    }
    IngameUIStatusEffect target;
    private void Update()
    {
        // �׽�Ʈ �ڵ�
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //int Id = Random.Range(1, 15);
            int Id = RandomId();
            StatusEffectData data = new StatusEffectData()
            {
                groupId = Id,
                duration = Random.Range(10, 30),
                type = IngameUIStatusEffect.IngameUIStatusEffectType.Normal
            };
            _serverAllocatedStatusEffectList.Add(data);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            _serverAllocatedStatusEffectList.RemoveAt(Random.Range(1, _serverAllocatedStatusEffectList.Count()));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            _serverAllocatedStatusEffectList.Clear();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            // ������ �׽�Ʈ
            int Id = RandomId();
            StatusEffectData data = new StatusEffectData()
            {
                groupId = Id,
                duration = Random.Range(10, 30),
                type = IngameUIStatusEffect.IngameUIStatusEffectType.StackLimitMaxCnt
            };
            _serverAllocatedStatusEffectList.Add(data);
        }


        RefreshData();

        int RandomId()
        {
            HashSet<int> validGroupIds = new HashSet<int>(_serverAllocatedStatusEffectList.Select(effect => effect.groupId));
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
        SyncTaskToServer();

        HashSet<int> activeTaskGroupIds = new HashSet<int>(_activeTaskStatusEffectsList.Select(effect => effect.GetGroupId()));
        // �̹� ������� ����Ʈ
        List<StatusEffectData> displayedStatusEffectsList = new List<StatusEffectData>();
        // ������� �ƴ� ����Ʈ
        List<StatusEffectData> nonDisplayedStatusEffectsList = new List<StatusEffectData>();

        SeparateStatusEffectsList();

        // �и��� �Ǿ��ٸ� ������ΰ� 10���ΰ�?
        // 10����� �̹� �ƽ� �̋��� Pending�� ��������.

        if (MAX_DISPLAY_STATUS_EFFECT_CNT > displayedStatusEffectsList.Count)
        {
            AdjustOutputListToMaxDisplayCnt();
        }

        // WaitingList���� �и��۾��� �Ϸ�
        _pendingStatusEffectList = nonDisplayedStatusEffectsList;

        // ���÷��� ����Ʈ�� �����ø���Ʈ�� �����������Ƿ�
        // ���÷��� ����Ʈ�� �̹� ������� ����Ʈ�� ���ԵǾ��ٸ� type�� ���ؼ� ��ü�۾�.
        HashSet<int> displayGroupIds = new HashSet<int>(displayedStatusEffectsList.Select(effect => effect.groupId));
        foreach (var data in displayedStatusEffectsList)
        {
            if (false == activeTaskGroupIds.Contains(data.groupId))
            {
                // �űԷ� ���� �����̻�
                // inactiveTask ����Ʈ���� ������ �����͸� �������ش�.
                AddNewSatatusEffect(_inactiveTaskStatusEffectsList[0], data);
                continue;
            }

            // type�� ���ؼ� ��ü�۾�.
            // �ٵ� �����ʿ��� List�� �ߺ��� �׷�ID�� ���ü��� �ִ��� Ȯ���ؾ���.
            // ��ü type�� groupID ���� ���������� üũ�ؾ���()
            foreach (var statusEffect in _activeTaskStatusEffectsList)
            {
                // ���⼭ �׷� ID�� ����ũ�ϴٸ� �׷���̵� ���Ѵ�.
                // �׷���̵� �����ϴٸ� ��üŸ���� �Ǵ��Ͽ�.
                // ��ü�ؾ��Ѵٸ� ��ü�ϰ� �ƴ϶�� �����.
                //statusEffect.ReplaceData(data);
            }
        }

        void SyncTaskToServer()
        {
            HashSet<int> serveraAllocatedGroupIds = new HashSet<int>(_serverAllocatedStatusEffectList.Select(effect => effect.groupId));

            // ���� ���� �����Ϳ� ���°� ��� ����Ʈ�� �ִٸ� �ٷ� ���̵�.
            // _activeTaskStatusEffectsList�� �ִ°� ���������Ϳ� ���ٸ� ����.
            // _pendingStatusEffectList Ŭ����
            _pendingStatusEffectList.Clear();
            List<IngameUIStatusEffect> RemoveTaskInActiveList = new();
            foreach (var effect in _activeTaskStatusEffectsList)
            {
                // ���⸦ ��ġ�� ���� �׷���̵��� ������ ������_activeTaskStatusEffectsList��s���� ��������.
                // _activeTaskStatusEffectsList���� ������ ���� �����Ͱ� ������ �ȵȴ�.
                if (false == serveraAllocatedGroupIds.Contains(effect.GetGroupId()))
                {
                    // ����
                    RemoveTaskInActiveList.Add(effect);
                }
            }
            foreach (var effect in RemoveTaskInActiveList)
            {
                effect.OnHide();
            }
        }
        void SeparateStatusEffectsList()
        {
            // �и� �۾�
            foreach (var data in _serverAllocatedStatusEffectList)
            {
                if (false == activeTaskGroupIds.Contains(data.groupId))
                {
                    // ������� �ƴ�
                    nonDisplayedStatusEffectsList.Add(data);
                }
                else
                {
                    // �����
                    displayedStatusEffectsList.Add(data);
                }
            }
        }
        void AdjustOutputListToMaxDisplayCnt()
        {
            // 10�� ���� �۴ٸ� 10������ ä���.
            // 10�� ���� �ʿ��� ����
            int fillCnt = MAX_DISPLAY_STATUS_EFFECT_CNT - displayedStatusEffectsList.Count;

            // ���� isNotDisplay�� fillCnt���� �۴ٸ�?
            if (fillCnt > nonDisplayedStatusEffectsList.Count)
            {
                // isNotDisplay.Count ��ŭ ä���.
                foreach (var data in nonDisplayedStatusEffectsList)
                {
                    displayedStatusEffectsList.Add(data);
                }
                nonDisplayedStatusEffectsList.Clear();
            }
            else
            {
                // ���� isNotDisplay�� fillCnt���� ū���
                for (int i = 0; i < fillCnt; i++)
                {
                    displayedStatusEffectsList.Add(nonDisplayedStatusEffectsList[i]);
                }
                for (int i = fillCnt - 1; i >= 0; i--)
                {
                    nonDisplayedStatusEffectsList.Remove(nonDisplayedStatusEffectsList[i]);
                }
            }
        }
    }

    private void AddNewSatatusEffect(IngameUIStatusEffect argEffect, StatusEffectData argData)
    {
        bool result = _inactiveTaskStatusEffectsList.Remove(argEffect);
        if (false == result)
        {
            Debug.LogError("argEffect is not exist in _inVaildEffectsList");
            return;
        }
        if (argData.type == IngameUIStatusEffect.IngameUIStatusEffectType.StackLimitMaxCnt)
        {
            target = argEffect;
        }
        _activeTaskStatusEffectsList.Add(argEffect);
        argEffect.transform.SetParent(_layoutGroup.transform);
        argEffect.transform.SetAsFirstSibling();
        argEffect.OnShow(argData);
    }

    // ���� ������ duration�� �����ٸ� �ݹ�
    private void MoveInValidEffectsList(IngameUIStatusEffect argEffect)
    {
        // �׽�Ʈ �ڵ�
        for (int i = 0; i < _serverAllocatedStatusEffectList.Count(); i++)
        {
            if (_serverAllocatedStatusEffectList[i].groupId == argEffect.GetGroupId())
            {
                _serverAllocatedStatusEffectList.RemoveAt(i);
            }
        }

        // ����� ��ü�۾�
        if (0 != _pendingStatusEffectList.Count())
        {
            argEffect.transform.SetAsFirstSibling();
            argEffect.OnShow(_pendingStatusEffectList[0]);
            _pendingStatusEffectList.RemoveAt(0);
            return;
        }

        // WatingList�� ���ٸ�.
        bool result = _activeTaskStatusEffectsList.Remove(argEffect);
        if (false == result)
        {
            Debug.LogError("argEffect is not exist in _inVaildEffectsList");
            return;
        }
        _inactiveTaskStatusEffectsList.Add(argEffect);
        argEffect.transform.SetParent(_disableGroup.transform);
    }
}
