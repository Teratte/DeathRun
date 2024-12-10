using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead_Obs : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerCtrl playerCtrl = other.GetComponent<PlayerCtrl>();
            if(playerCtrl.GetComponent<PhotonView>().IsMine)
            {
                playerCtrl.TakeDamage(100);

            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerCtrl playerCtrl = other.gameObject.GetComponent<PlayerCtrl>();
            if (playerCtrl.GetComponent<PhotonView>().IsMine)
            {
                playerCtrl.TakeDamage(100);

            }
        }
    }
}
