using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointReward : MonoBehaviour
{
    public int reward;
    public IntValue points;
    public IntValue totalRewards;
    public IntValue collectedRewards;
    public Sprite spriteEmpty;
    public Sprite spriteFilled;
    SpriteRenderer renderer;
    bool rewarded;
    private void Awake()
    {
        if (totalRewards)
            totalRewards.Value = 0;
        if (collectedRewards)
            collectedRewards.Value = 0;
    }
    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();

        if (totalRewards)
            totalRewards.Value+= reward;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Reward();
    }
    public void Reward()
    {
        if (rewarded)
            return;

        rewarded = true;
        points.Value += reward;
        renderer.sprite = spriteFilled;

        if (collectedRewards)
            collectedRewards.Value += reward;
    }
}
