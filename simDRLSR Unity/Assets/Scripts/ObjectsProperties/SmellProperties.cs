using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OntSenseCSharpAPI;


/*\item perfumado (fragrant): ex:floral, perfume, rosa, violeta; 
	\item amaderado (woody/resinous): ex: cedro, ervas, grama cortada, terra, mofado; 
	\item frutado (fruity (non-citrus): ex: aromático, abacaxi, cereja , morango, banana; 
	\item quimico (chemical): ex:  medicinal, anestésico, desinfetante, ácido, leite azedo; 
	\item fétido (decayed sickening): ex: putrefato, sujo, rançoso, fecal;
	\item mentolado (minty/peppermint): ex: fresco, aromático, anis, alcaçuz; 
	\item doce (sweet): ex: Baunilha, chocolate, amêndoa, caramelo, 
	\item pipoca (popcorn): ex:  amanteigado, pasta de amendoim, oleosa, noz, gorda, amêndoa; 
	\item acre (pungent sickening): ex: alho, cebola, queimado, fumaça;	
	\item limão (lemon): ex: cítrico, laranja, frutado.
    */
//public enum SmellType {NoSmell,FragrantSmell, WoodySmell, FruitySmell, ChemicalSmell, DecayedSmell, MintySmell,SweetSmell,PopcornSmell,PungentSmell,LemonSmell }


public class SmellProperties : MonoBehaviour {

    public OlfactoryAttribute smellType;
    private string nameSmellType;
    private ParticleSystem particleSystem;
    ParticleSystem.MainModule psmain;
    private Color particleColor;
   
    public float alphaColor = 0.65f;

    public Color color;
    public static Dictionary<OlfactoryAttribute, Color> DictSmellColors;

    // Use this for initialization
    void Start () {
        
        particleSystem = GetComponentsInChildren<ParticleSystem>()[0];
        psmain = particleSystem.main;
        nameSmellType = getSmellStatus();
        applyValues();
	}
	
	// Update is called once per frame

    public void applyValues()
    {
        DictSmellColors = new Dictionary<OlfactoryAttribute, Color>()
        {
            { OlfactoryAttribute.noSmell,new Color(0f, 0f, 0f, 0f) },
            { OlfactoryAttribute.fragrantSmell,new Color(0.951f, 0.647f, 1.000f, alphaColor) },
            { OlfactoryAttribute.woodySmell,new Color(0.324f, 0.236f, 0.048f, alphaColor) },
            { OlfactoryAttribute.fruitySmell,new Color(0.999f,0.688f,0.221f,alphaColor) },
            { OlfactoryAttribute.chemicalSmell,new Color(0.0f, 0.959f, 1f, alphaColor) },
            { OlfactoryAttribute.decayedSmell,new Color(0.42f,0.5f,0.28f,alphaColor) },
            { OlfactoryAttribute.mintySmell,new Color(0.927f,0.993f,0.927f,alphaColor) },
            { OlfactoryAttribute.sweetSmell,new Color(0.911f,0.999f,0.353f,alphaColor) },
            { OlfactoryAttribute.popcornSmell,new Color(1f, 0.926f, 0.684f, alphaColor) },
            { OlfactoryAttribute.pungentSmell,new Color(0.327f, 0.263f, 0.331f, alphaColor) },
            { OlfactoryAttribute.lemonSmell,new Color(0.280f,0.999f,0.221f,alphaColor) },
        };
        psmain.startColor = DictSmellColors[smellType];
        nameSmellType = getSmellStatus();
    }

    public string getSmellStatus()
    {
        string str = "Smell Type: " + smellType.ToString(); 
        return str;

    }

    public OlfactoryAttribute getSmellType()
    {
        return smellType;
    }
}
