using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LSystemManager: MonoBehaviour
{
    public int iterations = 4;
    public float rotAngle = 20f;
    public float lineLength = 1f;
    //[SerializeField] private List<char> alphabet = new List<char> { 'F', '+', '-', '[', ']' };
    public string axiom = "F";
    public List<char> LHS = new List<char>() { 'F' };
    public List<string> RHS = new List<string>() { "^F+[F]-[F]F" };
    public List<SingleLSystem> lsystems = new List<SingleLSystem>();

    public void AddLSystemToList(SingleLSystem lsystem)
    {
        if (!lsystems.Contains(lsystem))
        {
            lsystems.Add(lsystem);
        }
    }

    public void ClearLSystemsList()
    {
        foreach (SingleLSystem lsystem in lsystems)
        {
            if (lsystem != null)
            {
                Destroy(lsystem.gameObject);
            }
        }

        lsystems.Clear();
    }

    public void GenerateAllLSystems()
    {
        foreach (SingleLSystem lsystem in lsystems)
        {
            lsystem.lineLength = lineLength;
            lsystem.rotAngle = rotAngle;
            lsystem.axiom = axiom;
            lsystem.LHS = LHS;
            lsystem.RHS = RHS;
            lsystem.iterations = iterations;
            lsystem.GenerateLSystem();
        }
    }
}
