using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float damage;
    public float cadence;
    public bool dispersion;
    public int ammo;
    public bool melee;
    public bool isInversionWeapon;
    public bool isBazooka;
    [HideInInspector]
    public bool attacking;

    private Character ownerCharacter;
    private float timer;
    private List<GameObject> bullets;
    private AudioClip noBulletsAudio;
    public AudioClip shootAudio;
    private AudioSource audioSource;

    private void Awake()
    {
        bullets = new List<GameObject>();
        var bullet = Resources.Load<GameObject>("Prefabs/Bullet/Bullet");
        noBulletsAudio = Resources.Load<AudioClip>("Audio/NoBullet");

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
        if (this.ownerCharacter != null)
        {
            this.transform.position = this.ownerCharacter.GetWeaponPosition().position;
            timer += Time.deltaTime;

            if (timer >= cadence && this.ownerCharacter.GetJoystickController().IsPressButtonB())
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
        WeaponType weaponType = WeaponType.Bullet;
        if (isInversionWeapon)
        {
            weaponType = WeaponType.Inversion;
        }
        else if (isBazooka)
        {
            weaponType = WeaponType.Bazooka;
        }
        bulletController.ActivateAndMove(this.transform.position, number, ownerCharacter.isRight, weaponType);
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
        if (melee)
        {
            animator.Play("WeaponAnimation");
        }
        else
        {
            animator.Play("SmokeAnimation");
        }
    }

    public void SetOwner(Character character)
    {
        ownerCharacter = character;
    }

    public bool hasOwner()
    {
        return this.ownerCharacter != null;
    }
}
