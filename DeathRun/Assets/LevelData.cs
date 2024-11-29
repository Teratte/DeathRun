using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] private float LimitTime;

    [SerializeField] private GameObject[] SavePoints;

    private void Start()
    {
        GameManager.Instance.SetLimitedTime(LimitTime);

    }


}
