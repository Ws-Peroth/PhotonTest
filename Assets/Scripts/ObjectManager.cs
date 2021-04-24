using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using Photon.Chat;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager objectManager = null;
    public Queue<GameObject> bulletPool = new Queue<GameObject>();

    private void Start()
    {
        if(objectManager == null)
        {
            objectManager = this;
        }
    }

    public GameObject GetBullet()
    {
        GameObject bullet = null;

        if (bulletPool.Count == 0)
        {
            bullet = PhotonNetwork.Instantiate(nameof(Bullet), transform.position + new Vector3(SR.flipX ? -0.4f : 0.4f, -0.11f, 0),
                  Quaternion.identity);
        }
        else
        {
            bullet = bulletPool.Dequeue();
        }

        return bullet;
    }


}
