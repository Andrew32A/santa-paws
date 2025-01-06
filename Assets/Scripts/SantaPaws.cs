using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantaPaws : MonoBehaviour
{
    [Header("Health")]
    public List<GameObject> heartContainersList;
    public int maxHealth = 4;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger");
        if (other.gameObject.tag == "Enemy")
        {
            // delete last heart container in list
            Destroy(heartContainersList[heartContainersList.Count - 1]);
            TakeDamage(1);

            // reset combo when player takes damage
            ScoreManager.Instance.ResetCombo();

            // destroy enemy, player doesnt get points
            Destroy(other.gameObject);
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
