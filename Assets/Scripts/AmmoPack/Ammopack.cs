using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ammopack : NetworkBehaviour
{
    [SerializeField] GameObject ammoPackPrefab;

    //6. Ammo Packs
    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (!playerController) return;
            if (playerController.AmmoAmount >= playerController.MaxAmmo) return;

            playerController.AmmoAmount = playerController.MaxAmmo;

            int xPosition = Random.Range(-4, 4);
            int yPosition = Random.Range(-2, 2);

            GameObject newAmmoPack = Instantiate(ammoPackPrefab, new Vector3(xPosition, yPosition, 0), Quaternion.identity);
            NetworkObject no = newAmmoPack.GetComponent<NetworkObject>();
            no.Spawn();

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();
        }
    }
}
