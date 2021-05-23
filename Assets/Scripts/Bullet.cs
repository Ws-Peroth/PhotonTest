using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviour, IPunObservable
{
    public PhotonView PV;
    public GameObject MyPlayer;
    public int checkPlayer;
    int dir;
    Vector3 curPos = Vector3.zero;

    void Update()
    {
        // transform.Translate(Vector3.right * NetworkManager.networkManager.bulletSpeed * Time.deltaTime * dir);
        if(PV.IsMine)
            transform.Translate(Vector3.right * NetworkManager.networkManager.bulletSpeed * Time.deltaTime * dir);
        else if
            ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    void OnTriggerEnter2D(Collider2D col) // col을 RPC의 매개변수로 넘겨줄 수 없다
    {
        if (col.CompareTag("Ground")) {
            PV.RPC(nameof(CallDestroyBullet), RpcTarget.AllBuffered);
        }

        if (col.CompareTag("Player") && col.GetComponent<PhotonView>().IsMine) // 느린쪽에 맞춰서 Hit판정
        {
            if (checkPlayer != col.GetComponent<Player>().id)
            {
                print("chectPlayer : " + checkPlayer + "\nHitPlayerID : " + col.GetComponent<Player>().id);
                col.GetComponent<Player>().Hit();
                PV.RPC(nameof(CallDestroyBullet), RpcTarget.AllBuffered);
            }
        }
    }

    public void SetDir(int dir)
    {
        this.dir = dir;
    }


    [PunRPC] public void CallDestroyBullet()
    {
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameObject.transform.position);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
        }
    }
}
