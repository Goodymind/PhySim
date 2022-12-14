using Godot;
using System;
namespace PhySim.simulations
{
    public class TorqueMomentumRelation : Node
    {
        [Export] Curve TorqueSizeCurve;

        Spatial Wheel => GetNode<Spatial>("Spatial/Wheel");
        public double InitialMass => GetNode<HSlider>("CanvasLayer/Control/Initial Momentum/Mass/HSlider").Value;
        public double InitialRadius => GetNode<HSlider>("CanvasLayer/Control/Initial Momentum/Radius/HSlider").Value; //r
        public double FinalMass => GetNode<HSlider>("CanvasLayer/Control/Final Momentum/Mass/HSlider").Value;
        public double FinalRadius => GetNode<HSlider>("CanvasLayer/Control/Final Momentum/Radius/HSlider").Value;
        public double Time => GetNode<HSlider>("CanvasLayer/Control/Time").Value;
        public double InitialAngularVelocity => GetNode<HSlider>("CanvasLayer/Control/Initial Momentum/Angular Velocity/HSlider").Value;
        public double FinalAngularVelocity => GetNode<HSlider>("CanvasLayer/Control/Final Momentum/Angular Velocity/HSlider").Value;
        public double ChangeInAngularVelocity => FinalAngularVelocity - InitialAngularVelocity;
        public double InstantAngularVelocity = 0;
        public double ChangeInAngularMomentum => FinalMomentum - InitialMomentum;//Inertia * ChangeInAngularVelocity;
        public double AverageTorque => ChangeInAngularMomentum / Time;
        public double FinalMomentum => FinalRadius * FinalRadius * FinalMass * FinalAngularVelocity;
        public double InitialMomentum => InitialRadius * InitialRadius * InitialMass * InitialAngularVelocity;

        
        double t = 0;
        bool Simulating = false;
        public override void _Ready()
        {
            Simulating = GetNode<CheckButton>("CanvasLayer/Control/SpinCheck").Pressed;
            InstantAngularVelocity = InitialAngularVelocity;
        }
        public override void _PhysicsProcess(float delta)
        {
            Wheel.Rotation += new Vector3(0, (float)InstantAngularVelocity, 0) * delta;
            Wheel.Rotation = new Vector3(0, Wheel.Rotation.y % Mathf.Deg2Rad(360), 0);
            GetNode<Spatial>("Spatial/Dumbbell").Rotation = Wheel.Rotation;

            float currentRadius = Mathf.Lerp((float)InitialRadius, (float)FinalRadius,(float) (t/Time));
            Wheel.Scale = new Vector3(currentRadius,1,currentRadius);

            float currentMass = Mathf.Lerp((float)InitialMass, (float)FinalMass, (float)(t/Time));
            GetNode<Spatial>("Spatial/Dumbbell").Scale = Vector3.One * Mathf.Lerp(0, 3f, currentMass / 100); //100 = maximum mass of hsliders

            if (!Simulating || Time <= 0)
                return;
            if (t > Time)
            {
                Simulating = false;
                t = Time;
                GetNode<CheckButton>("CanvasLayer/Control/SpinCheck").Pressed = false;
                InstantAngularVelocity = DoubleLerp(InitialAngularVelocity, FinalAngularVelocity, (t / Time));
                return;
            }
            InstantAngularVelocity = DoubleLerp(InitialAngularVelocity, FinalAngularVelocity, (t / Time));
            t += (double)delta;

        }
        public override void _Process(float delta)
        {
            GetNode<Label>("CanvasLayer/Control/EQUATION/HBoxContainer/Torque").Text = Math.Round(AverageTorque,2) + " Nm";
            GetNode<Label>("CanvasLayer/Control/EQUATION/HBoxContainer/Final Momentum").Text = "("+Math.Round(FinalMomentum,2) + " kgm²/s";
            GetNode<Label>("CanvasLayer/Control/EQUATION/HBoxContainer/Initial Momentum").Text = Math.Round(InitialMomentum,2) + " kgm²/s)";
            GetNode<Label>("CanvasLayer/Control/EQUATION/HBoxContainer/Time").Text = Math.Round(Time,2) + " s";

            //max momentum value = 20,000, t = 2000,000
            (GetNode<Label>("CanvasLayer/Control/EQUATION/HBoxContainer/Torque").Get("custom_fonts/font") as DynamicFont).Size = (int)Mathf.Lerp(18,40,TorqueSizeCurve.InterpolateBaked(Mathf.Abs((float)AverageTorque/60000)));
            (GetNode<Label>("CanvasLayer/Control/EQUATION/HBoxContainer/Final Momentum").Get("custom_fonts/font") as DynamicFont).Size = (int)Mathf.Lerp(18,40,TorqueSizeCurve.InterpolateBaked(Mathf.Abs((float)FinalMomentum/6000)));
            (GetNode<Label>("CanvasLayer/Control/EQUATION/HBoxContainer/Initial Momentum").Get("custom_fonts/font") as DynamicFont).Size = (int)Mathf.Lerp(18,40,TorqueSizeCurve.InterpolateBaked(Mathf.Abs((float)InitialMomentum/6000)));
            (GetNode<Label>("CanvasLayer/Control/EQUATION/HBoxContainer/Time").Get("custom_fonts/font") as DynamicFont).Size = (int)Mathf.Lerp(18,40,(float)Time/20);

            GetNode<Label>("CanvasLayer/Control/CurrentTime").Text = "Current Time = " + Math.Round(t,2) + " s";
            GetNode<Label>("CanvasLayer/Control/Initial Momentum/Mass/HSlider/Label").Text = "Mass = " + InitialMass + " kg";
            GetNode<Label>("CanvasLayer/Control/Initial Momentum/Radius/HSlider/Label").Text = "Radius = " + InitialRadius + " m";
            GetNode<Label>("CanvasLayer/Control/Initial Momentum/Angular Velocity/HSlider/Label").Text = "Angular Velocity " + InitialAngularVelocity + " θ/s";
            GetNode<Label>("CanvasLayer/Control/Final Momentum/Mass/HSlider/Label").Text = "Mass = " + FinalMass + " kg";
            GetNode<Label>("CanvasLayer/Control/Final Momentum/Radius/HSlider/Label").Text = "Radius = " + FinalRadius + " m";
            GetNode<Label>("CanvasLayer/Control/Final Momentum/Angular Velocity/HSlider/Label").Text = "AngularVelocity = " +  FinalAngularVelocity + " θ/s";
            
            GetNode<Label>("CanvasLayer/Control/Time/Label").Text = Math.Round(Time,2) + "s";

        }

        private void SpinToggle(bool ButtonPress) //connected via signal
        {
            Simulating = ButtonPress;
        }
        private void Reset()
        {
            t = 0;
            InstantAngularVelocity = InitialAngularVelocity;
        }
        private double DoubleLerp(double a, double b, double t) => a + (b - a) * t;
    }
}