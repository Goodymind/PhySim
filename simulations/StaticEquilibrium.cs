using Godot;
using System;
namespace PhySim.simulations
{
    public class StaticEquilibrium : Node
    {
        private double Acceleration = 9.81;
        private double LeftTorque => LeftMassSlider.Value * LeftRadiusSlider.Value * Acceleration;
        private double RightTorque => RightMassSlider.Value * RightRadiusSlider.Value * Acceleration;
        private HSlider LeftMassSlider => GetNode<HSlider>("CanvasLayer/Control/LeftObject/Mass/Value");
        private HSlider LeftRadiusSlider => GetNode<HSlider>("CanvasLayer/Control/LeftObject/Radius/Value");
        private HSlider RightMassSlider => GetNode<HSlider>("CanvasLayer/Control/RightObject/Mass/Value");
        private HSlider RightRadiusSlider => GetNode<HSlider>("CanvasLayer/Control/RightObject/Radius/Value");

        private Sprite LeftMass =>  GetNode<Sprite>("Seesaw/Pivot/Bar/Left");
        private Sprite RightMass => GetNode<Sprite>("Seesaw/Pivot/Bar/Right");
        //can the program edit it?
        enum Targets { LeftMass, LeftRadius, RightMass, RightRadius }
        Targets Target = Targets.LeftMass;

        private CheckButton[] CheckButtons = new CheckButton[4];
        private HSlider[] Sliders = new HSlider[4];
        public override void _Ready()
        {
            CheckButtons[0] = GetNode<CheckButton>("CanvasLayer/Control/LeftObject/Mass/CheckButton");
            CheckButtons[1] = GetNode<CheckButton>("CanvasLayer/Control/LeftObject/Radius/CheckButton");
            CheckButtons[2] = GetNode<CheckButton>("CanvasLayer/Control/RightObject/Mass/CheckButton");
            CheckButtons[3] = GetNode<CheckButton>("CanvasLayer/Control/RightObject/Radius/CheckButton");

            Sliders[0] = LeftMassSlider;
            Sliders[1] = LeftRadiusSlider;
            Sliders[2] = RightMassSlider;
            Sliders[3] = RightRadiusSlider;
            ChangeTarget(0);
        }
        public override void _PhysicsProcess(float delta)
        {
            switch (Target)
            {
                case Targets.LeftMass:
                    LeftMassSlider.Value = RightTorque / (Acceleration * LeftRadiusSlider.Value);
                    break;
                case Targets.LeftRadius:
                    LeftRadiusSlider.Value = RightTorque / (Acceleration * LeftMassSlider.Value);
                    break;
                case Targets.RightMass:
                    RightMassSlider.Value = LeftTorque / (Acceleration * RightRadiusSlider.Value);
                    break;
                case Targets.RightRadius:
                    RightRadiusSlider.Value = LeftTorque / (Acceleration * RightMassSlider.Value);
                    break;
            }
            LeftMass.Position = new Vector2((float)-LeftRadiusSlider.Value * 15, -570 * LeftMass.Scale.y / 2);
            RightMass.Position = new Vector2((float)RightRadiusSlider.Value * 15, -570 * RightMass.Scale.y / 2);
            LeftMass.Scale = Vector2.One * Mathf.Clamp((Mathf.Lerp(0.1f, 0.3f, (float)LeftMassSlider.Value / 5)), 0.1f, 0.5f);
            RightMass.Scale = Vector2.One * Mathf.Clamp(Mathf.Lerp(0.1f, 0.3f, (float)RightMassSlider.Value / 5), 0.1f, 0.5f);
        }
        public override void _Process(float delta)
        {
            Display();
        }
        //call in process
        private void Display()
        {
            GetNode<Label>("CanvasLayer/Control/LeftObject/Mass/Value/Label").Text = "Mass (kg) = " + Math.Round(LeftMassSlider.Value,2);
            GetNode<Label>("CanvasLayer/Control/LeftObject/Radius/Value/Label").Text = "Distance (m) = " + Math.Round(LeftRadiusSlider.Value,2);
            GetNode<Label>("CanvasLayer/Control/RightObject/Mass/Value/Label").Text = "Mass (kg) = " + Math.Round(RightMassSlider.Value,2);
            GetNode<Label>("CanvasLayer/Control/RightObject/Radius/Value/Label").Text = "Distance (m) = " + Math.Round(RightRadiusSlider.Value,2);
            GetNode<Label>("CanvasLayer/Control/LeftObject/Torque/Label").Text = "Torque (Nm) = " + Math.Round(LeftTorque,2);
            GetNode<Label>("CanvasLayer/Control/RightObject/Torque/Label").Text = "Torque (Nm) = " + Math.Round(RightTorque,2);
        }
        private void ChangeTarget(int target)
        {
            Target = (Targets)target;
            for (int i = 0; i < CheckButtons.Length; i++)
            {
                if (i == target)
                {
                    CheckButtons[i].Disabled = true;
                    Sliders[i].Editable = false;
                    Sliders[i].Step = 0;
                    continue;
                }
                CheckButtons[i].Pressed = false;
                CheckButtons[i].Disabled = false;
                Sliders[i].Editable = true;
                Sliders[i].Step = 0.1;
            }
        }

    }
}