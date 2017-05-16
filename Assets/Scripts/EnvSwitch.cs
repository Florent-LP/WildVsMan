using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvSwitch : MonoBehaviour {

    [SerializeField] protected GameObject urbanEnv = null;
    [SerializeField] protected GameObject wildEnv = null;

    public void switchUrban()
    {
        if (wildEnv != null) wildEnv.SetActive(false);
        if (urbanEnv != null) urbanEnv.SetActive(true);
    }

    public void switchWild()
    {
        if (urbanEnv != null) urbanEnv.SetActive(false);
        if (wildEnv != null) wildEnv.SetActive(true);
    }

    private void Start()
    {
        switchWild();
    }
}
