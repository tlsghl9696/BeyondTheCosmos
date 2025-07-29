using UnityEngine;

public class ItemMover : MonoBehaviour
{
    public float scrollSpeed = 2f; // 왼쪽으로 이동할 속도 (배경 스크롤과 맞춰줄 것)

    private void Update()
    {
        // 왼쪽 방향으로 일정 속도로 이동
        transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);
    }
}
