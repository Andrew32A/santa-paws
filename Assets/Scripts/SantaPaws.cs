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
            // destroy the last heart container GameObject
            int lastIndex = heartContainersList.Count - 1;
            Destroy(heartContainersList[lastIndex]);

            // remove it from the list so the next damage will remove the *new* last heart
            heartContainersList.RemoveAt(lastIndex);

            // take damage
            TakeDamage(1);

            // reset combo when player takes damage
            ScoreManager.Instance.ResetCombo();

            // destroy enemy (player doesn't get points)
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
