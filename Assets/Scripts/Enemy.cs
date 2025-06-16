using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public GameObject healthBarPrefab;
    private GameObject healthBarInstance;
    private EnemyHealthBar healthBarScript;

    public Transform healthBarPosition; 

    void Start()
    {
        currentHealth = maxHealth;

        // Instanciar barra de vida
        healthBarInstance = Instantiate(healthBarPrefab, FindAnyObjectByType<Canvas>().transform);
        healthBarScript = healthBarInstance.GetComponent<EnemyHealthBar>();
    }

    void Update()
    {
        // Posicionar barra de vida en pantalla encima del enemigo
        Vector3 screenPos = Camera.main.WorldToScreenPoint(healthBarPosition.position);
        healthBarInstance.transform.position = screenPos;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        float percent = (float)currentHealth / maxHealth;
        healthBarScript.SetHealthPercent(percent);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(healthBarInstance);
        Destroy(gameObject);
    }
}
