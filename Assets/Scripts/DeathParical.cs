using UnityEngine;

public class DeathParical : MonoBehaviour
{
    [SerializeField] private GameObject _deathParthical;
    private void OnDestroy()
    {
        Instantiate(_deathParthical,transform.position, transform.rotation);
    }
}
