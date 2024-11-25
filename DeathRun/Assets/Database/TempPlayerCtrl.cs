using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerCtrl : MonoBehaviourPun, IPunObservable
{
    [SerializeField] TextMesh playerName;
    private PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        pv.ObservedComponents[0] = this;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerName.text);
        }
        else
        {

            SetPlayerName((string)stream.ReceiveNext());

        }
    }

    public void SetPlayerName(string _playerName)
    {
        playerName.text = _playerName;
    }
}
