using Godot;
using System;
namespace PhySim.simulations
{
    public class InclinedSlope : Node
    {
        private HSlider Distance => GetNode<HSlider>("CanvasLayer/Control/Parameters/Distance/Value");

        ///<summary>degrees</summary>
        private HSlider InclineAngle => GetNode<HSlider>("CanvasLayer/Control/Parameters/Angle/Value");
        private HSlider Time => GetNode<HSlider>("CanvasLayer/Control/Parameters/Time/Value");
        private HSlider Radius => GetNode<HSlider>("CanvasLayer/Control/Parameters/Radius/Value");
        private Polygon2D Slope => GetNode<Polygon2D>("CanvasLayer/Control/PlaneAnchor/Node2D/Slope");
        private Sprite Wheel => GetNode<Sprite>("CanvasLayer/Control/PlaneAnchor/Node2D/Plane/Object");
        private Vector2[] Points;
        private double gravity = 9.8;
        private double acceleration => 2 * gravity * Sin(InclineAngle.Value) / 3;
        private double time => (Math.Sqrt(2 * acceleration * Distance.Value) / acceleration);
        private Vector2 position => new Vector2((float)(-Distance.Value + acceleration * Time.Value * Time.Value / 2), -(float)Radius.Value);
       
        //angular kinematics
        private double angularDisplacement => (position.x + Distance.Value) / Radius.Value;

        public override void _PhysicsProcess(float delta)
        {
            Points = Slope.Polygon;
            Slope.Polygon = MToPx(new Vector2[] {
                new Vector2(-(float) (Distance.Value * Cos(InclineAngle.Value)), Points[1].y), Points [1],
                new Vector2(-(float) (Distance.Value * Cos(InclineAngle.Value)), -(float)(Distance.Value * Sin(InclineAngle.Value)))});
            GetNode<Node2D>("CanvasLayer/Control/PlaneAnchor/Node2D/Plane").RotationDegrees = (float)InclineAngle.Value;
            Time.MaxValue = time;
            Wheel.Position = MToPx(position);
            Wheel.Rotation = (float)angularDisplacement;
            Wheel.Scale = MToPx(Vector2.One * (float)Radius.Value / 1200);
            if (GetNode<CheckButton>("CanvasLayer/Control/Parameters/CheckButton").Pressed)
            {
                Time.Step = 0;
                Time.Value += delta;
            }
            else
            {
                Time.Step = 0.05;
            }
        }
        public override void _Process(float delta)
        {
            GetNode<Label>("CanvasLayer/Control/Parameters/Angle/Value/Label").Text = "Angle (degrees) = " + InclineAngle.Value;
            GetNode<Label>("CanvasLayer/Control/Parameters/Distance/Value/Label").Text = "Distance (m) = " + Distance.Value;
            GetNode<Label>("CanvasLayer/Control/Parameters/Time/Value/Label").Text = "Time (s) = " + Math.Round(Time.Value,2);
            GetNode<Label>("CanvasLayer/Control/Parameters/Radius/Value/Label").Text = "Radius (m) = " + Radius.Value;

            GetNode<Label>("CanvasLayer/Control/Control/Acceleration").Text = "Acceleration (m/s/s) = " + Math.Round(acceleration,2);
            GetNode<Label>("CanvasLayer/Control/Control/Velocity").Text = "Velocity (m/s) = " + Math.Round(acceleration * Time.Value,2);
            GetNode<Label>("CanvasLayer/Control/Control/Displacement").Text = "Displacement (m) = " + Math.Round(position.x + Distance.Value,2);
            GetNode<Label>("CanvasLayer/Control/Control/AngularDisplacement").Text = "Angular Displacement (rad) = " + Math.Round(angularDisplacement,2);
            GetNode<Label>("CanvasLayer/Control/Control/AngularVelocity").Text = "Angular Velocity (rad/s) = " + Math.Round(Time.Value != 0 ? angularDisplacement / Time.Value: 0, 2);
        }
        private double Cos(double degree) => Math.Cos(degree * Math.PI / 180);
        private double Sin(double degree) => Math.Sin(degree * Math.PI / 180);
        private double MToPx(double m) => m * 12;
        private Vector2[] MToPx(Vector2[] vectors)
        {
            Vector2[] pxVectors = new Vector2[vectors.Length];
            for (int i = 0; i < vectors.Length; i++)
            {
                pxVectors[i] = MToPx(vectors[i]);
            }
            return pxVectors;
        }
        private Vector2 MToPx(Vector2 vector) => new Vector2((float)MToPx(vector.x), (float)MToPx(vector.y));

    }
}