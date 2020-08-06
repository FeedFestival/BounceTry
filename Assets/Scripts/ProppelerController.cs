using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProppelerController : MonoBehaviour
{
    public bool BallIsReadyToBeLaunched;
    // privates
    private bool _mouseOverArea;
    private float _movingToFingerXPosition;
    private float _fingerXPosition;
    private int _movingTweenId;
    
    #region Public Methods

    public void Init()
    {
        BallIsReadyToBeLaunched = true;

        CallTheBallBack(true);
    }

    public void CallTheBallBack(bool instantly = false)
    {
        Game.Instance.CanvasController.CallBackButton.SetActive(false);
        
        if (instantly)
        {
            GameScore.Instance.ProppelerCollider.enabled = false;
            Game.Instance.Sphere.transform.position = Game.Instance.Proppeler.transform.position;
        }
        else
        {
            Game.Instance.CanvasController.ActivateCallBack(false);
            StartCoroutine(SlowdownBall());
        }
    }

    #endregion

    #region Events

    // Update is called once per frame
    private void Update()
    {
        if (_mouseOverArea)
        {
            if (Input.GetMouseButton(0))
            {
                _fingerXPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                MoveProppeler();
            }
        }
    }

    private void OnMouseEnter()
    {
        _mouseOverArea = true;
    }

    private void OnMouseExit()
    {
        _mouseOverArea = false;
    }

    #endregion

    #region Private Methods

    private IEnumerator SlowdownBall()
    {
        int tweenId = Game.Instance.Sphere.SlowdownBeforeCall();

        yield return new WaitForSeconds(2f);

        var dir = (Game.Instance.Proppeler.transform.position - Game.Instance.Sphere.transform.position).normalized;
        LeanTween.cancel(tweenId);
        Game.Instance.Sphere.State = Sphere.SphereState.Moving;
        Game.Instance.Sphere.transform.position = Game.Instance.Sphere.transform.position;
        Game.Instance.Sphere.ShootTo(new Vector3(dir.x, dir.y, 0));
    }

    private void ShootBall()
    {
        var dir = (new Vector3(_fingerXPosition, 0.5f, 0) - Game.Instance.Sphere.transform.position).normalized;
        Game.Instance.Sphere.ShootTo(dir);
        BallIsReadyToBeLaunched = false;
    }

    private void MoveProppeler()
    {
        if (BallIsReadyToBeLaunched)
        {
            ShootBall();
            return;
        }

        float diff;
        if (_movingToFingerXPosition < _fingerXPosition)
            diff = _fingerXPosition - _movingToFingerXPosition;
        else
            diff = _movingToFingerXPosition - _fingerXPosition;

        if (diff < 0.25f)
            return;

        var moveTo = new Vector3(_fingerXPosition, Game.Instance.Proppeler.transform.position.y, Game.Instance.Proppeler.transform.position.z);
        _movingToFingerXPosition = _fingerXPosition;

        LeanTween.cancel(_movingTweenId);
        _movingTweenId = LeanTween.move(Game.Instance.Proppeler, moveTo, 0.1f).id;
    }
    #endregion
}
