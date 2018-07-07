using UnityEngine;

public class TeleporterController : MonoBehaviour
{

    public GameObject connectedTeleporter;
    private AudioSource audioSource;
    private AudioClip teleporterAudio;
    private Vector3 connectedTeleporterPosition;

    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
        //GET audioclip to reproduce the sound of teleporter
        teleporterAudio = Resources.Load<AudioClip>("Audio/Teleport");
        audioSource.clip = teleporterAudio;
        connectedTeleporterPosition = connectedTeleporter.transform.position;
        connectedTeleporterPosition.y += 0.5f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var characterScript = collision.gameObject.GetComponent<Character>();
        if (collision.gameObject.tag == "Player" && !characterScript.teleported)
        {
            audioSource.Play();
            //Set this id to player to avoid loop using teleporter
            characterScript.teleported = true;
            collision.gameObject.transform.position = connectedTeleporterPosition;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Character>().teleported = false;
    }
}
