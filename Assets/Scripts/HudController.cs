using UnityEngine;
using UnityEngine.UIElements;

public class HudController : MonoBehaviour
{
    [SerializeField] private UIDocument _doc;
    [SerializeField] private InteractionController _interaction;

    private VisualElement _crosshair;

    private void Awake()
    {
        var root = _doc.rootVisualElement;
        _crosshair = root.Q<VisualElement>("Crosshair");
    }

    private void OnEnable()
    {
        _interaction.OnAimAtInteractableChanged += SetInteractState;
    }

    private void OnDisable()
    {
        _interaction.OnAimAtInteractableChanged -= SetInteractState;
    }

    private void SetInteractState(bool canInteract)
    {
        if (canInteract) _crosshair.AddToClassList("active");
        else _crosshair.RemoveFromClassList("active");
    }
}
