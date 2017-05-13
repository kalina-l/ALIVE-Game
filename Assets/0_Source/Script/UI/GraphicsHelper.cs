using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GraphicsHelper : MonoBehaviour {

    public static GraphicsHelper Instance;

    public GameObject lemo;

    public Font UIFont;

    public Color[] evaluationColor;

    public Sprite feedbackNegativeSprite;
    public Sprite feedbackNeutralSprite;
    public Sprite feedbackPositiveSprite;
    public ParticleSystem positiveFX;
    public ParticleSystem negativeFX;

    public Sprite speakerSprite;
    public Sprite cameraSprite;

    public Sprite alertBubbleSprite;

    public Sprite itemboxOpenSprite;
    public Sprite itemboxClosedSprite;
    public Sprite itemSlotSprite;
    public Sprite resetSprite;

    public Sprite sliderBackgroundSpirte;
    public Sprite sliderFillSprite;

    public Sprite radialSliderSprite;
    public Sprite iconEnergy;
    public Sprite iconHealth;
    public Sprite iconHunger;
    public Sprite iconSatisfaction;
    public Sprite iconSocial;

    public Sprite fist;
    public Sprite hand;

    public Color ButtonColorOn;
    public Color ButtonColorOff;
    public Color ContainerColor;
    public Color TextColor;
    public Color TextColorHidden { get { Color temp = TextColor; temp.a = 0; return temp; } }
    public Color SpriteColorWhite;
    public Color SpriteColorWhiteHidden { get { Color temp = SpriteColorWhite; temp.a = 0; return temp; } }
    public Color SpriteColorBlack;
    public Color SpriteColorBlackHidden { get { Color temp = SpriteColorBlack; temp.a = 0; return temp; } }

    public AnimationCurve SliderAnimation;
    public AnimationCurve AlertAnimation;

    void Awake()
    {
        Instance = this;
    }


    public Color LerpColor(Color c1, Color c2, float t)
    {
        Vector3 v1 = new Vector3(c1.r, c1.g, c1.b);
        Vector3 v2 = new Vector3(c2.r, c2.g, c2.b);

        Vector3 lerp = Vector3.Lerp(v1, v2, t);

        return new Color(lerp.x, lerp.y, lerp.z, Mathf.Lerp(c1.a, c2.a, t));
    }
    
}
