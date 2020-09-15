using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
    struct LightUnitSliderUIDescriptor
    {
        public LightUnitSliderUIDescriptor(LightUnitSliderUIRange[] valueRanges, float[] sliderDistribution, string cautionTooltip, string unitName, bool hasMarkers = true, bool clampValue = false)
        {
            this.valueRanges = valueRanges;
            this.cautionTooltip = cautionTooltip;
            this.sliderDistribution = sliderDistribution;
            this.unitName = unitName;
            this.hasMarkers = hasMarkers;
            this.clampValue = clampValue;

            sliderRange = new Vector2(
                this.valueRanges.Min(x => x.value.x),
                this.valueRanges.Max(x => x.value.y)
            );
        }

        public readonly float[] sliderDistribution;
        public readonly LightUnitSliderUIRange[] valueRanges;
        public readonly Vector2 sliderRange;
        public readonly string cautionTooltip;
        public readonly string unitName;
        public readonly bool hasMarkers;
        public readonly bool clampValue;
    }

    struct LightUnitSliderUIRange
    {
        public LightUnitSliderUIRange(Texture2D icon, string tooltip, Vector2 value)
        {
            this.content = new GUIContent(icon, tooltip);
            this.value = value;
        }

        public static LightUnitSliderUIRange CautionRange(string tooltip, float value) => new LightUnitSliderUIRange
        {
            // Load the buildin caution icon with provided tooltip.
            content = new GUIContent( EditorGUIUtility.TrIconContent("console.warnicon").image, tooltip),
            value = new Vector2(-1, value)
        };

        public GUIContent content;
        public Vector2    value;
    }

    static class LightUnitSliderDescriptors
    {
        // Lux
        public static LightUnitSliderUIDescriptor LuxDescriptor = new LightUnitSliderUIDescriptor(
        LightUnitValueRanges.LuxValueTable,
        LightUnitSliderDistributions.LuxDistribution,
        LightUnitTooltips.k_SunCaution,
           "Lux"
        );

        // Lumen
        public static LightUnitSliderUIDescriptor LumenDescriptor = new LightUnitSliderUIDescriptor(
            LightUnitValueRanges.LumenValueTable,
            LightUnitSliderDistributions.LinearDistribution,
            LightUnitTooltips.k_PunctualCaution,
            "Lumen"
        );

        // Candela
        public static LightUnitSliderUIDescriptor CandelaDescriptor = new LightUnitSliderUIDescriptor(
            LightUnitValueRanges.CandelaValueTable,
            LightUnitSliderDistributions.LinearDistribution,
            LightUnitTooltips.k_PunctualCaution,
            "Candela"
        );

        // EV100
        public static LightUnitSliderUIDescriptor EV100Descriptor = new LightUnitSliderUIDescriptor(
            LightUnitValueRanges.EV100ValueTable,
            LightUnitSliderDistributions.LinearDistribution,
            LightUnitTooltips.k_PunctualCaution,
            "EV"
        );

        // Nits
        public static LightUnitSliderUIDescriptor NitsDescriptor = new LightUnitSliderUIDescriptor(
            LightUnitValueRanges.NitsValueTable,
            LightUnitSliderDistributions.LinearDistribution,
            LightUnitTooltips.k_PunctualCaution,
            "Nits"
        );

        // Exposure
        public static LightUnitSliderUIDescriptor ExposureDescriptor = new LightUnitSliderUIDescriptor(
            LightUnitValueRanges.ExposureValueTable,
            LightUnitSliderDistributions.ExposureDistribution,
            LightUnitTooltips.k_ExposureCaution,
            "EV"
        );

        // Temperature
        public static LightUnitSliderUIDescriptor TemperatureDescriptor = new LightUnitSliderUIDescriptor(
            LightUnitValueRanges.KelvinValueTable,
            LightUnitSliderDistributions.LinearDistribution,
            LightUnitTooltips.k_TemperatureCaution,
            "Kelvin",
            false,
            true
        );

        private static class LightUnitValueRanges
        {
            // Shorthand helper for converting the pre-defined ranges into other units (Nits, EV, Candela).
            static float LuxToEV(float x) => LightUtils.ConvertLuxToEv(x, 1f);
            static float LuxToCandela(float x) => LightUtils.ConvertLuxToCandela(x, 1f);

            // Note: In case of area light, the intensity is scaled by the light size. How should this be reconciled in the UI?
            static float LumenToNits(float x) => LightUtils.ConvertRectLightLumenToLuminance(x, 1f, 1f);

            public static readonly LightUnitSliderUIRange[] LumenValueTable =
            {
                new LightUnitSliderUIRange(LightUnitIcon.ExteriorLight,  LightUnitTooltips.k_PunctualExterior,   new Vector2(3000, 40000)),
                new LightUnitSliderUIRange(LightUnitIcon.InteriorLight,  LightUnitTooltips.k_PunctualInterior,   new Vector2(300,  3000)),
                new LightUnitSliderUIRange(LightUnitIcon.DecorativeLight,LightUnitTooltips.k_PunctualDecorative, new Vector2(15,   300)),
                new LightUnitSliderUIRange(LightUnitIcon.Candlelight,    LightUnitTooltips.k_PunctualCandle,     new Vector2(0,    15)),
            };

            public static readonly LightUnitSliderUIRange[] NitsValueTable =
            {
                new LightUnitSliderUIRange(LightUnitIcon.ExteriorLight,   LightUnitTooltips.k_PunctualExterior,   new Vector2(LumenToNits(3000), LumenToNits(40000))),
                new LightUnitSliderUIRange(LightUnitIcon.InteriorLight,   LightUnitTooltips.k_PunctualInterior,   new Vector2(LumenToNits(300),  LumenToNits(3000))),
                new LightUnitSliderUIRange(LightUnitIcon.DecorativeLight, LightUnitTooltips.k_PunctualDecorative, new Vector2(LumenToNits(15),   LumenToNits(300))),
                new LightUnitSliderUIRange(LightUnitIcon.Candlelight,     LightUnitTooltips.k_PunctualCandle,     new Vector2(0,               LumenToNits(15))),
            };

            public static readonly LightUnitSliderUIRange[] LuxValueTable =
            {
                new LightUnitSliderUIRange(LightUnitIcon.BrightSky,     LightUnitTooltips.k_LuxBrightSky,     new Vector2(80000, 120000)),
                new LightUnitSliderUIRange(LightUnitIcon.Overcast,      LightUnitTooltips.k_LuxOvercastSky,   new Vector2(10000, 80000)),
                new LightUnitSliderUIRange(LightUnitIcon.SunriseSunset, LightUnitTooltips.k_LuxSunriseSunset, new Vector2(1,     10000)),
                new LightUnitSliderUIRange(LightUnitIcon.Moonlight,     LightUnitTooltips.k_LuxMoonlight,     new Vector2(0,     1)),
            };

            public static readonly LightUnitSliderUIRange[] CandelaValueTable =
            {
                new LightUnitSliderUIRange(LightUnitIcon.ExteriorLight,   LightUnitTooltips.k_PunctualExterior,   new Vector2(LuxToCandela(80000),  LuxToCandela(120000))),
                new LightUnitSliderUIRange(LightUnitIcon.InteriorLight,   LightUnitTooltips.k_PunctualInterior,   new Vector2(LuxToCandela(10000),  LuxToCandela(80000))),
                new LightUnitSliderUIRange(LightUnitIcon.DecorativeLight, LightUnitTooltips.k_PunctualDecorative, new Vector2(LuxToCandela(1),      LuxToCandela(10000))),
                new LightUnitSliderUIRange(LightUnitIcon.Candlelight,     LightUnitTooltips.k_PunctualCandle,     new Vector2(0,                       LuxToCandela(1))),
            };

            public static readonly LightUnitSliderUIRange[] EV100ValueTable =
            {
                new LightUnitSliderUIRange(LightUnitIcon.ExteriorLight,   LightUnitTooltips.k_PunctualExterior,   new Vector2(LuxToEV(80000),  LuxToEV(120000))),
                new LightUnitSliderUIRange(LightUnitIcon.InteriorLight,   LightUnitTooltips.k_PunctualInterior,   new Vector2(LuxToEV(10000),  LuxToEV(80000))),
                new LightUnitSliderUIRange(LightUnitIcon.DecorativeLight, LightUnitTooltips.k_PunctualDecorative, new Vector2(LuxToEV(1),      LuxToEV(10000))),
                new LightUnitSliderUIRange(LightUnitIcon.Candlelight,     LightUnitTooltips.k_PunctualCandle,     new Vector2(0,                  LuxToEV(1))),
            };

            // Same units as EV100, but we declare a new table since we use different icons in the exposure context.
            public static readonly LightUnitSliderUIRange[] ExposureValueTable =
            {
                new LightUnitSliderUIRange(LightUnitIcon.BrightSky,     LightUnitTooltips.k_ExposureBrightSky,     new Vector2(12, 16)),
                new LightUnitSliderUIRange(LightUnitIcon.Overcast,      LightUnitTooltips.k_ExposureOvercastSky,   new Vector2(8,  12)),
                new LightUnitSliderUIRange(LightUnitIcon.SunriseSunset, LightUnitTooltips.k_ExposureSunriseSunset, new Vector2(6,   8)),
                new LightUnitSliderUIRange(LightUnitIcon.InteriorLight, LightUnitTooltips.k_ExposureInterior,      new Vector2(3,   6)),
                new LightUnitSliderUIRange(LightUnitIcon.Moonlight,     LightUnitTooltips.k_ExposureMoonlitSky,    new Vector2(0,   3)),
                new LightUnitSliderUIRange(LightUnitIcon.MoonlessNight, LightUnitTooltips.k_ExposureMoonlessNight, new Vector2(-5,  0)),
            };

            public static readonly LightUnitSliderUIRange[] KelvinValueTable =
            {
                new LightUnitSliderUIRange(LightUnitIcon.BlueSky,          LightUnitTooltips.k_TemperatureBlueSky,        new Vector2(10000, 20000)),
                new LightUnitSliderUIRange(LightUnitIcon.Overcast,         LightUnitTooltips.k_TemperatureCloudySky,      new Vector2(6500,  10000)),
                new LightUnitSliderUIRange(LightUnitIcon.DirectSunlight,   LightUnitTooltips.k_TemperatureDirectSunlight, new Vector2(3500,   6500)),
                new LightUnitSliderUIRange(LightUnitIcon.IntenseAreaLight, LightUnitTooltips.k_TemperatureArtificial,     new Vector2(2500,   3500)),
                new LightUnitSliderUIRange(LightUnitIcon.Candlelight,      LightUnitTooltips.k_TemperatureCandle,        new Vector2(1500,   2500)),
            };
        }

        private static class LightUnitSliderDistributions
        {
            // Warning: All of these values need to be kept in sync with their associated descriptor's set of value ranges.
            public static float[] LuxDistribution = {0.0f, 0.05f, 0.5f, 0.9f, 1.0f};

            private const float LinearStep = 1 / 4f;
            public static float[] LinearDistribution =
            {
                0 * LinearStep,
                1 * LinearStep,
                2 * LinearStep,
                3 * LinearStep,
                4 * LinearStep
            };

            private const float ExposureStep = 1 / 6f;
            public static float[] ExposureDistribution =
            {
                0 * ExposureStep,
                1 * ExposureStep,
                2 * ExposureStep,
                3 * ExposureStep,
                4 * ExposureStep,
                5 * ExposureStep,
                6 * ExposureStep
            };
        }

        private static class LightUnitIcon
        {
            static string GetLightUnitIconPath() => HDUtils.GetHDRenderPipelinePath() +
                                                    "/Editor/RenderPipelineResources/Texture/LightUnitIcons/";

            // Note: We do not use the editor resource loading mechanism for light unit icons because we need to skin the icon correctly for the editor theme.
            // Maybe the resource reloader can be improved to support icon loading (thus supporting skinning)?
            static Texture2D GetLightUnitIcon(string name)
            {
                var path = GetLightUnitIconPath() + name + ".png";
                return EditorGUIUtility.TrIconContent(path).image as Texture2D;
            }

            // TODO: Move all light unit icons from the package into the built-in resources.
            public static Texture2D BlueSky          = GetLightUnitIcon("BlueSky");
            public static Texture2D ClearSky         = GetLightUnitIcon("ClearSky");
            public static Texture2D Candlelight      = GetLightUnitIcon("Candlelight");
            public static Texture2D DecorativeLight  = GetLightUnitIcon("DecorativeLight");
            public static Texture2D DirectSunlight   = GetLightUnitIcon("DirectSunlight");
            public static Texture2D ExteriorLight    = GetLightUnitIcon("ExteriorLight");
            public static Texture2D IntenseAreaLight = GetLightUnitIcon("IntenseAreaLight");
            public static Texture2D InteriorLight    = GetLightUnitIcon("InteriorLight");
            public static Texture2D MediumAreaLight  = GetLightUnitIcon("MediumAreaLight");
            public static Texture2D MoonlessNight    = GetLightUnitIcon("MoonlessNight");
            public static Texture2D Moonlight        = GetLightUnitIcon("Moonlight");
            public static Texture2D Overcast         = GetLightUnitIcon("Overcast");
            public static Texture2D CloudySky        = GetLightUnitIcon("CloudySky");
            public static Texture2D SoftAreaLight    = GetLightUnitIcon("SoftAreaLight");
            public static Texture2D SunriseSunset    = GetLightUnitIcon("SunriseSunset");
            public static Texture2D VeryBrightSun    = GetLightUnitIcon("VeryBrightSun");
            public static Texture2D BrightSky        = GetLightUnitIcon("BrightSky");
        }

        private static class LightUnitTooltips
        {
            // Caution
            public const string k_SunCaution         = "Higher than Sunlight";
            public const string k_PunctualCaution    = "Very high intensity light";
            public const string k_ExposureCaution    = "Higher than sunlight";
            public const string k_TemperatureCaution = "";

            // Lux / Directional
            public const string k_LuxBrightSky       = "High Sun";
            public const string k_LuxOvercastSky     = "Cloudy";
            public const string k_LuxSunriseSunset   = "Low Sun";
            public const string k_LuxMoonlight       = "Moon";

            // Punctual
            public const string k_PunctualExterior   = "Exterior";
            public const string k_PunctualInterior   = "Interior";
            public const string k_PunctualDecorative = "Decorative";
            public const string k_PunctualCandle     = "Candle";

            // Exposure
            public const string k_ExposureBrightSky     = "Sunlit Scene";
            public const string k_ExposureOvercastSky   = "Cloudy Scene";
            public const string k_ExposureSunriseSunset = "Low Sun Scene";
            public const string k_ExposureInterior      = "Interior Scene";
            public const string k_ExposureMoonlitSky    = "Moonlit Scene";
            public const string k_ExposureMoonlessNight = "Moonless Scene";

            // Temperature
            public const string k_TemperatureBlueSky        = "Blue Sky";
            public const string k_TemperatureCloudySky      = "Cloudy Sky";
            public const string k_TemperatureDirectSunlight = "Direct Sunlight";
            public const string k_TemperatureArtificial     = "Artificial";
            public const string k_TemperatureCandle         = "Candle";
        }
    }
}
