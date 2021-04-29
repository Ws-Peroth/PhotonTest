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

        BulletActiveTrue(bullet);

        return bullet;
    }
    
    public void DestroyBullet(GameObject bullet)
    {
        BulletActiveFalse(bullet);
        bulletPool.Enqueue(bullet);
    }

    public void BulletActiveTrue(GameObject obj)
    {
        obj.SetActive(true);
        obj.GetComponent<Bullet>().ChangeBulletStatus(true);
    }

    public void BulletActiveFalse(GameObject obj)
    {
        obj.GetComponent<Bullet>().ChangeBulletStatus(false);
        obj.SetActive(false);
    }

}
