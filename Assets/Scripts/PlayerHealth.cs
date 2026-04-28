using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    private Slider healthBar;
    private int startHealth = 100;
    private int currentHealth;

    public static bool isDead;


    void Start()
    {
        if (!IsOwner) return;
        healthBar = GameObject.Find("Health").GetComponentInChildren<Slider>();
        currentHealth = startHealth;
        healthBar.maxValue = startHealth;
        healthBar.value = currentHealth;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            Debug.Log("The player has died!");
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBar.value = currentHealth;
    }
}
