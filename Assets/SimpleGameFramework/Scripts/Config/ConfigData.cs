using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleGameFramework.Config
{
    /// <summary>
    /// 配置数据
    /// </summary>
    public struct ConfigData
    {
        public ConfigData(bool boolValue, int intValue, float floatValue, string stringValue)
        {
            BoolValue = boolValue;
            IntValue = intValue;
            FloatValue = floatValue;
            StringValue = stringValue;
        }

        public bool BoolValue { get; private set; }
        public int IntValue { get; private set; }
        public float FloatValue { get; private set; }
        public string StringValue { get; private set; }
    }

}
