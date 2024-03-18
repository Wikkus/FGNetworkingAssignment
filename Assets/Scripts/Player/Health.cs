using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>();
    private NetworkVariable<int> amountLives = new NetworkVariable<int>();

    private PlayerController _playerController;

    public override void OnNetworkSpawn()
    {
        _playerController = GetComponent<PlayerController>();
        if (!IsServer) return;
        currentHealth.Value = 100;
        maxHealth.Value = currentHealth.Value;

        amountLives.Value = 3;
    }
    
    public void TakeDamage(int damage){
        damage = damage<0? damage:-damage;
        currentHealth.Value += damage;
        if(currentHealth.Value <= 0)
        {
            Death();
        }
    }

    public void Heal(int amount)
    {
        currentHealth.Value += amount;
        if(currentHealth.Value > maxHealth.Value) 
        {
            currentHealth.Value = maxHealth.Value;
        }
    }

    //9. Player Death
    private void Death()
    {
        gameObject.GetComponent<NetworkTransformClientAuth>();

        amountLives.Value -= 1;
        _playerController.TeleportPlayer(new Vector3(9001, 9001, 0));
        _playerController.IsDead = true;
        Invoke(nameof(Respawn), 2);

    }

    //11. Limited respawn
    private void Respawn()
    {
        if (amountLives.Value <= 0)
        {
            NetworkManager.DisconnectClient(GetComponent<NetworkObject>().OwnerClientId);
            return;
        }
        currentHealth.Value = maxHealth.Value;

        int xPosition = Random.Range(-4, 4);
        int yPosition = Random.Range(-2, 2);

        _playerController.AmmoAmount = _playerController.MaxAmmo;
        _playerController.IsDead = false;
        _playerController.TeleportPlayer(new Vector3(xPosition, yPosition, 0));
    }
}
