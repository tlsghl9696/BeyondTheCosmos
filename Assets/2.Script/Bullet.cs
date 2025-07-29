using UnityEngine;
using System.Collections; // �ڷ�ƾ ����� ���� �ʿ��մϴ�.

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // �Ѿ� �ӵ�
    public float destroyDelay = 3f; // �Ѿ� �ڵ� �ı� �ð�

    // PlayerBulletSpawner���� ������ �÷��̾� �Ѿ��� ������ ��
    public int damageAmount = 10;
    // PlayerBulletSpawner���� ������ ��� �Ѿ� ����
    public bool isInstantKillBullet = false;

    // �� �Ѿ��� �÷��̾� �Ѿ����� �� �Ѿ����� �����ϴ� �÷���
    public bool isPlayerBullet = true;

    // �� �Ѿ��� ������ PlayerBulletSpawner ���� (������Ʈ Ǯ ��ȯ��)
    public PlayerBulletSpawner spawner;

    void OnEnable() // ������Ʈ Ǯ���� Ȱ��ȭ�� �� ȣ��˴ϴ�.
    {
        // �Ѿ��� �ٽ� Ȱ��ȭ�� ������ �ʱ� ���·� �����մϴ�.
        // ���� ���, Rigidbody2D�� �ִٸ� �ӵ��� �ʱ�ȭ�ϰų�,
        // ParticleSystem�� �ִٸ� ����� ������ �� �ֽ��ϴ�.
    }

    void OnDisable() // ������Ʈ Ǯ�� ���ư� ��Ȱ��ȭ�� �� ȣ��˴ϴ�.
    {
        // �Ѿ��� ��Ȱ��ȭ�� �� �ʿ��� ���� �۾��� �����մϴ�.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // �ӵ� �ʱ�ȭ
            rb.angularVelocity = 0f; // ȸ�� �ӵ� �ʱ�ȭ
        }
    }

    // PlayerBulletSpawner���� �� ��ũ��Ʈ ������ �����ϴ� �޼���
    public void SetSpawner(PlayerBulletSpawner playerBulletSpawner)
    {
        spawner = playerBulletSpawner;
    }

    // PlayerBulletSpawner���� �Ѿ� Ȱ��ȭ �� ȣ���ϴ� �ʱ�ȭ �޼���
    public void OnSpawned()
    {
        gameObject.SetActive(true); // �Ѿ� Ȱ��ȭ
        // �ʿ��� ��� ���⿡ �߰� �ʱ�ȭ ������ ���� �� �ֽ��ϴ�.
    }

    // PlayerBulletSpawner���� �Ѿ� ��Ȱ��ȭ �� ȣ���ϴ� ���� �޼���
    public void OnDespawned()
    {
        gameObject.SetActive(false); // �Ѿ� ��Ȱ��ȭ (Ǯ�� ���ư�)
        // �ʿ��� ��� ���⿡ �߰� ���� ������ ���� �� �ֽ��ϴ�.
    }

    void Update()
    {
        // Rigidbody2D�� ����Ѵٸ� �� �κ��� �ʿ� ���� �� �ֽ��ϴ�.
        // PlayerBulletSpawner���� �̹� �ӵ��� �����մϴ�.
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾� �Ѿ��� ��� (�±�: Bullet)
        if (isPlayerBullet)
        {
            // � �Ǵ� ������ �浹���� �� (���� 'Enemy' �±״� ������� �ʽ��ϴ�)
            if (other.CompareTag("Meteor") || other.CompareTag("Boss"))
            {
                // ���� ������Ʈ(MeteorHP �Ǵ� BossHP)�� ������ ó�� �� ���� �߰��� ����մϴ�.
                // �Ѿ��� ������Ʈ Ǯ�� ��ȯ�˴ϴ�.
                if (spawner != null)
                {
                    spawner.ReturnBulletToPool(gameObject);
                }
                else
                {
                    Destroy(gameObject); // �����ʰ� ������ �׳� �ı�
                }
            }
        }
        // �� �Ѿ��� ��� (�±�: EnemyBullet)
        else // !isPlayerBullet
        {
            if (other.CompareTag("Player"))
            {
                // PlayerHealth.cs���� ������ ó�� (EnemyBullet �±׷� ó��)
                // �Ѿ��� ������Ʈ Ǯ�� ��ȯ�˴ϴ�.
                if (spawner != null)
                {
                    spawner.ReturnBulletToPool(gameObject);
                }
                else
                {
                    Destroy(gameObject); // �����ʰ� ������ �׳� �ı�
                }
            }
        }
    }
}
