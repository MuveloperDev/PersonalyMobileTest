using UnityEngine;
using UnityEngine.UI;

public class IngameUIHPTile : MonoBehaviour
{
    public Sprite spriteToTile; // Ÿ�ϸ� �� ��������Ʈ
    public float fixedWidth; // ���� �ʺ�
    public float HPPercentage = 1; // ü�� ���� (0 ~ 1)

    void Start()
    {
        // ��������Ʈ�� ���� ũ�⸦ �����ɴϴ�.
        float spriteWidth = spriteToTile.bounds.size.x;

        // �� ���� Ÿ���� �ʿ����� ����մϴ�.
        int maxTileCount = Mathf.CeilToInt(fixedWidth / spriteWidth);

        // ü�� ������ ���� Ÿ�� ���� ����մϴ�.
        int tileCount = Mathf.CeilToInt(maxTileCount * HPPercentage);

        // Ÿ���� �����մϴ�.
        for (int i = 0; i < tileCount; i++)
        {
            // ���ο� ���� ������Ʈ�� �����մϴ�.
            GameObject newTile = new GameObject("Tile" + i);

            // ������ ���� ������Ʈ�� �θ� ���� ���� ������Ʈ�� �����մϴ�.
            newTile.transform.SetParent(transform);

            // ������ ���� ������Ʈ�� SpriteRenderer ������Ʈ�� �߰��ϰ�, ��������Ʈ�� �����մϴ�.
            SpriteRenderer renderer = newTile.AddComponent<SpriteRenderer>();
            renderer.sprite = spriteToTile;

            // ������ ���� ������Ʈ�� ��ġ�� �����մϴ�.
            newTile.transform.position = new Vector3(spriteWidth * i, 0, 0);
        }
    }
}
