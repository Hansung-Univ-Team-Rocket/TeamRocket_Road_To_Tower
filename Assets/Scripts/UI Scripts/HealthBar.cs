using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;         // 즉시 반응하는 체력바
    public Slider easeHealthSlider;     // 부드럽게 따라오는 체력바
    public float maxHealth = 100f;
    private float currentHealth;
    private float lerpSpeed = 0.05f;

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;

        healthSlider.value = currentHealth;
        easeHealthSlider.value = currentHealth;
    }

    void Update()
    {
        // 테스트용 데미지
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(10);
        }

        // 즉시 체력바 반영
        healthSlider.value = currentHealth;

        // 부드러운 체력바 반영
        if (easeHealthSlider.value != currentHealth)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, currentHealth, lerpSpeed);
        }

        // 체력바가 항상 카메라를 보게 (선택)
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0); 
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    
    }

   
}
