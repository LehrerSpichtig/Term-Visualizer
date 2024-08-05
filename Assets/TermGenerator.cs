using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class TermGenerator : MonoBehaviour
{
    // Define a tuple variable containing a string and an integer
    private string stringData = "3 1 * 0 2 2 ^ 1 4 2";
    private string[] stringDataArray;
    private string[][] symbols;
    private int[][] levels;
    private double[][] floatValues;
    private int maxLevel;
    private int currentLevel;
    private int TopLeftPosition = -30;
    private int LeftPosition = 0;
    private int backPosition = 70;

    private List<List<GameObject>> termObjects = new List<List<GameObject>>();



    // Prefabs for the different symbols
    public GameObject plusPrefab;
    public GameObject multiplicationPrefab;
    public GameObject cylinderPrefab;

    private GameObject currentSymbol;

    void Start()
    {
        Generate("10 1 + 0 2 2 * 1 4 2",0);
        GenerateTerm(0);
    }

    void GenerateTerm(int indx)
    {
        int i = 0;
        this.LeftPosition = this.TopLeftPosition;
        //GameObject[][] numbers = new GameObject[symbols.Length/2 + 1][];

        while (i < symbols[indx].Length)
        {
            if (floatValues[indx][i] > double.MinValue)
            {
                if (levels[indx][i] < this.currentLevel  || levels[indx].Length == 1)
                {
                    termObjects.Add(GenerateNumber(floatValues[indx][i],indx));
                    i++;
                }
                else
                {
                    termObjects.Add(GenerateCalculation(floatValues[indx][i], symbols[indx][i + 1], floatValues[indx][i + 2],indx));
                    i += 3;
                }
            }
            else
            {
                //Symbol ausgeben
                termObjects.Add(GenerateSymbol(symbols[indx][i],indx));
                i++;
            }
        }
        // einfacher Durchlauf
        int z = indx + 1;
        while (this.currentLevel > 0)
        {
            UpdateTerm(z);
            GenerateTerm(z++);
        }
    }
    //shoud be overriden after initialization to GenerateNumber(double number, index z)
    List<GameObject> GenerateNumber(double number, int indx)
    {
        int PrecommaDigits = (int)Math.Floor(number);
        int RightPosition = this.LeftPosition;
        List<GameObject> Objects = new List<GameObject>();
        UnityEngine.Vector3 center = new UnityEngine.Vector3(RightPosition, 0, indx*this.backPosition);


        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = center;
        Objects.Add(cube);

        int v = 0;
        int h = 0;
        bool waagr = false;
        int pos = -1;
        int i = 0;
        int j = 0;
        int diff = 1;
        while (i < PrecommaDigits - 1)
        {
            if (Math.Abs(v - h) == diff || v == h)
            {
                if (Math.Abs(v - h) == diff) diff += 1;
                if (j % 2 == 0) pos = (-1) * pos;
                j += 1;
                waagr = !waagr;
            }
            if (waagr)
            {
                v += pos;
                if (v > RightPosition) RightPosition = v;
            }
            else { h += pos; }
            UnityEngine.Vector3 versch = new UnityEngine.Vector3(v, h, 0);
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = center + versch * 2;
            i += 1;
            Objects.Add(c);
        }
        //TO DO: Nachkommazahlen

        this.LeftPosition = this.LeftPosition + RightPosition * 2 + 5;

        return Objects;

        //termObjects.Add(Objects);

    }

    IEnumerator SummenRoutine(float duration, double leftNumber, string operation, double rightNumber, int indx)
    {
        int RightPosition = this.LeftPosition;


        while (true)
        {
            List<GameObject> left = GenerateNumber(leftNumber, indx);
            List<GameObject> op = GenerateSymbol(operation, indx);
            List<GameObject> right = GenerateNumber(rightNumber, indx);

            yield return new WaitForSecondsRealtime(duration);
            foreach (GameObject o in op)
            {
                Destroy(o);
            }
            yield return new WaitForSecondsRealtime(duration);
            UnityEngine.Vector3 schieben = new UnityEngine.Vector3(-8, 0, 0);
            foreach (GameObject o in right)
            {
                o.transform.position += schieben;
            }
            yield return new WaitForSecondsRealtime(duration);

            foreach (GameObject o in left)
            {
                Destroy(o);
            }
            foreach (GameObject o in right)
            {
                Destroy(o);
            }
            this.LeftPosition = RightPosition;
            List<GameObject> summe = GenerateNumber(leftNumber + rightNumber, indx);
            yield return new WaitForSecondsRealtime(duration);
            foreach (GameObject o in summe)
            {
                Destroy(o);
            }
            this.LeftPosition = RightPosition;
        }

    }

    IEnumerator MultiplicationRoutine(float duration, double leftNumber, string operation, double rightNumber, int indx){
        int RightPosition = this.LeftPosition;
        UnityEngine.Vector3 schieben = new UnityEngine.Vector3(-5, 0, 0);

        while (true){

            List<GameObject> left = GenerateNumber(leftNumber, indx);
            List<GameObject> op = GenerateSymbol(operation, indx);
            List<GameObject> right = GenerateNumber(rightNumber, indx);
            yield return new WaitForSecondsRealtime(duration);
                        foreach (GameObject o in op)
            {
                Destroy(o);
            }
                        foreach (GameObject o in left)
            {
                Destroy(o);
            }
            foreach (GameObject o in right)
            {
                Destroy(o);
            }
            this.LeftPosition = RightPosition;
            List<GameObject>[] summanden = new List<GameObject>[(int) rightNumber];
            List<GameObject>[] pluse = new List<GameObject>[(int) rightNumber-1];

            for (int i = 0;i<rightNumber-1;i++){
                summanden[i] = GenerateNumber(leftNumber, indx);
                yield return new WaitForSecondsRealtime(duration/summanden.Length);
                pluse[i] = GenerateSymbol("+", indx);
            }
            summanden[summanden.Length-1] = GenerateNumber(leftNumber, indx);
            yield return new WaitForSecondsRealtime(duration/summanden.Length);
            for (int i = pluse.Length-1;i>=0;i--){
                foreach (GameObject o in pluse[i]){
                    Destroy(o);
                }
                yield return new WaitForSecondsRealtime(duration/summanden.Length);
                foreach (GameObject o in summanden[i+1]){
                    o.transform.position += schieben;
                }
                yield return new WaitForSecondsRealtime(duration/summanden.Length);
                int zwischensumme = summanden[i].Count + summanden[i+1].Count;
                this.LeftPosition = (int) summanden[i][0].transform.position.x;
                foreach (GameObject o in summanden[i+1]){
                    Destroy(o);
                }
                foreach (GameObject o in summanden[i]){
                    Destroy(o);
                }
                //this.LeftPosition -= 10;
                summanden[i] = GenerateNumber(zwischensumme, indx);
                yield return new WaitForSecondsRealtime(duration/summanden.Length);
            }
            foreach (GameObject o in summanden[0]){
                Destroy(o);
            }
            //this.LeftPosition = RightPosition + 10;
            List<GameObject> resultat = GenerateNumber(leftNumber*rightNumber, indx);
            yield return new WaitForSecondsRealtime(duration);
            foreach (GameObject o in resultat){
                Destroy(o);
            }
            this.LeftPosition = RightPosition;

        }

    }


    List<GameObject> GenerateCalculation(double leftNumber, string operation, double rightNumber, int indx)
    {
        List<GameObject> objects = new List<GameObject>();
        float duration;

        switch (operation)
        {
            case "+":

                duration = 2f;
                StartCoroutine(SummenRoutine(duration, leftNumber, operation, rightNumber, indx));

                //objects = GenerateNumber(leftNumber + rightNumber);

                break;
            case "-":
                break;
            case "*":
                duration = 2f;
                StartCoroutine(MultiplicationRoutine(duration, leftNumber, operation, rightNumber, indx));


                break;
            case "/":
                break;
            case "^":
                break;
            default:
                break;
        }

        return objects;

    }

    List<GameObject> GenerateSymbol(string operation, int indx)
    {
        List<GameObject> objects = new List<GameObject>();
        UnityEngine.Vector3 center = new UnityEngine.Vector3(this.LeftPosition, 0, indx*this.backPosition);

        GameObject mitte, links, rechts, unten, oben;
        switch (operation)
        {
            case "+":
                mitte = GameObject.CreatePrimitive(PrimitiveType.Cube);
                mitte.transform.position = center;
                objects.Add(mitte);
                rechts = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rechts.transform.position = center + UnityEngine.Vector3.right;
                objects.Add(rechts);
                links = GameObject.CreatePrimitive(PrimitiveType.Cube);
                links.transform.position = center + UnityEngine.Vector3.left;
                objects.Add(links);
                unten = GameObject.CreatePrimitive(PrimitiveType.Cube);
                unten.transform.position = center + UnityEngine.Vector3.down;
                objects.Add(unten);
                oben = GameObject.CreatePrimitive(PrimitiveType.Cube);
                oben.transform.position = center + UnityEngine.Vector3.up;
                objects.Add(oben);
                break;
            case "-":
                mitte = GameObject.CreatePrimitive(PrimitiveType.Cube);
                mitte.transform.position = center;
                objects.Add(mitte);
                rechts = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rechts.transform.position = center + UnityEngine.Vector3.right;
                objects.Add(rechts);
                links = GameObject.CreatePrimitive(PrimitiveType.Cube);
                links.transform.position = center + UnityEngine.Vector3.left;
                objects.Add(links);
                break;
            case "*":
                mitte = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mitte.transform.position = center;
                objects.Add(mitte);
                break;
            case "/":
                unten = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                unten.transform.position = center + UnityEngine.Vector3.down;
                objects.Add(unten);
                oben = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                oben.transform.position = center + UnityEngine.Vector3.up;
                objects.Add(oben);
                break;
            case "^":
                rechts = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rechts.transform.position = center + UnityEngine.Vector3.right;
                rechts.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 45);
                objects.Add(rechts);
                links = GameObject.CreatePrimitive(PrimitiveType.Cube);
                links.transform.position = center + UnityEngine.Vector3.left;
                links.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 45);
                objects.Add(links);
                oben = GameObject.CreatePrimitive(PrimitiveType.Cube);
                oben.transform.position = center + UnityEngine.Vector3.up;
                oben.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 45);
                objects.Add(oben);
                break;
            default:
                mitte = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                mitte.transform.position = center;
                objects.Add(mitte);
                break;
        }
        this.LeftPosition += 5;
        return objects;
        //termObjects.Add(objects);
    }


    private void Generate(string stringData, int indx)
    {
        this.stringData = stringData.Replace('.', ',');

        string[] stringDataArray = this.stringData.Split(' ');
        string[] symbols2 = new string[stringDataArray.Length / 2];
        int[] levels2 = new int[stringDataArray.Length / 2];
        double[] floatValues2 = new double[stringDataArray.Length / 2];

        int j = 0;
        this.maxLevel = 0;
        for (int i = 0; i < stringDataArray.Length; i += 2)
        {
            symbols2[j] = stringDataArray[i];
            int.TryParse(stringDataArray[i + 1], out int tryParsedInt);
            levels2[j] = tryParsedInt;
            if (levels2[j] > maxLevel) maxLevel = levels2[j];
            j++;
        }
        for (int i = 0; i < symbols2.Length; i += 2)
        {
            double.TryParse(symbols2[i], out double floatValue);
            floatValues2[i] = floatValue;
            if (i + 1 < symbols2.Length) floatValues2[i + 1] = double.MinValue;
        }

        this.currentLevel = maxLevel;
        this.symbols = new string[maxLevel+1][]; //stringDataArray.Length / 2];
        this.levels = new int[maxLevel+1][];
        this.floatValues = new double[levels.Length][];

        this.symbols[0] = symbols2;
        this.levels[0]  = levels2;
        this.floatValues[0] = floatValues2;
    }

    private void UpdateTerm(int indx)
    {//string[] symbols, int[] levels, float[] floatValues, int currentLevel){

        List<string> Symbols = symbols[indx-1].ToList<string>();
        List<int> Levels = levels[indx-1].ToList<int>();
        List<double> FloatValues = floatValues[indx-1].ToList<double>();


        int i = 0;
        List<int> Update = new();
        while (i < Levels.Count)
        {
            if (Levels[i] == currentLevel)
            {
                Update.Add(i);
                switch (Symbols[i + 1])
                {
                    case "+":
                        FloatValues[i] = FloatValues[i] + FloatValues[i + 2];
                        break;
                    case "-":
                        FloatValues[i] = FloatValues[i] - FloatValues[i + 2];
                        break;
                    case "*":
                        FloatValues[i] = FloatValues[i] * FloatValues[i + 2];
                        break;
                    case "/":
                        FloatValues[i] = FloatValues[i] / FloatValues[i + 2];
                        break;
                    case "^":
                        FloatValues[i] = Math.Pow(FloatValues[i], FloatValues[i + 2]);
                        break;
                }
                //FloatValues[i] = Math.Round(FloatValues[i], 1);
                Symbols[i] = Math.Round(FloatValues[i], 1).ToString();
                //Levels[i] = Levels[i]-1;            
                i += 3;
            }
            else
            {
                i++;
            }

        }

        for (int j = Update.Count - 1; j >= 0; j--)
        {
            Symbols.RemoveAt(Update[j] + 2);
            Symbols.RemoveAt(Update[j] + 1);
            FloatValues.RemoveAt(Update[j] + 2);
            FloatValues.RemoveAt(Update[j] + 1);
            Levels[Update[j]] -= 1;
            Levels.RemoveAt(Update[j] + 2);
            Levels.RemoveAt(Update[j] + 1);
        }

        
        this.currentLevel -= 1;
        this.symbols[indx] = Symbols.ToArray();
        this.floatValues[indx] = FloatValues.ToArray();
        this.levels[indx] = Levels.ToArray();

    }

    void GenerateSymbol((string, int) data)
    {
        // Destroy the previous symbol if it exists
        if (currentSymbol != null)
        {
            Destroy(currentSymbol);
        }

        // Generate the symbol based on the tuple values
        switch (data.Item1)
        {
            case "+":
                currentSymbol = Instantiate(plusPrefab, GetPosition(data.Item2), UnityEngine.Quaternion.identity);
                currentSymbol.AddComponent<Wiggle>();
                break;
            case "*":
                currentSymbol = Instantiate(multiplicationPrefab, GetPosition(data.Item2), UnityEngine.Quaternion.identity);
                currentSymbol.AddComponent<MoveUpDown>();
                break;
            default:
                currentSymbol = Instantiate(cylinderPrefab, GetPosition(data.Item2), UnityEngine.Quaternion.identity);
                break;
        }

        // Make the symbol easily viewable
        currentSymbol.transform.localScale = UnityEngine.Vector3.one * 2;
    }






    UnityEngine.Vector3 GetPosition(int value)
    {
        if (value <= 1)
        {
            return new UnityEngine.Vector3(-1, 0, -1);
        }
        else
        {
            return new UnityEngine.Vector3(1, 0, -2);
        }
    }
}

public class Wiggle : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(UnityEngine.Vector3.up, Mathf.Sin(Time.time * 5) * 20 * Time.deltaTime);
    }
}

public class MoveUpDown : MonoBehaviour
{
    void Update()
    {
        transform.position += new UnityEngine.Vector3(0, Mathf.Sin(Time.time * 2) * 0.01f, 0);
    }
}
