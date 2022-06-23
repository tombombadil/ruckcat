using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ruckcat;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HyperOverlay : HyperSceneObj
{
    public LayerMask _CullingMask;
    public Color OverlayColour;

    public override void Init()
    {
        base.Init();
        _maincam = Camera.main;
        SetCamSettings();
        SetUpCanvas();
    }


    private Camera _maincam;
    private Camera _OverlayCam;



    public void OpenCloseOverlay(bool isOpen)
    {
        _OverlayCam.enabled = isOpen;
        _overlaycanvas.enabled = isOpen;
    }



    public void SetOnTopCavas(LayerMask _CullMask)
    {
        _OverlayCam.cullingMask = _CullingMask;
    }





    private void SetCamSettings()
    {
        GameObject camobject = new GameObject("OverlayCam");
        camobject.AddComponent<Camera>();
        _OverlayCam = camobject.GetComponent<Camera>();

        _OverlayCam.transform.parent = _maincam.transform;
        _OverlayCam.transform.localPosition = Vector3.zero;
        _OverlayCam.transform.localEulerAngles = Vector3.zero;

        _OverlayCam.cullingMask = _CullingMask;
        _OverlayCam.fieldOfView = _maincam.fieldOfView;

        var cameraData = _OverlayCam.GetUniversalAdditionalCameraData();    //RenderType değiştirme
        cameraData.renderType = CameraRenderType.Overlay;

        var stackdata = _maincam.GetUniversalAdditionalCameraData();
        stackdata.cameraStack.Add(_OverlayCam);



        _OverlayCam.enabled = false;        
    }

    Canvas _overlaycanvas;
    private void SetUpCanvas()
    {
        GameObject _overlaycanvasobject = new GameObject("OverlayCanvas");
        _overlaycanvasobject.AddComponent<RectTransform>();
        _overlaycanvasobject.AddComponent<Canvas>();

        _overlaycanvasobject.AddComponent<CanvasScaler>();
        _overlaycanvasobject.AddComponent<GraphicRaycaster>();
       


        _overlaycanvas = _overlaycanvasobject.GetComponent<Canvas>();
        _overlaycanvas.worldCamera = _maincam;



        GameObject _overlayimage = new GameObject("Overlayimage");

        var _imagesetter = _overlayimage.AddComponent<Image>();

        _imagesetter.color = OverlayColour;

        _overlayimage.transform.parent = _overlaycanvas.transform;
        _overlayimage.transform.localScale = new Vector3(3,3,3);
        _overlayimage.transform.position = (_maincam.transform.forward * 5) + new Vector3( _maincam.transform.position.x, _maincam.transform.position.y, _maincam.transform.position.z);
        _overlayimage.transform.LookAt(_maincam.transform);

        _overlaycanvas.transform.parent = _maincam.transform;


        _overlaycanvas.enabled = false;
    }


}
