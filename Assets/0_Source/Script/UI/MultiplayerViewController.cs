using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerViewController : AbstractViewController {
    
    private float _switchTime = 2;

    private GameObject _remoteLemo;
    public AnimationController remoteAnimationController;

    public AnimationController RemoteCharacterAnimation { get; set; }

    private Transform _camera;

    private bool _isMultiplayerOn;
    private bool _isConnected;

    public MultiplayerViewController (Transform parent)
    {
        Rect = CreateContainer("MultiplayerUI", parent,
            new Vector2(-400, -80), new Vector2(130, 130),
            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f));

        View = Rect.gameObject;

        AddSprite(Rect, GraphicsHelper.Instance.searchSprite, GraphicsHelper.Instance.SpriteColorWhite);

        _camera = GraphicsHelper.Instance.camera;

        ApplicationManager.Instance.StartCoroutine(SearchAnimationRoutine());
    }
    
    //switch camera between singlePlayer and multiplayer view

    public void startMultiplayerView()
    {
        _isMultiplayerOn = true;

        showRemote();
        ApplicationManager.Instance.MoveItemAlert(true);
        RemoteCharacterAnimation = new AnimationController(_remoteLemo);
        ApplicationManager.Instance.StartCoroutine(moveCamera(_camera.position, GraphicsHelper.Instance.multiplayerCameraAnchor.position, _switchTime));
        float angle = Quaternion.Angle(_camera.rotation, GraphicsHelper.Instance.multiplayerCameraAnchor.rotation);
        ApplicationManager.Instance.StartCoroutine(rotateCamera(new Vector3(0, angle, 0), _switchTime));
    }

    public void endMultiplayerView()
    {
        _isMultiplayerOn = false;

        ApplicationManager.Instance.MoveItemAlert(false);
        ApplicationManager.Instance.StartCoroutine(moveCamera(_camera.position, GraphicsHelper.Instance.singleplayerCameraAnchor.position, _switchTime));
        float angle = Quaternion.Angle(_camera.rotation, GraphicsHelper.Instance.singleplayerCameraAnchor.rotation);
        ApplicationManager.Instance.StartCoroutine(rotateCamera(new Vector3(0, -angle, 0), _switchTime));
        UnityEngine.Object.Destroy(_remoteLemo);
    }

    public void Connect()
    {
        _isConnected = true;
    }

    public void Disconnect()
    {
        _isConnected = false;
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
    public void showRemote()
    {
        _remoteLemo = GameObject.Instantiate(GraphicsHelper.Instance.lemo, GraphicsHelper.Instance.multiplayerLemoAnchor);
        _remoteLemo.GetComponentInChildren<Renderer>().material = GraphicsHelper.Instance.materials[0];
        _remoteLemo.transform.localPosition = Vector3.zero;
    }

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
            Rect.sizeDelta = Vector2.zero;
            Rect.anchoredPosition = animationCenter;
            targetIndex = 0;

            while(!_isMultiplayerOn || _isConnected)
            {
                yield return 0;
            }

            timer = 0;

            //

            yield return new WaitForSeconds(2);

            while (timer < 1)
            {
                timer += Time.deltaTime * 8;

                Rect.sizeDelta = Vector2.Lerp(Vector2.zero, bigSize, curve.Evaluate(timer));

                yield return 0;
            }

            lastPosition = Rect.anchoredPosition;

            while (_isMultiplayerOn && !_isConnected)
            {
                timer = 0;

                //animate

                targetPosition = animationCenter + (targetCorners[targetIndex] * range);

                while (timer < 1 && (_isMultiplayerOn && !_isConnected))
                {
                    timer += Time.deltaTime * 2;

                    Rect.anchoredPosition = Vector2.Lerp(lastPosition, targetPosition, curve.Evaluate(timer));

                    yield return 0;
                }

                timer = 0;

                while(timer < 1 && (_isMultiplayerOn && !_isConnected))
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

            lastPosition = Rect.anchoredPosition;

            while (timer < 1)
            {
                timer += Time.deltaTime * 4;

                Rect.sizeDelta = Vector2.Lerp(bigSize, Vector2.zero, curve.Evaluate(timer));
                Rect.anchoredPosition = Vector2.Lerp(lastPosition, animationCenter, curve.Evaluate(timer));

                yield return 0;
            }
        }
    }

}
