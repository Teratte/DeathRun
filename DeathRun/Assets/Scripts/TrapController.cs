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
        // 함정 작동 로직
        Debug.Log("Trap Activated!");
        // 함정을 움직이거나 애니메이션을 재생
        capsuleColor.material.color = Color.red;
        // GetComponent<Animator>().SetTrigger("Activate");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
