using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


[CreateAssetMenu(fileName = "Data", menuName = "Charaters", order = 1)]
public class Character : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public int sortingLayer;
    public UnityEditor.Animations.AnimatorController controller;

}
