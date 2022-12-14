using Godot;
using System;
namespace PhySim.simulations
{
    public class StaticEquilbriumTension : Node
    {
        private HSlider LeftLocation;
        private HSlider RightLocation;
        private HSlider CenterLocation;
        private HSlider Weight;
        
        //in centimeters
        //0 to centerlocation
        private double leftLocation => LeftLocation.Value;
        private double rightLocation => RightLocation.Value;
        private double centerLocation => CenterLocation.Value;
        private double weight => Weight.Value;
        
        private Polygon2D LeftRope;
        private Polygon2D RightRope;
        private Polygon2D CenterForce;

        private double rightDistance => rightLocation - centerLocation;
        private double leftDistance => centerLocation - leftLocation;

        private double tensionRight => weight * leftDistance / (leftDistance + rightDistance);
        private double tensionLeft => weight - tensionRight;

        public override void _Ready()
        {
            LeftLocation = GetNode<HSlider>("Screen/LeftParameters/LeftLocation/HSlider");
            RightLocation = GetNode<HSlider>("Screen/LeftParameters/RightLocation/HSlider");
            CenterLocation = GetNode<HSlider>("Screen/LeftParameters/GravityCenter/HSlider");
            Weight = GetNode<HSlider>("Screen/LeftParameters/Weight/HSlider");

            LeftRope = GetNode<Polygon2D>("Screen/Objects/Simulation/LeftRope");
            RightRope = GetNode<Polygon2D>("Screen/Objects/Simulation/RightRope");
            CenterForce = GetNode<Polygon2D>("Screen/Objects/Simulation/Bar/ForceGravity");
        }

        public override void _PhysicsProcess(float delta)
        {
            LeftLocation.MaxValue = centerLocation - 1;
            RightLocation.MinValue = centerLocation + 1;


        }
        public override void _Process(float delta)
        {
            GetNode<Label>("Screen/LeftParameters/LeftLocation/HSlider/Label").Text = "Left Location (cm) = " + leftLocation;
            GetNode<Label>("Screen/LeftParameters/RightLocation/HSlider/Label").Text = "Right Location (cm) = " + rightLocation;
            GetNode<Label>("Screen/LeftParameters/GravityCenter/HSlider/Label").Text = "Center of Gravity Location (cm) = " + centerLocation;
            GetNode<Label>("Screen/LeftParameters/Weight/HSlider/Label").Text = "Weight (N) = " + weight;

            LeftRope.Position = CMToPx(new Vector2((float)leftLocation - 50, 0));
            RightRope.Position = CMToPx(new Vector2((float)rightLocation - 50, 0));
            CenterForce.Position = CMToPx(new Vector2((float)centerLocation - 50, 0));

            GetNode<Label>("Screen/Objects/Simulation/RightRope/Label").Text = Math.Round(tensionRight,2) + "N";
            GetNode<Label>("Screen/Objects/Simulation/LeftRope/Label").Text = Math.Round(tensionLeft,2) + "N";
            GetNode<Label>("Screen/Objects/Simulation/Bar/ForceGravity/Label").Text = weight + "N";

        }

        private double MToPx(double m) => m * 1200;
        private double CMToM(double cm) => cm/100;
        private double CMToPx(double cm) => MToPx(CMToM(cm));
        private Vector2 CMToPx(Vector2 cm) => new Vector2((float)CMToPx(cm.x),(float)CMToPx(cm.y));
    }
}