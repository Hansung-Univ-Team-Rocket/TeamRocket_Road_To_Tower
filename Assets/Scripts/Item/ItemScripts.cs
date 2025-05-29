using UnityEngine;

public class ItemScripts : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [Header("Item Poppin")]
    public float jumpForce = 5f;
    public float randSideForce = 2f;
    public float torqueForce = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetupColliders();

        if (rb != null )
        {
            Vector3 upperForce = Vector3.up * jumpForce;
            Vector3 sideForce = new Vector3(Random.Range(-randSideForce, randSideForce), 0, Random.Range(-randSideForce, randSideForce));

            rb.AddForce(upperForce + sideForce, ForceMode.Impulse); // 팡팡 튀는 Forcemode

            rb.AddTorque(Random.Range(-torqueForce, torqueForce), 
                Random.Range(-torqueForce, torqueForce), 
                Random.Range(-torqueForce, torqueForce), 
                ForceMode.Impulse); // 회전
        }
    }

    void SetupColliders()
    {
        // 기존 콜라이더
        BoxCollider forRigidBodyCollider = GetComponent<BoxCollider>();

        //트리거용
        SphereCollider triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = 0.8f;

        Debug.Log("Set Collider");
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (this.gameObject.tag == "AddMagazine")
            {
                WeaponScript[] weapons = collision.gameObject.GetComponentsInChildren<WeaponScript>(true);

                foreach (WeaponScript weapon in weapons)
                {
                    if(weapon.weaponType != WeaponScript.WEAPON_TYPE.PISTOL && weapon.weaponType != WeaponScript.WEAPON_TYPE.SWORD)
                    {
                        int minAmmo = (int)(weapon.maxBullet * 2 / 3);
                        int randAmmo = Random.Range(minAmmo, weapon.maxBullet);

                        weapon.AddAmmo(randAmmo);
                        Debug.Log($"Check MinAmmo: {minAmmo} |||| Check randAmmo: {randAmmo}");
                    }
                }
                Destroy(this.gameObject);
            }

            if (this.gameObject.tag == "Heal")
            {
                if (PlayerStatusInfo.playerHP >= PlayerStatusInfo.maxPlayerHP)
                {
                    Destroy(this.gameObject);
                    return;
                }
                PlayerStatusInfo.playerHP++;
                Destroy(this.gameObject);
            }
        }
    }
}
