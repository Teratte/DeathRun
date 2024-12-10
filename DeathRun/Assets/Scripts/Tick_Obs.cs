using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tick_Obs : MonoBehaviour
{
    private HashSet<PlayerCtrl> playersInTrigger = new HashSet<PlayerCtrl>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCtrl playerCtrl = other.GetComponent<PlayerCtrl>();
            if (playerCtrl.GetComponent<PhotonView>().IsMine && !playersInTrigger.Contains(playerCtrl))
            {
                playersInTrigger.Add(playerCtrl);
                StartCoroutine(ApplyDamage(playerCtrl));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCtrl playerCtrl = other.GetComponent<PlayerCtrl>();
            if (playersInTrigger.Contains(playerCtrl))
            {
                playersInTrigger.Remove(playerCtrl);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerCtrl playerCtrl = collision.gameObject.GetComponent<PlayerCtrl>();
            if (playerCtrl.GetComponent<PhotonView>().IsMine && !playersInTrigger.Contains(playerCtrl))
            {
                playersInTrigger.Add(playerCtrl);
                StartCoroutine(ApplyDamage(playerCtrl));
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerCtrl playerCtrl = collision.gameObject.GetComponent<PlayerCtrl>();
            if (playersInTrigger.Contains(playerCtrl))
            {
                playersInTrigger.Remove(playerCtrl);
            }
        }
    }

    private IEnumerator ApplyDamage(PlayerCtrl playerCtrl)
    {
        while (playersInTrigger.Contains(playerCtrl))
        {
            playerCtrl.TakeDamage(10);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
