﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(DepthScaler))]
public class PlayerController : MonoBehaviour
{
    public GameObject sprite;
    public SpriteRenderer spriteRenderer;
    public Camera cam;

    public Vector2 speed = new Vector2(10, 5);
    public Vector2 acceleration = new Vector2(10, 5);

    public float highBoundCamSize = 5.5f;
    public float lowBoundCamSize = 7;

    Rigidbody2D rigid;
    DepthScaler scaler;

    bool mouseIsMostRecent = false;
    Vector3 mouseWorldPosition = Vector2.zero;

    [HideInInspector, System.NonSerialized]
    public bool inputCapturedByHUD = false;

    public float animateSpeed = 0.5f;

    Sprite sprite1;
    public Sprite sprite2;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        scaler = GetComponent<DepthScaler>();
        sprite1 = spriteRenderer.sprite;
        InvokeRepeating("toggleSprite", animateSpeed, animateSpeed);
    }

    void toggleSprite()
    {
        spriteRenderer.sprite = spriteRenderer.sprite == sprite1 ? sprite2 : sprite1;
    }

    Vector2 getInput()
    {
        Vector2 result = Vector2.zero;
        if (inputCapturedByHUD)
        {
            return result;
        }
        if (Input.anyKey)
        {
            mouseIsMostRecent = false;
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) result.y += 1;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) result.y -= 1;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) result.x -= 1;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) result.x += 1;
        }
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            mouseIsMostRecent = true;
            mouseWorldPosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        }
        if (mouseIsMostRecent)
        {
            result = mouseWorldPosition - transform.position;
            result.x /= speed.x;
            result.y /= speed.y;
            if (result.magnitude < 0.25)
            {
                result = Vector2.zero;
            }
        }
        return result.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = getInput();
        Vector2 velocity = rigid.velocity;
        velocity.x = Mathf.Lerp(velocity.x, input.x * speed.x, acceleration.x * Time.deltaTime);
        velocity.y = Mathf.Lerp(velocity.y, input.y * speed.y, acceleration.y * Time.deltaTime);
        rigid.velocity = velocity;

        float turnThreshold = input.x + acceleration.x * velocity.x / speed.x;
        if (turnThreshold > 1)
        {
            sprite.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (turnThreshold < -1)
        {
            sprite.transform.localScale = new Vector3(1, 1, 1);
        }

        cam.orthographicSize = Mathf.LerpUnclamped(lowBoundCamSize, highBoundCamSize, scaler.depthRatio);
    }
}
