using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollObjects : MonoBehaviour
{
    private Camera cam;
    private float camLeftEdge;
    private int leftmostIndex;
    private int rightmostIndex;

    [SerializeField]
    private GameObject[] objects;
    [SerializeField]
    private float scrollSpeed;

    void Start() {
        cam = Camera.main;
        camLeftEdge = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)).x;
        leftmostIndex = 0;
        rightmostIndex = objects.Length - 1;
    }

    // Performs a looping scroll-to-the-left effect with all given objects
    void Update() {
        // Scroll all objects
        foreach (GameObject o in objects) {
            o.transform.position -= new Vector3(scrollSpeed * Time.deltaTime, 0, 0);
        }
        
        // If leftmost object has left the viewport, move its position to after the rightmost object
        Bounds bounds = objects[leftmostIndex].GetComponent<Renderer>().bounds;
        if (bounds.center.x + bounds.extents.x < camLeftEdge) {
            objects[leftmostIndex].transform.position = objects[rightmostIndex].transform.position + new Vector3(bounds.size.x, 0, 0);
            rightmostIndex = leftmostIndex;
            leftmostIndex++;
            leftmostIndex = leftmostIndex % objects.Length;
        }
    }
}
