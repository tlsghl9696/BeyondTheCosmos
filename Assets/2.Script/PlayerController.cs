using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // �Ѿ� �߻� ���� �������� PlayerBulletSpawner�� �̵��߽��ϴ�.
    // public GameObject projectilePrefab;
    // public Transform firePoint;
    // public float fireRate = 0.2f;
    // private float nextFireTime = 0f;

    public float speed = 10.0f;         // �̵� �ӵ�
    public float yRange = 4f;           // ���� �̵� ���� ����

    void Update()
    {
        // �ڵ� �߻� ������ PlayerBulletSpawner���� ó���˴ϴ�.
        // if (Time.time >= nextFireTime)
        // {
        //     Fire(); // �Ѿ� ����
        //     nextFireTime = Time.time + fireRate; // ���� �߻� �ð� ����
        // }

        // ���� �̵�
        float vertical = Input.GetAxis("Vertical");
        transform.Translate(Vector3.up * vertical * Time.deltaTime * speed); // ��/�Ʒ��� �̵�

        // �̵� ����
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, -yRange, yRange); // Y�� �̵� ����
        transform.position = pos;
    }

    // �Ѿ� �߻� �Լ��� PlayerBulletSpawner�� �̵��߽��ϴ�.
    // void Fire()
    // {
    //     Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    // }
}