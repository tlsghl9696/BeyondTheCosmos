using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float speed = 2f; // 운석 이동 속도
    public float destroyOffset = 1f; // 화면 밖으로 얼마나 더 나가야 파괴될지 (조절 가능)
    public int scoreValue = 10; // 이 운석이 주는 점수 (인스펙터에서 조절 가능)

    private Camera mainCamera; // 메인 카메라 참조
    private MeteorHP meteorHealth; // MeteorHP 컴포넌트 참조


    void Start()
    {
        mainCamera = Camera.main;
        meteorHealth = GetComponent<MeteorHP>();
        if (meteorHealth == null)
        {
            Debug.LogError("MeteorHP component not found on this Meteor GameObject!");
        }
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        if (screenPoint.x < -destroyOffset)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = other.GetComponentInParent<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }


            Destroy(gameObject);
        }
       
    }
}
