using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AmmopackSpawner : NetworkBehaviour
{
    [SerializeField] GameObject ammopackPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            int xPosition = Random.Range(-4, 4);
            int yPosition = Random.Range(-2, 2);

            GameObject newAmmoPack = Instantiate(ammopackPrefab, new Vector3(xPosition, yPosition, 0), Quaternion.identity);
            NetworkObject no = newAmmoPack.GetComponent<NetworkObject>();
            no.Spawn();
        }
    }
}
