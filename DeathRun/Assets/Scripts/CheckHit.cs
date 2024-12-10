using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tracer"))
        {
            Debug.Log("Hit");
            PlayerCtrl targetPlayer = other.GetComponent<PlayerCtrl>();
            if (targetPlayer != null)
            {
                Debug.Log("TakeDamage");
                targetPlayer.TakeDamage(10); // �ٸ� �÷��̾�� 10 ������
            }
        }
    }
}
