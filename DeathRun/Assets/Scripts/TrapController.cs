using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    // Start is called before the first frame update
    Renderer capsuleColor;

    void Start()
    {
        capsuleColor = gameObject.GetComponent<Renderer>();
    }

    public void ActivateTrap()
    {
        // ���� �۵� ����
        Debug.Log("Trap Activated!");
        // ������ �����̰ų� �ִϸ��̼��� ���
        capsuleColor.material.color = Color.red;
        // GetComponent<Animator>().SetTrigger("Activate");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
