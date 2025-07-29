using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float speed = 2f; // � �̵� �ӵ�
    public float destroyOffset = 1f; // ȭ�� ������ �󸶳� �� ������ �ı����� (���� ����)
    public int scoreValue = 10; // �� ��� �ִ� ���� (�ν����Ϳ��� ���� ����)

    private Camera mainCamera; // ���� ī�޶� ����
    private MeteorHP meteorHealth; // MeteorHP ������Ʈ ����


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
