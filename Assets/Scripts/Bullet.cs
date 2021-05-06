using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PV;
    public GameObject MyPlayer;
    public int checkPalyer;
    public bool bulletStatus;
    int dir;

    public void Start()
    {
        GetComponent<SpriteRenderer>().flipX = dir < 0;
    }

    void Update()
    {
        transform.Translate(Vector3.right * NetworkManager.networkManager.bulletSpeed * Time.deltaTime * dir);
    }

    void OnTriggerEnter2D(Collider2D col) // col을 RPC의 매개변수로 넘겨줄 수 없다
    {
        if (col.CompareTag("Ground")) {
            PV.RPC(nameof(DestroyRPC), RpcTarget.AllBuffered);
        }

        if (!PV.IsMine && col.CompareTag("Player") && col.GetComponent<PhotonView>().IsMine) // 느린쪽에 맞춰서 Hit판정
        {
            if (checkPalyer != col.GetComponent<Player>().id)
            {
                Debug.Log("Hit! (id : " + checkPalyer);
                col.GetComponent<Player>().Hit();
                PV.RPC(nameof(DestroyRPC), RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC] public void DirRPC(int dir) => this.dir = dir;

    [PunRPC]
    public void DestroyRPC()
    {
        ObjectManager.objectManager.bulletPool.Enqueue(gameObject);
        bulletStatus = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(gameObject.activeSelf);
        }
        else
        {
            transform.localPosition = (Vector3)stream.ReceiveNext();
            gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }
}
