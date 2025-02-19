using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public AudioClip ShootingClip;
    public GameObject bullet;
    public Transform spawnPoint;
    public float shootForce = 10f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject newBullet = Instantiate(bullet, spawnPoint.position, Quaternion.identity);
            Rigidbody rb = newBullet.GetComponent<Rigidbody>();
            rb.AddForce(spawnPoint.forward * shootForce, ForceMode.Impulse);

            StartCoroutine(DestroyBullet(newBullet));
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            SFX3DManager.Instance.Stop3dSound("Shooting");
        }
    }

    IEnumerator DestroyBullet(GameObject Bullet)
    {
        yield return new WaitForSeconds(3f);

        if (bullet != null)
        {
            Destroy(Bullet);
        }
    }
}
