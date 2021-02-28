using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Experimental.Rendering;

public class TestSaveImage : MonoBehaviour
{
    // Start is called before the first frame update
    public GraphicsFormat format = GraphicsFormat.R8G8B8A8_UNorm;
    public bool save = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(save){
            
            string filePath = "/home/josepedro/deepepper/simMDQN/DataGeneration-Phase/dataset/image_1_1.png";
            //MakeGrayscale3(filePath);
            save = false;

        }
        
    }

    
    public void TextureTest(string filePath){
       
        Texture2D tex = null;
        byte[] fileData;
        // Bitmap grayScaleBP = new System.Drawing.Bitmap(2, 2, System.Drawing.Imaging.PixelFormat.Format8bppGrayScale);

        if (File.Exists(filePath))     {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            tex = ChangeFormat(tex,format);
            print(tex.format);
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes("/home/josepedro/deepepper/simMDQN/DataGeneration-Phase/dataset/image_test_test.png", bytes);
        }else{
            print("File not found");
        }
    }
    /*
    public Bitmap MakeGrayscale3(string filePath)
    {
    Bitmap original = new Bitmap(filePath);
    //create a blank bitmap the same size as original
    Bitmap newBitmap = new Bitmap(original.Width, original.Height);

    //get a graphics object from the new image
    using(Graphics g = Graphics.FromImage(newBitmap)){

        //create the grayscale ColorMatrix
        ColorMatrix colorMatrix = new ColorMatrix(
            new float[][] 
            {
                new float[] {.3f, .3f, .3f, 0, 0},
                new float[] {.59f, .59f, .59f, 0, 0},
                new float[] {.11f, .11f, .11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

        //create some image attributes
        using(ImageAttributes attributes = new ImageAttributes()){

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                        0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
        }
    }
    return newBitmap;
    
    }
    */

    private  Texture2D ChangeFormat( Texture2D oldTexture, GraphicsFormat newFormat)
    {
        TextureCreationFlags flags = new TextureCreationFlags();
        //Create new empty Texture
        Texture2D newTex = new Texture2D(oldTexture.width, oldTexture.height, newFormat, flags);
        //Copy old texture pixels into new one
        newTex.SetPixels(oldTexture.GetPixels());
        //Apply
        newTex.Apply();

        return newTex;
    }

}
