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
        HashSet<int> validGroupIds = new HashSet<int>(_vaildEffects.Select(effect => effect.GetGroupId()));
        HashSet<int> serverGroupIds = new HashSet<int>(_serverEffectList.Select(effect => effect.groupId));

        // 먼저 서버 데이터에 없는게 출력 리스트에 있다면 바로 하이드.
        // _validEffects에 있는게 서버데이터에 없다면 삭제.
        // WatingList는 여기서 날리는게 맞다.
        _waitingList.Clear();
        List<IngameUIStatusEffect> RemoveList = new();
        foreach (var effect in _vaildEffects)
        {
            // 여기를 거치면 서버 그룹아이디의 개수가 무조건 _vaildEffects보다 많아진다.
            // _vaildEffects는 서버에 없는 데이터가 있으면 안된다.
            if (false == serverGroupIds.Contains(effect.GetGroupId()))
            {
                // 삭제
                RemoveList.Add(effect);
            }
        }
        foreach (var effect in RemoveList)
        {
            effect.OnHide();
        }

        // 이미 출력중인 리스트
        List<StatusEffectData> DisplayList = new List<StatusEffectData>();
        // 출력중이 아닌 리스트
        List<StatusEffectData> isNotDisplay = new List<StatusEffectData>();

        // 분리 작업
        foreach (var sel in _serverEffectList)
        {
            if (false == validGroupIds.Contains(sel.groupId))
            {
                // 출력중이 아님
                isNotDisplay.Add(sel);
            }
            else
            {
                // 출력중
                DisplayList.Add(sel);
            }
        }

        // 분리가 되었다면 출력중인게 10개인가?
        
        // 10개라면 이미 맥스 이떄는 Waiting에 보내야함.

        if (MAX_PRINT_STATUS_EFFECT_CNT > DisplayList.Count)
        {
            // 10개 보다 작다면 10개까지 채운다.

            // 10개 까지 필요한 개수
            int fillCnt = 10 - DisplayList.Count;

            // 만약 isNotDisplay가 fillCnt보다 작다면?
            if (fillCnt > isNotDisplay.Count)
            {
                // isNotDisplay.Count 만큼 채운다.
                foreach (var data in isNotDisplay)
                {
                    DisplayList.Add(data);
                }
                isNotDisplay.Clear();
            }
            else
            {
                // 만약 isNotDisplay가 fillCnt보다 큰경우
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
        // WaitingList까지 분리작업이 완료
        _waitingList = isNotDisplay;

        // 디스플레이 리스트와 웨이팅리스트가 나뉘어졌으므로
        // 디스플레이 리스트는 이미 출력중인 리스트에 포함되었다면 type을 비교해서 교체작업.
        HashSet<int> displayGroupIds = new HashSet<int>(DisplayList.Select(effect => effect.groupId));
        foreach (var data in DisplayList)
        {
            if (false == validGroupIds.Contains(data.groupId))
            {
                // 신규로 들어온 상태이상
                // 인벨리드 리스트에서 꺼내서 데이터를 새팅해준다.
                AddNewSatatusEffect(_inVaildEffects[0], data);
                continue;
            }


            // type을 비교해서 교체작업.
            // 근데 서버쪽에서 List에 중복된 그룹ID가 들어올수도 있는지 확인해야함.
            // 교체 type은 groupID 마다 동일한지도 체크해야함()
            foreach (var statusEffect in _vaildEffects)
            {
                // 여기서 타입 비교후 교체할 건 교체하고 남길건 남기면 된다.
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

    // 이건 개별로 duration이 끝난다면 콜백
    public void MoveInValidEffectsList(IngameUIStatusEffect argEffect)
    {
        // 테스트 코드
        for (int i = 0; i < _serverEffectList.Count(); i++)
        {
            if (_serverEffectList[i].groupId == argEffect.GetGroupId())
            {
                _serverEffectList.RemoveAt(i);
            }
        }

        // 대기줄 교체작업
        if (0 != _waitingList.Count())
        {
            argEffect.transform.SetAsFirstSibling();
            argEffect.OnShow(_waitingList[0]);
            _waitingList.RemoveAt(0);
            return;
        }
        // WatingList가 없다면.
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
