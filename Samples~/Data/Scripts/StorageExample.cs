using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ODU.Data
{
    public class StorageExample : MonoBehaviour
    {
        [Tooltip("Object we want the data for it's position")]
        public Transform RandomObjectData;
        [Tooltip("Object to then visualize the data on")]
        public Transform ByteObject;
        public Transform JSONObject;
        
        [Tooltip("Override the file?")]
        public bool OverrideFile = true;
        [Tooltip("Use JSON?")]
        public bool UseJson = false;
        [Tooltip("Use Byte Array?")]
        public bool UseByteArray = false;
        [Space]
        [Header("Keyboard Inputs")]
        public KeyCode SaveFile;
        public KeyCode StoreSomeData;
        public KeyCode ReadTheDataBack;
        //private variables
        private bool _dataSaved;
        byte[] rawData;
        
        [SerializeField]
        List<WKTransform> AllTransformData = new List<WKTransform>();
        
        /// <summary>
        /// Unity Generic Update to grab user input for testing
        /// </summary>
        public void Update()
        {
            if (Input.GetKeyDown(StoreSomeData))
            {
                if (UseByteArray || UseJson)
                {
                    StoreRuntimeData();
                }
            }
            if (Input.GetKeyDown(SaveFile))
            {
                if (UseByteArray)
                {
                    WriteByteFile();

                }
                if (UseJson)
                {
                    WriteJSONFile();
                }
            }
            if (Input.GetKeyDown(ReadTheDataBack))
            {
                RandomObjectData.GetComponent<MeshRenderer>().enabled = false;
                if (UseByteArray)
                {
                    ReadByteFile();
                }
                if (UseJson)
                {
                    ReadJSONFile();
                }
            }
        }
        public void StoreRuntimeData()
        {
            //create a new transform
            WKTransform wkTransform = new WKTransform();
            //set the transform to the current position of the object
            wkTransform.LocalLocation = new WKVector3 (RandomObjectData.localPosition);
            wkTransform.LocalRotation = new WKVector3(RandomObjectData.localRotation.eulerAngles);
            wkTransform.LocalScale = new WKVector3(RandomObjectData.localScale);
            wkTransform.Id = RandomObjectData.name;
            //add the transform to the list
            AllTransformData.Add(wkTransform);
        }
        public void ReadByteFile()
        {
            if (_dataSaved)
            {
                //go open the file
                string folderPath = Path.Combine(Application.persistentDataPath, "TestData");
                string filePath = Path.Combine(folderPath, "textfile.txt");
                FileStream stream = File.OpenRead(filePath);
                rawData = new byte[stream.Length];
                stream.Read(rawData, 0, rawData.Length);
                stream.Close();
                //now we have the data saved as a byte array
                //clear my current runtime data (we want to use the information from the file)
                AllTransformData.Clear();
                //pass the data from the file and convert it with the Utility class to the correct format
                AllTransformData = Utility.Instance.ReturnListWKStructTransform(rawData);
                //visualize and show this working
                StartCoroutine(GOThroughList(ByteObject));
            }
            else
            {
                Debug.LogWarning($"No data has been saved yet, please save the data first by hitting the {SaveFile.ToString()} key");
            }
        }
       
        public void WriteByteFile()
        {
            rawData = Utility.Instance.ReturnByteArrayFromList(AllTransformData.ToList());
            string folderPath = Path.Combine(Application.persistentDataPath, "TestData");
            Utility.Instance.CreateDirectory(folderPath, true);
            string textPath = Path.Combine(folderPath, "textfile.txt");
            Debug.Log($"FilePath:{textPath}");
            using (Stream file = File.Open(textPath, FileMode.OpenOrCreate))
            {
                file.Write(rawData, 0, rawData.Length);
            }
            _dataSaved = true;
        }

        
        public void WriteJSONFile()
        {
            string folderPath = Path.Combine(Application.persistentDataPath, "TestData");
            Utility.Instance.WriteJSONFile(folderPath, "textfile.json",true, AllTransformData);
            _dataSaved = true;
        }
        public void ReadJSONFile()
        {
            //clear struct data
            if (_dataSaved)
            {
                AllTransformData.Clear();
                string folderPath = Path.Combine(Application.persistentDataPath, "TestData");
                var jsonWork = Utility.Instance.ReadJSONFile(Path.Combine(folderPath, "textfile.json"), ref AllTransformData);
                if (jsonWork)
                {
                    //play it
                    StartCoroutine(GOThroughList(JSONObject));
                }
                else
                {
                    Debug.LogError($"Had an issue reading the JSON data");
                }
            }
            else
            {
                Debug.LogWarning($"Need to save the data first..");
            }
            
        }
        #region Function to Loop through Data
        IEnumerator GOThroughList(Transform otherItem)
        {
            for (int i = 0; i < AllTransformData.Count; i++)
            {
                yield return new WaitForSecondsRealtime(1f);
                var firstTransform = AllTransformData[i];
                otherItem.position = firstTransform.LocalLocation.UnityVector;
                otherItem.rotation = Quaternion.Euler(firstTransform.LocalRotation.UnityVector);
                otherItem.localScale = firstTransform.LocalScale.UnityVector;
            }
        }
        #endregion
    }
}