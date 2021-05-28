 using UnityEngine;
 using System.Collections;
 
 public class CaptureGray : MonoBehaviour {
     public int resWidth = 640; 
     public int resHeight = 480;
 
     private bool takeHiResShot = false;
     
      public RenderTexture rt;    
 
     public static string ScreenShotName(int width, int height) {
         return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png", 
                              Application.dataPath, 
                              width, height, 
                              System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
     }
 
     public void TakeHiResShot() {
         takeHiResShot = true;
     }
 
     void LateUpdate() {
         takeHiResShot |= Input.GetKeyDown("k");
         if (takeHiResShot) {
             /*
            ScreenCapture.CaptureScreenshot("SomeLevel");
             RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
             GetComponent<Camera>().targetTexture = rt;
             Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
             GetComponent<Camera>().Render();
             RenderTexture.active = rt;
             screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
             GetComponent<Camera>().targetTexture = null;
             RenderTexture.active = null; // JC: added to avoid errors
             Destroy(rt);
             byte[] bytes = screenShot.EncodeToPNG();
             string filename = ScreenShotName(resWidth, resHeight);
             System.IO.File.WriteAllBytes(filename, bytes);
             Debug.Log(string.Format("Took screenshot to: {0}", filename));
             */
             SaveTexture();
             takeHiResShot = false;
         }
     }

    public void SaveTexture () {
            byte[] bytes = toTexture2D(rt).EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
    }
    public Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }



     
 }