using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public AudioClip testSound;
    // Speed at which the enemy moves.
    public float speed = 5f;
    // Define the left and right limits along the x-axis.
    public float leftLimit = -50f;
    public float rightLimit = 50f;

    void Start()
    {
        // Start the coroutine when the game begins.
        StartCoroutine(MoveEnemy());
    }

    /// <summary>
    /// Coroutine that moves the enemy back and forth between leftLimit and rightLimit.
    /// </summary>
    IEnumerator MoveEnemy()
    {
        while (true)
        {
            // Move from the current position to the right limit.
            yield return StartCoroutine(MoveToPosition(new Vector3(rightLimit, transform.position.y, transform.position.z)));
            // Then move from the right limit to the left limit.
            yield return StartCoroutine(MoveToPosition(new Vector3(leftLimit, transform.position.y, transform.position.z)));
        }
    }

    /// <summary>
    /// Coroutine to smoothly move the enemy to a specified target position.
    /// </summary>
    /// <param name="target">The target position to move towards.</param>
    IEnumerator MoveToPosition(Vector3 target)
    {
        // Continue moving until the enemy is very close to the target.
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            // Move the enemy closer to the target each frame.
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null; // Wait for the next frame before continuing.
        }
        // Ensure the enemy's position is exactly the target (optional).
        transform.position = target;
    }
}
