﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    void Update()
    {
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);
    }
}
