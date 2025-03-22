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
    void Start()
    {
        PipeHolder.StartAIAgent();

    }
    private void Update()
    {
    }
}
