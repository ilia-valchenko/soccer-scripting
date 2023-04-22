using System.Collections;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private const int Speed = 500;
    private const int TurboSpeed = 1000;

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup

    private Rigidbody _playerRb;
    private GameObject _focalPoint;

    public bool hasPowerup;
    public int powerUpDuration = 5;
    public GameObject powerupIndicator;
    public ParticleSystem smokeParticle;

    void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
        _focalPoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = Input.GetAxis("Vertical");
        int speed = Input.GetKey(KeyCode.Space) ? TurboSpeed : Speed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.smokeParticle.Play();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            this.smokeParticle.Stop();
        }

        _playerRb.AddForce(_focalPoint.transform.forward * verticalInput * speed * Time.deltaTime);

        // Set powerup indicator position to beneath player
        this.powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);
        this.smokeParticle.transform.position = transform.position;
    }

    // If Player collides with powerup, activate powerup
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            this.hasPowerup = true;
            Destroy(other.gameObject);
            this.powerupIndicator.gameObject.SetActive(true);
           StartCoroutine(PowerupCooldown());
        }
    }

    // Coroutine to count down powerup duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position;

            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }
        }
    }
}
