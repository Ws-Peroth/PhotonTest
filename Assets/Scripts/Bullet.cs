using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    int dir;

    void Start() => Destroy(gameObject, 3.5f);

    void Update() => transform.Translate(Vector3.right * 7 * Time.deltaTime * dir);

    void OnTriggerEnter2D(Collider2D col) // col을 RPC의 매개변수로 넘겨줄 수 없다
    {
        if (col.CompareTag("Ground")) {
            PV.RPC(nameof(DestroyRPC), RpcTarget.AllBuffered);
        }

        if (!PV.IsMine && col.CompareTag("Player") && col.GetComponent<PhotonView>().IsMine) // 느린쪽에 맞춰서 Hit판정
        {
            col.GetComponent<Player>().Hit();
            PV.RPC(nameof(DestroyRPC), RpcTarget.AllBuffered);
        }
    }

    [PunRPC] public void DirRPC(int dir) => this.dir = dir;

    [PunRPC] public void DestroyRPC() => Destroy(gameObject);
}
