using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Text numOfBitsText; // number of bits text
    public Text errorPText;
    public Text errorCorrectedText;
    public Text berText1;
    public Text berText2;
    public Text ecrText1;
    public Text ecrText2;

    public void Reset()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }

}
