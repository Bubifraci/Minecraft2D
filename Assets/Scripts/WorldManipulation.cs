using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManipulation : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] ParticleSystem destroyPar;

    private GameObject highlightedObject;
    private Coroutine particleCourotine;

    void Update()
    {

            Vector3 mousePosition = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit)
            {
                if (hit.collider.tag == "DestroyableBlock")
                {
                    if (highlightedObject != null && highlightedObject != hit.collider.gameObject)
                    {
                        highlightedObject.GetComponent<SpriteRenderer>().color = Color.white;
                    }

                    highlightedObject = hit.collider.gameObject;
                    highlightedObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
                    if (Input.GetMouseButtonDown(0))
                    {
                        particleCourotine = StartCoroutine(PlayParticle(highlightedObject));
                    }
                }
            }
  
    }

    IEnumerator PlayParticle(GameObject target)
    {
        ParticleSystem particle = Instantiate(destroyPar, target.transform);
        particle.transform.localPosition = new Vector3(0, 0, -1f);
        var main = particle.main;
        main.startColor = getParticleColorBlock(target);
        float startTime = Time.time;

        bool isOnObject = true;
        while (Time.time - startTime < 2f && Input.GetMouseButton(0) && isOnObject)
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider.gameObject != target)
            {
                isOnObject = false;
            }
            yield return null;
        }
        Object.Destroy(particle);
        if(Time.time - startTime >= 2f)
        {
            Object.Destroy(target);
        }
    }

    Color getParticleColorBlock(GameObject target)
    {
        Color color;
        if (target.name.Contains("Gras"))
        {
            color = new Color(0, 137, 0, 0.8f);
        }
        else if (target.name.Contains("Dirt"))
        {
            color = new Color(50, 30, 0, 0.8f);
        }
        else if (target.name.Contains("Stone"))
        {
            color = new Color(111, 105, 100, 0.8f);
        }
        else
        {
            color = Color.white;
        }
        return color;
    }
}