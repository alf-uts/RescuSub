using UnityEngine;
using UnityEngine.UI;

public class OxygenLevel : MonoBehaviour
{
public Slider oxygenBar;
public float maxOxygen = 100f;
private float currentOxygen;
private bool hasExploded=false;

public Image oxygenFill;
public Color normalColor=Color.cyan;
public Color warningColor=Color.red;
public float warningThreshold=0.3f;
public float flashSpeed=5f;

public float oxygenDrainRate=10f;
public float oxygenRefillRate = 20f;

public float waterLevel=0f;
public GameObject explosionPrefab;

public void ResetOxygen()
{
    currentOxygen = maxOxygen;
    oxygenBar.value=currentOxygen;
}

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
    float oxygenPercent = currentOxygen/maxOxygen;
    if(oxygenPercent <= warningThreshold)
    {
        float t = Mathf.PingPong(Time.time*flashSpeed,1);
        oxygenFill.color=Color.Lerp(normalColor,warningColor,t);
    }
    else
    {
        oxygenFill.color=normalColor;
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
public void AddOxygen(float amount)
{
    currentOxygen += amount;
    currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
    Debug.Log($"增加氧气: {amount}, 当前氧气: {currentOxygen}");
}
void Explode()
{
    if(hasExploded) return;
    hasExploded=true;
    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    gameObject.SetActive(false);
    GameManager.Instance.LoseLife();
    Destroy(gameObject, 2f);
}
}

