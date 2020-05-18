using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace NthDimension.Forms.Widgets
{
    public class KeysConverter : TypeConverter, IComparer
    {
        private IDictionary keyNames;

        private List<string> displayOrder;

        private TypeConverter.StandardValuesCollection values;

        private const Keys FirstDigit = Keys.D0;

        private const Keys LastDigit = Keys.D9;

        private const Keys FirstAscii = Keys.A;

        private const Keys LastAscii = Keys.Z;

        private const Keys FirstNumpadDigit = Keys.NumPad0;

        private const Keys LastNumpadDigit = Keys.NumPad9;

        private IDictionary KeyNames
        {
            get
            {
                if (this.keyNames == null)
                {
                    this.Initialize();
                }
                return this.keyNames;
            }
        }

        private List<string> DisplayOrder
        {
            get
            {
                if (this.displayOrder == null)
                {
                    this.Initialize();
                }
                return this.displayOrder;
            }
        }

        private void AddKey(string key, Keys value)
        {
            this.keyNames[key] = value;
            this.displayOrder.Add(key);
        }

        private void Initialize()
        {
            this.keyNames = new Hashtable(34);
            this.displayOrder = new List<string>(34);
            this.AddKey("toStringEnter", Keys.Return);
            this.AddKey("F12", Keys.F12);
            this.AddKey("F11", Keys.F11);
            this.AddKey("F10", Keys.F10);
            this.AddKey("toStringEnd", Keys.End);
            this.AddKey("toStringControl", Keys.Control);
            this.AddKey("F8", Keys.F8);
            this.AddKey("F9", Keys.F9);
            this.AddKey("toStringAlt", Keys.Alt);
            this.AddKey("F4", Keys.F4);
            this.AddKey("F5", Keys.F5);
            this.AddKey("F6", Keys.F6);
            this.AddKey("F7", Keys.F7);
            this.AddKey("F1", Keys.F1);
            this.AddKey("F2", Keys.F2);
            this.AddKey("F3", Keys.F3);
            this.AddKey("toStringPageDown", Keys.Next);
            this.AddKey("toStringInsert", Keys.Insert);
            this.AddKey("toStringHome", Keys.Home);
            this.AddKey("toStringDelete", Keys.Delete);
            this.AddKey("toStringShift", Keys.Shift);
            this.AddKey("toStringPageUp", Keys.Prior);
            this.AddKey("toStringBack", Keys.Back);
            this.AddKey("0", Keys.D0);
            this.AddKey("1", Keys.D1);
            this.AddKey("2", Keys.D2);
            this.AddKey("3", Keys.D3);
            this.AddKey("4", Keys.D4);
            this.AddKey("5", Keys.D5);
            this.AddKey("6", Keys.D6);
            this.AddKey("7", Keys.D7);
            this.AddKey("8", Keys.D8);
            this.AddKey("9", Keys.D9);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(Enum[]) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Enum[]) || base.CanConvertTo(context, destinationType);
        }

        public int Compare(object a, object b)
        {
            return string.Compare(base.ConvertToString(a), base.ConvertToString(b), false, CultureInfo.InvariantCulture);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string text = ((string)value).Trim();
                if (text.Length == 0)
                {
                    return null;
                }
                string[] array = text.Split(new char[] {
					'+'
				});
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = array[i].Trim();
                }
                Keys keys = Keys.None;
                bool flag = false;
                for (int j = 0; j < array.Length; j++)
                {
                    object obj = this.KeyNames[array[j]];
                    if (obj == null)
                    {
                        obj = Enum.Parse(typeof(Keys), array[j]);
                    }
                    if (obj == null)
                    {
                        throw new FormatException("KeysConverterInvalidKeyName");
                    }
                    Keys keys2 = (Keys)obj;
                    if ((keys2 & Keys.KeyCode) != Keys.None)
                    {
                        if (flag)
                        {
                            throw new FormatException("KeysConverterInvalidKeyCombination");
                        }
                        flag = true;
                    }
                    keys |= keys2;
                }
                return keys;
            }
            else
            {
                if (value is Enum[])
                {
                    long num = 0L;
                    Enum[] array2 = (Enum[])value;
                    for (int k = 0; k < array2.Length; k++)
                    {
                        Enum value2 = array2[k];
                        num |= Convert.ToInt64(value2, CultureInfo.InvariantCulture);
                    }
                    return Enum.ToObject(typeof(Keys), num);
                }
                return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (value is Keys || value is int)
            {
                bool flag = destinationType == typeof(string);
                bool flag2 = false;
                if (!flag)
                {
                    flag2 = (destinationType == typeof(Enum[]));
                }
                if (flag | flag2)
                {
                    Keys keys = (Keys)value;
                    bool flag3 = false;
                    ArrayList arrayList = new ArrayList();
                    Keys keys2 = keys & Keys.Modifiers;
                    for (int i = 0; i < this.DisplayOrder.Count; i++)
                    {
                        string text = this.DisplayOrder[i];
                        Keys keys3 = (Keys)this.keyNames[text];
                        if ((keys3 & keys2) != Keys.None)
                        {
                            if (flag)
                            {
                                if (flag3)
                                {
                                    arrayList.Add("+");
                                }
                                arrayList.Add(text);
                            }
                            else
                            {
                                arrayList.Add(keys3);
                            }
                            flag3 = true;
                        }
                    }
                    Keys keys4 = keys & Keys.KeyCode;
                    bool flag4 = false;
                    if (flag3 & flag)
                    {
                        arrayList.Add("+");
                    }
                    for (int j = 0; j < this.DisplayOrder.Count; j++)
                    {
                        string text2 = this.DisplayOrder[j];
                        Keys keys5 = (Keys)this.keyNames[text2];
                        if (keys5.Equals(keys4))
                        {
                            if (flag)
                            {
                                arrayList.Add(text2);
                            }
                            else
                            {
                                arrayList.Add(keys5);
                            }
                            flag4 = true;
                            break;
                        }
                    }
                    if (!flag4 && Enum.IsDefined(typeof(Keys), (int)keys4))
                    {
                        if (flag)
                        {
                            arrayList.Add(keys4.ToString());
                        }
                        else
                        {
                            arrayList.Add(keys4);
                        }
                    }
                    if (flag)
                    {
                        StringBuilder stringBuilder = new StringBuilder(32);
                        foreach (string value2 in arrayList)
                        {
                            stringBuilder.Append(value2);
                        }
                        return stringBuilder.ToString();
                    }
                    return (Enum[])arrayList.ToArray(typeof(Enum));
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this.values == null)
            {
                ArrayList arrayList = new ArrayList();
                foreach (object current in this.KeyNames.Values)
                {
                    arrayList.Add(current);
                }
                arrayList.Sort(this);
                this.values = new TypeConverter.StandardValuesCollection(arrayList.ToArray());
            }
            return this.values;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
