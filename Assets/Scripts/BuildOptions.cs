using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BuildPlatform {
    DESKTOP,
    MOBILE
}

public class BuildOptions : MonoBehaviour
{
    [SerializeField]
    BuildPlatform buildPlatform;
    [SerializeField]
    private bool debugMode;
    [SerializeField]
    private GameObject[] desktopObjects;    // GameObjects that should only be active for the Desktop version
    [SerializeField]
    private GameObject[] mobileObjects;
    [SerializeField]
    private GameObject[] debugObjects;

    void Start()
    {
        if (buildPlatform == BuildPlatform.DESKTOP) {
            foreach (GameObject obj in desktopObjects) {
                obj.SetActive(true);
            }
            foreach (GameObject obj in mobileObjects) {
                obj.SetActive(false);
            }
        } else if (buildPlatform == BuildPlatform.MOBILE) {
            foreach (GameObject obj in mobileObjects) {
                obj.SetActive(true);
            }
            foreach (GameObject obj in desktopObjects) {
                obj.SetActive(false);
            }
        }
        if (debugMode) {
            foreach (GameObject obj in debugObjects) {
                obj.SetActive(true);
            }
        } else {
            foreach (GameObject obj in debugObjects) {
                obj.SetActive(false);
            }
        }
    }

}
