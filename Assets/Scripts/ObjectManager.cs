using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using Photon.Chat;

public class ObjectManager : MonoBehaviour, IPunPrefabPool
{
    public void Destroy(GameObject gameObject)
    {
        throw new System.NotImplementedException();
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        throw new System.NotImplementedException();
    }
}
