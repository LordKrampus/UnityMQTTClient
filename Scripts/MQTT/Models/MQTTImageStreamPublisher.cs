using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using ReferenceModels;
using ImageCores.TextureEnhancement;
using static UnityEngine.Rendering.DebugUI;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTT.Models 
{
    public class MQTTImageStreamPublisher: MQTTPublisher
    {
        private static string[] TOPICS = new string[] { "device/holo/image" };

        [Header("Source")]
        [SerializeField]
        private Camera cam;
        private WebCamDevice wcam;
        private RenderTexture _render;
        private RenderTexture _outTexture;
        private Texture2D _texture;

        [Header("Quality")]
        [SerializeField]
        private int xResolution;
        [SerializeField]
        private RescaleEnhancement _scaler;
        //private CopyEnhancement _copier;

        [Header("Log")]
        [SerializeField]
        private UnityEngine.UI.Text fpsText;

        // time count;
        private float timeStart = 0;
        private int fps = 0;
        private float TimeCount
        {
            set { 
                this.fps++;  
                if (value - timeStart >= 1f) {
                    this.timeStart = value;
                    this.fpsText.text = fps + " fps";  
                    this.fps = 0;  
                } 
            }
        }

        [Header("[deprecated] Stream")]
        [SerializeField]
        private bool listen;
        [SerializeField]
        private RawImage display;

        public override string[] Topics => TOPICS;

        public override void PublishMessage(string topic, string message)
        {
            base.PublishMessage(topic, message);
        }

        protected override void Awake()
        {
            base.Awake();

            ImageCores.Utils.TextureUtil.ReinitializeTexture(ref this._render, this.cam.pixelWidth, this.cam.pixelHeight, true);
            ImageCores.Utils.TextureUtil.ReinitializeTexture(ref this._outTexture, 1, 1, true);
            this._texture = ImageCores.Utils.TextureUtil.ReinitializeTexture(this._texture, 1, 1);

            this._scaler.TargedWidth = this.xResolution;
            //this._copier = new CopyEnhancement();
        }

        private bool _isStreaming = false;
        private void LateUpdate()
        {
            if (this._isStreaming)
                return;

            this._isStreaming = true;
            this.StartCoroutine(this.StreamFrame());
        }

        private IEnumerator StreamFrame()
        {
            int width = this.cam.pixelWidth,
                          height = this.cam.pixelHeight;
            yield return new WaitForEndOfFrame();

            // obtem imagem da camera em um renderer
            this.cam.targetTexture = this._render;
            this.cam.Render();
            this.cam.targetTexture = null;

            this._scaler.TargedWidth = this.xResolution;
            this._scaler.TargedHeight = this.xResolution * height / width;
            if (this._outTexture.width != this._scaler.TargedWidth || this._outTexture.height != this._scaler.TargedHeight) {
                ImageCores.Utils.TextureUtil.ReinitializeTexture(ref this._outTexture, this._scaler.TargedWidth, this._scaler.TargedHeight, true);
                this._texture = ImageCores.Utils.TextureUtil.ReinitializeTexture(this._texture, this._scaler.TargedWidth, this._scaler.TargedHeight);
            }
            Graphics.Blit(this._scaler.Enhance(this._render), this._outTexture);
            yield return new WaitForEndOfFrame();

            if (Process)
            {
                RenderTexture.active = this._outTexture;
                this._texture.ReadPixels(new Rect(0, 0, this._outTexture.width, this._outTexture.height), 0, 0);
                RenderTexture.active = null;
                byte[] bytesColors = _texture.EncodeToJPG();

                ReferenceModels.Image img = new ReferenceModels.Image(bytesColors, this._outTexture.width, this._outTexture.height);
                this.PublishMessage(this.Topics[0], UnityEngine.JsonUtility.ToJson(img, true));
                Debug.Log("$MQTT image sended.");
            }
            yield return new WaitForEndOfFrame();

            this.TimeCount = Time.unscaledTime;
            //Debug.Log(Time.unscaledTime - timeStart);
            this._isStreaming = false;
        }
    }
}




/*
          // textura 2D da imagem do renderer
          Texture2D cTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
          RenderTexture.active = this._render;
          cTexture.ReadPixels(new Rect(0, 0, this._render.width, this._render.height), 0, 0);
          RenderTexture.active = null;
          this._render = null;
          yield return new WaitForEndOfFrame();

          // ajusta escala
          //byte[] new_bytes, bytes = cTexture.EncodeToPNG();
          //this._scaler.Enhance(ref bytes, width, height, width * height  * 4, out new_bytes);
          this._scaler.Enhance(ref cTexture);
          width = this._scaler.TargedWidth;
          height = this._scaler.TargedHeight;
          yield return new WaitForEndOfFrame();
          */