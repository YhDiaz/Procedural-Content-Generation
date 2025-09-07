using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ParametersPanel : MonoBehaviour
{
    [SerializeField] private BinarySpacePartitioningSettings bspSettings;
    [SerializeField] private PerlinNoiseManager perlinNoiseManager;
    [SerializeField] private B_L_System lSystem;

    [Header("BSP Elements")]
    [SerializeField] private Toggle bspViewPartitionsToggle;
    [SerializeField] private Toggle bspAlternatePartitionsToggle;
    [SerializeField] private Slider bspMaximumDepthSlider;
    [SerializeField] private TextMeshProUGUI bspMaximumDepthSliderText;
    [SerializeField] private Slider bspPartitionCenterBiasSlider;
    [SerializeField] private TextMeshProUGUI bspPartitionCenterBiasSliderText;
    [SerializeField] private Slider bspRoomCoverageMinSlider;
    [SerializeField] private TextMeshProUGUI bspRoomCoverageMinSliderText;
    [SerializeField] private Slider bspRoomCoverageMaxSlider;
    [SerializeField] private TextMeshProUGUI bspRoomCoverageMaxSliderText;
    [SerializeField] private TMP_InputField bspRootTopLeftCornerX;
    [SerializeField] private TMP_InputField bspRootTopLeftCornerY;
    [SerializeField] private TMP_InputField bspRootBottomRightCornerX;
    [SerializeField] private TMP_InputField bspRootBottomRightCornerY;

    [Header("Perlin Noise Elements")]
    [SerializeField] private TMP_InputField perlinNoiseSeed;
    [SerializeField] private Slider perlinNoiseScaleSlider;
    [SerializeField] private TextMeshProUGUI perlinNoiseScaleSliderText;
    [SerializeField] private Slider perlinNoiseOctavesSlider;
    [SerializeField] private TextMeshProUGUI perlinNoiseOctavesSliderText;
    [SerializeField] private Slider perlinNoisePersistenceSlider;
    [SerializeField] private TextMeshProUGUI perlinNoisePersistenceSliderText;
    [SerializeField] private Slider perlinNoiseLacunaritySlider;
    [SerializeField] private TextMeshProUGUI perlinNoiseLacunaritySliderText;
    [SerializeField] private Slider perlinNoiseBrightnessSlider;
    [SerializeField] private TextMeshProUGUI perlinNoiseBrightnessSliderText;
    [SerializeField] private TMP_InputField perlinNoiseOffsetX;
    [SerializeField] private TMP_InputField perlinNoiseOffsetY;

    [Header("L-System Elements")]
    [SerializeField] private Slider lSystemIterationsSlider;
    [SerializeField] private TextMeshProUGUI lSystemIterationsSliderText;
    [SerializeField] private Slider lSystemRotationAngleSlider;
    [SerializeField] private TextMeshProUGUI lSystemRotationAngleSliderText;
    [SerializeField] private Slider lSystemLineLengthSlider;
    [SerializeField] private TextMeshProUGUI lSystemLineLengthSliderText;
    [SerializeField] private TMP_InputField lSystemAxiomInputField;
    [SerializeField] private TMP_InputField lSystemLHSInputField;
    [SerializeField] private TMP_InputField lSystemRHSInputField;

    private bool firstDeploy = true;

    public void OnInitializeBSPElements()
    {
        bspViewPartitionsToggle.isOn = bspSettings.viewPartitions;
        bspAlternatePartitionsToggle.isOn = bspSettings.alternatePartitions;
        bspMaximumDepthSlider.value = bspSettings.maximumDepth;
        bspPartitionCenterBiasSlider.value = bspSettings.partitionCenterBias;
        bspRoomCoverageMinSlider.value = bspSettings.roomCoverageMin;
        bspRoomCoverageMaxSlider.value = bspSettings.roomCoverageMax;
        UpdateMaximumDepthSliderText();
        UpdatePartitionCenterBiasSliderText();
        UpdateRoomCoverageMinSliderText();
        UpdateRoomCoverageMaxSliderText();
        Vector2 topLeft = bspSettings.rootTopLeftCorner;
        Vector2 bottomRight = bspSettings.rootBottomRightCorner;
        bspRootTopLeftCornerX.text = topLeft.x.ToString();
        bspRootTopLeftCornerY.text = topLeft.y.ToString();
        bspRootBottomRightCornerX.text = bottomRight.x.ToString();
        bspRootBottomRightCornerY.text = bottomRight.y.ToString();
    }

    public void OnInitializePerlinNoiseElements()
    {
        perlinNoiseSeed.text = perlinNoiseManager.seed.ToString();
        perlinNoiseScaleSlider.value = perlinNoiseManager.scale;
        perlinNoiseOctavesSlider.value = perlinNoiseManager.octaves;
        perlinNoisePersistenceSlider.value = perlinNoiseManager.persistence;
        perlinNoiseLacunaritySlider.value = perlinNoiseManager.lacunarity;
        perlinNoiseBrightnessSlider.value = perlinNoiseManager.brightness;
        UpdatePerlinNoiseScaleSliderText();
        UpdatePerlinNoiseOctavesSliderText();
        UpdatePerlinNoisePersistenceSliderText();
        UpdatePerlinNoiseLacunaritySliderText();
        UpdatePerlinNoiseBrightnessSliderText();
        Vector2 offset = perlinNoiseManager.offset;
        perlinNoiseOffsetX.text = offset.x.ToString();
        perlinNoiseOffsetY.text = offset.y.ToString();
    }

    public void OnInitializeLSystem()
    {
        lSystemIterationsSlider.value = lSystem.iterations;
        lSystemRotationAngleSlider.value = lSystem.rotAngle;
        lSystemLineLengthSlider.value = lSystem.lineLength;
        UpdateLSystemIterationsSliderText();
        UpdateLSystemRotationAngleSliderText();
        UpdateLSystemLineLengthSliderText();
        lSystemAxiomInputField.text = lSystem.axiom;
        lSystemLHSInputField.text = lSystem.LHS[0].ToString();
        lSystemRHSInputField.text = lSystem.RHS[0];
    }

    private void OnEnable()
    {
        if (!firstDeploy)
            return;

        OnInitializeBSPElements();
    }

    public void OnViewPartitionsToggleChanged()
        => bspSettings.viewPartitions = bspViewPartitionsToggle.isOn;

    public void OnAlternatePartitionsToggleChanged()
        => bspSettings.alternatePartitions = bspAlternatePartitionsToggle.isOn;

    public void OnMaximumDepthSliderChanged()
    {
        bspSettings.maximumDepth = (int)bspMaximumDepthSlider.value;
        UpdateMaximumDepthSliderText();
    }

    public void OnPartitionCenterBiasSliderChanged()
    {
        bspSettings.partitionCenterBias = bspPartitionCenterBiasSlider.value;
        UpdatePartitionCenterBiasSliderText();
    }

    public void OnRoomCoverageMinSliderChanged()
    {
        bspSettings.roomCoverageMin = bspRoomCoverageMinSlider.value;
        UpdateRoomCoverageMinSliderText();
    }

    public void OnRoomCoverageMaxSliderChanged()
    {
        bspSettings.roomCoverageMax = bspRoomCoverageMaxSlider.value;
        UpdateRoomCoverageMaxSliderText();
    }

    public void OnRootTopLeftCornerXChanged(string value)
    {
        if (float.TryParse(value, out float x))
            bspSettings.rootTopLeftCorner.x = x;
    }

    public void OnRootTopLeftCornerYChanged(string value)
    {
        if (float.TryParse(value, out float y))
            bspSettings.rootTopLeftCorner.y = y;
    }

    public void OnRootBottomRightCornerXChanged(string value)
    {
        if (float.TryParse(value, out float x))
            bspSettings.rootBottomRightCorner.x = x;
    }

    public void OnRootBottomRightCornerYChanged(string value)
    {
        if (float.TryParse(value, out float y))
            bspSettings.rootBottomRightCorner.y = y;
    }

    public void OnPerlinNoiseSeedChanged(string value)
    {
        if (int.TryParse(value, out int seed))
            perlinNoiseManager.seed = seed;
    }

    public void OnPerlinNoiseScaleSliderChanged()
    {
        perlinNoiseManager.scale = perlinNoiseScaleSlider.value;
        UpdatePerlinNoiseScaleSliderText();
    }

    public void OnPerlinNoiseOctavesSliderChanged()
    {
        perlinNoiseManager.octaves = (int)perlinNoiseOctavesSlider.value;
        UpdatePerlinNoiseOctavesSliderText();
    }

    public void OnPerlinNoisePersistenceSliderChanged()
    {
        perlinNoiseManager.persistence = perlinNoisePersistenceSlider.value;
        UpdatePerlinNoisePersistenceSliderText();
    }

    public void OnPerlinNoiseLacunaritySliderChanged()
    {
        perlinNoiseManager.lacunarity = perlinNoiseLacunaritySlider.value;
        UpdatePerlinNoiseLacunaritySliderText();
    }

    public void OnPerlinNoiseBrightnessSliderChanged()
    {
        perlinNoiseManager.brightness = perlinNoiseBrightnessSlider.value;
        UpdatePerlinNoiseBrightnessSliderText();
    }

    public void OnPerlinNoiseOffsetXChanged(string value)
    {
        if (float.TryParse(value, out float x))
            perlinNoiseManager.offset.x = x;
    }

    public void OnPerlinNoiseOffsetYChanged(string value)
    {
        if (float.TryParse(value, out float y))
            perlinNoiseManager.offset.y = y;
    }

    public void OnLSystemIterationsSliderChanged()
    {
        lSystem.iterations = (int)lSystemIterationsSlider.value;
        UpdateLSystemIterationsSliderText();
    }

    public void OnLSystemRotationAngleSliderChanged()
    {
        lSystem.rotAngle = lSystemRotationAngleSlider.value;
        UpdateLSystemRotationAngleSliderText();
    }

    public void OnLSystemLineLengthSliderChanged()
    {
        lSystem.lineLength = lSystemLineLengthSlider.value;
        UpdateLSystemLineLengthSliderText();
    }

    public void OnLSystemAxiomInputFieldChanged(string value)
        => lSystem.axiom = value;

    public void OnLSystemLHSInputFieldChanged(string value)
    {
        if (value.Length == 1)
            lSystem.LHS[0] = value[0];
    }

    public void OnLSystemRHSInputFieldChanged(string value)
        => lSystem.RHS[0] = value;

    private void UpdateTextDependingOn<T>(TextMeshProUGUI text, float value, int decimals = 2)
        => text.text = typeof(T) == typeof(int) ? ((int)value).ToString() : value.ToString($"F{decimals}");

    private void UpdateMaximumDepthSliderText()
        => UpdateTextDependingOn<int>(bspMaximumDepthSliderText, bspMaximumDepthSlider.GetComponent<Slider>().value);

    private void UpdatePartitionCenterBiasSliderText()
        => UpdateTextDependingOn<float>(bspPartitionCenterBiasSliderText, bspPartitionCenterBiasSlider.GetComponent<Slider>().value);

    private void UpdateRoomCoverageMinSliderText()
        => UpdateTextDependingOn<float>(bspRoomCoverageMinSliderText, bspRoomCoverageMinSlider.GetComponent<Slider>().value);

    private void UpdateRoomCoverageMaxSliderText()
        => UpdateTextDependingOn<float>(bspRoomCoverageMaxSliderText, bspRoomCoverageMaxSlider.GetComponent<Slider>().value);

    private void UpdatePerlinNoiseScaleSliderText()
        => UpdateTextDependingOn<float>(perlinNoiseScaleSliderText, perlinNoiseScaleSlider.GetComponent<Slider>().value, 3);

    private void UpdatePerlinNoiseOctavesSliderText()
        => UpdateTextDependingOn<int>(perlinNoiseOctavesSliderText, perlinNoiseOctavesSlider.GetComponent<Slider>().value);

    private void UpdatePerlinNoisePersistenceSliderText()
        => UpdateTextDependingOn<float>(perlinNoisePersistenceSliderText, perlinNoisePersistenceSlider.GetComponent<Slider>().value);

    private void UpdatePerlinNoiseLacunaritySliderText()
        => UpdateTextDependingOn<float>(perlinNoiseLacunaritySliderText, perlinNoiseLacunaritySlider.GetComponent<Slider>().value);

    private void UpdatePerlinNoiseBrightnessSliderText()
        => UpdateTextDependingOn<float>(perlinNoiseBrightnessSliderText, perlinNoiseBrightnessSlider.GetComponent<Slider>().value);

    private void UpdateLSystemIterationsSliderText()
        => UpdateTextDependingOn<int>(lSystemIterationsSliderText, lSystemIterationsSlider.GetComponent<Slider>().value);

    private void UpdateLSystemRotationAngleSliderText()
        => UpdateTextDependingOn<float>(lSystemRotationAngleSliderText, lSystemRotationAngleSlider.GetComponent<Slider>().value);

    private void UpdateLSystemLineLengthSliderText()
        => UpdateTextDependingOn<float>(lSystemLineLengthSliderText, lSystemLineLengthSlider.GetComponent<Slider>().value);
}
