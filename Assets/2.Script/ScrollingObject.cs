using UnityEngine;

public class ScrollingObject : MonoBehaviour
{
    public float speed = 10f; // 스크롤 속도

    public void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime); // 왼쪽으로 이동
    }
}
