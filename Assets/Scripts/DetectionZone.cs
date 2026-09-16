using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    [SerializeField] private MonsterController monster;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null) monster.OnPlayerEnter(playerHealth, other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            monster.OnPlayerExit();
        }
    }
}