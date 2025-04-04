using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Scripting;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    private float saveTimer = 20;
    void Start()
    {
        //PipeHolder.StartAIAgent();
        //SaveSystem.Load();
    }
    private void Update()
    {
        //if (saveTimer < 0)
        //{
        //    SaveSystem.Save();
        //    saveTimer = 20;
        //}
        //saveTimer -= Time.deltaTime;
    }
}
