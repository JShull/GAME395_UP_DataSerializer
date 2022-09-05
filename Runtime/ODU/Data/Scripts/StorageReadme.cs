using UnityEngine;


namespace ODU.Data
{
    /// <summary>
    /// This is a readme for the Storage system
    /// </summary>
    public class StorageReadme : MonoBehaviour
    {
        [Tooltip("Explains the general scene")]
        [TextArea(5,10)]
        public string Readme="Notice the two Gameobjects under the 'STORAGE STUFF' in the Hierarchy. The 'DATA_ObjectOne' contains the component that will allow you to serialize, deserialize, and write the data to a file.";
        [Tooltip("Explains how to use")]
        [TextArea(5, 10)]
        public string HowTo = "Hit Play, in the scene/editor window grab the DATA_ObjectOne, move it, then in the game window hit the save key <s>, keep doing this for a few movements. When you want to save the file, hit the <spacebar>. If you want to replay what you just did, hit the <d> key.";
        public void Start()
        {
            Debug.LogWarning($"Info: {Readme}");
            Debug.LogWarning($"How to Use: {HowTo}");
        }
    }
}
