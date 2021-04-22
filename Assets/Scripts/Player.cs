using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D RB;
    public Animator AN;
    public SpriteRenderer SR;
    public PhotonView PV;
    public Text NickNameText;
    public Image HealthImage;

    bool isGround;
    Vector3 curPos;

    void Awake()
    {
        // 닉네임
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
        HealthImage.fillAmount = 1f;
    }


    void Update()
    {
        if (PV.IsMine)
        {
            // ← → 이동
            float axis = Input.GetAxisRaw("Horizontal");
            RB.velocity = new Vector2(4 * axis, RB.velocity.y);

            if (axis != 0)
            {
                AN.SetBool("walk", true);
                PV.RPC(nameof(FlipXRPC), RpcTarget.AllBuffered, axis); // 재접속시 filpX를 동기화해주기 위해서 AllBuffered
            }
            else AN.SetBool("walk", false);


            // ↑ 점프, 바닥체크
            isGround = Physics2D.OverlapCircle((Vector2)transform.position 
                + new Vector2(0, -0.5f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));

            AN.SetBool("jump", !isGround);

            if (Input.GetKeyDown(KeyCode.UpArrow) && isGround)
                PV.RPC(nameof(JumpRPC), RpcTarget.All);


            // 스페이스 총알 발사
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PhotonNetwork.Instantiate( nameof(Bullet), 
                    transform.position + new Vector3(SR.flipX ? -0.4f : 0.4f, -0.11f, 0), 
                    Quaternion.identity).GetComponent<PhotonView>().
                    RPC(nameof(Bullet.DirRPC), RpcTarget.All, SR.flipX ? -1 : 1);

                AN.SetTrigger("shot");
            }
        }
        // IsMine이 아닌 것들은 부드럽게 위치 동기화
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }


    [PunRPC] void FlipXRPC(float axis) => SR.flipX = axis == -1;

    [PunRPC] void JumpRPC()
    {
        RB.velocity = Vector2.zero;
        RB.AddForce(Vector2.up * 700);
    }

    public void Hit()
    {
        HealthImage.fillAmount -= 0.1f;
        if (HealthImage.fillAmount <= 0.0f)
        {
            GameObject panel = GameObject.Find("RespawnPanel");
            if (panel != null)
            {
                panel.gameObject.SetActive(true);
            }
            NetworkManager.networkManager.SettingCameraWhenDisconnect();
            PV.RPC(nameof(DestroyRPC), RpcTarget.AllBuffered); // AllBuffered로 해야 제대로 사라져 복제버그가 안 생긴다
        }
    }

    [PunRPC] void DestroyRPC() => Destroy(gameObject);

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(HealthImage.fillAmount);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            HealthImage.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
