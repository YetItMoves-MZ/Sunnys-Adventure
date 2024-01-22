using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSignSO", menuName = "ScriptableObjects/Sign")]
public class SignSO : ScriptableObject
{
    public List<string> Content = new List<string>();
    public Color color;
}
