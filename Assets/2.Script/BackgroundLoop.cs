using UnityEngine;

// 배경 두 장을 왼쪽으로 스크롤시키고, 무한 반복되도록 순환시킴
public class BackgroundScroller : MonoBehaviour
{
    public Transform[] backgrounds; // 배경 2개 (왼쪽, 오른쪽 순서)
    public float scrollSpeed = 2f;

    private float backgroundWidth;

    private void Start()
    {
        // 배경의 실제 시각적 너비 측정 (SpriteRenderer 기준)
        backgroundWidth = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        // 각 배경을 왼쪽으로 이동
        foreach (Transform bg in backgrounds)
        {
            bg.position += Vector3.left * scrollSpeed * Time.deltaTime;

            // 왼쪽 끝으로 나갔으면 오른쪽으로 재배치
            if (bg.position.x <= -backgroundWidth)
            {
                // 다른 배경의 오른쪽에 붙이기
                bg.position += Vector3.right * backgroundWidth * 2f;
            }
        }
    }
}
