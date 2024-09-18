using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(Kiosk))]
public class KioskEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        //return base.CreateInspectorGUI();
        VisualElement i = new();
        ObjectField scriptField = new ObjectField("Script Reference")
        {
            objectType = typeof(Kiosk), // Set the object type to the script itself
            value = target // Set the value to the current script instance
        };
        scriptField.SetEnabled(false); // Disable the field
        i.Add(scriptField); // Add the field to the inspector

        // Create an empty spacer element with a fixed height
        VisualElement spacer = new VisualElement();
        spacer.style.height = 10;

#region REFERENCES
        var r = new Foldout();
        r.text = "References";
        r.style.unityFontStyleAndWeight = FontStyle.Bold;
        i.Add(r);

        SerializedProperty pImg = serializedObject.FindProperty("progressUI");
        var pImg_f = new PropertyField(pImg);
        pImg_f.tooltip = "Image to show the progress of the download";
        r.Add(pImg_f);
        
        var pImgGO = serializedObject.FindProperty("progress_GO");
        var pImgGO_f = new PropertyField(pImgGO);
        pImgGO_f.tooltip = "Reference to the progress game object";
        r.Add(pImgGO_f);

        var pAnim = serializedObject.FindProperty("animator");
        var pAnim_f = new PropertyField(pAnim);
        pAnim_f.tooltip = "Animator to control the panel";
        r.Add(pAnim_f);

        var pAutomatonD = serializedObject.FindProperty("automatonTargetDestination");
        var pAutomatonD_f = new PropertyField(pAutomatonD);
        pAutomatonD_f.tooltip = "Where the automaton should walk to";
        r.Add(pAutomatonD_f);

        var pHologramD = serializedObject.FindProperty("hologramTargetDestination");
        var pHologramD_f = new PropertyField(pHologramD);
        pHologramD_f.tooltip = "Where the hologram should appear at";
        r.Add(pHologramD_f);

        var pPopup = serializedObject.FindProperty("popup");
        var pPopup_f = new PropertyField(pPopup);
        pPopup_f.tooltip = "Popup that guides player to this kiosk";
        r.Add(pPopup_f);

        var pMapIcon = serializedObject.FindProperty("mapIcon");
        var pMapIcon_f = new PropertyField(pMapIcon);
        pMapIcon_f.tooltip = "Icon on the minimap";
        r.Add(pMapIcon_f);

        i.Add(spacer);
        #endregion

        #region SCANNING
        var s = new Foldout();
        s.text = "Scanning Values";
        s.style.unityFontStyleAndWeight = FontStyle.Bold;
        i.Add(s);
        
        var pSpeed = serializedObject.FindProperty("speedMultiplier");
        var pSpeed_f = new PropertyField(pSpeed);
        pSpeed_f.tooltip = "How fast the kiosk scans";
        s.Add(pSpeed_f);

        var pProgress = serializedObject.FindProperty("progress");
        var pProgress_f = new PropertyField(pProgress);
        pProgress_f.tooltip = "The progress of the download";
        s.Add(pProgress_f);

        var pLowColor = serializedObject.FindProperty("lowColor");
        var pLowColor_f = new PropertyField(pLowColor);
        pLowColor_f.tooltip = "Color to show when the download is low";
        s.Add(pLowColor_f);

        var pMedColor = serializedObject.FindProperty("medColor");
        var pMedColor_f = new PropertyField(pMedColor);
        pMedColor_f.tooltip = "Color to show when the download is medium";
        s.Add(pMedColor_f);

        var pHiColor = serializedObject.FindProperty("hiColor");
        var pHiColor_f = new PropertyField(pHiColor);
        pHiColor_f.tooltip = "Color to show when the download is high";
        s.Add(pHiColor_f);

        var pScanCompleted = serializedObject.FindProperty("scanCompleted");
        var pScanCompleted_f = new PropertyField(pScanCompleted);
        pScanCompleted_f.tooltip = "If the scan has been completed";
        s.Add(pScanCompleted_f);

        i.Add(spacer);
        #endregion

        #region 
        
        var h = new Foldout();
        h.text = "Hologram";
        h.style.unityFontStyleAndWeight = FontStyle.Bold;
        //i.Add(h);

        var pHasHologramPanels = serializedObject.FindProperty("hasHologramPanels");
        var pHasHologramPanels_f = new PropertyField(pHasHologramPanels);
        pHasHologramPanels_f.tooltip = "If the hologram has panels";

        var pHologram = serializedObject.FindProperty("hologram");
        var pHologram_f = new PropertyField(pHologram);
        pHologram_f.tooltip = "Hologram to show when the scan is completed";

        var pHologramData3D = serializedObject.FindProperty("hologram3DData");
        var pHologramData3D_f = new PropertyField(pHologramData3D);
        pHologramData3D_f.tooltip = "Data for the hologram that shows a 3D model";

        var pHologramSlideShowData = serializedObject.FindProperty("hologramslideShowData");
        var pHologramSlideShowData_f = new PropertyField(pHologramSlideShowData);
        pHologramSlideShowData_f.tooltip = "Data for the hologram that shows a slide show";

        var pType = serializedObject.FindProperty("type");
        var pType_f = new PropertyField(pType);
        pType_f.tooltip = "Type of hologram to show (affects what is shown on inspector)";

        h.Add(pHasHologramPanels_f);
        h.Add(pHologram_f);
        h.Add(pType_f);


        pType_f.RegisterValueChangeCallback(evt =>
        {
            // h.Clear();
            // pType_f.Bind(serializedObject);
            // pHasHologramPanels_f.Bind(serializedObject);
            // pHologram_f.Bind(serializedObject);
            // // pHologramData3D_f.Bind(serializedObject);

            // h.Add(pHasHologramPanels_f);
            // h.Add(pHologram_f);
            // h.Add(pType_f);
            // //UpdateVisibleFields(pType.enumValueIndex, h, pHologramData3D_f, pHologramSlideShowData_f);
            // switch (pType.enumValueIndex)
            // {
            //     case 0:
            //         h.Add(pHologramData3D_f);
            //         pHologramData3D_f.Bind(serializedObject);
            //         break;
            //     case 1:
            //         h.Add(pHologramSlideShowData_f);
            //         pHologramSlideShowData_f.Bind(serializedObject);
            //         break;
            //     default:
            //         break;

            // }
            // // Ensure the serialized object is updated properly
            // serializedObject.Update();

            // // Apply any changes made
            // serializedObject.ApplyModifiedProperties();

            // Only clear and update the specific hologram data section, not the entire foldout
            if (pType.enumValueIndex == 0) // Hologram3D case
            {
                // Ensure that you are not repeatedly adding the same field
                if (!h.Contains(pHologramData3D_f))
                {
                    h.Clear(); // Clear only this section
                    h.Add(pHologramData3D_f);
                    pHologramData3D_f.Bind(serializedObject);
                }
            }
            else if (pType.enumValueIndex == 1) // HologramSlideShow case
            {
                if (!h.Contains(pHologramSlideShowData_f))
                {
                    h.Clear(); // Clear only this section
                    h.Add(pHologramSlideShowData_f);
                    pHologramSlideShowData_f.Bind(serializedObject);
                }
            }

            // Rebind common elements and avoid unnecessary rebindings
            if (!h.Contains(pHasHologramPanels_f))
            {
                h.Add(pHasHologramPanels_f);
                pHasHologramPanels_f.Bind(serializedObject);
            }
            if (!h.Contains(pHologram_f))
            {
                h.Add(pHologram_f);
                pHologram_f.Bind(serializedObject);
            }
            if(!h.Contains(pType_f)){
                h.Add(pType_f);
                pType_f.Bind(serializedObject);
            }
            // Bind once to avoid performance issues
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();

        });

        // var pHologramData3D = serializedObject.FindProperty("hologram3DData");
        // var pHologramData3D_f = new PropertyField(pHologramData3D);
        // pHologramData3D_f.tooltip = "Data for the hologram that shows a 3D model";
        // h.Add(pHologramData3D_f);

        // var pHologramSlideShowData = serializedObject.FindProperty("hologramslideShowData");
        // var pHologramSlideShowData_f = new PropertyField(pHologramSlideShowData);
        // pHologramSlideShowData_f.tooltip = "Data for the hologram that shows a slide show";
        // h.Add(pHologramSlideShowData_f);\
        // h.Add(pHasHologramPanels_f);
        // h.Add(pHologram_f);
        // h.Add(pType_f);
        i.Add(h);

        #endregion
        return i;
    }
}
