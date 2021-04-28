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

    public GameObject InstantiateBullet()
    {
        GameObject bullet = null;

        if (bulletPool.Count == 0)
        {
            bullet = PhotonNetwork.Instantiate(
                nameof(Bullet),
                transform.position + new Vector3( 0, -0.11f, 0),
                Quaternion.identity);
        }
        else
        {
            bullet = bulletPool.Dequeue();
        }

        // bullet.SetActive(true);
        objMgrPhotoneView.RPC(nameof(SetActiveStatus), RpcTarget.AllBuffered, false, bullet);

        return bullet;
    }
    
    public void DestroyBullet(GameObject bullet)
    {
        // bullet.SetActive(false);
        objMgrPhotoneView.RPC(nameof(SetActiveStatus), RpcTarget.AllBuffered, false, bullet);
        bulletPool.Enqueue(bullet);
    }

    [PunRPC] public void SetActiveStatus(bool status, GameObject obj)
    {
        obj.SetActive(status);
    }

}
