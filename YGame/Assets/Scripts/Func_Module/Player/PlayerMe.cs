using UnityEngine;

public partial class PlayerMe : MonoBehaviour
{
    public static PlayerMe Instance { get; private set; } = null;
    private void Start()
    {
        Instance = this;
    }
    private void Update()
    {

    }
    private void OnDestroy()
    {
        Instance = null;
    }
}