namespace MyTownProject.Interaction
{
    public interface IInteractable
    {
        float MaxRange { get; }
        bool CanBeInteractedWith { get; }


        void OnFocus(string _string);
        void OnInteract();
        void OnLoseFocus();
    }
}