using UnityEngine;

public class Door : MonoBehaviour
{
    void Update()
    {
        // "Enemy" �±װ� ���� ��� ������Ʈ �˻�
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // ��� ���ŵ� ��� (�迭�� ����� ��)
        if (enemies.Length == 0)
        {
            // �� ��ũ��Ʈ�� ���� ������Ʈ�� ��Ȱ��ȭ
            gameObject.SetActive(false);
        }
    }
}
