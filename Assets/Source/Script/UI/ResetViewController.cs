using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetViewController : AbstractViewController {

    public ResetViewController(Transform parent)
    {
        Rect = CreateContainer("Reset", parent, Vector2.zero, new Vector2(1080, 1920), Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f);
        
        View = Rect.gameObject;

        RectTransform buttonRect = CreateContainer("ResetButton", Rect, new Vector2(-444, -863), new Vector2(100, 100), Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.one * 0.5f);
        AddSprite(buttonRect, null, GraphicsHelper.Instance.SpriteColorWhite);
        CreateButton(buttonRect, delegate { reset(); });
    }



    public void reset()
    {
        ApplicationManager.Instance.reset();
    }


}
