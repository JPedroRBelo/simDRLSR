using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchProperties : MonoBehaviour {


    /*As seguintes informações são disponibilizadas
\begin {itemize}
\item Temperatura (Temperature): temperatura em graus centigrados,
\item Pressão (pressure):  pressão exercida pelo objeto, parametrizada de 0 à 1,
\item Aspereza (roughness): nivel de rugosidade do objeto, parametrizada de 0 à 1,
\item Humidade (moistness): humidade presente no objeto parametrizada de 0 à 1,
\item Dureza (hardness): medida da resitência do material quando uma força compressiva é aplicada em sua superficie, parametrizada de 0 à 1, 
\end {itemize}
*/
    private const float NEUTRAL_PRESSURE = 0;
    [SerializeField]
    private bool roomTemperature = false;
    [SerializeField]    
    private float temperature = Constants.ROOM_TEMPERATURE;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float pressure;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float roughness;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float moistness;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float hardness;

    private bool pressed;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pressed = false;
        if (rb != null)
        {
            pressure = Mathf.Min(rb.mass, Constants.MAX_WEIGHT) / Constants.MAX_WEIGHT;
        }
        if (roomTemperature)
        {
            temperature = Constants.ROOM_TEMPERATURE;
        }
    }

    public float getTemperature()
    {
        return temperature;
    }

    public float getPressure()
    {
        return pressure;
    }

    public float getRoughness()
    {
        return roughness;
    }

    public float getMoistness()
    {
        return moistness;
    }

    public float getHardness()
    {
        return hardness;
    }

    public string getTouchStatus()
    {
        string str = "Temperature: " + temperature+" ºC";
        if (pressed)
            str += "\nPressure: " + pressure;
        else
            str += "\nPressure: " + NEUTRAL_PRESSURE;
        str += "\nRoughness: " + roughness;
        str += "\nMoistness: " + moistness;
        str += "\nHardness: " + hardness;
        return str;
    }

    public bool isPressed()
    {
        return pressed;
    }

    public void exertPressure(bool pressed)
    {
        this.pressed = pressed;
    }
}
