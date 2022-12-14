using System;
using Godot;
namespace PhySim.simulations
{
    public class ConservationEnergy : Node
    {
        private Polygon2D Slope => GetNode<Polygon2D>("CanvasLayer/Control/PlaneAnchor/Node2D/Slope");
        private HSlider Height => GetNode<HSlider>("CanvasLayer/Control/Parameters/Height/Value");
        private HSlider Time => GetNode<HSlider>("CanvasLayer/Control/Parameters/Time/Value");
        private HSlider CurrentHeight => GetNode<HSlider>("CanvasLayer/Control/Parameters/CurrentHeight/Value");
        private const double gravity = 9.8;
        private double velocity => Math.Sqrt(4 * gravity * (Height.Value - CurrentHeight.Value) / 3);
        private double finalvelocity => Math.Sqrt(4 * gravity * Height.Value / 3);
        private double distance => Math.Sqrt((200 * 200) + (Height.Value * Height.Value));
        private double displacement => distance - (distance / Height.Value * CurrentHeight.Value);
        private double acceleration => (finalvelocity * finalvelocity) / (2 * distance);
        private bool editHeight, editTime = false;
        Vector2[] polygons = new Vector2[3];
        public override void _Ready()
        {
            polygons[0] = new Vector2((float)MtoPx(-200), 0);
            polygons[1] = Vector2.Zero;
            polygons[2] = new Vector2((float)MtoPx(-200), 0);
        }
        public override void _Process(float delta)
        {
            CurrentHeight.MaxValue = Height.Value;
            Time.MaxValue = Math.Sqrt(2 * acceleration * distance) / acceleration;
            if (editHeight)
                EditHeight();
            if (editTime)
                EditTime();
            double xpos = Mathf.Lerp(0, -200, (float)(CurrentHeight.Value / Height.Value));
            GetNode<Sprite>("CanvasLayer/Control/PlaneAnchor/Node2D/Plane/Object").Position = MtoPx(xpos, CurrentHeight.Value + 8);
            GetNode<Label>("CanvasLayer/Control/Parameters/Height/Value/Label").Text = "Slope Height (m) =" + Height.Value;
            GetNode<Label>("CanvasLayer/Control/Parameters/CurrentHeight/Value/Label").Text = "Current Height (m) =" + CurrentHeight.Value;
            GetNode<Label>("CanvasLayer/Control/Parameters/Time/Value/Label").Text = "Time (Locked)(s) = " + Math.Round(Time.Value,2);
            GetNode<Label>("CanvasLayer/Control/Equation/Terms/Left/Numerator/hi").Text = Height.Value + "m";
            GetNode<Label>("CanvasLayer/Control/Equation/Terms/Left/Numerator/hf").Text = CurrentHeight.Value + "m";
            GetNode<Label>("CanvasLayer/Control/Equation/Terms/velocity").Text = "(" + Math.Round(velocity, 2) + "m/s)²";
            GetNode<Label>("CanvasLayer/Control/Control/Acceleration").Text = "Acceleration (m/s/s) = " + Math.Round(acceleration,2) + "m/s²";
            GetNode<Label>("CanvasLayer/Control/Control/Displacement").Text = "Displacement (m) = " + Math.Round(displacement,2);
            polygons[2] = MtoPx(new Vector2(-200, (float)Height.Value));
            Slope.Polygon = polygons;
        }
        private void EditHeight()
        {
            //CurrentHeight.Value = Height.Value * distance / displacement;
        }
        private void EditTime()
        {
            Time.Value = velocity / acceleration;

        }
        private void HeightEditStart()
        {
            editTime = true;
        }
        private void HeightEditEnd(bool value)
        {
            editTime = false;
        }
        private void TimeEditStart()
        {
            editHeight = true;
        }
        private void TimeEditEnd(bool value)
        {
            editHeight = false;
        }
        private double MtoPx(double m) => m * 3;
        private Vector2 MtoPx(Vector2 m) => new Vector2((float)MtoPx(m.x), -(float)MtoPx(m.y));
        private Vector2 MtoPx(double x, double y) => MtoPx(new Vector2((float)x, (float)y));
    }
}