using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [Header("Weapon's Basic Data | If the weapon is meele, set roundsPerMinute as animation delay")]
    public bool isMeele = true;
    public bool isShotgun = false;
    public int weaponDamage = 0;
    public float weaponReroadTime = 0;
    public float roundsPerMinute;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
