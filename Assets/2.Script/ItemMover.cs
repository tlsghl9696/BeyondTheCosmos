using UnityEngine;

public class ItemMover : MonoBehaviour
{
    public float scrollSpeed = 2f; // �������� �̵��� �ӵ� (��� ��ũ�Ѱ� ������ ��)

    private void Update()
    {
        // ���� �������� ���� �ӵ��� �̵�
        transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);
    }
}
