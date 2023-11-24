using System;
using System.Collections;
using UnityEngine;

namespace MQTT.Models
{
    public abstract class MQTTListenner : MQTTEntity, IMQTTListenner
    {
        public virtual void ProcessMessage(string topic, string message)
        {
            Debug.Log($".MQTT $listening ({topic}):\n{message}");
        }
    }
}
