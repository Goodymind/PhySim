using Godot;
using System;
namespace PhySim.simulations
{
    public class ConservationOfAngularMomentum : Node
    {
        public double InitialMass => GetNode<HSlider>("Node2D/CanvasLayer/Control/InitialMomentum/Mass/MassValue").Value;
        public double InitialRadius => GetNode<HSlider>("Node2D/CanvasLayer/Control/InitialMomentum/Radius/Radius").Value;
        public double InitialAngularVelocity => GetNode<HSlider>("Node2D/CanvasLayer/Control/InitialMomentum/AngularVelocity/AngularVelocity").Value;
        public HSlider FinalMass => GetNode<HSlider>("Node2D/CanvasLayer/Control/FinalMomentum/Mass/MassValue");
        public HSlider FinalRadius => GetNode<HSlider>("Node2D/CanvasLayer/Control/FinalMomentum/Radius/Radius");
        public HSlider FinalAngularVelocity => GetNode<HSlider>("Node2D/CanvasLayer/Control/FinalMomentum/AngularVelocity/AngularVelocity");
        enum FinalValues { Mass, AngularVelocity, Radius }
        FinalValues Target = FinalValues.AngularVelocity;
        public CheckButton[] CheckButtons = new CheckButton[3];
        public HSlider[] Sliders = new HSlider[3];
        public double InitialAngularMomentum => ComputeAngularMomentum(InitialMass, InitialRadius, InitialAngularVelocity);
        public override void _Ready()
        {
            CheckButtons[0] = GetNode<CheckButton>("%MassCheck");
            CheckButtons[1] = GetNode<CheckButton>("%AngularVelocityCheck");
            CheckButtons[2] = GetNode<CheckButton>("%RadiusCheck");
            Sliders[0] = FinalMass;
            Sliders[1] = FinalAngularVelocity;
            Sliders[2] = FinalRadius;
        }
        public override void _PhysicsProcess(float delta)
        {
            switch (Target)
            {
                case FinalValues.Mass: 
                    FinalMass.Value = InitialAngularMomentum / (FinalRadius.Value * FinalRadius.Value * FinalAngularVelocity.Value); 
                    break;
                case FinalValues.Radius: 
                    FinalRadius.Value = Math.Sqrt((float) (InitialAngularMomentum / (FinalMass.Value * FinalAngularVelocity.Value)));
                    break;
                case FinalValues.AngularVelocity: 
                    FinalAngularVelocity.Value = InitialAngularMomentum / (FinalMass.Value * FinalRadius.Value * FinalRadius.Value);
                break;
            }
            var wheel = GetNode<Spatial>("Spatial/Wheel");
            wheel.Rotation += new Vector3(0,(float)FinalAngularVelocity.Value,0) * delta;
            wheel.Scale = new Vector3((float)FinalRadius.Value, 1, (float)FinalRadius.Value);
            if (wheel.Rotation > new Vector3(0,Mathf.Deg2Rad(360),0))
                wheel.Rotation = new Vector3(0,wheel.Rotation.y % Mathf.Deg2Rad(360),0);
            GetNode<Spatial>("Spatial/Dumbbell").Rotation = wheel.Rotation;
            GetNode<Spatial>("Spatial/Dumbbell").Scale = Vector3.One * Mathf.Lerp(0, 3f, (float)FinalMass.Value / 25);;
            
        }
        public override void _Process(float delta)
        {
            UpdateText();
        }
        private double ComputeAngularMomentum(double Mass, double Radius, double AngularVelocity) => Radius * Radius * Mass * AngularVelocity;
        private void SwitchTarget(int target) //referenced by signal
        {
            Target = (FinalValues)target;
            for (int i = 0; i < CheckButtons.Length; i++)
            {
                if (target == i)
                {
                    CheckButtons[i].Pressed = true;
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
        private void UpdateText()
        {
            GetNode<Label>("Node2D/CanvasLayer/Control/InitialMomentum/Mass/MassValue/MassLabel").Text = "Mass (kg) = " +  Math.Round(InitialMass,4);
            GetNode<Label>("Node2D/CanvasLayer/Control/InitialMomentum/AngularVelocity/AngularVelocity/AngularVelocityLabel").Text = "Angular Velocity (rad/s) = " + Math.Round(InitialAngularVelocity,4);
            GetNode<Label>("Node2D/CanvasLayer/Control/InitialMomentum/Radius/Radius/RadiusLabel").Text = "Radius (m) = " + Math.Round(InitialRadius,4);

            GetNode<Label>("Node2D/CanvasLayer/Control/TotalMomentum").Text = "Angular Momentum (kgm²/s) = " + Math.Round(InitialAngularMomentum,4); 

            GetNode<Label>("Node2D/CanvasLayer/Control/FinalMomentum/Mass/MassValue/MassLabel").Text = "Mass (kg) = " + Math.Round(FinalMass.Value,4);
            GetNode<Label>("Node2D/CanvasLayer/Control/FinalMomentum/AngularVelocity/AngularVelocity/AngularVelocityLabel").Text = "Angular Velocity (rad/s) = " + Math.Round(FinalAngularVelocity.Value,4);
            GetNode<Label>("Node2D/CanvasLayer/Control/FinalMomentum/Radius/Radius/RadiusLabel").Text = "Radius (m) = " + Math.Round(FinalRadius.Value,4);

            GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Left/Mass").Text = InitialMass.ToString() + "kg";
            GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Left/AngularVelocity").Text = InitialAngularVelocity.ToString() + "rad/s";
            GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Left/Radius").Text = Math.Pow(InitialRadius,2).ToString() + "m²";
            GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Right/Mass").Text = Math.Round(FinalMass.Value,2).ToString() + "kg";
            GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Right/AngularVelocity").Text = Math.Round(FinalAngularVelocity.Value,2).ToString() + "rad/s";
            GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Right/Radius").Text = Math.Round(Math.Pow(FinalRadius.Value,2),2).ToString() + "m²";
            
            (GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Left/Mass").Get("custom_fonts/font") as DynamicFont).
                Size = (int)Mathf.Clamp(Mathf.Lerp(20,50,(float)InitialMass / 25),15,70);
            (GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Left/AngularVelocity").Get("custom_fonts/font") as DynamicFont).
                Size = (int)Mathf.Clamp(Mathf.Lerp(20,50,(float)InitialAngularVelocity / 25),15,70);
            (GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Left/Radius").Get("custom_fonts/font") as DynamicFont).
                Size = (int)Mathf.Clamp(Mathf.Lerp(20,50,(float)InitialRadius / 5),15,70);
            
            (GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Right/Mass").Get("custom_fonts/font") as DynamicFont).
                Size = (int)Mathf.Clamp(Mathf.Lerp(20,50,(float)FinalMass.Value / 25),15,70);
            (GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Right/AngularVelocity").Get("custom_fonts/font") as DynamicFont).
                Size = (int)Mathf.Clamp(Mathf.Lerp(20,50,(float)FinalAngularVelocity.Value / 25),15,70);
            (GetNode<Label>("Node2D/CanvasLayer/Control/Equation/Right/Radius").Get("custom_fonts/font") as DynamicFont).
                Size = (int)Mathf.Clamp(Mathf.Lerp(20,50,(float)FinalRadius.Value / 5),15,70);

        }
    }
}