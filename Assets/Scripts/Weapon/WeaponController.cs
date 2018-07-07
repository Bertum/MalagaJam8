﻿using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float damage;
    public float cadence;
    public bool dispersion;
    public int ammo;
    public bool melee;
    [HideInInspector]
    public bool attacking;
    private Character ownerCharacter;

    private float timer;
    private List<GameObject> bullets;
    private AudioClip noBulletsAudio;
    private AudioClip shootAudio;
    private AudioSource audioSource;

    private void Awake()
    {
        bullets = new List<GameObject>();
        var bullet = Resources.Load<GameObject>("Prefabs/Bullet/Bullet");
        noBulletsAudio = Resources.Load<AudioClip>("Audio/NoBulletAudio");
        shootAudio = Resources.Load<AudioClip>("Audio/ShootAudio");

        audioSource = GetComponent<AudioSource>();

        attacking = false;

        if (dispersion)
        {
            ammo *= 3;
        }

        for (int i = 0; i < ammo; i++)
        {
            var bulletInstance = Instantiate(bullet, this.transform.position, Quaternion.identity);
            bulletInstance.SetActive(false);
            bullets.Add(bulletInstance);
        }
    }


    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetKey(KeyCode.E) && timer >= cadence)
        {
            if (melee)
            {
                Attack();
            }
            else
            {
                Shoot();
            }
            timer = 0;
        }
    }

    private void Shoot()
    {
        if (ammo > 0)
        {
            if (dispersion)
            {
                for (int i = 0; i < 3; i++)
                {
                    ActivateBullet(i);
                }
            }
            else
            {
                ActivateBullet(0);
            }
            audioSource.clip = shootAudio;
            audioSource.Play();
            PlayAnimation();
        }
        else
        {
            audioSource.clip = noBulletsAudio;
            audioSource.Play();
        }
    }

    private void ActivateBullet(int number)
    {
        var bullet = bullets[ammo - 1];
        var bulletController = bullet.GetComponent<BulletController>();
        bulletController.damage = damage;
        bulletController.ActivateAndMove(this.transform.position, number, ownerCharacter.isRight);
        ammo--;
    }

    private void Attack()
    {
        attacking = true;
        PlayAnimation();
        attacking = false;
    }

    private void PlayAnimation()
    {
        var animator = GetComponentInChildren<Animator>();
        animator.Play("WeaponAnimation");
    }

    public void SetOwner(Character character)
    {
        ownerCharacter = character;
    }
}
