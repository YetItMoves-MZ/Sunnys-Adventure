using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterSO", menuName = "ScriptableObjects/MonsterStats")]
public class MonsterStatsSO : ScriptableObject
{
    public float speed;
    public float windUpTime;
}
