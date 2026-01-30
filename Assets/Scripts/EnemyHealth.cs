using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void RecibirDaño(float damage)
    {
        currentHealth -= damage;

        Debug.Log("Enemigo recibió daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Morir();
        }
    }

    void Morir()
    {
        Debug.Log("Enemigo eliminado");
        Destroy(gameObject);
    }
}
