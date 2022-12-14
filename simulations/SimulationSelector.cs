using Godot;
using System.Collections.Generic;
namespace PhySim.simulations
{
    public class SimulationSelector : Node
    {

        ResourceInteractiveLoader loader;
        int wait_frames;
        ulong time_max = 100; //ms;
        public Node CurrentScene;
        public static Dictionary<string, PackedScene> LoadedScenes = new Dictionary<string, PackedScene>();
        bool loading = false;
        public override void _Ready()
        {
            SetProcess(false);
        }
        public void Return()
        {
            //the name of the root node of the simulation is Node
            if (loading) return;
            if (CurrentScene.Name == "Node")
            {
                GoToScene("res://MainMenu.tscn");
            }
            else
            {
                GetTree().Quit();
            }
        }
        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
            {
                Return();
            }
        }
        /*
        public override void _Notification(int what)
        {
            if (what == MainLoop.NotificationWmGoBackRequest)
            {
                Return();
            }
        }
        */
        public void GoToScene(string path)
        {
            if (LoadedScenes.ContainsKey(path))
            {
                CurrentScene.QueueFree();
                CurrentScene = LoadedScenes[path].Instance();
                GetNode("/root").AddChild(CurrentScene);
                return;
            }
            loader = ResourceLoader.LoadInteractive(path);
            if (loader == null)
            {
                GD.Print("error");
                return;
            }
            SetProcess(true);
            CurrentScene.QueueFree();
            GetNode<AnimationPlayer>("AnimationPlayer").Play("loading");
            wait_frames = 15;
            loading = true;
        }
        public override void _Process(float delta)
        {
            if (loader == null)
            {
                SetProcess(false);
                return;
            }
            if (wait_frames > 0)
            {
                wait_frames -= 1;
                return;
            }
            var t = OS.GetTicksMsec();
            while (OS.GetTicksMsec() < t + time_max)
            {
                var err = loader.Poll();
                if (err == Error.FileEof)
                {
                    var resource = loader.GetResource();
                    loader = null;
                    SetNewScene(resource);
                    break;
                }
                if (err == Error.Ok)
                {
                    UpdateProgress();
                }
                else //error during loading.
                {
                    loader = null;
                    loading = false;
                    break;
                }
            }
        }
        private void SetNewScene(Resource resource)
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Stop();
            GetNode<Control>("CanvasLayer/Control").Visible = false;
            LoadedScenes[resource.ResourcePath] = resource as PackedScene;
            CurrentScene = (resource as PackedScene).Instance();
            GetNode("/root").AddChild(CurrentScene);
            loading = false;

        }
        private void UpdateProgress()
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("loading");
        }

    }
}

