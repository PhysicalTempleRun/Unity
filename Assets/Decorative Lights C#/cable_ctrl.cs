using System.Collections;
using UnityEngine;

public class cable_ctrl : MonoBehaviour
{
    private MeshRenderer[] meshRenderers;
    private float lightIntensity = 40f;
    private float speed = 0.15f;
    private Color[] colors = {Color.red, Color.green, Color.magenta, Color.yellow, Color.cyan, Color.white};

    IEnumerator Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        while (this)
        {
            yield return new WaitForSeconds(Random.Range(0, 1f));
            for (int c = 0; c < colors.Length; c++)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    Material mat = meshRenderers[i].materials[1];
                    Material matBehind = meshRenderers[Mathf.Clamp(i-1, 0, meshRenderers.Length)].materials[1];
                    mat.SetColor("_EmissiveColor", colors[c] * lightIntensity);
                    matBehind.SetColor("_EmissiveColor", colors[c] * 0);
                    yield return new WaitForSeconds(speed);
                }
            }
        }
    }

}
