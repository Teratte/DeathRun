using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.tag == "Player")
        {
            LevelData levelData = LevelData.Instance;
            if (levelData != null)
            {
                levelData.SetSavePoint(this.gameObject);
            }
        }
    }
}
