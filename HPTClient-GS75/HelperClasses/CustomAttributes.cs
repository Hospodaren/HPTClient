using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPTClient
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HPTReductionAttribute : Attribute
    {
        public HPTReductionAttribute()
        {
        }

        public HPTReductionAttribute(string name, string propertyName, bool requiresPRO, int order)
        {
            this.Name = name;
            this.PropertyName = propertyName;
            this.RequiresPRO = requiresPRO;
            this.Order = order;
        }

        public string Name { get; set; }

        public bool RequiresPRO { get; set; }

        public int Order { get; set; }

        public string PropertyName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class HPTMarkBetTabsToShowAttribute : Attribute
    {
        public HPTMarkBetTabsToShowAttribute()
        {
        }

        public HPTMarkBetTabsToShowAttribute(string name, string propertyName, int order, bool requiresPRO)
        {
            this.Name = name;
            this.PropertyName = propertyName;
            this.Order = order;
            this.RequiresPRO = requiresPRO;
        }

        public string Name { get; set; }

        public int Order { get; set; }

        public string PropertyName { get; set; }

        public bool RequiresPRO { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class HorseDataToShowAttribute : Attribute
    {
        public HorseDataToShowAttribute()
        {
        }

        public HorseDataToShowAttribute(string name, string propertyName, DataToShowUsage usage, int order)
        {
            this.Name = name;
            this.PropertyName = propertyName;
            this.Usage = usage;
            this.Order = order;
            this.RequiresPro = false;
        }

        public HorseDataToShowAttribute(string name, string propertyName, DataToShowUsage usage, int order, bool requiresPro)
        {
            this.Name = name;
            this.PropertyName = propertyName;
            this.Usage = usage;
            this.Order = order;
            this.RequiresPro = requiresPro;
        }

        public string Name { get; set; }

        public DataToShowUsage Usage { get; set; }

        //private DataToShowUsage usage;
        //public DataToShowUsage Usage
        //{
        //    get
        //    {
        //        if (this.usage == DataToShowUsage.All)
        //        {
        //            this.usage = DataToShowUsage.Everywhere;
        //        }
        //        return this.usage;
        //    }
        //    set
        //    {
        //        if (value == DataToShowUsage.All)
        //        {
        //            this.usage = DataToShowUsage.Everywhere;
        //        }
        //        else
        //        {
        //            this.usage = value;
        //        }
        //    }
        //}

        public int Order { get; set; }

        public string PropertyName { get; set; }

        public bool RequiresPro { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class HorseRankAttribute : Attribute
    {
        public HorseRankAttribute()
        {
        }

        public HorseRankAttribute(string name, int order, bool descending, bool isStatic, HPTRankCategory category, bool sort)
        {
            this.Name = name;
            this.Order = order;
            this.Descending = descending;
            this.IsStatic = isStatic;
            this.Category = category;
            this.Sort = sort;
        }

        public HorseRankAttribute(string name, int order, bool descending, bool isStatic, HPTRankCategory category, bool sort, bool useForBeginner, string nameBeginner)
        {
            this.Name = name;
            this.Order = order;
            this.Descending = descending;
            this.IsStatic = isStatic;
            this.Category = category;
            this.Sort = sort;
            this.UseForBeginner = useForBeginner;
            this.NameBeginner = nameBeginner;
        }

        public HorseRankAttribute(string name, int order, bool descending, bool isStatic, HPTRankCategory category, bool sort, bool useForBeginner, string nameBeginner, string displayPropertyName, string stringFormat, double valueForMissing)
        {
            this.Name = name;
            this.Order = order;
            this.Descending = descending;
            this.IsStatic = isStatic;
            this.Category = category;
            this.Sort = sort;
            this.UseForBeginner = useForBeginner;
            this.NameBeginner = nameBeginner;
            this.DisplayPropertyName = displayPropertyName;
            this.StringFormat = stringFormat;
            this.ValueForMissing = Convert.ToDecimal(valueForMissing);
        }

        public string Name { get; private set; }

        public bool Descending { get; private set; }

        public int Order { get; set; }

        public bool IsStatic { get; set; }

        public HPTRankCategory Category { get; set; }

        public bool Sort { get; set; }

        public bool UseForBeginner { get; set; }

        public string NameBeginner { get; set; }

        public string DisplayPropertyName { get; private set; }

        public string StringFormat { get; private set; }

        public decimal ValueForMissing { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class GroupReductionAttribute : Attribute
    {
        public GroupReductionAttribute()
        {
        }

        public GroupReductionAttribute(string name, int order, double minValue, double maxValue, double increment, double defaultMin, double defaultMax)
        {
            this.Name = name;
            this.Order = order;
            this.MinValue = Convert.ToDecimal(minValue);
            this.MaxValue = Convert.ToDecimal(maxValue);
            this.Increment = Convert.ToDecimal(increment);
            this.DefaultMin = Convert.ToDecimal(defaultMin);
            this.DefaultMax = Convert.ToDecimal(defaultMax);
        }

        public string Name { get; private set; }

        public int Order { get; private set; }

        public decimal MinValue { get; private set; }

        public decimal MaxValue { get; private set; }

        public decimal Increment { get; private set; }

        public decimal DefaultMin { get; private set; }

        public decimal DefaultMax { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RandomIntervalAttribute : Attribute
    {
        public RandomIntervalAttribute()
        {
        }

        public RandomIntervalAttribute(string name, int minValue, int maxValue, double factor)
        {
            this.Name = name;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Factor = Convert.ToDecimal(factor);
        }

        public string Name { get; private set; }
                
        public int MinValue { get; private set; }

        public int MaxValue { get; private set; }

        public decimal Factor { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CombinationTemplateParameterAttribute : Attribute
    {
        public CombinationTemplateParameterAttribute()
        {
        }

        public CombinationTemplateParameterAttribute(string name, int minValue, int maxValue, double factor)
        {
            this.Name = name;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Factor = Convert.ToDecimal(factor);
        }

        public string Name { get; private set; }

        public int MinValue { get; private set; }

        public int MaxValue { get; private set; }

        public decimal Factor { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CombinationTemplateHorseAttribute : Attribute
    {
        public CombinationTemplateHorseAttribute()
        {
        }

        public CombinationTemplateHorseAttribute(string name, int minValue, int maxValue, double factor)
        {
            this.Name = name;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Factor = Convert.ToDecimal(factor);
        }

        public string Name { get; private set; }

        public int MinValue { get; private set; }

        public int MaxValue { get; private set; }

        public decimal Factor { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
