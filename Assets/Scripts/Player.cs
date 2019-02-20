using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public State state;
    public ControlSchemeOverride controlSchemeOverride;
    public ControlScheme controlScheme;
    Rigidbody2D rb;

    [Header("Stopped")]
    public float crossHillSkiSpeedMax;
    [Range(0, 1)]
    public float crossHillSkiVelocityGain;
    [Range(0, 1)]
    public float crossHillSkiVelocityDecayMultiplier;
    public AnimationCurve crossHillSkiCurve;
    private float crossHillSkiVelocity;

    [Header("Skiing")]
    public int direction = 1;
    public float downhillMaxSpeed;
    public AnimationCurve downhillCurve;
    public AnimationCurve diagonalCurve;
    public float diagonalMax;
    [Range(0,1)]
    public float crosshillMultiplier;
    [Range(0, 1)]
    public float downhillMaxDiagonal;
    public float diagonalMaxSpeed;
    [Range(0, 1)]
    public float downhillMaxCross;
    [Range(0, 1)]
    public float downhillVelocityGain;
    private Vector2 downhillSkiVelocity;
    [Range(0, 1)]
    public float stoppingPercent;
    [Range(0,0.1f)]
    public float stopThreshold;

    // Start is called before the first frame update
    void Start()
    {
        switch(controlSchemeOverride)
        {
            case ControlSchemeOverride.ARROWS:
                controlScheme = ControlScheme._ArrowKeys;
                break;
            case ControlSchemeOverride.WASD:
                controlScheme = ControlScheme._WASD;
                break;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case State.STOPPED:
                Stopped();
                break;
            case State.SKIING:
                Skiing();
                break;
            case State.FALLEN:
                Fallen();
                break;
        }
    }

    void Stopped()
    {
        //the position to move to after all the math
        Vector2 goalPos = transform.position;

        //checks key inputs and then sets the base velocity for the curve
        if(Input.GetKey(controlScheme.right) && !Input.GetKey(controlScheme.left))
        {
            crossHillSkiVelocity += crossHillSkiVelocityGain * Time.deltaTime;
        }
        else if(Input.GetKey(controlScheme.left) && !Input.GetKey(controlScheme.right))
        {
            crossHillSkiVelocity -= crossHillSkiVelocityGain * Time.deltaTime;
        }
        else
        {
            //decays speed when no button is pushed
            if(crossHillSkiVelocity > 0)
            {
                crossHillSkiVelocity -= crossHillSkiVelocityGain * crossHillSkiVelocityDecayMultiplier * Time.deltaTime;
            }
            else if(crossHillSkiVelocity < 0)
            {
                crossHillSkiVelocity += crossHillSkiVelocityGain * crossHillSkiVelocityDecayMultiplier * Time.deltaTime;
            }
            else
            {
                crossHillSkiVelocity = 0;
            }
        }

        //Calculates goal position based on the curve and velocity
        if(crossHillSkiVelocity > Mathf.Epsilon)
        {
            goalPos += new Vector2(crossHillSkiCurve.Evaluate(crossHillSkiVelocity) * crossHillSkiSpeedMax, 0);
        }
        else if(crossHillSkiVelocity < Mathf.Epsilon)
        {
            goalPos -= new Vector2(crossHillSkiCurve.Evaluate(-crossHillSkiVelocity) * crossHillSkiSpeedMax, 0);
        }
        //Moves
        rb.MovePosition(goalPos);

        if(GameManager.instance.mayBegin)
        {
            if(Input.GetKey(controlScheme.down))
            {
                state = State.SKIING;
            }
        }
    }
    void Skiing()
    {
        Vector2 goalPos = transform.position;

        if(Input.GetKey(controlScheme.up))
        {
            downhillSkiVelocity *= stoppingPercent;
        }
        //Going straight down
        else if(Input.GetKey(controlScheme.down))
        {
            if(downhillSkiVelocity.x > 0)
            {
                downhillSkiVelocity += new Vector2(-downhillVelocityGain * crosshillMultiplier * Time.deltaTime, downhillVelocityGain * Time.deltaTime);
            }
            else if(downhillSkiVelocity.x < 0)
            {
                downhillSkiVelocity += new Vector2(downhillVelocityGain * crosshillMultiplier * Time.deltaTime, downhillVelocityGain * Time.deltaTime);
            }
            else
            {
                downhillSkiVelocity += new Vector2(0, downhillVelocityGain * Time.deltaTime);
            }
        }
        //Going hard either direction
        else if(Input.GetKey(controlScheme.right) && !Input.GetKey(controlScheme.left))
        {
            direction = 1;
            if(downhillSkiVelocity.y > downhillMaxCross)
            {
                downhillSkiVelocity = new Vector2(downhillSkiVelocity.x, downhillMaxCross);
            }
            if(downhillSkiVelocity.x < 1)
            {
                downhillSkiVelocity += new Vector2(crosshillMultiplier * downhillVelocityGain * Time.deltaTime, 0);
            }
        }
        else if(Input.GetKey(controlScheme.left) && !Input.GetKey(controlScheme.right))
        {
            direction = -1;
            if(downhillSkiVelocity.y > downhillMaxCross)
            {
                downhillSkiVelocity = new Vector2(downhillSkiVelocity.x, downhillMaxCross);
            }
            if(downhillSkiVelocity.x > -1)
            {
                downhillSkiVelocity -= new Vector2(crosshillMultiplier * downhillVelocityGain * Time.deltaTime, 0);
            }
        }
        else
        {
            if (direction == 1)
            {
                if(downhillSkiVelocity.y > downhillMaxDiagonal)
                {
                    downhillSkiVelocity = new Vector2(downhillSkiVelocity.x, downhillMaxDiagonal);
                }
                if(downhillSkiVelocity.x < diagonalMaxSpeed)
                {
                    downhillSkiVelocity += new Vector2(crosshillMultiplier * downhillVelocityGain * Time.deltaTime, 0);
                }
            }
            else if(direction == -1)
            {
                if (downhillSkiVelocity.y > downhillMaxDiagonal)
                {
                    downhillSkiVelocity = new Vector2(downhillSkiVelocity.x, downhillMaxDiagonal);
                }
                if(downhillSkiVelocity.x > -diagonalMaxSpeed)
                {
                    downhillSkiVelocity -= new Vector2(crosshillMultiplier * downhillVelocityGain * Time.deltaTime, 0);
                }
            }
        }

        if(downhillSkiVelocity.x > 0)
        {
            goalPos += new Vector2(diagonalCurve.Evaluate(downhillSkiVelocity.x) * diagonalMax, 0);
        }
        else if(downhillSkiVelocity.x < 0)
        {
            goalPos -= new Vector2(diagonalCurve.Evaluate(-downhillSkiVelocity.x) * diagonalMax, 0);
        }

        goalPos -= new Vector2(0, downhillCurve.Evaluate(downhillSkiVelocity.y) * downhillMaxSpeed);

        rb.MovePosition(goalPos);

        if(downhillSkiVelocity.y < stopThreshold)
        {
            state = State.STOPPED;
        }
    }
    void Fallen()
    {

    }

    public enum State{STOPPED, SKIING, FALLEN}
    public enum ControlSchemeOverride { ARROWS, WASD, CUSTOM };
}

[System.Serializable]
public class ControlScheme
{
    public KeyCode right, left, up, down;

    public ControlScheme(KeyCode r, KeyCode l, KeyCode u, KeyCode d)
    {
        right = r; left = l; up = u; down = d;
    }

    public static ControlScheme _ArrowKeys = new ControlScheme(KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.UpArrow, KeyCode.DownArrow);
    public static ControlScheme _WASD = new ControlScheme(KeyCode.D, KeyCode.A, KeyCode.W, KeyCode.S);
}