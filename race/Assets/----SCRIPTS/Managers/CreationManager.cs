using Formatting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreationManager : MonoBehaviour
{

    [Header("General")]
    public bool Creating;
    public bool CamMoving;
    GameManager gm;

    public List<Car> cars;
    public Car tempCar;

    bool EditingPreviousKart;
    int KartEditingID;

    [SerializeField] bool _DrawGizmos;

    public int CurrentMode = 0; // 0 - placement. 1 - delete. 2 - colour

    public Material deleteMaterial;
    public Material[] Colours;
    public int CurrentMaterial;
    // Update is called once per frame
    private void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");
        blockStorage = GameObject.Find("GameManager").GetComponent<BlockStorage>();

        gm = FindObjectOfType<GameManager>();
        if (gm.StartedEditingKart)
        {
            LoadKart(gm.EditingKartID);
            gm.StartedEditingKart = false;
            gm.EditingKartID = -1;
        }
        else
        {
            StartEditing();       
        }
    }

    void Update()
    {
        if (Creating)
        {
            CameraController();
            if (gridPoints == null) { CreateGrid(); }
            if (Input.GetKey(KeyCode.Tab) && Input.GetKeyDown(KeyCode.R)) { RefreshGrid(); }

            if (CurrentMode == 0) Placement();
            if (CurrentMode == 1) Deleting();
            if (CurrentMode == 2) Painting();
            UI();
            GetStats();
        }

        if(Input.GetKeyDown(KeyCode.Escape) && CurrentMode == 3)
        {
            CleanUp();
        }
    }

   

    public void ChangeMode(int Mode)
    {
        if(Mode == 1) {

            editPanel.GetComponent<Animator>().SetBool("Shown", false);
            shown = false;

            CurrentMode = Mode; 
            return; 
        }

        if (Mode == CurrentMode) 
        { 
            SwitchEditPanel(); 
        }
        else { 
            CurrentMode = Mode; 

            if (!shown) 
                SwitchEditPanel(); 
        }
    }

    #region Grid
    [Header("Grid")]
    public int GridXSize;
    public int GridYSize;
    public int GridZSize;
    List<GameObject> gridPoints;
    [SerializeField] GameObject gridConnect;

    void RefreshGrid()
    {
        for (int i = 0; i < gridPoints.Count; i++)
        {
            Destroy(gridPoints[i]);
        }
        for (int i = 0; i < transform.Find("grid").childCount; i++)
        {
            Destroy(transform.Find("grid").GetChild(i).gameObject);
        }
        CreateGrid();
    }
    void CreateGrid()
    {

        int i = 0;
        gridPoints = new List<GameObject>();
        int xSize = GridXSize + 1;
        int zSize = GridZSize + 1;
        float gridXP = xSize % 2 == 0 ? -0.25f : -0.5f;
        float gridZP = zSize % 2 == 0 ? -0.25f : -0.5f;
        Vector3 originPoint = new Vector3(-GridXSize / 4, -3.5f, -GridZSize / 4);
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                GameObject gridTemp = new GameObject();
                float xp = (float)x;
                float zp = (float)z;
                gridTemp.transform.position = originPoint + new Vector3(xp / 2f - 0.5f, 0, zp / 2f - 0.5f);
                gridTemp.transform.SetParent(transform.Find("grid"));
                gridTemp.transform.localScale = Vector3.one;
                gridTemp.transform.name = "gridp" + i;
                gridPoints.Add(gridTemp);
                i++;
            }
        }



        for (int g = 0; g < gridPoints.Count; g++)
        {
            GameObject currentPoint = gridPoints[g];
            GameObject point1try = null;
            GameObject point2try = null;
            string p1 = "gridp" + (g + zSize);
            string p2 = "gridp" + (g + 1);
            if (transform.Find("grid").transform.Find(p1)){ point1try = transform.Find("grid").transform.Find(p1).gameObject; } // get the adjacent grid points
            if (transform.Find("grid").transform.Find(p2) && ((g + 1) % zSize !=0 )) { point2try = transform.Find("grid").transform.Find(p2).gameObject; }

            if (point1try != null)
            {
                GameObject gridConnectPiece = Instantiate(gridConnect);
                gridConnectPiece.transform.position = (gridPoints[g].transform.position + point1try.transform.position) / 2;
                gridConnectPiece.transform.SetParent(gridPoints[g].transform);
                gridConnectPiece.transform.localScale = Vector3.one;
                gridConnectPiece.transform.name = "connect1";
                gridConnectPiece.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                GameObject gp = new GameObject(); gp.name = point1try.name; gp.transform.SetParent(currentPoint.transform);
            }

            if (point2try != null)
            {
                GameObject gridConnectPiece = Instantiate(gridConnect);
                gridConnectPiece.transform.position = (gridPoints[g].transform.position + point2try.transform.position) / 2;
                gridConnectPiece.transform.SetParent(gridPoints[g].transform);
                gridConnectPiece.transform.localScale = Vector3.one;
                gridConnectPiece.transform.name = "connect2";
                GameObject gp = new GameObject(); gp.name = point2try.name; gp.transform.SetParent(currentPoint.transform);
            }

        }
    }
    #endregion

    #region Camera

    [Header("Camera")]
    Vector3 mp1;
    Vector3 mp2;

    [SerializeField] float camMoveSpeed;
    [SerializeField] float camSpinSpeed;
    [SerializeField] float camZoomScale;
    public GameObject camObject;

    void CameraController()
    {
        //move camera
        float x = 0;
        float y = 0;
        float updown = 0;
        if (Input.GetKey(KeyCode.W))
        {
            y++;
        }
        if (Input.GetKey(KeyCode.S))
        {
            y--;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x++;
        }
        if (Input.GetKey(KeyCode.A))
        {
            x--;
        }
        if (Input.GetKey(KeyCode.E))
        {
            updown++;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            updown--;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            camObject.transform.position = new Vector3(0, -2, 0);
        }

        GameObject g = new GameObject();
        g.transform.position = Camera.main.transform.position;
        g.transform.LookAt(camObject.transform.position);
        Vector3 forward = g.transform.forward;
        Vector3 right = g.transform.right;
        Destroy(g);
        Vector3 moveY = forward * y;
        Vector3 moveX = right * x;
        Vector3 moveUp = new Vector3(0, updown, 0) * camMoveSpeed * Time.deltaTime;

        camObject.transform.position += ((moveY + moveX) * Time.deltaTime * camMoveSpeed + moveUp);

        //spin camera


        if (Input.GetMouseButton(1))
        {
            if (mp1 == new Vector3(0,0,0))
            {
                mp1 = Input.mousePosition;
            }
            mp2 = Input.mousePosition;
            float xdifference = mp1.x - mp2.x;
            float ydifference = mp1.y - mp2.y;
            camObject.transform.Rotate(new Vector3(ydifference * Time.deltaTime * camSpinSpeed, -xdifference*Time.deltaTime*camSpinSpeed, 0));
            mp1 = mp2;
            Vector3 rot = new Vector3(camObject.transform.localEulerAngles.x, camObject.transform.localEulerAngles.y, 0);
            camObject.transform.rotation = Quaternion.Euler(rot);
        }

        if (Input.GetMouseButtonUp(1))
        {
            mp1 = new Vector3(0,0,0);
        }

        //zoom camera

        float scrollInput = -Input.mouseScrollDelta.y;
        camZoomScale += scrollInput * Time.deltaTime * 20;
        camZoomScale = Mathf.Clamp(camZoomScale, 0.25f, 1.5f);
        Camera.main.transform.localPosition = new Vector3(0, 5, -10) * camZoomScale;
    }
    #endregion

    #region Placement

    [Header("Placement")]


    BlockStorage blockStorage;
    public int blockCurrentlyPlacing;
    [SerializeField] float RotationX;
    [SerializeField] float RotationY;
    [SerializeField] Vector3 placingPosition = new Vector3(0.25f, 0, 0.25f);
    public List<int> BlocksPlaced;
    public List<int> BlocksRemoved;


    [SerializeField] bool CanPlace;
    BuildingBlock currentlyPlacing;
    [SerializeField] GameObject tempObj;

    [SerializeField] Material canPlaceMaterial;
    [SerializeField] Material cantPlaceMaterial;
    [SerializeField] GameObject Seat;

    void Placement()
    {
        if (CamMoving) { return; }
        CanPlace = true;
        blockStorage = GameObject.Find("GameManager").GetComponent<BlockStorage>();
        if (blockStorage == null) { Debug.LogError("No Block Storage Found"); return;}
        if(currentlyPlacing == null) { currentlyPlacing = blockStorage.blocks[blockCurrentlyPlacing]; }
        if (Input.GetKeyDown(KeyCode.Alpha1)) //select block
        {
            UpdateCurrentPlacing(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateCurrentPlacing(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateCurrentPlacing(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UpdateCurrentPlacing(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UpdateCurrentPlacing(4);
        }

        MoveBlock();
        //CheckCanPlace();
        if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(0)&&MouseOnBlock) ) { PlaceBlock(); }

       
    }

    [SerializeField] LayerMask raycastMask;
    [SerializeField] string layer;

    [SerializeField] Vector3[] placingOffsets;
    [SerializeField] Vector3[] placingOffsetsEven;
    bool Right = false;
    bool Left = false;
    bool Forward = false;
    bool Back = false;
    bool Up = false;
    bool Down = false;
    bool Tyre = false;
    Vector3 pieceRot = Vector3.zero;
    bool MouseOnBlock = false;
    void MoveBlock()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
        direction.Normalize();

        Right = false;
        Left = false;
        Forward = false;
        Back = false;
        Up = false;
        Down = false;

        MouseOnBlock = false;

        if (Physics.Raycast(ray, out hit, 100f, raycastMask))
        {
            if(tempObj) tempObj.SetActive(true);
            Vector3 setPosition = Vector3.zero;
            //find closest grid point;
            Vector3 gridPosition = hit.point;
            gridPosition.x = Mathf.Floor(gridPosition.x * 2f) / 2f;
            gridPosition.y = Mathf.Floor(gridPosition.y * 2f) / 2f;
            gridPosition.z = Mathf.Floor(gridPosition.z * 2f) / 2f;
            gridPosition += new Vector3(.25f, .25f, .25f);

            //find direction
            Vector3 difference = hit.point - gridPosition;
            float absX = Mathf.Abs(difference.x);
            float absY = Mathf.Abs(difference.y);
            float absZ = Mathf.Abs(difference.z);

           

            if (absX > absZ && absX > absY)
            {
                if(difference.x > 0) { Right = true; } else { Left = true; }
            }
            else if (absY > absX && absY > absZ)
            {
                if (difference.y > 0) { Up = true; } else { Down = true; }
            }
            else if (absZ > absX && absZ > absY)
            {
                if (difference.z > 0) { Forward = true; } else { Back = true; }
            }

            //add offsets
            bool XEven = (currentlyPlacing.SizeX*2) % 2 == 0;
            bool YEven = (currentlyPlacing.SizeY * 2) % 2 == 0;
            bool ZEven = (currentlyPlacing.SizeZ * 2) % 2 == 0;

            setPosition = gridPosition;
            if (Right) { setPosition += XEven ? placingOffsetsEven[0] : placingOffsets[0]; }
            if (Left) { setPosition += XEven ? placingOffsetsEven[1] : placingOffsets[1]; }
            if (Forward) { setPosition += ZEven ? placingOffsetsEven[2] : placingOffsets[2]; }
            if (Back) { setPosition += ZEven ? placingOffsetsEven[3] : placingOffsets[3]; }
            if (Up) { setPosition += YEven ? placingOffsetsEven[4] : placingOffsets[4]; }
            if (Down) { setPosition += YEven ? placingOffsetsEven[5] : placingOffsets[5]; }
            setPosition += currentlyPlacing.blockOffset;
            placingPosition = setPosition;

            MouseOnBlock = true;
        }
        else
        {
            if(tempObj) tempObj.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.R)) { RotationX += 90; } //rotate
        if (Input.GetKeyDown(KeyCode.T)) { RotationY += 90; } //tilt

        RotationX = RotationX == 360 ? 0 : RotationX;
        RotationY = RotationY == 360 ? 0 : RotationY;

        pieceRot = Vector3.zero;

        if (tempObj) { 
            tempObj.transform.position = Vector3.Lerp(tempObj.transform.position, placingPosition, 10 * Time.deltaTime); 

            tempObj.transform.rotation = !currentlyPlacing.CanBeRotated ? Quaternion.Euler(pieceRot)  : Quaternion.Euler(new Vector3(0, RotationX, RotationY)); 

        }

        CheckCanPlace();
    }

    void CheckCanPlace()
    {
        if (!tempObj) { return; }
        Vector3[] checks = new Vector3[6] //check adjacent
        {
            (placingPosition + new Vector3(checkXOffset, 0, 0)),
            (placingPosition - new Vector3(checkXOffset, 0, 0)),
            (placingPosition + new Vector3(0, checkYOffset, 0)),
            (placingPosition - new Vector3(0, checkYOffset, 0)),
            (placingPosition + new Vector3(0, 0, checkZOffset)),
            (placingPosition - new Vector3(0, 0, checkZOffset))
        };

        int found=0;
        for (int i = 0; i < checks.Length; i++) {
            Collider[] adjacent = Physics.OverlapSphere(checks[i], 0.125f);
            for (int ad = 0; ad < adjacent.Length; ad++)
            {
                if(adjacent[ad].transform.tag == "buildingBlock")
                {
                    found++;
                }
            }
        }

        if (found == 0) { CanPlace = false; Debug.LogError("Can't place floating object"); }

        if (!blockStorage.HasBlock(blockCurrentlyPlacing)) { CanPlace = false; }

        //check collisions
        Collider[] collisions = Physics.OverlapBox(placingPosition, transform.localScale*0.25f);
        int colsFound = 0;
        for (int i = 0; i < collisions.Length; i++)
        {
            if(collisions[i].transform.tag == "buildingBlock" && collisions[i].gameObject!=tempObj) { colsFound++; Debug.Log("Collision with " + collisions[i].name); }
        }
        if (colsFound > 0) { CanPlace = false; Debug.LogError("Can't place inside other objects"); }

        if(Tyre && Up || Tyre && Down) { CanPlace = false; }


        MeshRenderer[] renderers = tempObj.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++) {
            if (renderers[i].materials.Length > 1) renderers[i].materials = new Material[1];
            renderers[i].material = CanPlace ? canPlaceMaterial : cantPlaceMaterial;
            Debug.Log(renderers[i].materials[0].name);
            //if (renderers[i].materials[1]) { 
            //    Debug.Log(renderers[i].materials[1].name);
            //    Destroy(renderers[i].materials[1]);
            //}
        }
    }

    float checkXOffset = 0;
    float checkYOffset = 0;
    float checkZOffset = 0;
    bool drawingGizmo;

    public void UpdateCurrentPlacing(int num)
    {
        Debug.Log("Updated Block to " + num);
        blockCurrentlyPlacing = num;
        blockStorage = GameObject.Find("GameManager").GetComponent<BlockStorage>();
        currentlyPlacing = blockStorage.blocks[blockCurrentlyPlacing];
        float sx = currentlyPlacing.SizeX; float sy = currentlyPlacing.SizeY; float sz = currentlyPlacing.SizeZ;

        ChangeMode(0);

        if (tempObj!=null) { Destroy(tempObj); }
        tempObj = Instantiate(currentlyPlacing.buildingPrefab);
        tempObj.transform.localScale = new Vector3(currentlyPlacing.SizeX, currentlyPlacing.SizeY, currentlyPlacing.SizeZ);
        tempObj.name = "temporaryObject";
        tempObj.transform.SetParent(transform);

        float xP = 0;
        float yP = 0;
        float zP = 0;

        bool diffX = (currentlyPlacing.SizeX * 2 * 2 == 0) != (sx * 2 % 2 == 0);
        bool diffY = (currentlyPlacing.SizeY * 2 * 2 == 0) != (sy * 2 % 2 == 0);
        bool diffZ = (currentlyPlacing.SizeZ * 2 * 2 == 0) != (sz * 2 % 2 == 0);

        if (diffX){ xP = 0.25f;}else { xP = 0f; }
        if (diffY) { yP = 0.25f; } else { yP = 0f; }
        if (diffZ) { zP = 0.25f; } else { zP = 0f; }
        placingPosition += new Vector3(xP, yP, zP);

        checkXOffset = currentlyPlacing.SizeX; //gizmos
        checkYOffset = currentlyPlacing.SizeY;
        checkZOffset = currentlyPlacing.SizeZ;
        drawingGizmo = true;

        bool tyreBefore = Tyre;
        Tyre = currentlyPlacing.blockType == GameManager.BlockType.Tyre;
        if(tyreBefore && !Tyre) { RotationX = 0; RotationY = 0; }
    }

    void PlaceBlock()
    {
        if (!CanPlace || tempCar==null || !tempObj || !blockStorage.HasBlock(blockCurrentlyPlacing) ) { return; }

        blockStorage.SubtractBlock(blockCurrentlyPlacing, 1);
        tempCar.blocks.Add(blockCurrentlyPlacing);
        tempCar.positions.Add(placingPosition);
        tempCar.rotations.Add(!currentlyPlacing.CanBeRotated ? pieceRot : new Vector3(0, RotationX, RotationY));
        tempCar.materials.Add(CurrentMaterial);
        BlocksPlaced.Add(blockCurrentlyPlacing);
        if (BlocksRemoved.Contains(blockCurrentlyPlacing)) BlocksRemoved.Remove(blockCurrentlyPlacing);

        GameObject.Find("GameManager").GetComponent<GameManager>().PlaySoundEffect(currentlyPlacing.placementSound);
        RefreshBlocks();
        GameObject.Find("CreationCanvas").transform.Find("main").Find("inventory").GetComponent<CreationInventory>().RefreshButtons();
    }

    void RefreshBlocks()
    {
        for (int i = 0; i < transform.Find("placedBlocks").childCount; i++)
        {
            Destroy(transform.Find("placedBlocks").GetChild(i).gameObject);
        }

        for (int i = 0; i < tempCar.blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[tempCar.blocks[i]];
            GameObject createdBL = Instantiate(bl.buildingPrefab);

            createdBL.transform.position = tempCar.positions[i];
            createdBL.transform.rotation = Quaternion.Euler(tempCar.rotations[i]);
            createdBL.transform.localScale = new Vector3(bl.SizeX, bl.SizeY, bl.SizeZ);
            createdBL.transform.SetParent(transform.Find("placedBlocks"));
            if (bl.canBeColoured)
            {
                MeshRenderer[] meshes = createdBL.GetComponentsInChildren<MeshRenderer>();
                for (int mesh = 0; mesh < meshes.Length; mesh++){ if(meshes[mesh].transform.parent.name == "paintable") meshes[mesh].material = blockStorage.materials[tempCar.materials[i]].LinkedMaterial; }
            }
            if (bl.blockType == GameManager.BlockType.Tyre)
            {
                createdBL.GetComponent<TyreConnectionCheck>().CheckConnections();
            }
            createdBL.transform.name = i.ToString();
            createdBL.layer = LayerMask.NameToLayer(layer);
        }

        MeshRenderer[] seatPaintables = Seat.transform.Find("paintable").GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < seatPaintables.Length; i++)
        {
            seatPaintables[i].material = blockStorage.materials[tempCar.seatColour].LinkedMaterial;
        }

        NameInput.text = tempCar.KartName;
    }

    private void OnDrawGizmos()
    {
        if (!_DrawGizmos) { return; }

        if (drawingGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(tempObj.transform.position + new Vector3(checkXOffset, 0, 0), .25f);
            Gizmos.DrawSphere(tempObj.transform.position + new Vector3(-checkXOffset, 0, 0), .25f);

            Gizmos.DrawSphere(tempObj.transform.position + new Vector3(0, checkYOffset, 0), .25f);
            Gizmos.DrawSphere(tempObj.transform.position + new Vector3(0, -checkYOffset, 0), .25f);

            Gizmos.DrawSphere(tempObj.transform.position + new Vector3(0, 0, checkZOffset), .25f);
            Gizmos.DrawSphere(tempObj.transform.position + new Vector3(0, 0, -checkZOffset), .25f);

            Gizmos.DrawCube(tempObj.transform.position, tempObj.transform.localScale*0.9f);
        }

    }

    public void UpdateSeat(int Seat)
    {
        if (GameObject.Find("GameManager").GetComponent<BlockStorage>().seatUnlocked[Seat])
        {
            tempCar.SeatSelected = Seat;
        }
    }
    #endregion

    #region Deleting

    int currentlyDeleting;
    bool currentlyHovering;
    void Deleting()
    {
        if(tempObj) tempObj.SetActive(false);
        transform.Find("deleteObject").GetComponent<deleteBox>().UpdateMaterial(deleteMaterial);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
        direction.Normalize();
        currentlyHovering = false;
        if (Physics.Raycast(ray, out hit, 100f, raycastMask))
        {
            if (hit.transform.name != "seat")
            {
                currentlyHovering = true;
                currentlyDeleting = int.Parse(hit.transform.name);
                transform.Find("deleteObject").transform.position = hit.transform.position;

                transform.Find("deleteObject").GetComponent<deleteBox>().sizeX = hit.transform.localScale.x;
                transform.Find("deleteObject").GetComponent<deleteBox>().sizeY = hit.transform.localScale.y;
                transform.Find("deleteObject").GetComponent<deleteBox>().sizeZ = hit.transform.localScale.z;
                if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement()))
                {
                    Delete(currentlyDeleting);
                }
            }
        }

        transform.Find("deleteObject").gameObject.SetActive(currentlyHovering);
    }

    void Delete(int block)
    {
        int thisBlock = tempCar.blocks[block];
        blockStorage.AddBlock(thisBlock, 1);
        tempCar.blocks.RemoveAt(block);
        tempCar.positions.RemoveAt(block);
        tempCar.rotations.RemoveAt(block);
        tempCar.materials.RemoveAt(block);
        BlocksPlaced.Remove(thisBlock);
        BlocksRemoved.Add(thisBlock);
        GameObject.Find("GameManager").GetComponent<GameManager>().PlaySoundEffect(0);
        RefreshBlocks();
        GameObject.Find("CreationCanvas").transform.Find("main").Find("inventory").GetComponent<CreationInventory>().RefreshButtons();
    }

    #endregion

    #region Painting

    int currentlyPainting;
    void Painting()
    {
        if(tempObj) tempObj.SetActive(false);
        transform.Find("deleteObject").GetComponent<deleteBox>().UpdateMaterial(Colours[CurrentMaterial]);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
        direction.Normalize();
        currentlyHovering = false;

        if (Physics.Raycast(ray, out hit, 100f, raycastMask))
        {
            if (hit.transform.name != "seat")
            {
                currentlyHovering = true;
                currentlyDeleting = int.Parse(hit.transform.name);

                transform.Find("deleteObject").transform.position = hit.transform.position;
                transform.Find("deleteObject").GetComponent<deleteBox>().sizeX = hit.transform.localScale.x;
                transform.Find("deleteObject").GetComponent<deleteBox>().sizeY = hit.transform.localScale.y;
                transform.Find("deleteObject").GetComponent<deleteBox>().sizeZ = hit.transform.localScale.z;

                if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement()))
                {
                    Paint(currentlyDeleting);
                }
            }
            else
            {
                currentlyHovering = true;
                transform.Find("deleteObject").transform.position = hit.transform.position;
                transform.Find("deleteObject").GetComponent<deleteBox>().sizeX = hit.transform.localScale.x;
                transform.Find("deleteObject").GetComponent<deleteBox>().sizeY = hit.transform.localScale.y;
                transform.Find("deleteObject").GetComponent<deleteBox>().sizeZ = hit.transform.localScale.z;

                if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement()))
                {
                    PaintSeat();
                }
            }
        }

        transform.Find("deleteObject").gameObject.SetActive(currentlyHovering);
    }

    void Paint(int block)
    {
        tempCar.materials[block] = CurrentMaterial;
        GameObject.Find("GameManager").GetComponent<GameManager>().PlaySoundEffect(1);
        RefreshBlocks();
        GameObject.Find("CreationCanvas").transform.Find("main").Find("inventory").GetComponent<CreationInventory>().RefreshButtons();
    }

    void PaintSeat()
    {
        tempCar.seatColour = CurrentMaterial;
        GameObject.Find("GameManager").GetComponent<GameManager>().PlaySoundEffect(1);
        RefreshBlocks();
        GameObject.Find("CreationCanvas").transform.Find("main").Find("inventory").GetComponent<CreationInventory>().RefreshButtons();
    }

    public void ChangeCurrentPaint(int id)
    {
        CurrentMaterial = id;
    }

    #endregion

    #region KartStats
    [Header("Kart Stats")]
    public float Weight;
    public float MaxSpeed;
    public float Acceleration;
    public float BoostGain;
    public float BoostCapacity;
    public float BoostUsage;
    public float BoostAcceleration;
    public float BoostMaxSpeed;
    public float SteerSpeed;

    float WeightStatValue;
    float AccelStatValue;
    float SpeedStatValue;
    float BoostStatValue;
    float SteerStatValue;

    void GetStats()
    {
        tempCar.ReturnStats(blockStorage, out Weight, out MaxSpeed, out Acceleration, out BoostGain, out BoostCapacity, out BoostAcceleration, out BoostUsage, out BoostMaxSpeed, out SteerSpeed);

        tempCar.ReturnVisualStats(blockStorage, out WeightStatValue, out SpeedStatValue, out AccelStatValue, out BoostStatValue, out SteerStatValue);
    }

    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] GameObject blocksButton;
    [SerializeField] GameObject paintButton;
    [SerializeField] GameObject destroyButton;
    [SerializeField] GameObject switchButton;

    [SerializeField] GameObject blocksPanel;
    [SerializeField] GameObject paintPanel;
    [SerializeField] GameObject testPanel;
    public bool shown=true;
    [SerializeField] GameObject editPanel;
    [SerializeField] GameObject mainmenuPanel;

    [SerializeField] Color unselectedColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Color removeSelectedColor;
    [SerializeField] Color paintSelectColor;


    [SerializeField] Slider WeightStatSlider;
    [SerializeField] Slider MaxSpeedStatSlider;
    [SerializeField] Slider AccelerationStatSlider;
    [SerializeField] Slider BoostStatSlider;
    [SerializeField] Slider SteeringStatSlider;

    [SerializeField] InputField NameInput;

    public bool ShowInformation;
    public List<int> BlocksToDisplay = new List<int>();
    public Text MouseBlockShowName;
    public GameObject MouseBlockStats;
    public GameObject[] MouseShowStatsSliders;

    void UI()
    {
        #region Buttons
        if (CurrentMode == 0) //build
        {
            editPanel.SetActive(true);
            blocksButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = selectedColor;
            blocksButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = selectedColor;
            paintButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = unselectedColor;
            paintButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = unselectedColor;
            destroyButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = unselectedColor;
            destroyButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = unselectedColor;
            //blocksButton.GetComponent<Image>().color = selectedColor;
            //paintButton.GetComponent<Image>().color = unselectedColor;
            //destroyButton.GetComponent<Image>().color = unselectedColor;

            blocksPanel.SetActive(true);
            paintPanel.SetActive(false);
        }

        if (CurrentMode == 1)        //delete
        {
            editPanel.SetActive(true);
            blocksButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = unselectedColor;
            blocksButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = unselectedColor;
            paintButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = unselectedColor;
            paintButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = unselectedColor;
            destroyButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = removeSelectedColor;
            destroyButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = removeSelectedColor;
            // blocksButton.GetComponent<Image>().color = unselectedColor;
            // paintButton.GetComponent<Image>().color = unselectedColor;
            //destroyButton.GetComponent<Image>().color = selectedColor;

            blocksPanel.SetActive(true);
            paintPanel.SetActive(false);
        }

        if (CurrentMode == 2)          //paint
        {
            editPanel.SetActive(true);
            blocksButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = unselectedColor;
            blocksButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = unselectedColor;
            paintButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = paintSelectColor;
            paintButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = paintSelectColor;
            destroyButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = unselectedColor;
            destroyButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = unselectedColor;
            // blocksButton.GetComponent<Image>().color = unselectedColor;
            // paintButton.GetComponent<Image>().color = selectedColor;
            // destroyButton.GetComponent<Image>().color = unselectedColor;

            blocksPanel.SetActive(false);
            paintPanel.SetActive(true);
        }

        if (CurrentMode == 3)
        {
            blocksButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = unselectedColor;
            blocksButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = unselectedColor;
            paintButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = unselectedColor;
            paintButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = unselectedColor;
            destroyButton.transform.Find("mask").Find("background1").GetComponent<Image>().color = unselectedColor;
            destroyButton.transform.Find("mask").Find("background2").GetComponent<Image>().color = unselectedColor;
            editPanel.SetActive(false);
        }
        #endregion

        #region Stats
        WeightStatSlider.value = Mathf.Lerp(WeightStatSlider.value, WeightStatValue, 2*Time.deltaTime);
        MaxSpeedStatSlider.value = Mathf.Lerp(MaxSpeedStatSlider.value, SpeedStatValue, 2*Time.deltaTime);
        AccelerationStatSlider.value = Mathf.Lerp(AccelerationStatSlider.value, AccelStatValue, 2*Time.deltaTime);
        BoostStatSlider.value = Mathf.Lerp(BoostStatSlider.value, BoostStatValue, 2*Time.deltaTime);
        SteeringStatSlider.value = Mathf.Lerp(SteeringStatSlider.value, SteerStatValue, 2*Time.deltaTime);

        tempCar.KartName = NameInput.text;
        #endregion

        if(BlocksToDisplay.Count > 0)
        {
            MouseBlockShowName.gameObject.SetActive(true);

            BuildingBlock block = blockStorage.blocks[BlocksToDisplay[0]];
            MouseBlockShowName.text = block.DisplayName;
            MouseBlockShowName.color = FindObjectOfType<CustomisationManager>().GetColorFromRarity(block.Rarity);

            if (ShowInformation)
            {
                MouseBlockStats.SetActive(true);
                
            }
            else
            {
                MouseBlockStats.SetActive(false);
            }
        }
        else
        {
            MouseBlockShowName.gameObject.SetActive(false);
            MouseBlockStats.gameObject.SetActive(false);
        }
    }

    public void OpenTestPanel()
    {
        testPanel.SetActive(true);
    }

    public void CloseTestPanel()
    {
        testPanel.SetActive(false);
    }

    public void OpenMainMenuPanel()
    {
        mainmenuPanel.SetActive(true);
    }

    public void CloseMainMenuPanel()
    {
        mainmenuPanel.SetActive(false);
    }

    public void SwitchEditPanel()
    {
        shown = !shown;
        editPanel.GetComponent<Animator>().SetBool("Shown", shown);
    }

    #endregion

    #region Test Kart
    [Header("Kart Testing")]

    [SerializeField] GameObject KartPrefab;
    GameObject createdKart;
    public void StartTesting()
    {
        transform.Find("placedBlocks").gameObject.SetActive(false);
        transform.Find("seat").gameObject.SetActive(false);
        createdKart = Instantiate(KartPrefab);
        createdKart.transform.localScale = Vector3.one;
        createdKart.GetComponent<KartController>().thisKart = tempCar;
        createdKart.GetComponent<KartController>().BuildKart();
        createdKart.GetComponent<CameraFollow>().cam = Camera.main;
        createdKart.transform.position = Vector3.zero;
        transform.Find("deleteObject").gameObject.SetActive(false);
        CloseTestPanel();
        Creating = false;
        ChangeMode(3);
        UI();
        tempObj.SetActive(false);
    }

    public void CleanUp()
    {
        transform.Find("placedBlocks").gameObject.SetActive(true);
        transform.Find("seat").gameObject.SetActive(true);
        Camera.main.transform.localEulerAngles = new Vector3(20, 0, 0);
        Camera.main.transform.localPosition = new Vector3(0, 5, -10);
        Destroy(createdKart);
        Creating = true;
        ChangeMode(0);
        tempObj.SetActive(true);
    }


    #endregion

    #region Saving and Loading


    void StartEditing()
    {
        tempCar = new Car();
        tempCar.blocks = new List<int>();
        tempCar.materials = new List<int>();
        tempCar.positions = new List<Vector3>();
        tempCar.rotations = new List<Vector3>();
        tempCar.seatColour = 5; //default colour to red. 
    }
    public void SaveKart()
    {
        if (EditingPreviousKart == true)
        {
            FindObjectOfType<Player>().playersKart[KartEditingID] = tempCar;
        }
        else
        {
            GameObject.Find("GameManager").GetComponent<Player>().SaveKart(tempCar);
        }

        GameObject.Find("GameManager").GetComponent<GameManager>().LoadScene("MAINMENU");
    }

    public void ExitWithoutSaving()
    {
        for (int i = 0; i < BlocksPlaced.Count; i++)
        {
            blockStorage.amountInInventory[BlocksPlaced[i]]++;
        }

        for (int i = 0; i < BlocksRemoved.Count; i++)
        {
            blockStorage.amountInInventory[BlocksRemoved[i]]--;
        }

        GameObject.Find("GameManager").GetComponent<GameManager>().LoadScene("MAINMENU");
    }

    public void LoadKart(int kart)
    {
        Car c = GameObject.Find("GameManager").GetComponent<Player>().playersKart[kart];

        tempCar = Format.CopyCar(c);

        KartEditingID = kart;
        EditingPreviousKart = true;
        RefreshBlocks();
    }
    #endregion

    #region Mouse Check

    int UILayer;
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    #endregion
}
