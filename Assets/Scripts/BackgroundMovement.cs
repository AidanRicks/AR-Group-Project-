using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    [SerializeField] private Transform camera;
    private bool canFollow = false;

    private void Update()
    {
        if (canFollow == true)
        {
            transform.position = new Vector3
            (
                camera.position.x,
                camera.position.y,
                transform.position.z
            );
        }
    }


        private void OnTrigger2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                // Player in the box collider = following
                canFollow = true;
            }
        }

}