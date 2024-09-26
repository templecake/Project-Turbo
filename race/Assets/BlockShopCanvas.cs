using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Formatting;

public class BlockShopCanvas : MonoBehaviour
{
    public Text CubitText;
    public Text AdbitText;
    public Text GolbitText;

    Player p;

    private void Start()
    {
        p = FindObjectOfType<Player>();
    }

    void Update()
    {
        CubitText.text = Format.FormatInt(p.Cubits);
        AdbitText.text = Format.FormatInt(p.AdventureCubits);
        GolbitText.text = Format.FormatInt(p.GoldenCubits);
    }
}
