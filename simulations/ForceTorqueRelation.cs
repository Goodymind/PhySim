using Godot;
using System;
namespace PhySim.simulations
{
    public class ForceTorqueRelation : Node
    {
        HSlider Weight => GetNode<HSlider>("CanvasLayer/Control/Main/Weight/HSlider");
        HSlider Angle => GetNode<HSlider>("CanvasLayer/Control/Main/ArmAngle/HSlider");
        HSlider Radius => GetNode<HSlider>("CanvasLayer/Control/Main/Radius/HSlider");
        double Torque => Radius.Value * Weight.Value * Mathf.Sin(Mathf.Deg2Rad((float) (90 - Angle.Value)));
        //t = rf sin angle
        public override void _PhysicsProcess(float delta)
        {
            GetNode<Node2D>("Node2D/Crane/Arm/Hand").GlobalRotation = Mathf.LerpAngle(GetNode<Node2D>("Node2D/Crane/Arm/Hand").GlobalRotation, 0, 9.81f * delta);
            GetNode<Node2D>("Node2D/Crane/Arm").RotationDegrees = (float) Angle.Value - 45;
            GetNode<Node2D>("Node2D/Crane/Arm").Scale = Vector2.One * Mathf.Lerp(0, 2, (float)(Radius.Value / Radius.MaxValue));
            GetNode<Node2D>("Node2D/Crane/Arm/Hand/Weights").GlobalScale = Vector2.One * Mathf.Lerp(0,5,(float)(Weight.Value / Weight.MaxValue)) * .05f;
        }
        public override void _Process(float delta)
        {
            GetNode<Label>("CanvasLayer/Control/Main/Equation/Torque").Text = Math.Round(Torque,2) + "Nm";
            GetNode<Label>("CanvasLayer/Control/Main/Equation/Radius").Text = Radius.Value + "m";
            GetNode<Label>("CanvasLayer/Control/Main/Equation/Force").Text = Weight.Value + "N";
            GetNode<Label>("CanvasLayer/Control/Main/Equation/Angle").Text = (90 - Angle.Value) + "*";

            GetNode<Label>("CanvasLayer/Control/Main/ArmAngle/HSlider/Label").Text = "Force Angle = " + (90 - Angle.Value);
            GetNode<Label>("CanvasLayer/Control/Main/Weight/HSlider/Label").Text = "Weight (N) = " + Weight.Value + "N";
            GetNode<Label>("CanvasLayer/Control/Main/Radius/HSlider/Label").Text = "Radius (m) = " + Radius.Value + "m";
        }

    }
}