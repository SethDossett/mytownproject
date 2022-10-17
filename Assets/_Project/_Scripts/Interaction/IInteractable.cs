using UnityEngine;
namespace MyTownProject.Interaction
{
    public interface IInteractable
    {
        bool IsVisible { get; }
        float MaxNoticeRange { get; }
        float MaxNoticeAngle { get; }
        bool DoesAngleMatter { get; }
        float MaxInteractRange { get; }
        bool CanBeInteractedWith { get; }
        bool CanBeTargeted { get; }
        string Prompt { get; }
        bool Hovered { get; }
        bool Targeted { get; }
        bool BeenTargeted { get; }
        Vector3 InteractionPointOffset { get; }


        void OnFocus(string _string);
        void OnInteract(TargetingSystem targetingSystem);
        void OnLoseFocus();
        void SetHovered(bool setTrue);
        void SetTargeted(bool setTrue);
        void SetBeenTargeted(bool setTrue);
    }
}