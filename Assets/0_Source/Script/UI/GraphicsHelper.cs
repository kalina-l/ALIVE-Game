using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GraphicsHelper : MonoBehaviour {

    public static GraphicsHelper Instance;

    public Transform camera;

    public GameObject lemo;
    public Material[] materials;
    public Transform itemAnchor;
    public Transform multiplayerLemoAnchor;
    public Transform multiplayerCameraAnchor;
    public Transform singleplayerCameraAnchor;

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
    public Sprite itemBackgroundSprite;
    public Sprite resetSprite;

    public Sprite sliderBackgroundSpirte;
    public Sprite sliderFillSprite;

    public Sprite tooltipBackgroundSprite;

    public Sprite emotionSliderBGSpirte;
    public Sprite emotionSliderPointerSprite;

    public Sprite radialBackground;
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

    public Sprite UIContainer;
    public Sprite UIButton;
    public Sprite UIButton_pressed;
    public Color UIColor1;
    public Color UIColor2;

    public AnimationCurve SliderAnimation;
    public AnimationCurve AlertAnimation;

    public GameObject cakePrefab;
    public GameObject ballPrefab;

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
    
    public GameObject GetItemObject(string itemName)
    {
        switch (itemName)
        {
            case "Ball":
                return ballPrefab;
            case "Cake":
                return cakePrefab;
        }

        return null;
    }
}
