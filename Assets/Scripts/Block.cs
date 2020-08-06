using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool HasMultipleColliders;
    public bool AutoInit;

    public int Health = 2;
    public int Points;
    public int InitialHealth;
    public bool IsAvailable;
    public int RowIndex;
    public int ColumnIndex;

    public SpriteRenderer BlockSprite;

    //

    public string TypeName;

    #region Public Methods

    void Start()
    {
        if (AutoInit)
            Init();
    }

    public void Init()
    {
        Health = InitialHealth;

        if (Points == 0)
            Points = 1;

        var rand = (int)UnityEngine.Random.Range(1, 5);
        switch (rand)
        {
            case 1:
                BlockSprite.color = Game.Instance.red;
                break;
            case 2:
                BlockSprite.color = Game.Instance.blue;
                break;
            case 3:
                BlockSprite.color = Game.Instance.green;
                break;
            case 4:
            case 5:
                BlockSprite.color = Game.Instance.yellow;
                break;
        }

        if (HasMultipleColliders)
        {
            var _hits = GetComponentsInChildren<HitListener>();
            foreach (var hitListener in _hits)
            {
                hitListener.Init(Hit, GetBlock);
            }
        }
    }

    public Block GetBlock()
    {
        return this;
    }

    public void Hit()
    {
        Health--;
        if (Health < 1)
        {
            Destroy();
        }
    }

    #endregion

    #region Event

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RedZone"))
        {
            GameScore.Instance.RegisterDamage(15);
            Destroy(dangerZone: true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("RedZone"))
        //    Destroy();
        //else
        Hit();
    }

    #endregion

    #region Private Methods

    private void Destroy(bool dangerZone = false)
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
        IsAvailable = true;
        Game.Instance.CheckIfRowIsDestroyed(RowIndex, ColumnIndex, dangerZone);
    }

    #endregion
}
