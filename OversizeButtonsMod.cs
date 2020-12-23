using PiTung;
using PiTung.Components;
using PiTung.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class OversizeButtons : Mod
{
    public override string Name => "Oversize Buttons For PiTUNG 2.4.X";
    public override string PackageName => "me.jimmy.GiantButtons.PiTUNG2.4.X";
    public override string Author => "Iamsodarncool, Modified by Ryan";
    public override Version ModVersion => new Version("1.1");


    public override void BeforePatch()
    {
        ComponentRegistry.CreateNew("LargePanelButton", "Large Panel Button", CreatePanelButtonOfSize(3, 2));
        ComponentRegistry.CreateNew("GiantPanelButton", "Giant Panel Button", CreatePanelButtonOfSize(3, 3));
        ComponentRegistry.CreateNew("EnormousPanelButton", "Enormous Panel Button", CreatePanelButtonOfSize(15, 2));

        // This game's sequal has been in devolpment for over 2 years. I hope the world is ready
        ComponentRegistry.CreateNew("CollosalPanelButton", "Collosal Panel Button", CreatePanelButtonOfSize(51, 51));
        // On second thought, the world is still not ready to see this one
        // ComponentRegistry.CreateNew("TitanicPanelButton", "Titanic Panel Button", CreatePanelButtonOfSize(401, 601));

        // add buttons from file
        if (File.Exists($"{Directory.GetCurrentDirectory()}/sizes_b.txt")) 
        {
            string[] sizes = File.ReadAllLines($"{Directory.GetCurrentDirectory()}/sizes_b.txt");
            foreach(string size in sizes)
            {
                try
                {
                    int[] xysize = Array.ConvertAll(size.Split(' '), s => int.Parse(s)); // split the string into a string array at every space and convert that into an int array
                    ComponentRegistry.CreateNew($"{xysize[0]} x {xysize[1]} PanelButton", $"{xysize[0]} x {xysize[1]} PanelButton", CreatePanelButtonOfSize(xysize[0], xysize[1]));
                }
                catch (Exception ex) // Catches all exceptions
				{
                    if (ex is ArgumentException)
                    {
                        IGConsole.Log($"Error! Cant load button of size {size} twice");
                    }
                    else
                    {
                        IGConsole.Log($"Error! Cant load button of size {size}, correct format is \"{{X size of button}} {{Y size of button}}\" ");
                    }
                }
            }
        }
        Shell.RegisterCommand<Add_Button>();
        Shell.RegisterCommand<Remove_Button>();
    }

    public static CustomBuilder CreatePanelButtonOfSize(int x, int z)
    {
        return PrefabBuilder
            .Custom(() =>
            {
                var obj = UnityEngine.Object.Instantiate(References.Prefabs.PanelButton);
                obj.transform.GetChild(0).localScale = new Vector3(x - 0.3f, 0.2f, z - 0.3f); // this is the collider of the button that you have to click on to press it
                obj.transform.GetChild(1).localScale = new Vector3(x - 0.3f, 0.2f, z - 0.3f); // this is the actual button itself that moves when you interact with it
                obj.transform.GetChild(2).localScale = new Vector3(x, 0.33333f, z); // this is the base of the button, the white part right below the brown part
                obj.transform.GetChild(4).localScale = new Vector3(x - 0.35f, 0.833333f, z - 0.35f); // this is the back panel that the output sits on top of. Dimensions carefully chosen so that mounts can fit between them

                // if it is an even number wide, we have to shift everything in the component so that it still lines up with the grid
                if (x % 2 == 0)
                {
                    for (int i = 0; i < obj.transform.childCount; i++)
                    {
                        obj.transform.GetChild(i).transform.localPosition += new Vector3(0.5f, 0, 0);
                    }
                }
                // ditto with height
                if (z % 2 == 0)
                {
                    for (int i = 0; i < obj.transform.childCount; i++)
                    {
                        obj.transform.GetChild(i).transform.localPosition += new Vector3(0, 0, 0.5f);
                    }
                }

                return obj;
            });
    }
}
public class Add_Button : Command
{
    public override string Name => "addButton";
    public override string Usage => $"{Name} x_size y_size";
    public override string Description => "Adds a button of specified size to the component list";

    public override bool Execute(IEnumerable<string> args)
    {
        if (args.Count() < 2)
        {
            IGConsole.Log("Not enough arguments!");
            return false;
        }
        string size = string.Join(" ", args.ToArray()); //turns the array into a string
        try
        {
            int[] xysize = Array.ConvertAll(size.Split(' '), s => int.Parse(s)); // split the string into a string array at every space and convert that into an int array
            ComponentRegistry.CreateNew($"{xysize[0]} x {xysize[1]} PanelButton", $"{xysize[0]} x {xysize[1]} PanelButton", OversizeButtons.CreatePanelButtonOfSize(xysize[0], xysize[1]));
        }
        catch (Exception ex) // Catches all exceptions
        {
            if (ex is ArgumentException)
            {
                IGConsole.Log($"Error! Cant make button of size {size}, It already exists!");
            }
            else
            {
                IGConsole.Log($"Error! Cant make button of size {size}, correct format is \"{{X size of button}} {{Y size of button}}\" ");
            }
            return false;
        }
        string[] file = { };
        if (File.Exists($"{Directory.GetCurrentDirectory()}/sizes_b.txt"))
        {
            file = File.ReadAllLines($"{Directory.GetCurrentDirectory()}/sizes_b.txt");
        }
        List<string> list = new List<string>();
        list.AddRange(file);
        list.AddRange(new string[]{size});
        file = list.ToArray();
        File.WriteAllLines($"{Directory.GetCurrentDirectory()}/sizes_b.txt", file);
        IGConsole.Log($"Added button of size {size} to component list");
        return true;
    }
}

public class Remove_Button : Command
{
    public override string Name => "removeButton";
    public override string Usage => $"{Name} x_size y_size";
    public override string Description => "removes a button of specified size to the component list";

    public override bool Execute(IEnumerable<string> args)
    {
        if (args.Count() < 2)
        {
            IGConsole.Log("Not enough arguments!");
            return false;
        }
        string size = string.Join(" ", args.ToArray()); //turns the array into a string
        try
        {
            int[] xysize = Array.ConvertAll(size.Split(' '), s => int.Parse(s)); // split the string into a string array at every space and convert that into an int array
        }
        catch (Exception ex) // Catches all exceptions
        {
            IGConsole.Log($"Error! Cant remove button of size {size}, correct format is \"{{X size of button}} {{Y size of button}}\" ");
            return false;
        }
        string[] file = { };
        if (File.Exists($"{Directory.GetCurrentDirectory()}/sizes_b.txt"))
        {
            file = File.ReadAllLines($"{Directory.GetCurrentDirectory()}/sizes_b.txt");
			if (file.Contains(size)) 
            {
                List<string> fileList = file.ToList();
                fileList.Remove(size);
                file = fileList.ToArray();
			}
			else 
            {
                IGConsole.Log($"Error! Cant remove button of size {size}, It doesn't exist!");
                return false;
            }
            File.WriteAllLines($"{Directory.GetCurrentDirectory()}/sizes_b.txt", file);
        }
		else 
        {
            IGConsole.Log($"Error! Cant remove button of size {size}, There are no custom buttons to delete!");
            return false;
        }
        IGConsole.Log($"Reomved button of size {size} from component list. NOTE: this change will only take effect when you restart your game!!!");
        return true;
    }
}
