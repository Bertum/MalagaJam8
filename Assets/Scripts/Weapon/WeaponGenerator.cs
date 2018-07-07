using System.Collections.Generic;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour
{
    private float timer;
    private float timeToNextWeapon;
    private List<GameObject> prefabsList;

    private void Awake()
    {
        prefabsList = new List<GameObject>();
        timeToNextWeapon = Random.Range(3, 10);
        prefabsList.Add(Resources.Load<GameObject>("Prefabs/Weapons/Bazooka"));
        prefabsList.Add(Resources.Load<GameObject>("Prefabs/Weapons/InversionWeapon"));
        prefabsList.Add(Resources.Load<GameObject>("Prefabs/Weapons/Katana"));
        prefabsList.Add(Resources.Load<GameObject>("Prefabs/Weapons/Knife"));
        prefabsList.Add(Resources.Load<GameObject>("Prefabs/Weapons/Pan"));
        prefabsList.Add(Resources.Load<GameObject>("Prefabs/Weapons/Pistol"));
        prefabsList.Add(Resources.Load<GameObject>("Prefabs/Weapons/Shotgun"));
        prefabsList.Add(Resources.Load<GameObject>("Prefabs/Weapons/Uzi"));
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToNextWeapon)
        {
            var rnd = Random.Range(0, 8);
            var selectedPrefab = prefabsList[rnd];
            var weapon = Instantiate(selectedPrefab, this.transform.position, Quaternion.identity);
            timer = 0;
        }
    }
}
