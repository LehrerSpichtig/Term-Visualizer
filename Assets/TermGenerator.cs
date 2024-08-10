using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TermGenerator : MonoBehaviour
{
    // Define a tuple variable containing a string and an integer
    private string stringData = "3 1 * 0 2 2 ^ 1 4 2";
    private string[] stringDataArray;
    private string[][] symbols;
    private int[][] levels;
    private double[][] floatValues;

    private float[][] positionsNwidths;

    private int maxLevel;
    private int currentLevel;
    private int TopLeftPosition = -30;
    private int LeftPosition = 0;
    private int backPosition = 70;

    private int upOrDown = 1;

    private List<List<GameObject>> termObjects = new List<List<GameObject>>();



    // Prefabs for the different symbols
    public GameObject plusPrefab;
    public GameObject multiplicationPrefab;
    public GameObject cylinderPrefab;

    private GameObject currentSymbol;

    void Start()
    {
        Generate("10 1 + 0 2 2 ^ 1 4 2",0);
        GenerateTerm(0);
    }

    void GenerateTerm(int indx)
    {
        int i = 0;
        this.LeftPosition = this.TopLeftPosition;
        this.positionsNwidths[indx][0] = this.TopLeftPosition;
        //GameObject[][] numbers = new GameObject[symbols.Length/2 + 1][];
        this.upOrDown = 1;

        bool oneHandled = false;

        while (i < symbols[indx].Length)
        {
            if (floatValues[indx][i] > double.MinValue)
            {
                if (levels[indx][i] < this.currentLevel  || levels[indx].Length == 1)
                {
                    termObjects.Add(GenerateNumber(floatValues[indx][i],indx,i));
                    i++;
                }
                else
                {
                    if (!oneHandled){
                        oneHandled = true;
                        termObjects.Add(GenerateCalculation(floatValues[indx][i], symbols[indx][i + 1], floatValues[indx][i + 2],indx,i));
                    i += 3;
                    }
                    else {
                        termObjects.Add(GenerateNumber(floatValues[indx][i],indx,i));
                        i++;

                    }

                }
            }
            else
            {
                //Symbol ausgeben
                termObjects.Add(GenerateSymbol(symbols[indx][i],indx,i));
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

    List<GameObject> GenerateNumber(double number, int indx, int localIndex)
    {
        int PrecommaDigits = (int)Math.Floor(number);
        int RightPosition = this.LeftPosition;
        /*int halfWidth = HalfWidthOfNumber(number);
        if (halfWidth != 0){
            positionsNwidths[indx][localIndex] += halfWidth;
        }*/
        
        RightPosition = (int) positionsNwidths[indx][localIndex];//this.LeftPosition;
        
        
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
            UnityEngine.Vector3 versch = new(v, h, 0);
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = center + versch * 2;
            i += 1;
            Objects.Add(c);
        }
        //TO DO: Nachkommazahlen

        this.LeftPosition = this.LeftPosition + RightPosition * 2 + 5;
        if (localIndex < this.positionsNwidths[indx].Length-1){
            this.positionsNwidths[indx][localIndex+1] =positionsNwidths[indx][localIndex] + RightPosition * 2 + 5;
        }
            

        return Objects;

        //termObjects.Add(Objects);

    }

    List<GameObject> GenerateNumber2(double number, UnityEngine.Vector3 position){
        int PrecommaDigits = (int)Math.Floor(number);

        List<GameObject> Objects = new List<GameObject>();
        UnityEngine.Vector3 center = position;


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
            }
            else { h += pos; }
            UnityEngine.Vector3 versch = new(v, h, 0);
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = center + versch * 2;
            i += 1;
            Objects.Add(c);
        }
        //TO DO: Nachkommazahlen

        return Objects;
    }

    int HalfWidthOfNumber(double number){
        int RightPosition = 0;
        int v = 0;
        int h = 0;
        bool waagr = false;
        int pos = -1;
        int i = 0;
        int j = 0;
        int diff = 1;
        while (i < number - 1)
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
            i += 1;
        }
        return RightPosition * 2;
    }

    IEnumerator SummenRoutine(float duration, double leftNumber, string operation, double rightNumber, int indx, int localIndex)
    {
        int RightPosition = this.LeftPosition;
        RightPosition = (int) this.positionsNwidths[indx][localIndex];


        while (true)
        {
            List<GameObject> left = GenerateNumber(leftNumber, indx, localIndex);
            List<GameObject> op = GenerateSymbol(operation, indx, localIndex+1);
            List<GameObject> right = GenerateNumber(rightNumber, indx, localIndex+2);
            int HalWidthRightN = HalfWidthOfNumber(rightNumber);

            yield return new WaitForSecondsRealtime(duration);
            foreach (GameObject o in op)
            {
                Destroy(o);
            }
            yield return new WaitForSecondsRealtime(duration);
            UnityEngine.Vector3 schieben = new UnityEngine.Vector3(-(HalWidthRightN+5), 0, 0);
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
            List<GameObject> summe = GenerateNumber(leftNumber + rightNumber, indx, localIndex + 1);
            yield return new WaitForSecondsRealtime(duration);
            foreach (GameObject o in summe)
            {
                Destroy(o);
            }
            this.LeftPosition = RightPosition;
        }

    }

    IEnumerator MultiplicationRoutine(float duration, double leftNumber, string operation, double rightNumber, int indx, int localIndex){
        int RightPosition = this.LeftPosition;
        RightPosition = (int) this.positionsNwidths[indx][localIndex];

        int wL = HalfWidthOfNumber(leftNumber);
        UnityEngine.Vector3 schieben = new UnityEngine.Vector3(-(wL+5), 0, 0);

        while (true){

            List<GameObject> left = GenerateNumber(leftNumber, indx, localIndex);
            List<GameObject> op = GenerateSymbol(operation, indx, localIndex + 1);
            List<GameObject> right = GenerateNumber(rightNumber, indx, localIndex + 2);
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

            
            UnityEngine.Vector3 start = new UnityEngine.Vector3(RightPosition, 10*this.upOrDown, indx*10);
            UnityEngine.Vector3 run = start;

            for (int i = 0;i<rightNumber-1;i++){
                summanden[i] = GenerateNumber2(leftNumber, run);
                yield return new WaitForSecondsRealtime(duration/summanden.Length);
                run += new UnityEngine.Vector3(wL+5,0,0);

                pluse[i] = GenerateSymbol2("+", run);
                run += new UnityEngine.Vector3(wL+5,0,0);
            }
            summanden[summanden.Length-1] = GenerateNumber2(leftNumber, run);
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
                run -= new UnityEngine.Vector3(2*wL+10,0,0);
                //this.LeftPosition -= 10;
                summanden[i] = GenerateNumber2(zwischensumme, run);
                yield return new WaitForSecondsRealtime(duration/summanden.Length);
            }
            foreach (GameObject o in summanden[0]){
                Destroy(o);
            }
            //this.LeftPosition = RightPosition + 10;
            List<GameObject> resultat = GenerateNumber(leftNumber*rightNumber, indx, localIndex + 1);
            yield return new WaitForSecondsRealtime(duration);
            foreach (GameObject o in resultat){
                Destroy(o);
            }
            this.LeftPosition = RightPosition;

        }

    }


    IEnumerator PowerRoutine(float duration, double leftNumber, string operation, double rightNumber, int indx, int localIndex){
        int RightPosition = this.LeftPosition;
        RightPosition = (int) this.positionsNwidths[indx][localIndex];

        int wL = HalfWidthOfNumber(leftNumber);
        UnityEngine.Vector3 schieben = new UnityEngine.Vector3(-(wL+5), 0, 0);

        while (true){

            List<GameObject> left = GenerateNumber(leftNumber, indx, localIndex);
            List<GameObject> op = GenerateSymbol(operation, indx, localIndex + 1);
            List<GameObject> right = GenerateNumber(rightNumber, indx, localIndex + 2);
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
            List<GameObject>[] factors = new List<GameObject>[(int) rightNumber];
            List<GameObject>[] dots = new List<GameObject>[(int) rightNumber-1];

            
            UnityEngine.Vector3 start = new UnityEngine.Vector3(RightPosition, 10*this.upOrDown, indx*10);
            UnityEngine.Vector3 run = start;

            for (int i = 0;i<rightNumber-1;i++){
                factors[i] = GenerateNumber2(leftNumber, run);
                yield return new WaitForSecondsRealtime(duration/dots.Length);
                run += new UnityEngine.Vector3(wL+5,0,0);

                dots[i] = GenerateSymbol2("*", run);
                run += new UnityEngine.Vector3(wL+5,0,0);
            }
            factors[factors.Length-1] = GenerateNumber2(leftNumber, run);
            yield return new WaitForSecondsRealtime(duration/factors.Length);
            for (int i = dots.Length-1;i>=0;i--){
                foreach (GameObject o in dots[i]){
                    //Destroy(o);
                    // Get the Renderer component from the new cube
                    var cubeRenderer = o.GetComponent<Renderer>();

                     // Call SetColor using the shader property name "_Color" and setting the color to red
                     cubeRenderer.material.SetColor("_Color", Color.red);
                }
                
                foreach (GameObject o in factors[i+1]){
                    //o.transform.position += schieben;
                    var cubeRenderer = o.GetComponent<Renderer>();
                    cubeRenderer.material.SetColor("_Color", Color.red);
                    yield return new WaitForSecondsRealtime(duration/factors[i+1].Count);
                }
                foreach (GameObject o in factors[i]){
                    //o.transform.position += schieben;
                    var cubeRenderer = o.GetComponent<Renderer>();
                    cubeRenderer.material.SetColor("_Color", Color.red);
                    yield return new WaitForSecondsRealtime(duration/factors[i+1].Count);
                }
                yield return new WaitForSecondsRealtime(duration/factors.Length);
                int zwischenproduct = factors[i].Count * factors[i+1].Count;
                this.LeftPosition = (int) factors[i][0].transform.position.x;
                foreach (GameObject o in factors[i+1]){
                    Destroy(o);
                }
                foreach (GameObject o in dots[i]){
                    Destroy(o);
                }
                foreach (GameObject o in factors[i]){
                    Destroy(o);
                }
                run -= new UnityEngine.Vector3(2*wL+10,0,0);
                GameObject leftBracket = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                var BracketRenderer = leftBracket.GetComponent<Renderer>();
                BracketRenderer.material.SetColor("_Color", Color.blue);
                leftBracket.transform.position = run;
                run += new UnityEngine.Vector3(wL+5,0,0);
                List<GameObject>[] leftSummanden = new List<GameObject>[(int)(zwischenproduct/leftNumber)];
                List<GameObject>[] pluse = new List<GameObject>[leftSummanden.Length-1];
            for (int j = 0;j<zwischenproduct/leftNumber-1;j++){
                leftSummanden[j] = GenerateNumber2(leftNumber, run);
                yield return new WaitForSecondsRealtime(duration/((int) (zwischenproduct/leftNumber)));
                run += new UnityEngine.Vector3(wL+5,0,0);

                pluse[j] = GenerateSymbol2("+", run);
                run += new UnityEngine.Vector3(wL+5,0,0);
            }
            leftSummanden[(int)(zwischenproduct/leftNumber)-1] = GenerateNumber2(leftNumber, run);
            run += new UnityEngine.Vector3(wL+5,0,0);
            GameObject rightBracket = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            BracketRenderer = rightBracket.GetComponent<Renderer>();
            BracketRenderer.material.SetColor("_Color", Color.blue);
            rightBracket.transform.position = run;
            run -= new UnityEngine.Vector3(2*wL+10,0,0);
            yield return new WaitForSecondsRealtime(duration);
            //aufsummieren 
            double zwischensumme = leftNumber;
            for (int j = pluse.Length-1;j>=0;j--){
                
                foreach (GameObject o in pluse[j]){
                    Destroy(o);
                }
                yield return new WaitForSecondsRealtime(duration/5);
                foreach (GameObject o in leftSummanden[j+1]){
                    o.transform.position /*-= new UnityEngine.Vector3(wL+5,0,0);*/+= schieben;
                }
                rightBracket.transform.position /*-= new UnityEngine.Vector3(wL+5,0,0);*/+= schieben;
                yield return new WaitForSecondsRealtime(duration/leftSummanden.Length);
                zwischensumme += leftNumber;
                
                foreach (GameObject o in leftSummanden[j+1]){
                    Destroy(o);
                }
                foreach (GameObject o in leftSummanden[j]){
                    Destroy(o);
                }

                run -= new UnityEngine.Vector3(2*wL+10,0,0);
                //this.LeftPosition -= 10;
                leftSummanden[j] = GenerateNumber2(zwischensumme, run);
                yield return new WaitForSecondsRealtime(duration/leftSummanden.Length);
            }
            foreach (GameObject o in leftSummanden[0]){
                Destroy(o);
            }
            Destroy(leftBracket);
            Destroy(rightBracket);


            

                //this.LeftPosition -= 10;
                factors[i] = GenerateNumber2(zwischenproduct, run);
                yield return new WaitForSecondsRealtime(duration/factors.Length);
            }
            foreach (GameObject o in factors[0]){
                Destroy(o);
            }
            //this.LeftPosition = RightPosition + 10;
            List<GameObject> resultat = GenerateNumber(Math.Pow(leftNumber,rightNumber), indx, localIndex + 1);
            yield return new WaitForSecondsRealtime(duration);
            foreach (GameObject o in resultat){
                Destroy(o);
            }
            this.LeftPosition = RightPosition;

        }

    }

    List<GameObject> GenerateCalculation(double leftNumber, string operation, double rightNumber, int indx, int localIndex)
    {
        List<GameObject> objects = new List<GameObject>();
        float duration;

        switch (operation)
        {
            case "+":

                duration = 2f;
                StartCoroutine(SummenRoutine(duration, leftNumber, operation, rightNumber, indx, localIndex));

                //objects = GenerateNumber(leftNumber + rightNumber);

                break;
            case "-":
                break;
            case "*":
                duration = 2f;
                this.upOrDown *= -1;
                StartCoroutine(MultiplicationRoutine(duration, leftNumber, operation, rightNumber, indx, localIndex));



                break;
            case "/":
                this.upOrDown *= -1;
                break;
            case "^":
                duration = 2f;
                this.upOrDown *= -1;
                StartCoroutine(PowerRoutine(duration, leftNumber, operation, rightNumber, indx, localIndex));


                break;
            default:
                break;
        }

        return objects;

    }

    List<GameObject> GenerateSymbol(string operation, int indx, int localIndex)
    {
        List<GameObject> objects = new List<GameObject>();
        int RightPosition = (int) this.positionsNwidths[indx][localIndex];
        UnityEngine.Vector3 center = new UnityEngine.Vector3(RightPosition/*this.LeftPosition*/, 0, indx*this.backPosition);

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
        this.positionsNwidths[indx][localIndex + 1] = RightPosition + 5;
        return objects;
        //termObjects.Add(objects);
    }

    List<GameObject> GenerateSymbol2(string operation, UnityEngine.Vector3 position){
        List<GameObject> objects = new List<GameObject>();
        
        UnityEngine.Vector3 center = position;

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

        return objects;
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
        int anzahlOperationen=0;
        foreach (double t in floatValues2){
            if (t == double.MinValue) anzahlOperationen += 1;
        }
        this.symbols = new string[anzahlOperationen/*maxLevel*/+1][]; //stringDataArray.Length / 2];
        this.levels = new int[anzahlOperationen/*maxLevel*/+1][];
        this.floatValues = new double[anzahlOperationen/*maxLevel*/+1][];
        this.positionsNwidths = new float[anzahlOperationen/*maxLevel*/+1][];

        this.symbols[0] = symbols2;
        this.levels[0]  = levels2;
        this.floatValues[0] = floatValues2;
        this.positionsNwidths[0] = new float[stringDataArray.Length/2];
    }
    private void UpdateTerm(int indx)
    {//string[] symbols, int[] levels, float[] floatValues, int currentLevel){

        List<string> Symbols = symbols[indx-1].ToList<string>();
        List<int> Levels = levels[indx-1].ToList<int>();
        List<double> FloatValues = floatValues[indx-1].ToList<double>();

        

        int i = 0;
        List<int> Update = new List<int>();
        bool oneHandled = false;
        List<string> strich = new List<string>();strich.Add("+");strich.Add("-");
        List<string> punkt = new List<string>();punkt.Add("*");punkt.Add("/");
        while (i < Levels.Count & !oneHandled)
        {
            if (Levels[i] == currentLevel)
            {
                Update.Add(i);
                switch (Symbols[i + 1])
                {
                    case "+":
                        FloatValues[i] = FloatValues[i] + FloatValues[i + 2];
                        oneHandled =true;
                        break;
                    case "-":
                        FloatValues[i] = FloatValues[i] - FloatValues[i + 2];
                        oneHandled =true;
                        break;
                    case "*":
                        FloatValues[i] = FloatValues[i] * FloatValues[i + 2];
                        oneHandled =true;
                        break;
                    case "/":
                        FloatValues[i] = FloatValues[i] / FloatValues[i + 2];
                        oneHandled =true;
                        break;
                    case "^":
                        FloatValues[i] = Math.Pow(FloatValues[i], FloatValues[i + 2]);
                        oneHandled =true;
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
            string operation = Symbols[Update[j]+1];
            Symbols.RemoveAt(Update[j] + 2);
            Symbols.RemoveAt(Update[j] + 1);
            FloatValues.RemoveAt(Update[j] + 2);
            FloatValues.RemoveAt(Update[j] + 1);
            Levels[Update[j]] -= 1;
            Levels.RemoveAt(Update[j] + 2);
            Levels.RemoveAt(Update[j] + 1);
            if (strich.Contains(operation) && Update[j]+1<Symbols.Count && strich.Contains(Symbols[Update[j]+1]) && Levels[Update[j]+1]==Levels[Update[j]]){
                Levels[Update[j]] += 1;
            }
            if (punkt.Contains(operation) && Update[j]+1<Symbols.Count && punkt.Contains(Symbols[Update[j]+1]) && Levels[Update[j]+1]==Levels[Update[j]]){
                Levels[Update[j]] += 1;
            }

        }

        //UNBEDINGT "this" anfügen!!
        if (!Levels.Contains(currentLevel)){
            this.currentLevel -= 1;
        }
        
        this.symbols[indx] = Symbols.ToArray();
        this.floatValues[indx] = FloatValues.ToArray();
        this.levels[indx] = Levels.ToArray();
        this.positionsNwidths[indx] = new float[this.levels[indx].Length];

    }
    /*private void UpdateTerm(int indx)
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
        this.positionsNwidths[indx] = new float[this.levels[indx].Length];

    }*/



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
