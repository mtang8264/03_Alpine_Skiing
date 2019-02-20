﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayer : MonoBehaviour
{
    public State state;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    SpriteReference sprites;
    public bool actionPressed ;
    public float actionMultiplier;
    [Header("Directions")]
    public Direction direction;
    private Direction lastDirection = Direction.SLOPE_RIGHT;
    public Vector2 velocity;
    private Vector2 goalVelocity;
    public Vector2 hardTurn, slopeTurn, bombing;
    public float speed;
    public float stoppingValue = 1f;
    public float stopSpeed;
    [Header("Controls")]
    public KeyCode up;
    public KeyCode down, left, right, action;

    private bool flip = false;
    private float fallTime;
    private bool fall1 = false;
    public float fallDuration;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sprites = GetComponent<SpriteReference>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(action))
        {
            actionPressed = true;
        }

        switch(state)
        {
            case State.STOPPED:
                actionPressed = false;
                if(Input.GetKey(right) && !Input.GetKey(left))
                {
                    goalVelocity = Vector2.right;
                }
                else if(Input.GetKey(left) && !Input.GetKey(right))
                {
                    goalVelocity = Vector2.left;
                }
                else
                {
                    goalVelocity = Vector2.zero;
                }

                if(GameManager.instance.mayBegin && Input.GetKey(down))
                {
                    state = State.SKIING;
                }
                break;

            case State.SKIING:
                if(!Input.GetKey(up))
                {
                    stoppingValue = 1f;
                }

                if(Input.GetKey(up) && !Input.GetKey(down))
                {
                    actionPressed = false;
                    stoppingValue -= stopSpeed * Time.deltaTime;
                    if(stoppingValue <= 0)
                    {
                        stoppingValue = 1f;
                        state = State.STOPPED;
                    }
                }

                if(Input.GetKey(down) && !Input.GetKey(up))
                {
                    direction = Direction.BOMBING;
                }
                else if(Input.GetKey(right) && !Input.GetKey(left))
                {
                    direction = Direction.HARD_RIGHT;
                    lastDirection = Direction.SLOPE_RIGHT;
                }
                else if(Input.GetKey(left) && !Input.GetKey(right))
                {
                    direction = Direction.HARD_LEFT;
                    lastDirection = Direction.SLOPE_LEFT;
                }
                else if(direction == Direction.HARD_LEFT)
                {
                    direction = Direction.SLOPE_LEFT;
                }
                else if(direction == Direction.HARD_RIGHT)
                {
                    direction = Direction.SLOPE_RIGHT;
                }
                else if(direction == Direction.BOMBING)
                {
                    direction = lastDirection;
                }

                switch(direction)
                {
                    case Direction.BOMBING:
                        goalVelocity = -bombing;
                        break;
                    case Direction.HARD_LEFT:
                        goalVelocity = new Vector2(-hardTurn.x, -hardTurn.y);
                        break;
                    case Direction.HARD_RIGHT:
                        goalVelocity = new Vector2(hardTurn.x, -hardTurn.y);
                        break;
                    case Direction.SLOPE_LEFT:
                        goalVelocity = new Vector2(-slopeTurn.x, -slopeTurn.y);
                        break;
                    case Direction.SLOPE_RIGHT:
                        goalVelocity = new Vector2(slopeTurn.x, -slopeTurn.y);
                        break;
                }
                break;

            case State.FALLEN:
                if(Time.time > fallTime + fallDuration)
                {
                    state = State.SKIING;
                }
                break;
        }
        float actionMultiplication = !actionPressed ? 1 : actionMultiplier;
        velocity = Vector2.Lerp(velocity, goalVelocity, 0.1f);
        Vector2 goalPos = (Vector2)transform.position + new Vector2(velocity.x, velocity.y * stoppingValue * actionMultiplication) * Time.deltaTime * speed;
        goalPos = new Vector2(goalPos.x, goalPos.y);
        rb.MovePosition(goalPos);

        SpriteUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 9)
        {
            state = State.FALLEN;
            fallTime = Time.time;

            velocity = collision.contacts[0].normal;
            goalVelocity = Vector2.zero;

            fall1 = Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y) ? true : false;
        }
    }

    public void SpriteUpdate()
    {
        switch(state)
        {
            case State.STOPPED:
                if(Input.GetKey(right) && !Input.GetKey(left))
                {
                    spriteRenderer.sprite = sprites.GetSprite("RightCross");
                    flip = false;
                }
                else if(Input.GetKey(left) && !Input.GetKey(right))
                {
                    spriteRenderer.sprite = sprites.GetSprite("LeftCross");
                    flip = true;
                }
                else
                {
                    spriteRenderer.sprite = sprites.GetSprite("Idle");
                }
                break;

            case State.SKIING:
                flip = false;
                switch(direction)
                {
                    case Direction.BOMBING:
                        spriteRenderer.sprite = sprites.GetSprite("Down");
                        break;
                    case Direction.HARD_LEFT:
                        spriteRenderer.sprite = Input.GetKey(up) ? sprites.GetSprite("LeftCross") : sprites.GetSprite("Left");
                        break;
                    case Direction.HARD_RIGHT:
                        spriteRenderer.sprite = Input.GetKey(up) ? sprites.GetSprite("RightCross") : sprites.GetSprite("Right");
                        break;
                    case Direction.SLOPE_LEFT:
                        spriteRenderer.sprite = Input.GetKey(up) ? sprites.GetSprite("LeftCross") : sprites.GetSprite("DownLeft");
                        break;
                    case Direction.SLOPE_RIGHT:
                        spriteRenderer.sprite = Input.GetKey(up) ? sprites.GetSprite("RightCross") : sprites.GetSprite("DownRight");
                        break;
                }
                break;

            case State.FALLEN:
                switch(fall1)
                {
                    case true:
                        spriteRenderer.sprite = sprites.GetSprite("Fall1");
                        break;
                    case false:
                        spriteRenderer.sprite = sprites.GetSprite("Fall0");
                        break;
                }
                break;
        }
        transform.localScale = flip ? new Vector3(-1, 1) : new Vector3(1, 1);
    }

    public enum Direction { HARD_LEFT, SLOPE_LEFT, BOMBING, SLOPE_RIGHT, HARD_RIGHT };
    public enum State { STOPPED, SKIING, FALLEN };
}