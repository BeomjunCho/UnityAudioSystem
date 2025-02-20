using System.Collections;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]
public class Enemy : MonoBehaviour
{
    // Speed at which the enemy moves.
    public float speed = 5f;
    // Define the left and right limits along the x-axis.
    public float leftLimit = -50f;
    public float rightLimit = 50f;

    // audio
    private StudioEventEmitter emitter;

    void Start()
    {
        // Ensure AudioManager is not null before calling it
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager instance is missing!");
            return;
        }

        if (FmodEvents.Instance == null)
        {
            Debug.LogError("FmodEvents instance is missing!");
            return;
        }

        // Ensure event is assigned properly
        if (FmodEvents.Instance.enemy.IsNull)
        {
            Debug.LogError("Enemy FMOD event is null!");
            return;
        }

        emitter = AudioManager.Instance.InitializeEventEmitter(FmodEvents.Instance.enemy, this.gameObject);
        if (emitter == null)
        {
            Debug.LogError("Failed to initialize FMOD Event Emitter!");
            return;
        }
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
            emitter.Play();
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
