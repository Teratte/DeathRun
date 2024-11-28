using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerCtrl : MonoBehaviourPun, IPunObservable
{
    [SerializeField] TextMesh playerName;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        pv.ObservedComponents[0] = this;

        playerName.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        gameObject.tag = pv.IsMine ? (string)PhotonNetwork.LocalPlayer.CustomProperties["PlayerTag"] : (string)pv.Owner.CustomProperties["PlayerTag"];
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
