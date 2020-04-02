using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainScreenController : MonoBehaviour
{
    public GameObject prefab;
    public GameObject parentObject;
    public Text mainStart;
    public Text iterationStart;

    private float delay = 1;

    public Color colorDied;
    public Color colorLiving;

    public InputField iterationInput;

    private GameObject tempGameObject;

    private int widthTotal;
    private int heightTotal;

    private List<Point> points;
    List<Color> neighbourColors;

    public Image[,] gameObjects;

    private void OnEnable()
    {
        widthTotal = PlayerPrefs.GetInt("width");
        heightTotal = PlayerPrefs.GetInt("height");

        colorDied = new Color32(213, 30, 51, 255);
        colorLiving = new Color32(73, 149, 7, 255);

        gameObjects = new Image[heightTotal, widthTotal];

        SetCellSizeAndCanvas();

        InitiateCells();
    }
    public void OnBackButtonClick()
    {
        SceneManager.LoadScene(0);
    }

    public void OnResetClick()
    {
        for (int i = 0; i < heightTotal; i++)
        {
            for (int j = 0; j < widthTotal; j++)
            {
                gameObjects[i, j].color = colorDied;
            }
        }
    }

    public void OnStableClick()
    {
        int centerX = (int)(widthTotal / 2)-1;
        int centerY = (int)(heightTotal / 2)-1;

        gameObjects[centerX, centerY].color = colorLiving;
        gameObjects[centerX + 1, centerY + 1].color = colorLiving;
        gameObjects[centerX + 2, centerY + 1].color = colorLiving;
        gameObjects[centerX+3, centerY].color = colorLiving;
        gameObjects[centerX +1, centerY -1].color = colorLiving;
        gameObjects[centerX +2, centerY -1].color = colorLiving;
    }

    public void OnGliderClick()
    {
        int centerX = (int)(widthTotal / 2) - 1;
        int centerY = (int)(heightTotal / 2) - 1;

        gameObjects[centerY, centerX].color = colorLiving;
        gameObjects[centerY+1, centerX].color = colorLiving;
        gameObjects[centerY+1, centerX-1].color = colorLiving;
        gameObjects[centerY-1, centerX-1].color = colorLiving;
        gameObjects[centerY, centerX+1].color = colorLiving;
    }

    public void OnOscillatorClick()
    {
        int centerX = (int)(widthTotal / 2) - 1;
        int centerY = (int)(heightTotal / 2) - 1;

        gameObjects[centerY, centerX].color = colorLiving;
        gameObjects[centerY+1, centerX].color = colorLiving;
        gameObjects[centerY-1, centerX].color = colorLiving;
    }

    public void OnRandomClick()
    {
        for (int i = 0; i < heightTotal; i++)
        {
            for (int j = 0; j < widthTotal; j++)
            {
                gameObjects[i, j].color = RandomColor();
            }
        }
    }
    public void ChangeSpeed(bool plus)
    {
        if (plus)
        {
            delay -= 0.05f;
        }
        else
        {
            delay += 0.05f;
        }
        if (delay < 0)
        {
            delay = 0.04f; //Set max speed because delay cannot be less than 0 seconds
        }
    }

    public void ToggleStart()
    {
        bool starting;

        if (mainStart.text.Equals("START"))
        {
            starting = true;
            mainStart.text = "STOP";
        }
        else
        {
            starting = false;
            mainStart.text = "START";
        }

        if (starting)
        {
            StartCoroutine(NextAuto());
        }
        else
        {
            StopAllCoroutines();
        }

    }

    public void PlayXTimes()
    {
        bool starting;
        if (string.IsNullOrEmpty(iterationInput.text))
        {
            return;
        }

        if (iterationStart.text.Equals("PLAY x TIMES"))
        {
            starting = true;
            iterationStart.text = "STOP";
        }
        else
        {
            starting = false;
            iterationStart.text = "PLAY x TIMES";
        }

        if (starting)
        {
            StartCoroutine(NextAuto(int.Parse(iterationInput.text)));
        }
        else
        {
            StopAllCoroutines();
        }
    }

    private Color RandomColor()
    {
        if (Random.Range((int)0, (int)2) == 1)
        {
            return colorLiving;
        }
        else
        {
            return colorDied;
        }
    }
    IEnumerator NextAuto()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(delay);
            NextStep();
        } 
    }

    IEnumerator NextAuto(int times)
    {
        for (int i=0; i<times; i++)
        {
            yield return new WaitForSeconds(delay);
            NextStep();
        }
    }

    public void NextStep()
    {
        Color[,] newColors = new Color[heightTotal, widthTotal];
        for (int i = 0; i < heightTotal; i++)
        {
            for (int j = 0; j < widthTotal; j++)
            {
                points = new List<Point>();
                neighbourColors = new List<Color>();

                points.Add(new Point(i - 1, j - 1,widthTotal,heightTotal)); //Left Upper
                points.Add(new Point(i - 1, j ,widthTotal,heightTotal)); //Upper
                points.Add(new Point(i - 1, j + 1,widthTotal,heightTotal)); //Right Upper
                points.Add(new Point(i, j - 1,widthTotal,heightTotal)); //Left
                points.Add(new Point(i , j + 1,widthTotal,heightTotal)); //Right
                points.Add(new Point(i + 1, j - 1,widthTotal,heightTotal)); //Left Lower
                points.Add(new Point(i + 1, j,widthTotal,heightTotal)); //Lower
                points.Add(new Point(i + 1, j + 1,widthTotal,heightTotal)); //Right Lower

                foreach (Point point in points)
                {
                    neighbourColors.Add(gameObjects[point.i, point.j].color);
                }

                newColors[i, j] = CheckNeighbours(neighbourColors, gameObjects[i, j].color);
            }
        }

        //Replace old colors
        for (int i = 0; i < heightTotal; i++)
        {
            for (int j = 0; j < widthTotal; j++)
            {
                gameObjects[i, j].color = newColors[i, j];
            }
        }
    }

    private Color CheckNeighbours(List<Color> colors, Color myColor)
    {
        int counter = 0;
        foreach (Color color in colors)
        {
            if (color.Equals(colorLiving))
            {
                counter++;
                if (counter > 3)
                {
                    //A bit of optimalization
                    return colorDied;
                    // Cell has got more neighbours than 3 so dies anyway by overcrowding
                    // It's no needed to checking rest of cells
                }
            }
        }
        if (myColor.Equals(colorLiving)) //If cell is living
        {
            // If cell is living and wants to survive, only 2 and 3 neighbours are allowed
            if (counter == 2 || counter == 3)
            {
                return colorLiving; // Cell survived
            }
        }
        else
        {
            if (counter == 3)
            {
                return colorLiving; // Cell was born
            }
        }
        return colorDied; // Cell has got less than 2 neighbours so dies by underpopulation
    }

    private void InitiateCells()
    {
        for (int i = 0; i < heightTotal; i++)
        {
            for (int j = 0; j < widthTotal; j++)
            {
                tempGameObject = Instantiate(prefab, parentObject.transform);
                gameObjects[i, j] = tempGameObject.GetComponent<Image>();
                gameObjects[i, j].color = colorDied;
            }
        }
    }

    private void SetCellSizeAndCanvas()
    {
        float canvasWidth = parentObject.GetComponent<RectTransform>().sizeDelta.x;
        float canvasHeight = parentObject.GetComponent<RectTransform>().sizeDelta.y;
        float cellX = canvasWidth / widthTotal;
        float cellY = canvasHeight / heightTotal;
        float squareCellSide;
        int multiplier;

        //Cells need to be a squares with lower values (to fill canvas)
        if (cellX < cellY)
        {
            squareCellSide = cellX;
            multiplier = widthTotal;
        }
        else
        {
            squareCellSide = cellY;
            multiplier = heightTotal;
        }
        //Cell size is well counted
        parentObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(squareCellSide, squareCellSide);

        //Recalculate width and height of canvas
        parentObject.GetComponent<RectTransform>().sizeDelta = new Vector2((widthTotal * squareCellSide), (heightTotal * squareCellSide));
    }
}
public struct Point
{
    public int i;
    public int j;
    public Point(int i, int j,int widthTotal, int heightTotal)
    {
        if (i < 0)
        {
            this.i = heightTotal - 1;
        }
        else if (i == heightTotal)
        {
            this.i = 0;
        }
        else
        {
            this.i = i;
        }

        if (j < 0)
        {
            this.j = widthTotal - 1;
        }
        else if (j == widthTotal)
        {
            this.j = 0;
        }
        else
        {
            this.j = j;
        }
    }
    public override string ToString()
    {
        return "[" + this.i + "," + this.j + "]";
    }

}

