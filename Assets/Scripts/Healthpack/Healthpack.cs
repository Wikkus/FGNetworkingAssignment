using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Healthpack : NetworkBehaviour
{ 
    [SerializeField] GameObject healthpackPrefab;

    //2. Health Packs
    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer)
        {
            Health health = other.GetComponent<Health>();
            if (!health) return;
            if (health.currentHealth.Value >= health.maxHealth.Value) return;

            health.Heal(10);

            int xPosition = Random.Range(-4, 4);
            int yPosition = Random.Range(-2, 2);

            GameObject newHealthPack = Instantiate(healthpackPrefab, new Vector3(xPosition, yPosition, 0), Quaternion.identity);
            NetworkObject no = newHealthPack.GetComponent<NetworkObject>();
            no.Spawn();

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();
        }
    }
}
