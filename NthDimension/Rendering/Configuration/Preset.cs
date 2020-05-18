/* LICENSE
 * Copyright (C) 2008 - 2018 SYSCON Technologies, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@polytronic.gr) http://www.polytronic.gr
 * 
 * This file is part of nthDimension Platform
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

namespace NthDimension.Rendering.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class Preset
    {
        public string presetKey;
        public string presetName;
        public SettingTypes presetType;
        public List<Setting> presetSettings = new List<Setting>();

        public Preset(string pkey, string name, SettingTypes type)
        {
            presetKey = pkey;
            presetName = name;
            presetType = type;
        }

        public void SetValue(FieldInfo param, object value)
        {
            Setting newSetting = new Setting();
            newSetting.parameter = param;
            newSetting.value = value;
            presetSettings.Add(newSetting);
        }

        Type GetTypePreset()
        {
            switch (presetType)
            {
                case SettingTypes.ST_Video:
                    return typeof(VideoSettings);

                case SettingTypes.ST_Sound:
                    return typeof(SoundSettings);

                case SettingTypes.ST_Game:
                    return typeof(GameSettings);

                case SettingTypes.ST_Mesh:
                    return typeof (MeshSettings);
            }

            throw new Exception("Unknown preset type");
        }

        public void SetValue(string paramName, object value)
        {
            Type cType = GetTypePreset();

            FieldInfo fi = cType.GetField(paramName);
            if (fi == null)
            {
                throw new Exception(string.Format("Parameter {0} not found for preset type {1}", paramName, presetType.ToString()));
            }
            SetValue(fi, value);
        }

        public void SetAllQualityLevels(QualityLevel newLevel)
        {
            Type cType = GetTypePreset();
            FieldInfo[] fields = cType.GetFields();

            foreach (FieldInfo f in fields)
            {
                if (f.FieldType == typeof(QualityLevel))
                {
                    SetValue(f, newLevel);
                }
            }
        }

        public void ApplyToSettings(object obj)
        {
            foreach (Setting s in presetSettings)
            {
                s.parameter.SetValue(obj, s.value);
            }
        }
    }
}
