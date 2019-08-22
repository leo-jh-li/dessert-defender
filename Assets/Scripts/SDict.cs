using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple, serializable dictionary that is visible in the inspector
[System.Serializable]
public class SDict<K,V>
{
    [SerializeField]
    public K key;
    [SerializeField]
    public V value;
}
