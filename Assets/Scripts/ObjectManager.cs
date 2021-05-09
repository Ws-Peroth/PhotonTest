using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using Photon.Chat;

public class ObjectManager : MonoBehaviour
{
    public PhotonView objMgrPhotoneView;
    public static ObjectManager objectManager = null;
    public Queue<GameObject> bulletPool = new Queue<GameObject>();

    private void Start()
    {
        if(objectManager == null)
        {
            objectManager = this;
        }
    }

    public GameObject InstantiateBullet(GameObject player)
    {
        GameObject bullet = null;

        if (bulletPool.Count == 0)
        {
            bullet = PhotonNetwork.Instantiate(
                nameof(Bullet),
                player.transform.localPosition + new Vector3(0.4f, -0.11f, 0),
                Quaternion.identity);
        }
        else
        {
            bullet = bulletPool.Dequeue();
            if(bullet == null)
            {
                Destroy(bullet);

                bullet = PhotonNetwork.Instantiate(
                nameof(Bullet),
                transform.position + new Vector3(0.4f, -0.11f, 0),
                Quaternion.identity);
            }
        }
        BulletInitialization(bullet, player);
        return bullet;
    }

    public void BulletInitialization(GameObject obj, GameObject player)
    {
        bool isFlipX = player.GetComponent<SpriteRenderer>().flipX;
        obj.SetActive(true);

        obj.GetComponent<SpriteRenderer>().flipX = isFlipX;
        obj.transform.localPosition = player.transform.localPosition;
        obj.transform.position = player.transform.position + new Vector3(isFlipX ? -0.4f : 0.4f, -0.11f, 0);
        obj.transform.rotation = Quaternion.identity;
        obj.GetComponent<Bullet>().checkPalyer = player.GetComponent<Player>().id;
        obj.GetComponent<PhotonView>().RPC(nameof(Bullet.DirRPC), RpcTarget.All, isFlipX ? -1 : 1);

        // obj.GetComponent<Bullet>().bulletStatus = true;
    }
}
