using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public enum SphereState
    {
        None,
        Moving,
        CallBall,
        Slowdown
    }

    [Header("SphereBehaviour")]
    
    public SphereState State;
    public float RotationsPerMinute;

    public float desiredSpeed;
    public float minDesiredSpeed;

    [Header("SphereOptions")]

    public Transform SphereObject;
    public Transform RayT;
    public LineRenderer LineRenderer;

    public bool DoSlowDownTime;

    //
    private Rigidbody rb;
    private Vector3 rotationDirection;

    private LTDescr _ltSlowdown;

    private LTDescr _timeSlowdown;
    private float _minTime = 0.75f;
    private float _timeToSlowdown = 0.17f;

    private const float _rayLenght = 28f;
    private bool _drawReflection;

    #region Public Methods

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ShootTo(Vector3 dir)
    {
        rb.AddForce(new Vector3(dir.x, dir.y, 0) * minDesiredSpeed);
        State = SphereState.Moving;
    }

    public int SlowdownBeforeCall()
    {
        var curDir = GetComponent<Rigidbody>().velocity.normalized;
        var curPos = transform.position;
        var newPos = new Vector3(curPos.x + curDir.x * 3f, curPos.y + curDir.y * 3f, 0);

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        RotationsPerMinute = 0.2f;
        State = Sphere.SphereState.CallBall;

        // raycast
        RayT.LookAt(curDir, Vector3.up);
        RaycastHit hitInfo;
        if (Physics.Raycast(RayT.position, curDir, out hitInfo, 4f))
        {
            var x = (curDir.x > 0) ? 0.25f : -0.25f;
            var y = (curDir.y > 0) ? 0.25f : -0.25f;
            newPos = new Vector3(curPos.x + x, curPos.y + y, 0f);
        }
        _ltSlowdown = LeanTween.move(gameObject, newPos, 2f);
        _ltSlowdown.setEase(LeanTweenType.easeOutBack);

        return _ltSlowdown.id;
    }

    #endregion

    #region Events
    void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.GetComponent<HitListener>)

        GameScore.Instance.RegisterHit(collision.gameObject.tag, collision.gameObject, collision.contacts[0].point);

        RotationsPerMinute = UnityEngine.Random.Range(1f, 2f);
        rotationDirection = new Vector3(
            UnityEngine.Random.Range(0f, 360f)
            , UnityEngine.Random.Range(0f, 360f)
            , UnityEngine.Random.Range(0f, 360f)
            );
    }

    void FixedUpdate()
    {
        if (State == SphereState.Moving || State == SphereState.CallBall)
            SphereObject.Rotate(
                rotationDirection.x * RotationsPerMinute * Time.deltaTime
                , rotationDirection.y * RotationsPerMinute * Time.deltaTime
                , rotationDirection.z * RotationsPerMinute * Time.deltaTime
                );

        if (State == SphereState.Moving)
        {
            //Debug.Log(rb.velocity.magnitude);
            if (rb.velocity.magnitude > desiredSpeed || rb.velocity.magnitude < minDesiredSpeed)
                rb.velocity = desiredSpeed * rb.velocity.normalized;
        }

        if (_drawReflection)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, rb.velocity, out hitInfo, _rayLenght))
            {
                //Debug.DrawRay(transform.position, rb.velocity * _rayLenght, Color.red, 1f);
                if (hitInfo.transform.CompareTag("Proppeler"))
                {
                    if (DoSlowDownTime == true)
                    {
                        float dist = Vector3.Distance(transform.position, hitInfo.point);
                        if (dist > 7)
                        {
                            if (_timeSlowdown == null)
                                _timeSlowdown = LeanTween.value(Time.timeScale, _minTime, _timeToSlowdown).setOnUpdate(SlowDownTime);
                        }
                    }
                    
                    if (LineRenderer.gameObject.activeSelf == false)
                        LineRenderer.gameObject.SetActive(true);
                    LineRenderer.transform.position = hitInfo.point;

                    // this is the collider surface normal
                    var normal = hitInfo.normal;
                    var reflect = Vector3.Reflect(Vector3.Normalize(rb.velocity), normal);

                    //LineRenderer.positionCount = 2;
                    var dir = (transform.position - LineRenderer.transform.position).normalized;
                    //var reflect = Vector3.Reflect(transform.position, rb.velocity).normalized;

                    LineRenderer.SetPosition(0, reflect * 8f);
                    LineRenderer.SetPosition(1, Vector3.zero);
                    LineRenderer.SetPosition(2, dir * 1.5f);

                    //Debug.Break();
                }
                else
                {
                    if (DoSlowDownTime == true && _timeSlowdown != null)
                        _timeSlowdown = LeanTween.value(Time.timeScale, 1f, _timeToSlowdown/2).setOnUpdate(SlowDownTime);
                    LineRenderer.gameObject.SetActive(false);
                }
            }
            else
            {
                if (DoSlowDownTime == true && _timeSlowdown != null)
                    _timeSlowdown = LeanTween.value(Time.timeScale, 1f, _timeToSlowdown/2).setOnUpdate(SlowDownTime);
                LineRenderer.gameObject.SetActive(false);
            }
        }
    }

    private void SlowDownTime(float value)
    {
        Time.timeScale = value;
    }

    void LateUpdate()
    {
        if (rb.velocity.y < 0)
        {
            _drawReflection = true;
        }
        else
        {
            _drawReflection = false;
            LineRenderer.gameObject.SetActive(false);
            if (DoSlowDownTime == true && _timeSlowdown != null)
                _timeSlowdown = LeanTween.value(Time.timeScale, 1f, _timeToSlowdown/2).setOnUpdate(SlowDownTime);
        }
    }

    #endregion

    #region Private Methods

    #endregion

    void Update()
    {
        // TEST
        
    }
}
