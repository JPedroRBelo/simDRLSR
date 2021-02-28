using System;
using System.Collections;
using System.Collections.Generic;

public static class Constants {


    /*Constantes de velocidade do Focus
     * 
     * MAXSPD = velocidade máxima, o Focus movimenta instantaneamente
     * NATURALSPD = velocidade que permite o Focus mover de forma natural até o ponto desejado
     * */
    public const float MAXSPD = 1f;
    public const float NATURALSPD = 0.05f;
    public const float NATURAL_LOOK_SPD = 3f;

    //Max weight felt by the robot
    public const float MAX_WEIGHT = 100f;
    public const float ROOM_TEMPERATURE = 20f;

    public const float DIST_HUMAN = 2.3f;
    public const float DIST_DOOR = 0f;
    public const float DIST_OBJECT = 0.41f;
    public const float DIST_SWITCH = 0.41f;
    public const float DIST_TAP = 0.41f;
    public const float DIST_DRAWER = 0f;
    public const float DIST_DEFAULT = 0.2f;


    //Tags utilizadas a fim de identificar cada referencia da cena
    public const string TAG_OBJECT = "Object";
    public const string TAG_LOCATION = "Location";
    public const string TAG_KNOWLOCATIONS = "KnowLocations";
    public const string TAG_PUBLICCHAIR = "PublicChair";
    public const string TAG_WORKCHAIR = "WorkChair";
    public const string TAG_MAGAZINE = "Magazine";
    public const string TAG_SIDEDOOR = "SideDoor";


    public const string TAG_HAND = "Hand";
    public const string TAG_POSITION = "Position";
    public const string TAG_WALL = "Wall";
    public const string TAG_FLOOR = "Floor";
    public const string TAG_CEILING = "Ceiling";
    public const string TAG_WINDOW = "Window";
    public const string TAG_SWITCH = "Switch";
    public const string TAG_FURNITURE = "Furniture";
    public const string TAG_DECOR = "Decor";
    public const string TAG_DOOR = "Door";
    public const string TAG_HUMAN = "Human";
    public const string TAG_TAP = "Tap";
    public const string TAG_DRAWER = "Drawer";
    public const string TAG_BODYPART = "BodyPart";
    public const string TAG_BODYSENSOR = "BodySensor";
    public const string TAG_WATER = "Water";
    public const string TAG_PIE = "Pie";
    public const string TAG_CRACKER = "Cracker";
    public const string TAG_MUG = "Mug";
    public const string TAG_GLASS = "Glass";
    public const string TAG_PLATE = "Plate";
    public const string TAG_BOOK = "Book";
    public const string TAG_CUP_CAKE = "Cup_Cake";
    public const string TAG_BOX = "Box";
    public const string TAG_BOOK_SHELF = "Book_Shelf";
    public const string TAG_SALMON = "Salmon_Pack";
    public const string TAG_MEAT = "Meat_Pack";
    public const string TAG_COOKINGPOT = "Cooking _Pot";
    public const string TAG_FRYINGPAN = "Frying_Pan";
    public const string TAG_SOFA = "Sofa";
    public const string TAG_CARPET = "Carpet";
    public const string TAG_TV = "TV";
    public const string TAG_STAND = "Stand";
    public const string TAG_FRIDGE = "Fridge";
    public const string TAG_CABINET = "Cabinet";
    public const string TAG_PHOTOFRAME = "Photo_Frame";
    public const string TAG_STOVE = "Stove";
    public const string TAG_PAINTING = "Painting";
    public const string TAG_CHAIR = "Chair";
    public const string TAG_COUNTER = "Counter";
    public const string TAG_TABLE = "Table";
    public const string TAG_ROBOT = "Robot";




    public const string TAG_DROPDOWN = "UIDropdown";

