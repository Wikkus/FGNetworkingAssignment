using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthpackSpawner : NetworkBehaviour
{
    [SerializeField] GameObject healthpackPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            int xPosition = Random.Range(-4, 4);
            int yPosition = Random.Range(-2, 2);

            GameObject newHealthPack = Instantiate(healthpackPrefab, new Vector3(xPosition, yPosition, 0), Quaternion.identity);
            NetworkObject no = newHealthPack.GetComponent<NetworkObject>();
            no.Spawn();
        }
    }
}
