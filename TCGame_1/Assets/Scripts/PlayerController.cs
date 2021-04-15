using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputsController Inputs;
    public float Speed = 0.0f;

    public float ShootRange = 20.0f;
    public float ShootTime = 0.1f;
    public ParticleSystem[] particles;

    private float ShootTimer;


    private Transform MyTransform;

    private bool InputShooting;
    private Vector3 InputDirection;
    private Vector3 InputShootDirection;

    private bool UseGamePad = false;
    private bool ShootLeft = true;

    private void Shoot()
    {
        Vector3 origin = MyTransform.position + MyTransform.forward * 0.5f + MyTransform.right * (ShootLeft ? -0.55f : 0.55f);
        int index = ShootLeft ? 0:1;
        particles[index].Play();
        
        ShootLeft = !ShootLeft;

        Debug.DrawRay(origin, MyTransform.forward, Color.red, 0.1f);

        if(Physics.Raycast(MyTransform.position, MyTransform.forward, ShootRange))
        {
            Debug.Log("hit");
        }
    }

    private void UpdateDirection(Vector2 direction)
    {
        InputDirection = new Vector3(direction.x, 0.0f, direction.y);
    }

    private void UpdateShootDirection(Vector2 direction)
    {
        InputShootDirection = new Vector3(direction.x, 0.0f, direction.y);
    }

    private void UpdateShoot(bool next)
    {
        InputShooting = next;
    }

    private void ChangeMode()
    {
        UseGamePad = !UseGamePad;
    }

    private void Update() {

        ShootTimer += Time.deltaTime;

        if(InputDirection.sqrMagnitude > 0.0f)
        {
            MyTransform.Translate(InputDirection * Speed * Time.deltaTime, Space.World);
        }

        if(UseGamePad && InputShootDirection.sqrMagnitude > 0.0f)
        {
            MyTransform.LookAt(MyTransform.position + InputShootDirection);
        }else if(!UseGamePad)
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit);

            if(hit.collider != null)
            {
                Vector3 LookAtPos = new Vector3(hit.point.x, MyTransform.position.y, hit.point.z);
                MyTransform.LookAt(LookAtPos);
            }
        }

        if(InputShooting && ShootTimer >= ShootTime)
        {
            Shoot();
            ShootTimer = 0.0f;
        }
    }

    private void Awake() {
        MyTransform = transform;

        Inputs = new InputsController();

        Inputs.Player.Shoot.started += _ => UpdateShoot(true);
        Inputs.Player.Shoot.canceled += _ => UpdateShoot(false);
        Inputs.Player.Move.performed += ctx => UpdateDirection(ctx.ReadValue<Vector2>());
        Inputs.Player.Move.canceled += ctx => UpdateDirection(ctx.ReadValue<Vector2>());

        Inputs.Player.ShootDirection.performed += ctx => UpdateShootDirection(ctx.ReadValue<Vector2>());
        Inputs.Player.ShootDirection.canceled += ctx => UpdateShootDirection(ctx.ReadValue<Vector2>());

        Inputs.Player.ChangeMode.performed += _ => ChangeMode();
    }

    private void OnEnable() {
        Inputs.Enable();
    }

    private void OnDisable() {
        Inputs.Disable();
    }

}
