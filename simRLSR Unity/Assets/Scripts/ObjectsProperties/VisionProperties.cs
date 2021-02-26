using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OntSenseCSharpAPI;

//-material: basic component used in the elaboration of the object being visualized, examples: wood, metal, plastic, rubber, glass, rock, organic.
//public enum MaterialType {Unknown, Wood, Metal, Plastic, Rubber, Glass, Rock, Organic, Paper, Cloth}
public class VisionProperties : MonoBehaviour {
    
    
    [SerializeField]
    //[Range(0.0f, 1.0f)]
    private Color color;
    [SerializeField]
    //[Range(0.0f, 1.0f)]
    private OntSenseCSharpAPI.Material material = OntSenseCSharpAPI.Material.unknownMaterial;
    [SerializeField]
    private string uri = "";
    private Rigidbody rb;
    void Start()
    {
    }

    public float getRed()
    {
        return color.r;
    }

    public float getGreen()
    {
        return color.g;
    }

    public float getBlue()
    {
        return color.b;
    }

    public RGBValue getRGB()
    {
        RGBValue rgb = new RGBValue(color.r, color.g, color.b);
        return rgb;
    }

    public string getVisionStatus()
    {
        string str = "RGB: " + getBlue().ToString("F3") + ", "+ getGreen().ToString("F3") + ", " + getRed().ToString("F3");
        str += "\nMaterial: " + material.ToString();
        return str;
    }

    public string getTag()
    {
        return transform.tag;
    }

    public string getName()
    {
        return transform.name;
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public OntSenseCSharpAPI.Material getMaterial()
    {
        return material;
    }

    public string getURI()
    {
        return uri;
    }
    
}


