//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;

//public class DataSaveManager : MonoBehaviour
//{
//    public List<string> saveDataList = new List<string>();
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    void GenerateData()
//    {
//        saveDataList.Add("Data1");
//        saveDataList.Add("Data2");
//        saveDataList.Add("Data3");
//        saveDataList.Add("Data4");
//        saveDataList.Add("Data5");
//    }

//    void SaveData()
//    {
//        string json = JsonUtility.ToJson(saveDataList, true);
//        string filePath = Application.streamingAssetsPath + "/saveData.json";

//        using (StreamWriter sw = new StreamWriter(filePath))
//        {
//            sw.Write(json);
//            sw.Close();
//            sw.Dispose();
//        }
//    }

//    void LoadData()
//    {
//        string json;
//        string filePath = Application.streamingAssetsPath + "/saveData.json";

//        if (File.Exists(filePath))
//        {
//            using (StreamReader sr = new StreamReader(filePath))
//            {
//                json = sr.ReadToEnd();
//                sr.Close();

//            }
//            saveDataList = JsonUtility.FromJson<List<string>>(json);
//        }
//        else
//        {
//            GenerateData();
//        }

//    }

//    public void OnClickSaveButton()
//    {
//        SaveData();
//    }
//}
