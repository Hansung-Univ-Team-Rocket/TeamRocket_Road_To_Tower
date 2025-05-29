/*
using UnityEngine;
using UnityEngine.UI;

public class PlayerDash : MonoBehaviour
{
    public Image dashCooldownImage;  // 채워지는 쿨타임 이미지
    public float dashCooldown = 10f; // 대쉬 쿨타임

    private float cooldownTimer = 0f;
    private bool isCooldown = false;

    void Update()
    {
        // 스페이스바 누르면 대시 시작
        if (Input.GetKeyDown(KeyCode.Space) && !isCooldown)
        {
            Dash();
        }

        // 쿨타임 진행 중이면 fillAmount 증가
        if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            // 아래에서 위로 차오르게 (0 → 1)
            dashCooldownImage.fillAmount = 1f - (cooldownTimer / dashCooldown);

            if (cooldownTimer <= 0f)
            {
                isCooldown = false;
                dashCooldownImage.fillAmount = 1f;
            }
        }
    }

    void Dash()
    {
        isCooldown = true;
        cooldownTimer = dashCooldown;

        // 시작 시 1에서 시작 (이미지 차있게) 
        dashCooldownImage.fillAmount = 1f;
    }
}
*/