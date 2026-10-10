using Unity.Cinemachine;
using UnityEngine;

// Bildschirme, die schmaler als das Referenzformat sind (z.B. 16:10 oder 4:3), zoomen etwas heraus,
// damit mindestens die Breite des Referenzformats sichtbar bleibt. Breitere Bildschirme behalten
// die feste Höhe der Kamera. Es wird nur der Kamera-Zustand angepasst, nicht der Wert im Inspector.
[ExecuteAlways]
public class CameraAspectFit : CinemachineExtension
{
    [Tooltip("Seitenverhältnis, dessen Breite mindestens sichtbar sein soll (16:9 = 1.7778)")]
    [SerializeField] private float referenceAspect = 16f / 9f;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Finalize || Screen.height <= 0) return;

        float aspect = (float)Screen.width / Screen.height;
        if (aspect >= referenceAspect) return;

        state.Lens.OrthographicSize *= referenceAspect / aspect;
    }
}
