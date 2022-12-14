using Godot;
using PhySim.simulations;
namespace PhySim
{
    public class ReturnToMainMenu : Button
    {
        SimulationSelector Selector => GetNode<SimulationSelector>("/root/SimulationSelector");
        private void Return()
        {
            Selector.GoToScene("res://MainMenu.tscn");
        }
    }
}