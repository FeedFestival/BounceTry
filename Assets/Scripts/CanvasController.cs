using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public GameObject CallBackButton;
    public Text PointRect;

    public RectTransform CanvasRect;

    //

    Vector2 ViewportPosition;
    Vector2 WorldObject_ScreenPosition;
    private bool _followProppeler;

    void Awake()
    {
        CallBackButton.SetActive(true);
    }

    internal void ActivateCallBack(bool activate = true)
    {
        _followProppeler = activate;
        CallBackButton.SetActive(activate);
    }

    public void ShowcasePoints(int points, Vector3 hitPoint)
    {
        var pointsPos = Camera.main.WorldToViewportPoint(hitPoint);
        pointsPos = new Vector2(
        ((pointsPos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((pointsPos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        PointRect.GetComponent<RectTransform>().anchoredPosition = pointsPos;
        PointRect.text = "+" + points.ToString();
    }

    void LateUpdate()
    {
        if (_followProppeler)
            FollowProppeler();
    }

    private void FollowProppeler()
    {
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

        ViewportPosition = Camera.main.WorldToViewportPoint(Game.Instance.Proppeler.transform.position);
        WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        //now you can set the position of the ui element
        CallBackButton.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
    }
}
