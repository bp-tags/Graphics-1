using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System;
using UnityEditor.SceneTemplate;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
    [InitializeOnLoad]
    partial class HDWizard : EditorWindow
    {
        static class Style
        {
            public static readonly GUIContent title = EditorGUIUtility.TrTextContent("Render Pipeline Wizard");

            public const string hdrpProjectSettingsPathLabel = "Default Resources Folder";
            public const string hdrpProjectSettingsPathTooltip = "Resources Folder will be the one where to get project elements related to HDRP as default scene and default settings.";
            public const string firstTimeInitLabel = "Populate / Reset";
            public const string firstTimeInitTooltip = "Populate or override Default Resources Folder content with required assets and assign it in GraphicSettings.";
            public const string newSceneLabel = "Default Scene Prefab";
            public const string newSceneTooltip = "This prefab contains scene elements that are used when creating a new scene in HDRP.";
            public const string newDXRSceneLabel = "Default DXR Scene Prefab";
            public const string newDXRSceneTooltip = "This prefab contains scene elements that are used when creating a new scene in HDRP when ray-tracing is activated in the HDRenderPipelineAsset.";
            public const string hdrpConfigLabel = "HDRP";
            public const string hdrpConfigTooltip = "This tab contains configuration check for High Definition Render Pipeline.";
            public const string hdrpVRConfigLabel = "HDRP + VR";
            public const string hdrpVRConfigTooltip = "This tab contains configuration check for High Definition Render Pipeline along with Virtual Reality configuration.";
            public const string hdrpDXRConfigLabel = "HDRP + DXR";
            public const string hdrpDXRConfigTooltip = "This tab contains configuration check for High Definition Render Pipeline along with DirectX Raytracing configuration.";
            public const string showOnStartUp = "Show on start";

            public const string defaultSettingsTitle = "Default Path Settings";
            public const string configurationTitle = "Configuration Checking";
            public const string migrationTitle = "Project Migration Quick-links";

            public const string installConfigPackageLabel = "Install Configuration Editable Package";
            public const string installConfigPackageInfoInCheck = "Checking if the local config package is installed in your project's LocalPackage folder.";
            public const string installConfigPackageInfoInProgress = "The local config package is being installed in your project's LocalPackage folder.";
            public const string installConfigPackageInfoFinished = "The local config package is already installed in your project's LocalPackage folder.";

            public const string migrateAllButton = "Upgrade Project Materials to High Definition Materials";
            public const string migrateSelectedButton = "Upgrade Selected Materials to High Definition Materials";
            public const string migrateLights = "Upgrade Unity Builtin Scene Light Intensity for High Definition";

            public const string hdrpVersionLast = "You are using High-Definition Render Pipeline lastest {0} version."; //{0} will be replaced when displayed by the version number.
            public const string hdrpVersionNotLast = "You are using High-Definition Render Pipeline {0} version. A new {1} version is available."; //{0} and {1} will be replaced when displayed by the version number.
            public const string hdrpVersionWithLocalPackage = "You are using High-Definition Render Pipeline local {0} version. Last packaged version available is {1}."; //{0} and {1} will be replaced when displayed by the version number.
            public const string hdrpVersionChecking = "Checking last version available for High-Definition Render Pipeline.";

            //configuration debugger
            public const string resolve = "Fix";
            public const string resolveAll = "Fix All";
            public const string resolveAllQuality = "Fix All Qualities";
            public const string resolveAllBuildTarget = "Fix All Platforms";

            public struct ConfigStyle
            {
                public readonly string label;
                public readonly string error;
                public readonly string button;
                public ConfigStyle(string label, string error, string button = resolve)
                {
                    this.label = label;
                    this.error = error;
                    this.button = button;
                }
            }

            public static readonly ConfigStyle hdrpColorSpace = new ConfigStyle(
                label: "Color space",
                error: "Only linear color space supported!");
            public static readonly ConfigStyle hdrpLightmapEncoding = new ConfigStyle(
                label: "Lightmap encoding",
                error: "Only high quality lightmap supported!",
                button: resolveAllBuildTarget);
            public static readonly ConfigStyle hdrpShadow = new ConfigStyle(
                label: "Shadows",
                error: "Shadow must be set to activated! (either on hard or soft)");
            public static readonly ConfigStyle hdrpShadowmask = new ConfigStyle(
                label: "Shadowmask mode",
                error: "Only distance shadowmask supported at the project level! (You can still change this per light.)",
                button: resolveAllQuality);
            public static readonly ConfigStyle hdrpAsset = new ConfigStyle(
                label: "Asset configuration",
                error: "There are issues in the HDRP asset configuration. (see below)",
                button: resolveAll);
            public static readonly ConfigStyle hdrpAssetAssigned = new ConfigStyle(
                label: "Assigned",
                error: "There is no HDRP asset assigned to the render pipeline!");
            public static readonly ConfigStyle hdrpAssetRuntimeResources = new ConfigStyle(
                label: "Runtime resources",
                error: "There is an issue with the runtime resources!");
            public static readonly ConfigStyle hdrpAssetEditorResources = new ConfigStyle(
                label: "Editor resources",
                error: "There is an issue with the editor resources!");
            public static readonly ConfigStyle hdrpBatcher = new ConfigStyle(
                label: "SRP Batcher",
                error: "SRP Batcher must be enabled!");
            public static readonly ConfigStyle hdrpAssetDiffusionProfile = new ConfigStyle(
                label: "Diffusion profile",
                error: "There is no diffusion profile assigned in the HDRP asset!");
            public static readonly ConfigStyle hdrpScene = new ConfigStyle(
                label: "Default scene prefab",
                error: "Default scene prefab must be set to create HD templated scene!");
            public static readonly ConfigStyle hdrpVolumeProfile = new ConfigStyle(
                label: "Default volume profile",
                error: "Default volume profile must be assigned in the HDRP asset!");

            public static readonly ConfigStyle vrActivated = new ConfigStyle(
                label: "VR activated",
                error: "VR need to be enabled in Player Settings!");

            public static readonly ConfigStyle dxrAutoGraphicsAPI = new ConfigStyle(
                label: "Auto graphics API",
                error: "Auto Graphics API is not supported!");
            public static readonly ConfigStyle dxrD3D12 = new ConfigStyle(
                label: "Direct3D 12",
                error: "Direct3D 12 is needed!");
            public static readonly ConfigStyle dxrScreenSpaceShadow = new ConfigStyle(
                label: "Screen Space Shadow",
                error: "Screen Space Shadow is required!");
            public static readonly ConfigStyle dxrReflections = new ConfigStyle(
                label: "Reflections",
                error: "Screen Space Reflections are required!");
            public static readonly ConfigStyle dxrStaticBatching = new ConfigStyle(
                label: "Static Batching",
                error: "Static Batching is not supported!");
            public static readonly ConfigStyle dxrActivated = new ConfigStyle(
                label: "DXR activated",
                error: "DXR is not activated!");
            public static readonly ConfigStyle dxrResources = new ConfigStyle(
                label: "DXR resources",
                error: "There is an issue with the DXR resources!");
            public static readonly ConfigStyle dxrShaderConfig = new ConfigStyle(
                label: "DXR shader config",
                error: "There is an issue with the DXR shader config!");
            public static readonly ConfigStyle dxrScene = new ConfigStyle(
                label: "Default DXR scene prefab",
                error: "Default DXR scene prefab must be set to create HD templated scene!");

            public const string hdrpAssetDisplayDialogTitle = "Create or Load HDRenderPipelineAsset";
            public const string hdrpAssetDisplayDialogContent = "Do you want to create a fresh HDRenderPipelineAsset in the default resource folder and automatically assign it?";
            public const string diffusionProfileSettingsDisplayDialogTitle = "Create or Load DiffusionProfileSettings";
            public const string diffusionProfileSettingsDisplayDialogContent = "Do you want to create a fresh DiffusionProfileSettings in the default resource folder and automatically assign it?";
            public const string scenePrefabTitle = "Create or Load HD default scene";
            public const string scenePrefabContent = "Do you want to create a fresh HD default scene in the default resource folder and automatically assign it?";
            public const string dxrScenePrefabTitle = "Create or Load DXR HD default scene";
            public const string dxrScenePrefabContent = "Do you want to create a fresh DXR HD default scene in the default resource folder and automatically assign it?";
            public const string displayDialogCreate = "Create One";
            public const string displayDialogLoad = "Load One";
            public const string displayDialogCancel = "Cancel";
        }

        enum Configuration
        {
            HDRP,
            HDRP_VR,
            HDRP_DXR
        }
        
        enum ConfigPackageState
        {
            BeingChecked,
            Missing,
            Present,
            BeingFixed
        }

        Configuration m_Configuration;
        VisualElement m_BaseUpdatable;
        VisualElement m_InstallConfigPackageHelpbox = null;
        VisualElement m_InstallConfigPackageButton = null;
        Label m_InstallConfigPackageHelpboxLabel;
        ObjectField m_DefaultScene;
        ObjectField m_DefaultDXRScene;

        [MenuItem("Window/Render Pipeline/HD Render Pipeline Wizard", priority = 10000)]
        static void OpenWindow()
        {
            var window = GetWindow<HDWizard>("HD Render Pipeline Wizard");
            window.minSize = new Vector2(420, 450);
        }

        void OnGUI()
        {
            foreach (VisualElementUpdatable updatable in m_BaseUpdatable.Children().Where(c => c is VisualElementUpdatable))
                updatable.CheckUpdate();
        }

        static HDWizard()
        {
            LoadReflectionMethods();
            WizardBehaviour();
        }

        #region SCRIPT_RELOADING

        static int frameToWait;
        
        static void WizardBehaviourDelayed()
        {
            if (frameToWait > 0)
                --frameToWait;
            else if (HDProjectSettings.wizardIsStartPopup)
            {
                EditorApplication.update -= WizardBehaviourDelayed;

                //Application.isPlaying cannot be called in constructor. Do it here
                if (Application.isPlaying)
                    return;

                OpenWindow();
            }
        }
        
        static void WizardBehaviour()
        {
            //We need to wait at least one frame or the popup will not show up
            frameToWait = 10;
            EditorApplication.update += WizardBehaviourDelayed;
        }
        
        [Callbacks.DidReloadScripts]
        static void ResetDelayed()
        {
            //remove it from domain reload but keep it in editor opening
            frameToWait = 0;
            EditorApplication.update -= WizardBehaviourDelayed;
        }

        #endregion

        #region DRAWERS

        private void OnEnable()
        {
            titleContent = Style.title;
            
            HDEditorUtils.AddStyleSheets(rootVisualElement, HDEditorUtils.FormatingPath); //.h1
            HDEditorUtils.AddStyleSheets(rootVisualElement, HDEditorUtils.WizardSheetPath);

            var scrollView = new ScrollView(ScrollViewMode.Vertical);
            rootVisualElement.Add(scrollView);
            var container = scrollView.contentContainer;

            container.Add(CreateHdrpVersionChecker());

            container.Add(CreateTitle(Style.defaultSettingsTitle));
            container.Add(CreateFolderData());

            container.Add(CreateInstallConfigPackageArea());

            container.Add(CreateTitle(Style.configurationTitle));
            container.Add(CreateTabbedBox(
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? new[] {
                        (Style.hdrpConfigLabel, Style.hdrpConfigTooltip),
                        (Style.hdrpVRConfigLabel, Style.hdrpVRConfigTooltip),
                        (Style.hdrpDXRConfigLabel, Style.hdrpDXRConfigTooltip),
                    }
                    : new[] {
                        (Style.hdrpConfigLabel, Style.hdrpConfigTooltip),
                        //VR only supported on window
                        //DXR only supported on window
                    },
                out m_BaseUpdatable));

            m_BaseUpdatable.Add(new FixAllButton(
                Style.resolveAll,
                () =>
                {
                    bool isCorrect = IsHDRPAllCorrect();
                    switch (m_Configuration)
                    {
                        case Configuration.HDRP_VR:
                            isCorrect &= IsVRAllCorrect();
                            break;
                        case Configuration.HDRP_DXR:
                            isCorrect &= IsDXRAllCorrect();
                            break;
                    }
                    return isCorrect;
                },
                () =>
                {
                    FixHDRPAll();
                    switch (m_Configuration)
                    {
                        case Configuration.HDRP_VR:
                            FixVRAll();
                            break;
                        case Configuration.HDRP_DXR:
                            FixDXRAll();
                            break;
                    }
                }));

            AddHDRPConfigInfo(m_BaseUpdatable);

            var vrScope = new HiddableUpdatableContainer(()
                => m_Configuration == Configuration.HDRP_VR);
            AddVRConfigInfo(vrScope);
            vrScope.Init();
            m_BaseUpdatable.Add(vrScope);
            
            var dxrScope = new HiddableUpdatableContainer(()
                => m_Configuration == Configuration.HDRP_DXR);
            AddDXRConfigInfo(dxrScope);
            dxrScope.Init();
            m_BaseUpdatable.Add(dxrScope);
            
            container.Add(CreateTitle(Style.migrationTitle));
            container.Add(CreateLargeButton(Style.migrateAllButton, UpgradeStandardShaderMaterials.UpgradeMaterialsProject));
            container.Add(CreateLargeButton(Style.migrateSelectedButton, UpgradeStandardShaderMaterials.UpgradeMaterialsSelection));
            container.Add(CreateLargeButton(Style.migrateLights, UpgradeStandardShaderMaterials.UpgradeLights));

            container.Add(CreateWizardBehaviour());
        }

        VisualElement CreateFolderData()
        {
            var defaultResourceFolder = new TextField(Style.hdrpProjectSettingsPathLabel)
            {
                tooltip = Style.hdrpProjectSettingsPathTooltip,
                name = "DefaultResourceFolder",
                value = HDProjectSettings.projectSettingsFolderPath
            };
            defaultResourceFolder.Q<Label>().AddToClassList("normal");
            defaultResourceFolder.RegisterValueChangedCallback(evt
                => HDProjectSettings.projectSettingsFolderPath = evt.newValue);

            var repopulate = new Button(Repopulate)
            {
                text = Style.firstTimeInitLabel,
                tooltip = Style.firstTimeInitTooltip,
                name = "Repopulate"
            };

            var row = new VisualElement() { name = "ResourceRow" };
            row.Add(defaultResourceFolder);
            row.Add(repopulate);

            return row;
        }

        VisualElement CreateTabbedBox((string label, string tooltip)[] tabs, out VisualElement innerBox)
        {
            var toolbar = new ToolbarRadio();
            toolbar.AddRadios(tabs);
            toolbar.SetValueWithoutNotify(HDProjectSettings.wizardActiveTab);
            m_Configuration = (Configuration)HDProjectSettings.wizardActiveTab;
            toolbar.RegisterValueChangedCallback(evt =>
            {
                int index = evt.newValue;
                m_Configuration = (Configuration)index;
                HDProjectSettings.wizardActiveTab = index;
            });

            var outerBox = new VisualElement() { name = "OuterBox" };
            innerBox = new VisualElement { name = "InnerBox" };

            outerBox.Add(toolbar);
            outerBox.Add(innerBox);

            return outerBox;
        }

        VisualElement CreateWizardBehaviour()
        {
            var toggle = new Toggle(Style.showOnStartUp)
            {
                value = HDProjectSettings.wizardIsStartPopup,
                name = "WizardCheckbox"
            };
            toggle.RegisterValueChangedCallback(evt
                => HDProjectSettings.wizardIsStartPopup = evt.newValue);
            return toggle;
        }

        VisualElement CreateLargeButton(string title, Action action)
            => new Button(action)
            {
                text = title,
                name = "LargeButton"
            };

        VisualElement CreateInstallConfigPackageArea()
        {
            VisualElement area = new VisualElement()
            {
                name = "InstallConfigPackageArea"
            };
            m_InstallConfigPackageButton = CreateLargeButton(Style.installConfigPackageLabel, () =>
            {
                UpdateDisplayOfConfigPackageArea(ConfigPackageState.BeingFixed);
                InstallLocalConfigurationPackage(() =>
                    UpdateDisplayOfConfigPackageArea(ConfigPackageState.Present));
            });
            m_InstallConfigPackageHelpbox = new HelpBox(HelpBox.Kind.Info, Style.installConfigPackageInfoInCheck);
            m_InstallConfigPackageHelpboxLabel = m_InstallConfigPackageHelpbox.Q<Label>();
            area.Add(m_InstallConfigPackageButton);
            area.Add(m_InstallConfigPackageHelpbox);

            UpdateDisplayOfConfigPackageArea(ConfigPackageState.BeingChecked);

            RefreshDisplayOfConfigPackageArea();
            return area;
        }

        void CreateSceneTemplate(string name, string description, SceneAsset sceneBinded, Texture2D snapshot)
        {
            var newAsset = ScriptableObject.CreateInstance<SceneTemplateAsset>();
            newAsset.name = name;

            newAsset.BindScene(sceneBinded);
            foreach (var dependency in newAsset.dependencies)
            {
                dependency.instantiationMode =
                    dependency.dependency is UnityEngine.Rendering.VolumeProfile
                    ? TemplateInstantiationMode.Clone
                    : TemplateInstantiationMode.Reference;
            }
            newAsset.templateName = name;
            newAsset.description = description;
            newAsset.addToDefaults = true;
            newAsset.preview = snapshot;

            AssetDatabase.CreateAsset(newAsset, $"Assets/{HDProjectSettings.projectSettingsFolderPath}/{name}.asset");
            ProjectWindowUtil.ShowCreatedAsset(newAsset);
        }
        
        void UpdateDisplayOfConfigPackageArea(ConfigPackageState state)
        {
            switch (state)
            {
                case ConfigPackageState.Present:
                    m_InstallConfigPackageButton.SetEnabled(false);
                    m_InstallConfigPackageButton.focusable = false;
                    m_InstallConfigPackageHelpbox.style.display = DisplayStyle.Flex;
                    m_InstallConfigPackageHelpboxLabel.text = Style.installConfigPackageInfoFinished;
                    break;

                case ConfigPackageState.Missing:
                    m_InstallConfigPackageButton.SetEnabled(true);
                    m_InstallConfigPackageButton.focusable = true;
                    m_InstallConfigPackageHelpbox.style.display = DisplayStyle.None;
                    break;
                    
                case ConfigPackageState.BeingChecked:
                    m_InstallConfigPackageButton.SetEnabled(false);
                    m_InstallConfigPackageButton.focusable = false;
                    m_InstallConfigPackageHelpbox.style.display = DisplayStyle.Flex;
                    m_InstallConfigPackageHelpboxLabel.text = Style.installConfigPackageInfoInCheck;
                    break;
                    
                case ConfigPackageState.BeingFixed:
                    m_InstallConfigPackageButton.SetEnabled(false);
                    m_InstallConfigPackageButton.focusable = false;
                    m_InstallConfigPackageHelpbox.style.display = DisplayStyle.Flex;
                    m_InstallConfigPackageHelpboxLabel.text = Style.installConfigPackageInfoInProgress;
                    break;
            }
        }

        void GroupEntriesForDisplay(VisualElement container, InclusiveScope filter)
        {
            foreach (var entry in entries.Where(e => filter.Contains(e.scope)))
                container.Add(new ConfigInfoLine(
                    entry.configStyle.label,
                    entry.configStyle.error,
                    entry.configStyle.button,
                    () => entry.check(),
                    () => entry.fix(fromAsync: false),
                    entry.indent));
        }

        void AddHDRPConfigInfo(VisualElement container)
            => GroupEntriesForDisplay(container, InclusiveScope.HDRP);
        void AddVRConfigInfo(VisualElement container)
            => GroupEntriesForDisplay(container, InclusiveScope.VR);
        void AddDXRConfigInfo(VisualElement container)
            => GroupEntriesForDisplay(container, InclusiveScope.DXR);

        Label CreateTitle(string title)
        {
            var label = new Label(title);
            label.AddToClassList("h1");
            return label;
        }

        HelpBox CreateHdrpVersionChecker()
        {
            var helpBox = new HelpBox(HelpBox.Kind.Info, Style.hdrpVersionChecking);

            m_LastAvailablePackageRetriever.ProcessAsync(k_HdrpPackageName, version =>
            {
                m_UsedPackageRetriever.ProcessAsync(k_HdrpPackageName, packageInfo =>
                {
                    if (packageInfo.source == PackageManager.PackageSource.Local)
                    {
                        helpBox.kind = HelpBox.Kind.Info;
                        helpBox.text = String.Format(Style.hdrpVersionWithLocalPackage, packageInfo.version, version);
                    }
                    else if(new Version(packageInfo.version) < new Version(version))
                    {
                        helpBox.kind = HelpBox.Kind.Warning;
                        helpBox.text = String.Format(Style.hdrpVersionNotLast, packageInfo.version, version);
                    }
                    else if (new Version(packageInfo.version) == new Version(version))
                    {
                        helpBox.kind = HelpBox.Kind.Info;
                        helpBox.text = String.Format(Style.hdrpVersionLast, version);
                    }
                });
            });

            return helpBox;
        }

        #endregion
    }
}
