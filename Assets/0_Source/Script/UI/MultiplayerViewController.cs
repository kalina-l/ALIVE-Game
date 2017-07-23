using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerViewController : AbstractViewController {
    
    private float _switchTime = 2;

    private GameObject _remoteLemo;
    public AnimationController remoteAnimationController;
    private GameObject _itemObject;

    public AnimationController RemoteCharacterAnimation { get; set; }

    private RectTransform _searchRect;

    private Transform _camera;

    private bool _isMultiplayerOn;

    private Image _localAlert;
    private Image _localAlertIcon;

    private Image _remoteAlert;
    private Image _remoteAlertIcon;

    private Vector2 _alertSize;

    private IEnumerator rotateCameraRoutine;
    private IEnumerator moveCameraRoutine;

    public MultiplayerViewController (Transform parent)
    {
        _remoteLemo = GraphicsHelper.Instance.remoteLemo;
        RemoteCharacterAnimation = new AnimationController(_remoteLemo);

        Rect = CreateContainer("MultiplayerUI", parent,
            new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f));

        View = Rect.gameObject;

        _searchRect = CreateContainer("Search", Rect,
            new Vector2(-400, -80), new Vector2(130, 130),
            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f));

        AddSprite(_searchRect, GraphicsHelper.Instance.searchSprite, GraphicsHelper.Instance.SpriteColorWhite);

        _alertSize = new Vector2(300, 300);

        //local alert
        RectTransform localAlertRect = CreateContainer("LocalAlert", Rect,
            new Vector2(-320, -100), Vector2.zero,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1, 0));

        localAlertRect.localScale = new Vector3(-1, 1, 1);

        _localAlert = AddSprite(localAlertRect, GraphicsHelper.Instance.alertBubbleSprite, GraphicsHelper.Instance.SpriteColorWhite);

        RectTransform localIconRect = CreateContainer("Icon", localAlertRect,
            new Vector2(0, 30), new Vector2(128, 128),
            new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f));
        localIconRect.localScale = new Vector3(-1, 1, 1);

        localIconRect.offsetMin = new Vector2(86, 116);
        localIconRect.offsetMax = new Vector2(-86, -56);

        _localAlertIcon = AddSprite(localIconRect, null, GraphicsHelper.Instance.SpriteColorWhite);
        
        //remote alert
        RectTransform remoteAlertRect = CreateContainer("RemoteAlert", Rect,
            new Vector2(320, -100), Vector2.zero,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1, 0));
        _remoteAlert = AddSprite(remoteAlertRect, GraphicsHelper.Instance.alertBubbleSprite, GraphicsHelper.Instance.SpriteColorWhite);

        RectTransform remoteIconRect = CreateContainer("Icon", remoteAlertRect,
            new Vector2(0, 30), new Vector2(128, 128),
            new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f));

        remoteIconRect.offsetMin = new Vector2(86, 116);
        remoteIconRect.offsetMax = new Vector2(-86, -56);

        _remoteAlertIcon = AddSprite(remoteIconRect, null, GraphicsHelper.Instance.SpriteColorWhite);

        _camera = GraphicsHelper.Instance.camera;

        ApplicationManager.Instance.StartCoroutine(SearchAnimationRoutine());
    }
    
    //switch camera between singlePlayer and multiplayer view

    public void startMultiplayerView()
    {
        _isMultiplayerOn = true;
        
        ApplicationManager.Instance.MoveItemAlert(true);

        if(moveCameraRoutine != null) ApplicationManager.Instance.StopCoroutine(moveCameraRoutine);
        moveCameraRoutine = moveCamera(_camera.position, GraphicsHelper.Instance.multiplayerCameraAnchor.position, _switchTime);
        ApplicationManager.Instance.StartCoroutine(moveCameraRoutine);

        if (rotateCameraRoutine != null) ApplicationManager.Instance.StopCoroutine(rotateCameraRoutine);
        float angle = Quaternion.Angle(_camera.rotation, GraphicsHelper.Instance.multiplayerCameraAnchor.rotation);
        rotateCameraRoutine = rotateCamera(new Vector3(0, angle, 0), _switchTime);
        ApplicationManager.Instance.StartCoroutine(rotateCameraRoutine);
    }

    public void endMultiplayerView()
    {
        _isMultiplayerOn = false;
        
        ApplicationManager.Instance.MoveItemAlert(false);
        if (moveCameraRoutine != null) ApplicationManager.Instance.StopCoroutine(moveCameraRoutine);
        moveCameraRoutine = moveCamera(_camera.position, GraphicsHelper.Instance.singleplayerCameraAnchor.position, _switchTime);
        ApplicationManager.Instance.StartCoroutine(moveCameraRoutine);
        if (rotateCameraRoutine != null) ApplicationManager.Instance.StopCoroutine(rotateCameraRoutine);
        float angle = Quaternion.Angle(_camera.rotation, GraphicsHelper.Instance.singleplayerCameraAnchor.rotation);
        rotateCameraRoutine = rotateCamera(new Vector3(0, -angle, 0), _switchTime);
        ApplicationManager.Instance.StartCoroutine(rotateCameraRoutine);
        //UnityEngine.Object.Destroy(_remoteLemo);
    }
        
    private IEnumerator moveCamera(Vector3 source, Vector3 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            _camera.position = Vector3.Lerp(source, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        _camera.position = target;
    }

    IEnumerator rotateCamera(Vector3 byAngles, float overTime)
    {
        var fromAngle = _camera.rotation;
        var toAngle = Quaternion.Euler(_camera.eulerAngles + byAngles);
        for (var t = 0f; t < 1; t += Time.deltaTime / overTime)
        {
            _camera.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }
    }

    //show remote
    //public void showremote()
    //{
    //    //_remotelemo = gameobject.instantiate(graphicshelper.instance.lemo, graphicshelper.instance.multiplayerlemoanchor);
    //    //_remotelemo.getcomponentinchildren<renderer>().material = graphicshelper.instance.materials[0];
    //    //_remotelemo.transform.localposition = vector3.zero;
    //}

    public void setupTexture(Material material)
    {
        _remoteLemo.GetComponentInChildren<Renderer>().material = material;
    }

    private IEnumerator SearchAnimationRoutine()
    {
        Vector2 bigSize = new Vector2(130, 130);

        Vector2 animationCenter = new Vector2(-400, -80);
        float range = 100;
        Vector2[] targetCorners = new Vector2[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) };
        int targetIndex = 0;
        AnimationCurve curve = GraphicsHelper.Instance.AlertAnimation;
        float timer = 0;
        Vector2 lastPosition = animationCenter;
        Vector2 targetPosition = lastPosition;

        while (true)
        {
            _searchRect.sizeDelta = Vector2.zero;
            _searchRect.anchoredPosition = animationCenter;
            targetIndex = 0;
            
            while (ApplicationManager.Instance.Multiplayer.IsConnected || !_isMultiplayerOn)
            {
                yield return 0;
            }

            timer = 0;

            yield return new WaitForSeconds(2);

            while (timer < 1)
            {
                timer += Time.deltaTime * 8;

                _searchRect.sizeDelta = Vector2.Lerp(Vector2.zero, bigSize, curve.Evaluate(timer));

                yield return 0;
            }

            lastPosition = _searchRect.anchoredPosition;
            
            while (_isMultiplayerOn && !ApplicationManager.Instance.Multiplayer.IsConnected)
            {
                timer = 0;

                //animate

                targetPosition = animationCenter + (targetCorners[targetIndex] * range);
                
                while (timer < 1 && (_isMultiplayerOn && !ApplicationManager.Instance.Multiplayer.IsConnected))
                {
                    timer += Time.deltaTime * 2;

                    _searchRect.anchoredPosition = Vector2.Lerp(lastPosition, targetPosition, curve.Evaluate(timer));

                    yield return 0;
                }

                timer = 0;

                while(timer < 1 && (_isMultiplayerOn && !ApplicationManager.Instance.Multiplayer.IsConnected))
                {
                    timer += Time.deltaTime * 2;
                    yield return 0;
                }

                targetIndex++;
                lastPosition = targetPosition;

                if (targetIndex >= targetCorners.Length)
                    targetIndex = 0;
            }

            timer = 0;

            //hide

            lastPosition = _searchRect.anchoredPosition;

            while (timer < 1)
            {
                timer += Time.deltaTime * 4;

                _searchRect.sizeDelta = Vector2.Lerp(bigSize, Vector2.zero, curve.Evaluate(timer));
                _searchRect.anchoredPosition = Vector2.Lerp(lastPosition, animationCenter, curve.Evaluate(timer));

                yield return 0;
            }
        }
    }

    //Multiplayer Alerts
    public void ShowMultiplayerRequest(int activityID, bool isLocal)
    {
        if (isLocal) {
            switch (activityID)
            {
                case 13:
                    //Hug
                    _localAlertIcon.sprite = Resources.Load<Sprite>("Graphics/Multiplayer/Hug");
                    break;
                case 23:
                    //Ball
                    _localAlertIcon.sprite = Resources.Load<Sprite>("Graphics/Items/Ball");
                    break;
                case 33:
                    //Cake
                    _localAlertIcon.sprite = Resources.Load<Sprite>("Graphics/Items/Cake");
                    break;
            }
        } else
        {
            _remoteAlertIcon.rectTransform.localScale = Vector3.one;
            switch (activityID)
            {
                case 13:
                    //Hug
                    _remoteAlertIcon.sprite = Resources.Load<Sprite>("Graphics/Multiplayer/Hug");
                    break;
                case 23:
                    //Ball
                    _remoteAlertIcon.sprite = Resources.Load<Sprite>("Graphics/Items/Ball");
                    break;
                case 33:
                    //Cake
                    _remoteAlertIcon.sprite = Resources.Load<Sprite>("Graphics/Items/Cake");
                    break;
            }
        }
        
        ApplicationManager.Instance.StartCoroutine(ShowAlertRoutine(isLocal));
    }

    public void ShowMultiplayerResponse(bool accept, bool isLocal)
    {
        if (isLocal)
        {
            if (accept)
            {
                _localAlertIcon.sprite = Resources.Load<Sprite>("Graphics/Multiplayer/Ok");
            } else
            {
                _localAlertIcon.sprite = Resources.Load<Sprite>("Graphics/Multiplayer/No");
            }
        }
        else
        {
            _remoteAlertIcon.rectTransform.localScale = new Vector3(-1, 1, 1);

            if (accept)
            {
                _remoteAlertIcon.sprite = Resources.Load<Sprite>("Graphics/Multiplayer/Ok");
            }
            else
            {
                _remoteAlertIcon.sprite = Resources.Load<Sprite>("Graphics/Multiplayer/No");
            }
        }

        ApplicationManager.Instance.StartCoroutine(ShowAlertRoutine(isLocal));
    }

    private IEnumerator ShowAlertRoutine(bool isLocal)
    {
        float timer = 0;
        AnimationCurve curve = GraphicsHelper.Instance.AlertAnimation;

        while (timer < 1)
        {
            timer += Time.deltaTime * 8;

            if (isLocal)
            {
                _localAlert.rectTransform.sizeDelta = Vector2.Lerp(Vector2.zero, _alertSize, curve.Evaluate(timer));
            }
            else
            {
                _remoteAlert.rectTransform.sizeDelta = Vector2.Lerp(Vector2.zero, _alertSize, curve.Evaluate(timer));
            }

            yield return 0;
        }

        yield return new WaitForSeconds(0.5f);
        if (isLocal)
        {
            if (!ApplicationManager.Instance.Multiplayer.getLocalAlert()) yield return new WaitForSeconds(1f);
            else
            {
                while (ApplicationManager.Instance.Multiplayer.getLocalAlert())
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        else
        {
            if (!ApplicationManager.Instance.Multiplayer.getRemoteAlert()) yield return new WaitForSeconds(1f);
            else
            {
                while (ApplicationManager.Instance.Multiplayer.getRemoteAlert())
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }


        timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime * 4;

            if (isLocal)
            {
                _localAlert.rectTransform.sizeDelta = Vector2.Lerp(_alertSize, Vector2.zero, curve.Evaluate(timer));
            }
            else
            {
                _remoteAlert.rectTransform.sizeDelta = Vector2.Lerp(_alertSize, Vector2.zero, curve.Evaluate(timer));
            }

            yield return 0;
        }

    }

    public void addItem(int id)
    {
        Transform parent = GraphicsHelper.Instance.remoteItemAnchor;

        _itemObject = GameObject.Instantiate(GraphicsHelper.Instance.GetItemObject(id), parent);

        //_itemObject.GetComponent<ItemBoxObject>().Setup(this);
    }

    public void removeItem()
    {
        if (_itemObject != null)
        {
            UnityEngine.Object.Destroy(_itemObject);
        }
    }
}
