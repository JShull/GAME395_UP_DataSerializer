using System.Collections.Generic;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Text;

namespace ODU.Data{

    public class Utility:MonoBehaviour{
        public static Utility Instance {get; private set;}
        private char[] invalidFilenameChars;
        private char[] invalidPathChars;
        private char[] parseTextImagefileChars;
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                invalidFilenameChars = Path.GetInvalidFileNameChars();
                invalidPathChars = Path.GetInvalidPathChars();
                parseTextImagefileChars = new char[] { '.', '/', '\\', ' ', '(', ')', '[', ']', '{', '}', '<', '>', ':', ';', ',', '\'', '\"', '|', '?', '!', '@', '#', '$', '%', '^', '&', '*', '-', '+', '=', '~', '`' };
                Array.Sort(invalidFilenameChars);
                Array.Sort(invalidPathChars);
            }
            else
            {
                Destroy(this);
            }
        }
        #region Byte Converter Example Functions
        /// <summary>
        /// Some direct byte array conversions to a specific example class WKStructTransform
        /// Convert a byte Array back to a List<WKStructTransform>
        /// </summary>
        /// <param name="PassedDict"></param>
        /// <returns></returns>
        public List<WKTransform> ReturnListWKStructTransform(byte[] PassedDict){
            List<WKTransform> theList = new List<WKTransform>();
            theList=Deserialize<List<WKTransform>>(PassedDict);
            return theList;
        }
        
        /// <summary>
        /// Convert to Byte Array from WKStructTransform
        /// </summary>
        /// <param name="theList"></param>
        /// <returns></returns>
        public byte[] ReturnByteArrayFromList(List<WKTransform> theList){
            return Serialize(theList);
        }
        /// <summary>
        /// Convert string to byte array
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        public byte[] ReturnByteArray(string stringData)
        {
            return System.Text.Encoding.Default.GetBytes(stringData);
        }

        #endregion
        #region De/Serialization Functions
        /// <summary>
        /// Serialize a class/struct to a file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private byte[] Serialize<T>(T obj)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binSerializer = new BinaryFormatter();
                binSerializer.Serialize(memStream, obj);
                return memStream.ToArray();
            }
        }

        /// <summary>
        /// Deserialize a class/struct from a file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedObj"></param>
        /// <returns></returns>
        private T Deserialize<T>(byte[] serializedObj)
        {
            T obj = default(T);
            if (serializedObj == null)
            {
                return obj;
            }
            if (serializedObj.Length == 0)
            {
                return obj;
            }
            using (MemoryStream memStream = new MemoryStream(serializedObj))
            {
                BinaryFormatter binSerializer = new BinaryFormatter();
                obj = (T)binSerializer.Deserialize(memStream);
            }
            return obj;
        }
        #endregion
        #region JSON Converter Functions
        /// <summary>
        /// Read a json file with a path and return that type and if it worked
        /// </summary>
        /// <typeparam name="T">the type to deserialize the data into</typeparam>
        /// <param name="path">full path to the file</param>
        /// <param name="dataType">our data type</param>
        /// <returns></returns>
        public bool ReadJSONFile<T>(string path, ref T dataType)
        {
            if (File.Exists(path))
            {
                using (StreamReader file = new System.IO.StreamReader(path))
                {

                    try
                    {
                        var modelData = file.ReadToEnd();
                        dataType = JsonConvert.DeserializeObject<T>(modelData);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error on Data {ex.Message}");
                        return false;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"No MRegion file at {path}");
                return false;
            }
        }
        /// <summary>
        /// Write an object of T as JSON to a file
        /// </summary>
        /// <typeparam name="T">data class we are working with</typeparam>
        /// <param name="directory">the Directory</param>
        /// <param name="fileName">the fileName</param>
        /// <param name="overwriteFile">overwrite?</param>
        /// <param name="dataType">our data class/struct</param>
        public void WriteJSONFile<T>(string directory, string fileName, bool overwriteFile,T allSTructData)
        {
            ///If we need to do anything special with the MRegion data type
            var cleanFilePath = SanitizePath(directory, '~');
            var cleanFileName = SanitizeFileName(fileName, '~');
            var fullPath = Path.Combine(cleanFilePath, cleanFileName);
            Debug.LogWarning($"Full Clean Path: {fullPath}");
            ///this is just to make sure our users don't mess up directory and/or filename issues
            string _class = JsonConvert.SerializeObject(allSTructData, Formatting.Indented, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                TypeNameHandling = TypeNameHandling.Auto
            });
            if (File.Exists(fullPath))
            {
                if (overwriteFile)
                {
                    File.Delete(fullPath);
                    Debug.LogWarning($"Deleting old content at {fullPath}");
                }
                else
                {
                    Debug.LogWarning($"File already exists at {fullPath}, appending to the bottom");
                }
            }
            else
            {
                using (FileStream fs = File.Create(fullPath))
                {
                }
                File.WriteAllText(fullPath, string.Empty);
                Debug.LogWarning($"Created a new file at {fullPath}");
            }
            File.WriteAllText(fullPath, _class);
            return;
        }
        
        #endregion
        #region File IO Utilities

        /// <summary>
        /// Created a directory at a path
        /// </summary>
        /// <param name="path">path for the directory</param>
        /// <param name="overrideName">override or not?</param>
        /// <returns></returns>
        public bool CreateDirectory(string path, bool overrideName)
        {
            try
            {
                // Determine whether the directory exists.
                var pathExist = Directory.Exists(path);

                if (pathExist && !overrideName)
                {
                    Debug.LogWarning($"That path exists already: {path}, and you decided to not override");
                    return false;
                }
                if (pathExist && overrideName)
                {
                    Debug.LogWarning($"Directory already existed but you decided to overwrite this at {path}");
                    DirectoryInfo tempDir = new DirectoryInfo(path);
                    //clean them up
                    Debug.LogWarning($"DirectoryInfo Name: {tempDir.Name}");
                    if (tempDir.Name != "Images")
                    {
                        foreach (FileInfo file in tempDir.GetFiles())
                        {
                            file.Delete();
                        }
                    }

                    foreach (DirectoryInfo dir in tempDir.GetDirectories())
                    {
                        //check if it is "Images"
                        if (dir.Name != "Images")
                        {
                            Debug.LogWarning($"Directory Name: {dir.Name}");
                            foreach (FileInfo file in dir.GetFiles())
                            {
                                file.Delete();
                            }
                            dir.Delete();
                        }
                    }

                    return true;
                }
                if (!pathExist)
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                    Debug.LogWarning($"The directory at {path} was created.");
                    return true;
                }
                // Try to create the directory.


                // Delete the directory.
            }
            catch (Exception e)
            {
                Debug.LogError($"The process failed: {e.ToString()}");
                return false;
            }
            return false;
        }
        public string SanitizeFileName(string input, char errorChar)
        {
            return Sanitize(input, invalidFilenameChars, errorChar);
        }
        public string SanitizePath(string input, char errorChar)
        {
            return Sanitize(input, invalidPathChars, errorChar);
        }
        private string Sanitize(string input, char[] invalidChars, char errorChar)
        {
            // null always sanitizes to null
            if (input == null) { return null; }
            StringBuilder result = new StringBuilder();
            foreach (var characterToTest in input)
            {
                // we binary search for the character in the invalid set. This should be lightning fast.
                if (Array.BinarySearch(invalidChars, characterToTest) >= 0)
                {
                    // we found the character in the array of 
                    result.Append(errorChar);
                }
                else
                {
                    // the character was not found in invalid, so it is valid.
                    result.Append(characterToTest);
                }
            }
            return result.ToString();
        }
        
        #endregion
    }
    
    
}
