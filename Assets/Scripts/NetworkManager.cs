using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager networkManager = null;

    public InputField nickNameInput;
    public GameObject disconnectPanel;
    public GameObject respawnPanel;
    public GameObject panelBundle;
    public Transform mainCameraParent;


    void Awake()
    {

        if(networkManager == null)
        {
            networkManager = this;
        }

        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        panelBundle.SetActive(true);
        disconnectPanel.SetActive(true);
        respawnPanel.SetActive(false);
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = nickNameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    public override void OnJoinedRoom()
    {
        disconnectPanel.SetActive(false);
        StartCoroutine(nameof(DestroyBullet));
        Spawn();
    }


    IEnumerator DestroyBullet()
    {
        yield return null;

        foreach (GameObject GO in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            GO.GetComponent<PhotonView>().RPC(nameof(Bullet.DestroyRPC), RpcTarget.All);
        }

    }

    public void Spawn()
    {
        GameObject player = PhotonNetwork.Instantiate(nameof(Player), new Vector3(0f, 4, 0), Quaternion.identity);
        Camera.main.transform.SetParent(player.transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);
        respawnPanel.SetActive(false);
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            SettingCameraWhenDisconnect();
            PhotonNetwork.Disconnect();
        }
    }

    public void SettingCameraWhenDisconnect()
    {
        Camera.main.transform.SetParent(mainCameraParent);
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        disconnectPanel.SetActive(true);
        respawnPanel.SetActive(false);
    }
    
}
