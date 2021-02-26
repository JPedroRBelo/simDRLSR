

public enum ImageType
{
    Grey,
    Depth
};


public class ImageToSaveProperties
{
    public string filename;
    public string path;
    
    public int width;
    public int height;
    public ImageType type;
    public string basename;

    public ImageToSaveProperties(string filename, string path,int width, int height,  ImageType type){
        this.filename = filename;
        this.path = path;
        this.width = width;
        this.height = height;
        this.type = type;
        this.basename = filename;
    }
}