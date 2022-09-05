using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ODU.Data
{

    [System.Serializable]
    public class WKVector3
    {
        [JsonProperty]
        public float x;
        [JsonProperty]
        public float y;
        [JsonProperty]
        public float z;
        [JsonIgnore]
        public Vector3 UnityVector
        {
            get
            {
                return new Vector3(x, y, z);
            }
        }
        public WKVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
        /// <summary>
        /// Convert and return WKVEctor3 List
        /// </summary>
        /// <param name="vList"></param>
        /// <returns></returns>
        public static List<WKVector3> GetSerialList(List<Vector3> vList)
        {
            List<WKVector3> list = new List<WKVector3>(vList.Count);
            for (int i = 0; i < vList.Count; i++)
            {
                list.Add(new WKVector3(vList[i]));
            }
            return list;
        }
        /// <summary>
        /// Convert and return Vector3 List from WKVector List
        /// </summary>
        /// <param name="vList"></param>
        /// <returns></returns>
        public static List<Vector3> GetSerialList(List<WKVector3> vList)
        {
            List<Vector3> list = new List<Vector3>(vList.Count);
            for (int i = 0; i < vList.Count; i++)
            {
                list.Add(vList[i].UnityVector);
            }
            return list;
        }
    }
    [System.Serializable]
    public class WKTransform
    {
        [JsonProperty]
        public string Id { get; set; }
        [JsonProperty]
        public WKVector3 LocalLocation { get; set; }
        [JsonProperty]
        public WKVector3 LocalRotation { get; set; }
        [JsonProperty]
        public WKVector3 LocalScale { get; set; }
    }
}