using Godot;
using PhySim.simulations;
using System.Collections.Generic;
namespace PhySim
{
    public class MainMenu : Control
    {
        private SimulationSelector SimSelector => GetNodeOrNull<SimulationSelector>("/root/SimulationSelector");

        [Export(PropertyHint.MultilineText)] private Dictionary<string, string> Simulations;
        private Dictionary<int, string> IDToPath;
        private ItemList SimulationList => GetNodeOrNull<ItemList>("SimulationList");
        public override void _Ready()
        {
            SimSelector.CurrentScene = this;
            IDToPath = new Dictionary<int, string>();
            int id = 0;
            foreach(var pair in Simulations)
            {
                SimulationList.AddItem(pair.Key);
                IDToPath.Add(id, pair.Value);
                id++;
            }
        }
        private void SimulationSelected(int index)
        {
            string path = IDToPath[index];
            SimSelector.GoToScene(path);
        }
    }
}