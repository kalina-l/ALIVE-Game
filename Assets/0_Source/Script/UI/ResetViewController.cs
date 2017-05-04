using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetViewController : AbstractViewController {

    public ResetViewController(Transform parent)
    {
        Rect = CreateContainer("Reset", parent, new Vector2(0, 256), new Vector2(256, 256), Vector2.zero, Vector2.zero, new Vector2(0, 1));


        View = Rect.gameObject;

        RectTransform buttonRect = CreateContainer("ResetButton", Rect, new Vector2(20, 20), new Vector2(200, 200), Vector2.zero, Vector2.zero, Vector2.zero);


        AddSprite(buttonRect, GraphicsHelper.Instance.resetSprite, GraphicsHelper.Instance.SpriteColorWhite);
        CreateButton(buttonRect, delegate { reset(); });
    }



    public void reset()
    {
        ApplicationManager.Instance.reset();
    }


}
