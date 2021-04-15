using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public Transform Camera;
    public float Speed = 5.0f;
    public float Distance = 10.0f;

    private Transform MyTransform;

    private void Awake() {
        MyTransform = transform;
        Camera.localPosition = Vector3.forward * Distance;
    }

    private void LateUpdate() {
        MyTransform.position = Vector3.Lerp(MyTransform.position, Player.position, Speed * Time.deltaTime);
    }
}
