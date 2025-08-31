using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class B_L_System : MonoBehaviour
{
    [SerializeField] public LineRenderer linePrefab;
    [SerializeField, Range(1, 7)] private int iterations = 4;
    [SerializeField, Range(0, 360)] private float rotAngle = 20f;
    [SerializeField, Range(.01f, 5)] private float lineLength = 1f;
    //[SerializeField] private List<char> alphabet = new List<char> { 'F', '+', '-', '[', ']' };
    [SerializeField] private string axiom = "F";
    [SerializeField] private List<char> LHS = new List<char>() { 'F', 'G' };
    [SerializeField] private List<string> RHS = new List<string>() { "F-G+F+G-F", "GG" };

    Stack<(LineRenderer, Quaternion)> stack = new Stack<(LineRenderer, Quaternion)>();
    LineRenderer currentLine;
    Quaternion currentRotation;
    Vector3 currentPosition;
    string currentString;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateLSystem();
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;
        GenerateLSystem();
    }

    private void GenerateLSystem()
    {
        currentString = axiom;
        for (int i = 0; i < iterations; i++)
        {
            string newString = "";
            foreach (char c in currentString)
            {
                if (LHS.Contains(c))
                {
                    int index = LHS.IndexOf(c);
                    newString += RHS[index];
                }
                else
                    newString += c.ToString();
            }
            currentString = newString;
        }

        currentPosition = Vector3.zero; 
        currentRotation = Quaternion.identity; // Check if this is pointing up

        foreach (Transform child in transform) // Clear previous lines
        {
            Destroy(child.gameObject);
        }

        currentLine = Instantiate(linePrefab, currentPosition, Quaternion.identity, transform);
        currentLine.positionCount = 1;
        currentLine.SetPosition(0, currentPosition);
        foreach (char c in currentString)
        {
            switch (c)
            {
                case '+':
                    currentRotation *= Quaternion.Euler(0, 0, rotAngle);
                    break;
                case '-':
                    currentRotation *= Quaternion.Euler(0, 0, -rotAngle);
                    break;
                case '[':
                    stack.Push((currentLine, currentRotation));
                    currentLine = Instantiate(linePrefab, currentPosition, Quaternion.identity, transform);
                    currentLine.positionCount = 1;
                    currentLine.SetPosition(0, currentPosition);
                    break;
                case ']':
                    if (stack.Count > 0)
                    {
                        (currentLine, currentRotation) = stack.Pop();
                        currentPosition = currentLine.GetPosition(currentLine.positionCount - 1);
                    }
                    else
                    {
                        Debug.LogWarning("Stack is empty when trying to pop. Check Rules and make sure all opened brackets get closed, and viceversa");
                    }
                    break;
                default:
                    if (LHS.Contains(c))
                    {
                        Vector3 startPosition = currentPosition;
                        currentPosition += currentRotation * Vector3.up * lineLength;
                        currentLine.positionCount += 1;
                        currentLine.SetPosition(currentLine.positionCount - 1, currentPosition);
                    }
                    else
                        Debug.LogWarning($"Unrecognized character '{c}' in L-system string.");
                    continue;
            }
        }
    }
}
