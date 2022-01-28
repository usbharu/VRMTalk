﻿using UnityEditor;
using UnityEngine;
using UnityLogging;

namespace VRMTalk.Editor
{
    public class GameObjectPreview
    {

        private PreviewRenderUtility previewRenderUtility;

        public GameObjectPreview()
        {
            init();
        }

        private void init()
        {
            Logging.Log("Init Preview","Preview");
            if (previewRenderUtility != null)
            {
                previewRenderUtility.Cleanup();
            }

            previewRenderUtility = new PreviewRenderUtility(true);
            System.GC.SuppressFinalize(previewRenderUtility);
            var camera = previewRenderUtility.camera;
            camera.fieldOfView = 30f;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 1000;
            camera.transform.position = new Vector3(0f,1f,-2f);
        }

        public Texture CreatePreviewTexture(GameObject obj)
        {
            if (obj == null)
            {
                return null;
            }
            
            previewRenderUtility.BeginPreview(new Rect(0,0,512,512),GUIStyle.none);

            previewRenderUtility.lights[0].transform.localEulerAngles = new Vector3(30, 30, 30);
            previewRenderUtility.lights[0].intensity = 2;
            previewRenderUtility.AddSingleGO(obj);
            previewRenderUtility.camera.Render();

            return previewRenderUtility.EndPreview();

        }

        public void Cleanup()
        {
            previewRenderUtility.Cleanup();
        }
        
        
    }
}