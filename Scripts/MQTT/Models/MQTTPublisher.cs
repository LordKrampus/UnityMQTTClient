using System;
using UnityEngine;

namespace MQTT.Models
{
    public abstract class MQTTPublisher : MQTTEntity, IMQTTPublisher
    {
        public virtual void PublishMessage(string topic, string message)
        {
            //Debug.Log($".MQTT $publishing ({topic}):\n{message}");
            this.MQTTCC.Publish(topic, message);
        }
    }
}
