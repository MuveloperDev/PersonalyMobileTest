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

    // 삭제 추가 리프레사로 나눌것 
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
        // 테스트 코드
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
            // 스택형 테스트
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

    // 데이터 받을때 콜백으로 사용 예정.
    public void GetCurrentStatusEffect()
    {
        // 서버에서 받아온다.
        // StatusEffectList 에서 그룹핑 아이디가 겹칠일이 있는가?
        // 즉, 그룹핑 아이디를 교체한 후 리스트를 보내주는가 아닌가

        // StatusEffectList 업데이트
        RefreshData();
    }

    // _serverEffectList가 업데이트 된 직후 호출
    void RefreshData()
    {
        SyncTaskToServer();

        HashSet<int> activeTaskGroupIds = new HashSet<int>(_activeTaskStatusEffectsList.Select(effect => effect.GetGroupId()));
        // 이미 출력중인 리스트
        List<StatusEffectData> displayedStatusEffectsList = new List<StatusEffectData>();
        // 출력중이 아닌 리스트
        List<StatusEffectData> nonDisplayedStatusEffectsList = new List<StatusEffectData>();

        SeparateStatusEffectsList();

        // 분리가 되었다면 출력중인게 10개인가?
        // 10개라면 이미 맥스 이떄는 Pending에 보내야함.

        if (MAX_DISPLAY_STATUS_EFFECT_CNT > displayedStatusEffectsList.Count)
        {
            AdjustOutputListToMaxDisplayCnt();
        }

        // WaitingList까지 분리작업이 완료
        _pendingStatusEffectList = nonDisplayedStatusEffectsList;

        // 디스플레이 리스트와 웨이팅리스트가 나뉘어졌으므로
        // 디스플레이 리스트는 이미 출력중인 리스트에 포함되었다면 type을 비교해서 교체작업.
        HashSet<int> displayGroupIds = new HashSet<int>(displayedStatusEffectsList.Select(effect => effect.groupId));
        foreach (var data in displayedStatusEffectsList)
        {
            if (false == activeTaskGroupIds.Contains(data.groupId))
            {
                // 신규로 들어온 상태이상
                // inactiveTask 리스트에서 꺼내서 데이터를 새팅해준다.
                AddNewSatatusEffect(_inactiveTaskStatusEffectsList[0], data);
                continue;
            }

            // type을 비교해서 교체작업.
            // 근데 서버쪽에서 List에 중복된 그룹ID가 들어올수도 있는지 확인해야함.
            // 교체 type은 groupID 마다 동일한지도 체크해야함()
            foreach (var statusEffect in _activeTaskStatusEffectsList)
            {
                // 여기서 그룹 ID가 유니크하다면 그룹아이디를 비교한다.
                // 그룹아이디가 동일하다면 교체타입을 판단하여.
                // 교체해야한다면 교체하고 아니라면 남긴다.
                //statusEffect.ReplaceData(data);
            }
        }

        void SyncTaskToServer()
        {
            HashSet<int> serveraAllocatedGroupIds = new HashSet<int>(_serverAllocatedStatusEffectList.Select(effect => effect.groupId));

            // 먼저 서버 데이터에 없는게 출력 리스트에 있다면 바로 하이드.
            // _activeTaskStatusEffectsList에 있는게 서버데이터에 없다면 삭제.
            // _pendingStatusEffectList 클리어
            _pendingStatusEffectList.Clear();
            List<IngameUIStatusEffect> RemoveTaskInActiveList = new();
            foreach (var effect in _activeTaskStatusEffectsList)
            {
                // 여기를 거치면 서버 그룹아이디의 개수가 무조건_activeTaskStatusEffectsList에s보다 많아진다.
                // _activeTaskStatusEffectsList에는 서버에 없는 데이터가 있으면 안된다.
                if (false == serveraAllocatedGroupIds.Contains(effect.GetGroupId()))
                {
                    // 삭제
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
            // 분리 작업
            foreach (var data in _serverAllocatedStatusEffectList)
            {
                if (false == activeTaskGroupIds.Contains(data.groupId))
                {
                    // 출력중이 아님
                    nonDisplayedStatusEffectsList.Add(data);
                }
                else
                {
                    // 출력중
                    displayedStatusEffectsList.Add(data);
                }
            }
        }
        void AdjustOutputListToMaxDisplayCnt()
        {
            // 10개 보다 작다면 10개까지 채운다.
            // 10개 까지 필요한 개수
            int fillCnt = MAX_DISPLAY_STATUS_EFFECT_CNT - displayedStatusEffectsList.Count;

            // 만약 isNotDisplay가 fillCnt보다 작다면?
            if (fillCnt > nonDisplayedStatusEffectsList.Count)
            {
                // isNotDisplay.Count 만큼 채운다.
                foreach (var data in nonDisplayedStatusEffectsList)
                {
                    displayedStatusEffectsList.Add(data);
                }
                nonDisplayedStatusEffectsList.Clear();
            }
            else
            {
                // 만약 isNotDisplay가 fillCnt보다 큰경우
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

    // 개별 상태의 duration이 끝난다면 콜백
    private void MoveInValidEffectsList(IngameUIStatusEffect argEffect)
    {
        // 테스트 코드
        for (int i = 0; i < _serverAllocatedStatusEffectList.Count(); i++)
        {
            if (_serverAllocatedStatusEffectList[i].groupId == argEffect.GetGroupId())
            {
                _serverAllocatedStatusEffectList.RemoveAt(i);
            }
        }

        // 대기줄 교체작업
        if (0 != _pendingStatusEffectList.Count())
        {
            argEffect.transform.SetAsFirstSibling();
            argEffect.OnShow(_pendingStatusEffectList[0]);
            _pendingStatusEffectList.RemoveAt(0);
            return;
        }

        // WatingList가 없다면.
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