    //Tipo de Parametros
    public const string PAR_ROTATION = "Rotation";
    public const string PAR_INTEGER = "Integer";
    public const string PAR_ANIMATIONTIME = "Integer";
    public const string PAR_STRING = "String";
    public const string PAR_NULL = "Null";
    public const string PAR_HAND = "Hand";
    public const string TGGL_LEFT = "LHToggle";
    public const string TGGL_RIGHT = "RHToggle";
    public const string TGGL_SMLLPON = "SmellPartOn";

    public const string LAYER_SMELLPART = "SmellParticle";


    public const string DOOR_OUT_OPEN = "OutOpened";
    public const string DOOR_OUT_CLOSED = "OutClosed";
    public const string DOOR_IN_OPEN = "InOpened";
    public const string DOOR_IN_CLOSED = "InClosed";

    public const string REF_LOCATION = "LocationReference";
    public const string REF_OUTPUT = "OutputReference";
    //Posição de referencia do GameObject. Ex: posição onde deve-se colocar um objeto
    public const string REF_POSITION = "PositionReference";
    public const string REF_CONTAINER = "ContainerReference";
    /* Nome do gameobject que auxilia no posicionamento "dos pés", auxiliando no movimento "agachar".
     * Caso ele não esteja localizado como filho direto do transform principal (Robot Kyle, por exemplo),
     * deve-se colocar o nome como "Pai/AnklePosition" 
     */
    public const string POS_ANKLE = "AnklePosition";
    public const string NAME_TASTESENSOR = "TasteSensor";
    public const string NAME_SMELLSENSOR = "SmellSensor";

    public static string getTypeOfTag(string tag)
    {
        switch (tag)
        {
            case Constants.TAG_DOOR:
                return Constants.TAG_SWITCH;
            case Constants.TAG_PIE:
                return Constants.TAG_OBJECT;
            case Constants.TAG_GLASS:
                return Constants.TAG_OBJECT;
            case Constants.TAG_MUG:
                return Constants.TAG_OBJECT;
            case Constants.TAG_PLATE:
                return Constants.TAG_OBJECT;
            case Constants.TAG_BOOK:
                return Constants.TAG_OBJECT;
            case Constants.TAG_CRACKER:
                return Constants.TAG_OBJECT;
            case Constants.TAG_PHOTOFRAME:
                return Constants.TAG_OBJECT;
            case Constants.TAG_FRYINGPAN:
                return Constants.TAG_OBJECT;
            case Constants.TAG_COOKINGPOT:
                return Constants.TAG_OBJECT;
            case Constants.TAG_SALMON:
                return Constants.TAG_OBJECT;
            case Constants.TAG_MEAT:
                return Constants.TAG_OBJECT;
            case Constants.TAG_BOX:
                return Constants.TAG_OBJECT;
            case Constants.TAG_CUP_CAKE:
                return Constants.TAG_OBJECT;
            case Constants.TAG_DRAWER:
                return Constants.TAG_SWITCH;
            case Constants.TAG_FURNITURE:
                return Constants.TAG_LOCATION;
            case Constants.TAG_TABLE:
                return Constants.TAG_LOCATION;
            case Constants.TAG_BOOK_SHELF:
                return Constants.TAG_LOCATION;
            case Constants.TAG_SOFA:
                return Constants.TAG_LOCATION;
            case Constants.TAG_CARPET:
                return Constants.TAG_LOCATION;
            case Constants.TAG_STOVE:
                return Constants.TAG_LOCATION;
            case Constants.TAG_COUNTER:
                return Constants.TAG_LOCATION;
            case Constants.TAG_CABINET:
                return Constants.TAG_LOCATION;
            case Constants.TAG_FLOOR:
                return Constants.TAG_LOCATION;
            case Constants.TAG_WALL:
                return Constants.TAG_LOCATION;
            case Constants.TAG_HUMAN:
                return Constants.TAG_HUMAN;
            case Constants.TAG_WATER:
                return Constants.TAG_LOCATION;
            case Constants.TAG_TAP:
                return Constants.TAG_SWITCH;
            default:
                break;
        }
        return tag;
    }

}
