using System;
using UnityEngine;
public class Entity : MonoBehaviour
{

    [SerializeField] private int hitPoints;
    [SerializeField] private int armorClass;

    [SerializeField] private string damage;
    [SerializeField] private int thac0;

    public int HitPoints
    {
        get { return hitPoints; }
        set { hitPoints = value; }
    }

    public int ArmorClass
    {
        get { return armorClass; }
    }

    public string Damage
    {
        get { return damage; }
    }

    public int Thac0
    {
        get { return thac0; }
    }
}