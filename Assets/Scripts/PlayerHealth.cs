using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int startHealth = 100;
    [SerializeField] private int currentHealth;

    public static bool isDead;


    void Start()
    {
        if (!IsOwner) return;
        currentHealth = startHealth;
    }

    void Update()
    {
        if (!IsOwner) return;

        Debug.Log(currentHealth);

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            Debug.Log("The player has died!");
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
    }
}
