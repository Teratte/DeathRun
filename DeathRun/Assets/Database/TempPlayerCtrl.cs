using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerCtrl : MonoBehaviour
{
    [SerializeField] TextMesh playerName;

    public void SetPlayerName(string _playerName)
    {
        playerName.text = _playerName;
    }
}
