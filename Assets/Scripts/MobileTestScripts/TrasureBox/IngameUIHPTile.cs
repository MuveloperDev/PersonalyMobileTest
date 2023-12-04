using UnityEngine;
using UnityEngine.UI;

public class IngameUIHPTile : MonoBehaviour
{
    public Sprite spriteToTile; // 타일링 할 스프라이트
    public float fixedWidth; // 고정 너비
    public float HPPercentage = 1; // 체력 비율 (0 ~ 1)

    void Start()
    {
        // 스프라이트의 가로 크기를 가져옵니다.
        float spriteWidth = spriteToTile.bounds.size.x;

        // 몇 개의 타일이 필요한지 계산합니다.
        int maxTileCount = Mathf.CeilToInt(fixedWidth / spriteWidth);

        // 체력 비율에 따른 타일 수를 계산합니다.
        int tileCount = Mathf.CeilToInt(maxTileCount * HPPercentage);

        // 타일을 생성합니다.
        for (int i = 0; i < tileCount; i++)
        {
            // 새로운 게임 오브젝트를 생성합니다.
            GameObject newTile = new GameObject("Tile" + i);

            // 생성한 게임 오브젝트의 부모를 현재 게임 오브젝트로 설정합니다.
            newTile.transform.SetParent(transform);

            // 생성한 게임 오브젝트에 SpriteRenderer 컴포넌트를 추가하고, 스프라이트를 설정합니다.
            SpriteRenderer renderer = newTile.AddComponent<SpriteRenderer>();
            renderer.sprite = spriteToTile;

            // 생성한 게임 오브젝트의 위치를 설정합니다.
            newTile.transform.position = new Vector3(spriteWidth * i, 0, 0);
        }
    }
}
