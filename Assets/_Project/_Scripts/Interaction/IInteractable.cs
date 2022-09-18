namespace MyTownProject.Interaction
{
    public interface IInteractable
    {
        bool IsVisible { get; }
        float MaxNoticeRange { get; }
        float MaxInteractRange { get; }
        bool CanBeInteractedWith { get; }
        bool CanBeTargeted { get; }
        string Prompt { get; }


        void OnFocus(string _string);
        void OnInteract(TargetingSystem targetingSystem);
        void OnLoseFocus();
    }
}