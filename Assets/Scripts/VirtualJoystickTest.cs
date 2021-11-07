using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualJoystickTest : MonoBehaviour
{
    [SerializeField] private VirtualJoystick inputSource;
    private Rigidbody rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rigid.velocity = inputSource.Direction*2.5f;
    }
}
