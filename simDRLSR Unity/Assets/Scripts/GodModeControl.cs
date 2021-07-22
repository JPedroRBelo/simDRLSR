/*using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class GodModeControl: MonoBehaviour {


        public float surfaceOffset = 0.1f;
        private enum TargetStates { Off, Set, Fixed };
        private TargetStates targetState;
        public Transform targetPicker;
    private Transform objectTarget;
        public Vector3 targetOffset;
        public float distance = 5.0f;
        private float maxDistance = Mathf.Infinity;
        private float minDistance = -Mathf.Infinity;
        public float xSpeed = 200.0f;
        public float ySpeed = 200.0f;
        public int yMinLimit = -80;
        public int yMaxLimit = 80;
        public int zoomRate = 80;
        public float panSpeed = 0.3f;
        public float zoomDampening = 5.0f;

        private float xDeg = 0.0f;
        private float yDeg = 0.0f;
        private float currentDistance;
        private float desiredDistance;
        private Quaternion currentRotation;
        private Quaternion desiredRotation;
        private Quaternion rotation;
        private Vector3 position;

    private Vector3 targetPickerPosition;
    private enum PickerMode { Object,Position};
    private PickerMode mode;
        private Ray ray;
        private RaycastHit hit;

    private Renderer atRenderer;
    private Renderer lastRenderer;
    private Dictionary<Material, string> dictAtMaterialShader;
    private Dictionary<Material, string> dictLastMaterialShader;

    private GameObject highlightedGObject;

    void Start() {
        if (!objectTarget)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            objectTarget = go.transform;
        }
        highlightedGObject = null;
        dictAtMaterialShader = new Dictionary<Material, string>();
        dictLastMaterialShader = new Dictionary<Material, string>();
        lastRenderer = null;
        atRenderer = null;

        distance = Vector3.Distance(transform.position, objectTarget.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
        targetState = TargetStates.Off;

        mode = PickerMode.Object;
        Debug.Log("RHS>>> " + this.name + " " + this.GetType() + " is ready.");
    }



    void Update()
    {
        if (GetComponent<Camera>().enabled)
        {
            if (targetState == TargetStates.Off)
            {
                targetPicker.gameObject.SetActive(false);
            }
            else
            {
                targetPicker.gameObject.SetActive(true);
            }
            if (mode == PickerMode.Object)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        //print(EventSystem.current.currentSelectedGameObject == dropActions.gameObject);
                        GameObject aux = EventSystem.current.currentSelectedGameObject;
                        if (aux == null)
                        {

                            ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                            if (Physics.Raycast(ray, out hit))
                            {
                                string itemName = hit.collider.gameObject.name;
                                GameObject gO = hit.collider.gameObject;
                                GameObject originalGO = gO;
                                print(originalGO.name);
                                if (itemName.Contains("Collider", StringComparison.OrdinalIgnoreCase) || itemName.Contains("GameObject", StringComparison.OrdinalIgnoreCase))
                                {
                                    gO = hit.collider.transform.parent.gameObject;
                                }
                                highlightObject(gO,originalGO);
                                
                            }

                        }
                    }
                }

            }
            else
            {
                
                    //print(EventSystem.current.currentSelectedGameObject == dropActions.gameObject);
                    GameObject aux = EventSystem.current.currentSelectedGameObject;
                    if (aux == null)
                    {
                        if (targetState == TargetStates.Set)
                        {
                            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                            RaycastHit hit;
                            if (!Physics.Raycast(ray, out hit))
                            {
                                return;
                            }
                            targetPicker.position = hit.point + hit.normal * surfaceOffset;
                            if (Input.GetMouseButtonDown(0))
                            {
                                targetPickerPosition = targetPicker.position;
                                targetState = TargetStates.Fixed;
                            }
                        }
                        else if (targetState == TargetStates.Fixed)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                targetState = TargetStates.Set;
                            }
                        }
                    }               
            }
        }
        else
        {
            resetHighlight();
        }        
    }

    public void chooseOnEnvironment()
    {
        if (targetState == TargetStates.Off)
        {
            targetState = TargetStates.Set;
            mode = PickerMode.Position;
        }
        else
        {
            highlightMode();
        }
    }

    public void highlightMode()
    {
        targetState = TargetStates.Off;
        mode = PickerMode.Object;
    }

    private void highlightObject(GameObject gO, GameObject originalGO)
    {
        atRenderer = originalGO.GetComponent<Renderer>();
        if (atRenderer == null)
        {
            atRenderer = originalGO.GetComponent<Renderer>();
        }
        dictAtMaterialShader = new Dictionary<Material, string>();
        if (atRenderer != null && lastRenderer != atRenderer)
        {
            foreach (Material m in atRenderer.materials)
            {
                dictAtMaterialShader.Add(m, m.shader.name);
                m.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
                //m.shader = Shader.Find("Outlined/Regular");
                highlightedGObject = gO;
            }
            if (lastRenderer == null)
            {
                lastRenderer = atRenderer;
                dictLastMaterialShader = dictAtMaterialShader;
            }
            targetPickerPosition = gO.transform.position;
        }
        if (lastRenderer != null && lastRenderer != atRenderer)
        {
            foreach (Material m in lastRenderer.materials)
            {
                m.shader = Shader.Find(dictLastMaterialShader[m]);
            }
            dictLastMaterialShader = dictAtMaterialShader;
            lastRenderer = atRenderer;
        }
        
    }

    private void resetHighlight()
    {
        if (lastRenderer != null)
        {
            foreach (Material m in lastRenderer.materials)
            {
                m.shader = Shader.Find(dictLastMaterialShader[m]);
            }
        }
        lastRenderer = atRenderer;
        atRenderer = null;
        highlightedGObject = null;
    }

    void LateUpdate()
    {
        if (GetComponent<Camera>().enabled)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //print(EventSystem.current.currentSelectedGameObject == dropActions.gameObject);
                GameObject aux = EventSystem.current.currentSelectedGameObject;
                if (aux == null)
                {
                    // If Control and Alt and Middle button? ZOOM!
                    if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
                    {
                        desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
                    }
                    // If middle mouse and left alt are selected? ORBIT
                    if (Input.GetMouseButton(1))
                    {
                        xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                        yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                        ////////OrbitAngle

                        //Clamp the vertical axis for the orbit
                        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
                        // set camera rotation 
                        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
                        currentRotation = transform.rotation;

                        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
                        transform.rotation = rotation;
                    }
                    // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
                    if (Input.GetMouseButton(0))
                    {
                        //grab the rotation of the camera so we can move in a psuedo local XY space
                        objectTarget.rotation = transform.rotation;
                        objectTarget.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
                        objectTarget.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
                    }

                    desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate; //* Mathf.Abs(desiredDistance);
                    currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

                    position = objectTarget.position - (rotation * Vector3.forward * currentDistance + targetOffset);

                    transform.position = position;
                }
            }
        }
    }

   public GameObject getHighlightedGObject()
    {
        return highlightedGObject;
    }

    public Vector3 getTargetPosition()
    {
        return targetPickerPosition;
    }
   
        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
}

*/
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class GodModeControl : MonoBehaviour
{


    public float surfaceOffset = 0.1f;
    private enum TargetStates { Off, Set, Fixed };
    private TargetStates targetState;
    public Transform targetPicker;
    private Transform objectTarget;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    //private float maxDistance = Mathf.Infinity;
    //private float minDistance = -Mathf.Infinity;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 80;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    private Vector3 targetPickerPosition;
    private enum PickerMode { Object, Position };
    private PickerMode mode;
    private Ray ray;
    private RaycastHit hit;

    private Renderer atRenderer;
    private Renderer lastRenderer;
    private Dictionary<GameObject, Dictionary<Material, string>> dictAtChilds;
    private Dictionary<GameObject, Dictionary<Material, string>> dictLastChilds;
    private Dictionary<Material, string> dictAtMaterialShader;
    private Dictionary<Material, string> dictLastMaterialShader;


    private GameObject highlightedGObject;

    void Start()
    {
        if (!objectTarget)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            objectTarget = go.transform;
        }
        highlightedGObject = null;
        dictAtMaterialShader = new Dictionary<Material, string>();
        dictAtChilds = new Dictionary<GameObject, Dictionary<Material, string>>();
        dictLastMaterialShader = new Dictionary<Material, string>();
        dictLastChilds = new Dictionary<GameObject, Dictionary<Material, string>>();
        lastRenderer = null;
        atRenderer = null;

        distance = Vector3.Distance(transform.position, objectTarget.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
        targetState = TargetStates.Off;

        mode = PickerMode.Object;
        Debug.Log("RHS>>> " + this.name + " " + this.GetType() + " is ready.");
    }



    void Update()
    {
        if (GetComponent<Camera>().enabled)
        {
            if (targetState == TargetStates.Off)
            {
                targetPicker.gameObject.SetActive(false);
            }
            else
            {
                targetPicker.gameObject.SetActive(true);
            }
            if (mode == PickerMode.Object)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        //print(EventSystem.current.currentSelectedGameObject == dropActions.gameObject);
                        GameObject aux = EventSystem.current.currentSelectedGameObject;
                        if (aux == null)
                        {

                            ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                            if (Physics.Raycast(ray, out hit))
                            {
                                string itemName = hit.collider.gameObject.name;
                                GameObject gO = hit.collider.gameObject;

                                if (itemName.Contains("Collider", StringComparison.OrdinalIgnoreCase) || itemName.Contains("GameObject", StringComparison.OrdinalIgnoreCase))
                                {
                                    gO = hit.collider.transform.parent.gameObject;
                                }
                                highlightObject(gO);

                            }

                        }
                    }
                }

            }
            else
            {

                //print(EventSystem.current.currentSelectedGameObject == dropActions.gameObject);
                GameObject aux = EventSystem.current.currentSelectedGameObject;
                if (aux == null)
                {
                    if (targetState == TargetStates.Set)
                    {
                        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (!Physics.Raycast(ray, out hit))
                        {
                            return;
                        }
                        targetPicker.position = hit.point + hit.normal * surfaceOffset;
                        if (Input.GetMouseButtonDown(0))
                        {
                            targetPickerPosition = targetPicker.position;
                            targetState = TargetStates.Fixed;
                        }
                    }
                    else if (targetState == TargetStates.Fixed)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            targetState = TargetStates.Set;
                        }
                    }
                }
            }
        }
        else
        {
            resetHighlight();
        }
    }

    public void chooseOnEnvironment()
    {
        if (targetState == TargetStates.Off)
        {
            targetState = TargetStates.Set;
            mode = PickerMode.Position;
        }
        else
        {
            highlightMode();
        }
    }

    public void highlightMode()
    {
        targetState = TargetStates.Off;
        mode = PickerMode.Object;
    }

    private void highlightObject(GameObject gO)
    {
        atRenderer = gO.GetComponent<Renderer>();
        dictAtMaterialShader = new Dictionary<Material, string>();
        dictAtChilds = new Dictionary<GameObject, Dictionary<Material, string>>();
        if (atRenderer != null && lastRenderer != atRenderer)
        {
            //Highlight no gameobject principal
            foreach (Material m in atRenderer.materials)
            {
                dictAtMaterialShader.Add(m, m.shader.name);
                m.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
                //m.shader = Shader.Find("Outlined/Regular");
                highlightedGObject = gO;
            }
            //HighLight nos gameobjects filhos
            foreach (Transform child in gO.transform)
            {
                Renderer auxRender = child.gameObject.GetComponent<Renderer>();
                                    if (auxRender != null)
                    {
                        Dictionary<Material, string> auxDict = new Dictionary<Material, string>();
                        foreach (Material m in auxRender.materials)
                        {
                            auxDict.Add(m, m.shader.name);
                            m.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
                        }
                        dictAtChilds.Add(child.gameObject, auxDict);
                    }
                
            }
            if (lastRenderer == null)
            {
                lastRenderer = atRenderer;
                dictLastChilds = dictAtChilds;
                dictLastMaterialShader = dictAtMaterialShader;
            }
            targetPickerPosition = gO.transform.position;
        }
        if (lastRenderer != null && lastRenderer != atRenderer)
        {
            foreach (Material m in lastRenderer.materials)
            {
                m.shader = Shader.Find(dictLastMaterialShader[m]);
            }
            //HighLight nos gameobjects filhos
            foreach (var item in dictLastChilds)
            {
                Renderer auxRenderer = item.Key.GetComponent<Renderer>();
                if (auxRenderer != null)
                {
                    Dictionary<Material, string> auxDict = item.Value;
                    foreach (Material m in auxRenderer.materials)
                    {
                        m.shader = Shader.Find(auxDict[m]);
                    }
                }
                else
                {
                    Debug.Log("RHS>>> Error! Renderer do not found!");
                }
            }
            dictLastMaterialShader = dictAtMaterialShader;
            dictLastChilds = dictAtChilds;
            lastRenderer = atRenderer;
        }
    }

    private void printDict(Dictionary<GameObject, Dictionary<Material, string>> dict)
    {
        foreach(var item in dict)
        {
            print(item.Key);
        }
    }

    private void resetHighlight()
    {
        print("ResetHIghlight");
        if (lastRenderer != null)
        {
            foreach (Material m in lastRenderer.materials)
            {
                m.shader = Shader.Find(dictLastMaterialShader[m]);
            }
            //HighLight nos gameobjects filhos
            print("NC: " + dictLastChilds.Count());
            foreach (var gO in dictLastChilds)
            {
                print("CHilds-<");
                Renderer auxRenderer = gO.Key.GetComponent<Renderer>();
                if (auxRenderer != null)
                {
                    Dictionary<Material, string> auxDict = gO.Value;
                    print("Materials-<");
                    foreach (Material m in auxRenderer.materials)
                    {
                        m.shader = Shader.Find(auxDict[m]);
                        print("Rvertendo: " + m.shader.name);
                    }
                }
                else
                {
                    Debug.Log("RHS>>> Error! Renderer do not found!");
                }
            }
        }

        dictLastChilds = dictAtChilds;
        lastRenderer = atRenderer;
        dictAtChilds = new Dictionary<GameObject, Dictionary<Material, string>>();
        atRenderer = null;
        highlightedGObject = null;
    }


    void LateUpdate()
    {
        if (GetComponent<Camera>().enabled)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //print(EventSystem.current.currentSelectedGameObject == dropActions.gameObject);
                GameObject aux = EventSystem.current.currentSelectedGameObject;
                if (aux == null)
                {

                    // If Control and Alt and Middle button? ZOOM!
                    if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
                    {
                        desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
                    }
                    // If middle mouse and left alt are selected? ORBIT
                    if (Input.GetMouseButton(1))
                    {
                        xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                        yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                        ////////OrbitAngle

                        //Clamp the vertical axis for the orbit
                        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
                        // set camera rotation 
                        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
                        currentRotation = transform.rotation;

                        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
                        transform.rotation = rotation;
                    }
                    // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
                    if (Input.GetMouseButton(0))
                    {
                        //grab the rotation of the camera so we can move in a psuedo local XY space
                        objectTarget.rotation = transform.rotation;
                        objectTarget.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
                        objectTarget.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
                    }

                    desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate; //* Mathf.Abs(desiredDistance);
                    currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

                    position = objectTarget.position - (rotation * Vector3.forward * currentDistance + targetOffset);

                    transform.position = position;
                }
            }
        }
    }

    public GameObject getHighlightedGObject()
    {
        return highlightedGObject;
    }

    public Vector3 getTargetPosition()
    {
        return targetPickerPosition;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}

