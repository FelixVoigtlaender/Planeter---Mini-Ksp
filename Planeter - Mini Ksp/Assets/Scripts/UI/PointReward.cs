using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointReward : MonoBehaviour
{
    public int reward;
    public IntValue points;
    public Sprite spriteEmpty;
    public Sprite spriteFilled;
    SpriteRenderer renderer;
    bool rewarded;
    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
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
    }
}
