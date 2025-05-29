using UnityEngine;

public class Door : MonoBehaviour
{
    void Update()
    {
        // "Enemy" 태그가 붙은 모든 오브젝트 검색
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 모두 제거된 경우 (배열이 비었을 때)
        if (enemies.Length == 0)
        {
            // 이 스크립트가 붙은 오브젝트를 비활성화
            gameObject.SetActive(false);
        }
    }
}
