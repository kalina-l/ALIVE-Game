using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerViewController : AbstractViewController {

    private float _switchTime = 3;

    private GameObject _remoteLemo;
    public AnimationController remoteAnimationController;

    public AnimationController RemoteCharacterAnimation { get; set; }

    private Transform _camera;

    public MultiplayerViewController ()
    {
        _camera = GraphicsHelper.Instance.camera;
    }

    //switch camera between singlePlayer and multiplayer view
    public void startMultiplayerView()
    {
        showRemote();
        RemoteCharacterAnimation = new AnimationController(_remoteLemo);
        ApplicationManager.Instance.StartCoroutine(moveCamera(GraphicsHelper.Instance.singleplayerCameraAnchor.position, GraphicsHelper.Instance.multiplayerCameraAnchor.position, _switchTime));
        float angle = Quaternion.Angle(_camera.rotation, GraphicsHelper.Instance.multiplayerCameraAnchor.rotation);
        ApplicationManager.Instance.StartCoroutine(rotateCamera(new Vector3(0, angle, 0), _switchTime));
    }

    public void endMultiplayerView()
    {   
        ApplicationManager.Instance.StartCoroutine(moveCamera(GraphicsHelper.Instance.multiplayerCameraAnchor.position, GraphicsHelper.Instance.singleplayerCameraAnchor.position, _switchTime));
        float angle = Quaternion.Angle(_camera.rotation, GraphicsHelper.Instance.singleplayerCameraAnchor.rotation);
        ApplicationManager.Instance.StartCoroutine(rotateCamera(new Vector3(0, angle, 0), _switchTime));
        UnityEngine.Object.Destroy(_remoteLemo);
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
        _remoteLemo.transform.localPosition = Vector3.zero;
    }

}
