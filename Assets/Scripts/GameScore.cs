using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScore : MonoBehaviour
{
    private static GameScore _game;
    public static GameScore Instance
    {
        get { return _game; }
    }
    //

    public bool DontDie;

    public Text HealthText;
    public Text WaveText;
    public Text ScoreText;
    public Image DamageEffectImage;

    [HideInInspector]
    public MeshCollider ProppelerCollider;
    //
    public int _colliderHits;
    public int _hitsToAddNewRow;
    public readonly int _minHitsToAddNewRow = 4;
    public int _currentHits;

    private int Score;
    private int Health = 100;

    private readonly float _maxPossibleDistance = 33f;

    [SerializeField]
    public BlockType _previousBlockTypeHit;
    private BlockType _blockType;

    private Vector3 _pastPreviousHitPoint;
    private Vector3 _previousHitPoint;
    private Vector3 _currentHitPoint;

    private IEnumerator _onDamageTaken;
    private float _damageLerpTime;

    void Awake()
    {
        _game = this;

        ProppelerCollider = Game.Instance.Proppeler.GetComponent<MeshCollider>();
    }

    void Start()
    {
        _previousBlockTypeHit = BlockType.Proppeler;

        ResetHits();

        _damageLerpTime = 0.15f;
    }

    public void ResetHits()
    {
        if (_hitsToAddNewRow > _minHitsToAddNewRow)
            _hitsToAddNewRow--;

        _currentHits = _hitsToAddNewRow;
        WaveText.text = _currentHits.ToString();
    }

    internal void RegisterHit(string tag, GameObject objHit, Vector3 hitPoint)
    {
        if (ProppelerCollider.enabled == false)
            ProppelerCollider.enabled = true;

        _previousBlockTypeHit = _blockType;
        _blockType = (BlockType)Enum.Parse(typeof(BlockType), tag);

        RegisterHitPoint(hitPoint);

        if (_blockType == BlockType.Proppeler)
            CheckForNewRow();

        CheckIfWeCanCallTheBallBack();

        if (_blockType == BlockType.Block)
        {
            var blockClass = objHit.GetComponent<Block>();
            if (blockClass == null)
            {
                var hitListener = objHit.GetComponent<HitListener>();
                blockClass = hitListener.GetBlock();
            }

            RegisterPoint(blockClass);
        }

        if (_blockType == BlockType.RedZone)
        {
            RegisterDamage(30);
        }
    }

    private void RegisterHitPoint(Vector3 hitPoint)
    {
        _pastPreviousHitPoint = _previousHitPoint;
        _previousHitPoint = _currentHitPoint;
        _currentHitPoint = hitPoint;
    }

    private void CheckForNewRow()
    {
        _currentHits--;
        WaveText.text = _currentHits.ToString();

        if (_currentHits == 0)
        {
            ResetHits();
            Game.Instance.AddNewRow();
        }
    }

    private void CheckIfWeCanCallTheBallBack()
    {
        // check if we can call the ball back.
        if (_blockType == BlockType.SidePerimeter)
        {
            _colliderHits++;
            if (_colliderHits > 3)
            {
                Game.Instance.CanvasController.ActivateCallBack();
                _colliderHits = 0;
            }
        }
        else
        {
            _colliderHits = 0;
        }
    }

    private void RegisterPoint(Block block)
    {
        if (block == null)
            Debug.Break();
        int scoreToAdd = block.Points;
        if (_previousBlockTypeHit == BlockType.Proppeler)
        {
            // bonus if you hit the first time: 100%
            scoreToAdd = (scoreToAdd * 2);

            // bonus points based on distance from proppeler to sphere
            // the bigger the better
            var distance = Vector3.Distance(_previousHitPoint, _currentHitPoint);
            var percentToHit = UsefullUtils.GetValuePercent(distance, _maxPossibleDistance);

            float bonusDistancePoints;
            if (percentToHit > 40)  // bigger value decreases points
            {
                // bonus points based on distance from a contact point to proppeler
                // the smaller the better
                distance = Vector3.Distance(_pastPreviousHitPoint, _previousHitPoint);
                var percentToProppeler = UsefullUtils.GetValuePercent(distance, _maxPossibleDistance);
                percentToProppeler = percentToHit - percentToProppeler;

                bonusDistancePoints = UsefullUtils.GetPercent(scoreToAdd, percentToProppeler);
                scoreToAdd += (int)Mathf.Ceil(bonusDistancePoints);
            }

            bonusDistancePoints = UsefullUtils.GetPercent(scoreToAdd, percentToHit);
            scoreToAdd += (int)Mathf.Ceil(bonusDistancePoints);


            // score is a bit high, so lets trim it down a bit.
            //
            // WE CAN REMOVE THIS LATER
            //
            //var percentToDecrease = 
            scoreToAdd = (int)Mathf.Ceil(UsefullUtils.GetPercent(scoreToAdd, 60));

            Score += scoreToAdd;
        }
        else
        {
            Score += scoreToAdd;
        }

        ScoreText.text = Score.ToString();
        Game.Instance.CanvasController.ShowcasePoints(scoreToAdd, _currentHitPoint);
    }

    public void RegisterDamage(int damage)
    {
        if (!DontDie)
            Health -= damage;

        if (Health < 0)
            Health = 0;

        if (_onDamageTaken == null)
        {
            LeanTween.color(DamageEffectImage.GetComponent<RectTransform>(), Game.Instance.DamagedAmbientColor, _damageLerpTime);
            _onDamageTaken = onColorChange();
            StartCoroutine(_onDamageTaken);
        }
        else {
            ChangeColorBack();
        }
        
        HealthText.text = Health.ToString();

        if (Health < 1)
        {
            StartCoroutine(SlowdownBall());
        }
    }

    private IEnumerator SlowdownBall()
    {
        int tweenId = Game.Instance.Sphere.SlowdownBeforeCall();

        yield return new WaitForSeconds(2f);

        LeanTween.cancel(tweenId);
        Main.Instance.CompletedGame = new GameData(Score);
        Main.Instance.OnEndGame();
    }

    IEnumerator onColorChange()
    {
        yield return new WaitForSeconds(0.1f);
        ChangeColorBack();
    }

    private void ChangeColorBack() {
        LeanTween.color(DamageEffectImage.GetComponent<RectTransform>(), Game.Instance.AmbientColor, _damageLerpTime / 2);
        _onDamageTaken = null;
    }
}

