using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt.Messages;

using MQTT.Models;

namespace MQTT 
{

    public class MQTTClientController : M2MqttUnityClient
    {
        private static string LISTENNERS_TOPIC = "notice/listenners";

        private static MQTTClientController instance;

        private List<string> _eventTopics = new List<string>();
        private List<string> _eventMessages = new List<string>();
        private List<string> _subscribedTopics = new List<string>();
        private List<byte> _qosLevels = new List<byte>();
        private List<MQTTListenner> _listenners = new List<MQTTListenner>();

        public bool IsConnected => base.client != null;

        public static MQTTClientController Instance {
            get
            {
                if(instance == null)
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
            // <...>
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            // <...>
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

            this.SubscribeTopics();
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

        protected override void OnConnectionFailed(string errorMessage)
        {
            base.OnConnectionFailed(errorMessage);
            // <...>
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            // <...>
        }

        protected override void OnConnectionLost()
        {
            base.OnConnectionLost();
            // <...>
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
            foreach(MQTTListenner listenner in this._listenners)
            {
                if (listenner.Topics.Contains(topic))
                {
                    listenner.ProcessMessage(topic, msg);
                }
            }
        }

        private int GetTopicId(string topic)
        {
            for(int i = 0; i < this._subscribedTopics.Count; i++)
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
            base.autoConnect = true;

            base.Start();
        }

        protected override void Start()
        { 
            // do nothing
        }

        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

            if (this._eventMessages.Count > 0)
            {
                int topicIndex;
                for(int i = 0; i < this._eventMessages.Count; i++)
                {
                    topicIndex = this.GetTopicId(this._eventTopics[i]);
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

    } // end : classe

} // end : namespace

