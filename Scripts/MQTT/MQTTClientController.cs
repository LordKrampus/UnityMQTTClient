using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt.Messages;

using MQTT.Models;
using UnityEngine.UI;

namespace MQTT 
{

    public class MQTTClientController : M2MqttUnityClient
    {
        private static string LISTENNERS_TOPIC = "notice/listenners";

        private static MQTTClientController instance;
        private string _message = "";
        private bool _isConnected = false;
        [SerializeField]
        private Text messageDisplay;

        private List<string> _eventTopics = new List<string>();
        private List<string> _eventMessages = new List<string>();
        private List<string> _subscribedTopics = new List<string>();
        private List<byte> _qosLevels = new List<byte>();
        [SerializeField]
        private List<MQTTListenner> _listenners = new List<MQTTListenner>();

        private OnConnectionEvent onConnection = null;
        private OnDisconnectionEvent onDisconnection;

        public bool IsConnected => _isConnected;
        public string Message => this._message;

        // connection delegates
        //public delegate void OnDisconnectedEvent();

        public OnConnectionEvent OnConnection
        {
            set => this.onConnection += value;
        }

        public OnDisconnectionEvent OnDisconnectoin
        {
            set => this.onDisconnection += value;
        }

        public delegate void OnConnectionEvent();
        public delegate void OnDisconnectionEvent();

        public static MQTTClientController Instance {
            get
            {
                if (instance == null)
                {
                    var gameObject = new GameObject("MQTTClientController");
                    instance = gameObject.AddComponent<MQTTClientController>();
                    DontDestroyOnLoad(gameObject);
                }
                return instance;
            }
        }

        public void Publish(string topic, string message)
        {
            base.client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        public void Publish(string topic, byte[] message)
        {
            base.client.Publish(topic, message, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        public void AddListenner(MQTTListenner listenner)
        {
            this._listenners.Add(listenner);
            this.Publish(LISTENNERS_TOPIC, "Added new Listenner");
        }

        public void RemoveListenner(MQTTListenner listenner)
        {
            this._listenners.Add(listenner);
        }

        public void SetBrokerAddress(string brokerAddress)
        {
            base.brokerAddress = brokerAddress;
        }

        public void SetBrokerPort(string brokerPort)
        {
            int.TryParse(brokerPort, out base.brokerPort);
        }

        public void SetEncrypted(bool isEncrypted)
        {
            base.isEncrypted = isEncrypted;
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            this._message += "connecting";
        }

        protected override void OnConnected()
        {
            base.OnConnected();

            if(onConnection != null)
                this.onConnection();

            this._isConnected = true;
            this._message = "connected";
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            base.OnConnectionFailed(errorMessage);
            this._message = "!failed to connect.";
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            if (onDisconnection != null)
                this.onConnection();

            this._isConnected = false;
            this._message = "discoonnect";
        }

        protected override void OnConnectionLost()
        {
            base.OnConnectionLost();
            this._message += " !connection Lost.";
        }

        public void SubscribeTopic(string topic, byte qosLevel)
        {
            if (this._subscribedTopics.Contains(topic))
            {
                Debug.LogWarning($".MQTT $Topic ({topic}) already added.");
                return;
            }

            this._subscribedTopics.Add(topic);
            this._qosLevels.Add(qosLevel);

            //this.UnsubscribeTopics();
            //this.SubscribeTopics();

            //client.Subscribe(new string[] { topic }, new byte[] { qosLevel });
        }

        public void UnsubscribeTopic(string topic)
        {
            int index = this._subscribedTopics.IndexOf(topic);
            this._subscribedTopics.RemoveAt(index);
            this._qosLevels.RemoveAt(index);
        }

        protected override void SubscribeTopics()
        {
            client.Subscribe(this._subscribedTopics.ToArray(), this._qosLevels.ToArray());
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(this._subscribedTopics.ToArray());
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            this.StoreMessage(topic, msg);
        }

        private void StoreMessage(string topic, string eventMsg)
        {
            this._eventTopics.Add(topic);
            this._eventMessages.Add(eventMsg);
        }

        /// <summary>
        /// !!! To addd an action of consumption for listenners
        /// </summary>
        /// <param name="msg"></param>
        private void ProcessMessage(string msg, int topicIndex)
        {
            string topic = this._subscribedTopics[topicIndex];
            byte qosLevel = this._qosLevels[topicIndex];
            foreach (MQTTListenner listenner in this._listenners)
            {
                if (listenner.Topics.Contains(topic))
                {
                    listenner.ProcessMessage(topic, msg);
                }
            }
        }

        private int GetTopicId(string topic)
        {
            for (int i = 0; i < this._subscribedTopics.Count; i++)
            {
                if (this._subscribedTopics[i].Equals(topic))
                {
                    return i;
                }
            }
            return -1;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.autoConnect = true;
            base.Start();
        }

        protected override void Update()
        {
            if (!IsConnected)
                return;
            
            base.Update();

            Debug.Log($"$MQTT Update - messages {this._eventMessages.Count}.");
            Debug.Log($"$MQTT Update - listenners {this._listenners.Count}.");

            if (this._eventMessages.Count > 0)
            {
                int topicIndex;
                for (int i = 0; i < this._eventMessages.Count; i++)
                {
                    topicIndex = this.GetTopicId(this._eventTopics[i]);
                    if (topicIndex == -1)
                        continue;
                    this.ProcessMessage(this._eventMessages[i], topicIndex);
                }
                this._eventMessages.Clear();
                this._eventTopics.Clear();
            }
        }

        private void OnEnable()
        {
            base.Connect();
        }

        private void OnDestroy()
        {
            base.Disconnect();
        }

        private void LateUpdate()
        {
            if (this.messageDisplay != null)
                this.messageDisplay.text = this._message;
        }

    } // end : classe

} // end : namespace

