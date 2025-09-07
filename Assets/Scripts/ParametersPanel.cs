using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ParametersPanel : MonoBehaviour
{
    [SerializeField] private BinarySpacePartitioningSettings bspSettings;

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

    private void Start()
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

    private void UpdateTextDependingOn<T>(TextMeshProUGUI text, float value)
        => text.text = typeof(T) == typeof(int) ? ((int)value).ToString() : value.ToString("F2");

    private void UpdateMaximumDepthSliderText()
        => UpdateTextDependingOn<int>(bspMaximumDepthSliderText, bspMaximumDepthSlider.GetComponent<Slider>().value);

    private void UpdatePartitionCenterBiasSliderText()
        => UpdateTextDependingOn<float>(bspPartitionCenterBiasSliderText, bspPartitionCenterBiasSlider.GetComponent<Slider>().value);

    private void UpdateRoomCoverageMinSliderText()
        => UpdateTextDependingOn<float>(bspRoomCoverageMinSliderText, bspRoomCoverageMinSlider.GetComponent<Slider>().value);

    private void UpdateRoomCoverageMaxSliderText()
        => UpdateTextDependingOn<float>(bspRoomCoverageMaxSliderText, bspRoomCoverageMaxSlider.GetComponent<Slider>().value);
}
