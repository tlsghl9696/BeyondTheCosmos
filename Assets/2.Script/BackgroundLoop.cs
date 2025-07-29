using UnityEngine;

// ��� �� ���� �������� ��ũ�ѽ�Ű��, ���� �ݺ��ǵ��� ��ȯ��Ŵ
public class BackgroundScroller : MonoBehaviour
{
    public Transform[] backgrounds; // ��� 2�� (����, ������ ����)
    public float scrollSpeed = 2f;

    private float backgroundWidth;

    private void Start()
    {
        // ����� ���� �ð��� �ʺ� ���� (SpriteRenderer ����)
        backgroundWidth = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        // �� ����� �������� �̵�
        foreach (Transform bg in backgrounds)
        {
            bg.position += Vector3.left * scrollSpeed * Time.deltaTime;

            // ���� ������ �������� ���������� ���ġ
            if (bg.position.x <= -backgroundWidth)
            {
                // �ٸ� ����� �����ʿ� ���̱�
                bg.position += Vector3.right * backgroundWidth * 2f;
            }
        }
    }
}
