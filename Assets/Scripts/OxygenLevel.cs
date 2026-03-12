using UnityEngine;
using UnityEngine.UI;

public class OxygenLevel : MonoBehaviour
{
public Slider oxygenBar;
public float maxOxygen = 100f;
private float currentOxygen;
private bool hasExploded=false;

public float oxygenDrainRate=10f;
public float oxygenRefillRate = 20f;

public float waterLevel=0f;
public GameObject explosionPrefab;

void Start()
{
    currentOxygen = maxOxygen;
    oxygenBar.maxValue = maxOxygen;
    oxygenBar.value = currentOxygen;
}

void Update()
{
    if(hasExploded) return;
    if(transform.position.y<waterLevel)
    {
        DrainOxygen();
    }
    else 
    {
        RefillOxygen();
    }
    oxygenBar.value=currentOxygen;
    if(currentOxygen<=0 && !hasExploded)
    {
        Explode();
    }
}

void DrainOxygen()
{
    currentOxygen -= oxygenDrainRate*Time.deltaTime;
    currentOxygen = Mathf.Clamp(currentOxygen,0,maxOxygen);
}

void RefillOxygen()
{
    currentOxygen += oxygenRefillRate*Time.deltaTime;
    currentOxygen = Mathf.Clamp(currentOxygen,0,maxOxygen);
}

void Explode()
{
    hasExploded=true;
    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    Destroy(gameObject);
}
}

