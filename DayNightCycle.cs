using UnityEngine;

public class DayNightCycleWithSkyboxes : MonoBehaviour
{
    [Header("Lights")]
    public Light sun; // Directional light for the sun
    public Light moon; // Directional light for the moon
    public Light[] morningLights; // Lights to turn on during the morning
    public Light[] nightLights; // Lights to turn on during the night

    [Header("Cycle Settings")]
    public float dayLength = 120f; // Full day cycle in seconds (24 hours)
    private float timeOfDay = 0f; // Current time of day (0 to 24 hours)
    private bool isNight = true; // Start with night

    [Header("Lighting")]
    public Gradient sunColor; // Sunlight color gradient
    public Gradient moonColor; // Moonlight color gradient
    public AnimationCurve sunIntensityCurve; // Sunlight intensity curve
    public AnimationCurve ambientLightIntensityCurve; // Ambient light intensity curve
    public Gradient ambientColor; // Ambient light gradient for day and night

    [Header("Skyboxes")]
    public Material daySkybox; // Skybox for the day
    public Material nightSkybox; // Skybox for the night

    [Header("Stars")]
    public GameObject starField; // Star field (particle system or GameObject)

    void Start()
    {
        if (sun == null || moon == null || daySkybox == null || nightSkybox == null || starField == null)
        {
            Debug.LogError("DayNightCycleWithSkyboxes: Missing references in the inspector. Script disabled.");
            enabled = false;
            return;
        }

        InitializeCycle();
    }

    void Update()
    {
        timeOfDay += (Time.deltaTime / dayLength) * 24f; // Progress timeOfDay (0 to 24 hours)
        if (timeOfDay >= 24f) timeOfDay -= 24f; // Wrap around for continuous cycle

        UpdateLighting();
        HandleTransitions();
    }

    void UpdateLighting()
    {
        float normalizedTime = timeOfDay / 24f; // Normalize timeOfDay to 0–1
        sun.transform.rotation = Quaternion.Euler((normalizedTime * 360f) - 90f, 170f, 0f);
        sun.color = sunColor.Evaluate(normalizedTime);
        sun.intensity = sunIntensityCurve.Evaluate(normalizedTime);

        moon.transform.rotation = Quaternion.Euler((normalizedTime * 360f) + 90f, 170f, 0f);
        moon.color = moonColor.Evaluate(normalizedTime);
        moon.intensity = sunIntensityCurve.Evaluate(1f - normalizedTime);

        float ambientIntensity = ambientLightIntensityCurve.Evaluate(normalizedTime);
        RenderSettings.ambientLight = ambientColor.Evaluate(normalizedTime) * ambientIntensity;
    }

    void HandleTransitions()
    {
        if (timeOfDay >= 7f && timeOfDay < 17f && isNight)
        {
            StartDay();
        }
        else if ((timeOfDay >= 17f || timeOfDay < 7f) && !isNight)
        {
            StartNight();
        }
    }

    void StartDay()
    {
        isNight = false;
        RenderSettings.skybox = daySkybox;
        DynamicGI.UpdateEnvironment();

        moon.gameObject.SetActive(false);
        starField.SetActive(false);
        SetNightLights(false);
        SetMorningLights(true);
    }

    void StartNight()
    {
        isNight = true;
        RenderSettings.skybox = nightSkybox;
        DynamicGI.UpdateEnvironment();

        moon.gameObject.SetActive(true);
        starField.SetActive(true);
        SetMorningLights(false);
        SetNightLights(true);
    }

    void SetMorningLights(bool state)
    {
        foreach (Light light in morningLights)
        {
            if (light != null)
                light.enabled = state;
        }
    }

    void SetNightLights(bool state)
    {
        foreach (Light light in nightLights)
        {
            if (light != null)
                light.enabled = state;
        }
    }

    void InitializeCycle()
    {
        if (timeOfDay >= 7f && timeOfDay < 17f)
        {
            StartDay();
        }
        else
        {
            StartNight();
        }
    }
}
