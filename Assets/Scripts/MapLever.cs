using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MapLever : MonoBehaviour
{
    public int iLeveToLoad;
    public string sLevelToLoad;

    public bool useTntegerToLoadLevel = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionGameObject = collision.gameObject;

        if(collisionGameObject.name =="Player")
        {
            LoadSceneMode();
        }
    }

    void LoadSceneMode()
    {
        if(useTntegerToLoadLevel)
        {
            SceneManager.LoadScene(iLeveToLoad);
        }
        else
        {
            SceneManager.LoadScene(iLeveToLoad);
        }
    }
}

