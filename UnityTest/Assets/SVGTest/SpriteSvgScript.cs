using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSvgScript : BaseSvgScript
{
    void Start()
    {
        List<VectorUtils.Geometry> geometries = GetGeometries();
 
        var sprite = VectorUtils.BuildSprite(geometries, pixelsPerUnit, VectorUtils.Alignment.Center, Vector2.zero, 128, flipYAxis);
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}