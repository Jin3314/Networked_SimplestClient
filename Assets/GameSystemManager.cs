using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameSystemManager : MonoBehaviour
{

    GameObject inputFieldUserName, inputFieldPassword, buttonSubmit, toggleLogin, toggleCreate;

    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
         
        foreach (GameObject go in allObjects)
        {
            if (go.name == "InputFieldUserName")
                inputFieldUserName = go;
            else if (go.name == "InputFieldPassword")
                inputFieldPassword = go;
            else if (go.name == "ButtonSubmit")
                buttonSubmit = go;
            else if (go.name == "ToggleLogin")
                toggleLogin = go;
            else if (go.name == "ToggleCreate")
                toggleCreate = go;
        }

        buttonSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SubmitButtonPressed()
    {
        Debug.Log("we good!");
    }

}
