using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;         // 즉시 반응하는 체력바
    public Slider easeHealthSlider;     // 부드럽게 따라오는 체력바

    [SerializeField] Enemy_FSM fsm;
    float lerpSpeed = 0.05f;
    Transform cameraTransform;
    private int maxHealth;


    void Start()
    {
        fsm = GetComponentInParent<Enemy_FSM>();

        if(fsm == null )
        {
            Debug.LogError("Null");

            return;
        }

        cameraTransform = Camera.main.transform;
        maxHealth = fsm.hp;

        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;

        healthSlider.value = maxHealth;
        easeHealthSlider.value = maxHealth;
    }

    void Update()
    {
        if (fsm == null) return;

        // 현재 체력 비율 계산 (0~1)
        float healthPercentage = (float)fsm.hp / maxHealth;

        // 즉시 체력바 반영
        healthSlider.value = fsm.hp;

        // 부드러운 체력바 반영
        if (Mathf.Abs(easeHealthSlider.value - fsm.hp) > 0.01f)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, fsm.hp, lerpSpeed);
        }

        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     Debug.Log($"Current HP: {enemyFSM.hp}/{maxHealth}, Slider: {healthSlider.value}/{healthSlider.maxValue}");
        // }

        if (cameraTransform != null)
        {
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0);
        }
    }

    public void UpdateMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;
    }
}
