using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    public enum ESpriteName
    {
        Regular,
        OnMoving,
        OnRange,
        OnAllied,
    }
    SpriteRenderer spriteRenderer;
    [SerializeField] List<Sprite> sprites;
    ESpriteName currentSprite = ESpriteName.Regular;
    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    public void Show()
    {
        spriteRenderer.enabled = true;
    }
    public void Hide()
    {
        spriteRenderer.enabled = false;
    }
    public void ChangeSprite(ESpriteName _sprite)
    {
        if (_sprite != currentSprite)
        {
            spriteRenderer.sprite = sprites[(int)_sprite];
            currentSprite = _sprite;
        }
    }
}
