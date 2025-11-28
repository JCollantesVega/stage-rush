using Postgrest.Attributes;
using Postgrest.Models;
using UnityEngine;

public class Car : BaseModel
{
    
    public long Id;

    public string Model;
    public GameObject Prefab;
}