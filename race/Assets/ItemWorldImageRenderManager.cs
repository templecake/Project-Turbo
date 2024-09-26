using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldImageRenderManager : MonoBehaviour
{
    [SerializeField] float UpdateTime;
    float timePassed = 0;

    public RenderTexture LoadingTexture;
    public RenderTexture[] BlockItemTextures;
    public RenderTexture[] CustomisationItemTextures;
    public RenderTexture[] PaintItemTextures;
    public RenderTexture[] CrateItemTextures;

    public GameObject PaintItemObject;


    BlockStorage bs;
    CustomisationManager cM;
    CrateManager crateM;

    public Transform DisplayItemHolder;
    public Camera displayItemCamera;

    bool Finished = false;

    private void Start()
    {
        bs = GetComponent<BlockStorage>();
        cM = GetComponent<CustomisationManager>();
        crateM = GetComponent<CrateManager>();
        BlockItemTextures = new RenderTexture[bs.blocks.Count];
        for (int i = 0; i < BlockItemTextures.Length; i++)
        {
            BlockItemTextures[i] = LoadingTexture;
        }

        CustomisationItemTextures = new RenderTexture[cM.AllCustomisations.Length];
        for (int i = 0; i < CustomisationItemTextures.Length; i++)
        {
            CustomisationItemTextures[i] = LoadingTexture;
        }

        PaintItemTextures = new RenderTexture[bs.materials.Count];
        for (int i = 0; i < PaintItemTextures.Length; i++)
        {
            PaintItemTextures[i] = LoadingTexture;
        }

        CrateItemTextures = new RenderTexture[crateM.AllCrates.Length];
        for (int i = 0; i < CrateItemTextures.Length; i++)
        {
            CrateItemTextures[i] = LoadingTexture;
        }

    }

    void Update()
    {
        if (Finished) return;
        timePassed += Time.deltaTime;
        if (timePassed >= UpdateTime)
        {
            UnloadLastTexture();
            timePassed = 0;
            LoadNextTexture();
        } 
    }

    GameObject displayItem = null;
    void UnloadLastTexture()
    {
      displayItemCamera.targetTexture = null;
      if(displayItem) Destroy(displayItem);
      displayItemCamera.gameObject.SetActive(false);
    }

    int customOn = 0;
    int blockOn = 0;
    int paintOn = 0;
    int crateOn = 0;
    void LoadNextTexture()
    {
       if(customOn < CustomisationItemTextures.Length)
       {
            displayItemCamera.gameObject.SetActive(true);
            RenderTexture newTexture = new RenderTexture(1000, 1000, 0);
            displayItemCamera.targetTexture = newTexture;
            displayItem = Instantiate(cM.AllCustomisations[customOn].InWorldModel, DisplayItemHolder);
            displayItem.transform.localScale = Vector3.one;
            displayItem.transform.localPosition = Vector3.zero;
            CustomisationItemTextures[customOn] = newTexture;
            Debug.Log("Texture Loaded: " + cM.AllCustomisations[customOn].DisplayName);
            customOn++;
            return;
       }

       if(blockOn < BlockItemTextures.Length)
       {
            displayItemCamera.gameObject.SetActive(true);
            RenderTexture newTexture = new RenderTexture(1000, 1000, 0);
            displayItemCamera.targetTexture = newTexture;
            displayItem = Instantiate(bs.blocks[blockOn].buildingPrefab, DisplayItemHolder);
            displayItem.transform.localScale = Vector3.one;
            displayItem.transform.localPosition = Vector3.zero;
            BlockItemTextures[blockOn] = newTexture;
            Debug.Log("Texture Loaded: " + bs.blocks[blockOn].DisplayName);
            blockOn++;
            return;
       }

        if (paintOn < PaintItemTextures.Length)
        {
            displayItemCamera.gameObject.SetActive(true);
            RenderTexture newTexture = new RenderTexture(1000, 1000, 0);
            displayItemCamera.targetTexture = newTexture;
            displayItem = Instantiate(PaintItemObject, DisplayItemHolder);
            displayItem.transform.localScale = Vector3.one;
            displayItem.transform.localPosition = Vector3.zero;
            displayItem.GetComponent<Renderer>().material = bs.materials[paintOn].LinkedMaterial;
            PaintItemTextures[paintOn] = newTexture;
            Debug.Log("Texture Loaded: " + bs.materials[paintOn].DisplayName);
            paintOn++;
            return;
        }

        if (crateOn < CrateItemTextures.Length)
        {
            displayItemCamera.gameObject.SetActive(true);
            RenderTexture newTexture = new RenderTexture(1000, 1000, 0);
            displayItemCamera.targetTexture = newTexture;
            displayItem = Instantiate(crateM.AllCrates[crateOn].CrateObject, DisplayItemHolder);
            displayItem.transform.localScale = Vector3.one;
            displayItem.transform.localPosition = Vector3.zero;
            CrateItemTextures[crateOn] = newTexture;
            Debug.Log("Texture Loaded: " + crateM.AllCrates[crateOn].CrateDisplayName);
            crateOn++;
            return;
        }

        displayItemCamera.gameObject.SetActive(false);
        Finished = true;
    }
}
