using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProtectionStateUI : MonoBehaviour
{
    [SerializeField] Image sprite;
    [SerializeField] List<Sprite> sprites = new List<Sprite>();
    enum ProtectionsSprite
    {
        Positive,
        Negative,
    }
    public void ChangeSprite(Unit.ProtectionState _protectionState)
    {
        switch (_protectionState)
        {
            case Unit.ProtectionState.None:
                sprite.enabled = false;
                break;
            case Unit.ProtectionState.Surrounded:
                sprite.enabled = true;
                sprite.sprite = sprites[(int)ProtectionsSprite.Negative];
                break;
            case Unit.ProtectionState.Covered:
                sprite.enabled = true;
                sprite.sprite = sprites[(int)ProtectionsSprite.Positive];
                break;
            default:
                break;
        }
    }
}
