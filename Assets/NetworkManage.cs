using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManage : MonoBehaviourPunCallbacks
{
    public string playerName;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public void Connect() 
        => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = name;
        PhotonNetwork.JoinOrCreateRoom
            ("Room", 
            new RoomOptions { MaxPlayers = 2}, 
            null);
    }
}
