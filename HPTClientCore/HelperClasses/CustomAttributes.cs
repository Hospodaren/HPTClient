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
            Name = name;
            PropertyName = propertyName;
            RequiresPRO = requiresPRO;
            Order = order;
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
            Name = name;
            PropertyName = propertyName;
            Order = order;
            RequiresPRO = requiresPRO;
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
            Name = name;
            PropertyName = propertyName;
            Usage = usage;
            Order = order;
            RequiresPro = false;
        }

        public HorseDataToShowAttribute(string name, string propertyName, DataToShowUsage usage, int order, bool requiresPro)
        {
            Name = name;
            PropertyName = propertyName;
            Usage = usage;
            Order = order;
            RequiresPro = requiresPro;
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
            Name = name;
            Order = order;
            Descending = descending;
            IsStatic = isStatic;
            Category = category;
            Sort = sort;
        }

        public HorseRankAttribute(string name, int order, bool descending, bool isStatic, HPTRankCategory category, bool sort, bool useForBeginner, string nameBeginner)
        {
            Name = name;
            Order = order;
            Descending = descending;
            IsStatic = isStatic;
            Category = category;
            Sort = sort;
            UseForBeginner = useForBeginner;
            NameBeginner = nameBeginner;
        }

        public HorseRankAttribute(string name, int order, bool descending, bool isStatic, HPTRankCategory category, bool sort, bool useForBeginner, string nameBeginner, string displayPropertyName, string stringFormat, double valueForMissing)
        {
            Name = name;
            Order = order;
            Descending = descending;
            IsStatic = isStatic;
            Category = category;
            Sort = sort;
            UseForBeginner = useForBeginner;
            NameBeginner = nameBeginner;
            DisplayPropertyName = displayPropertyName;
            StringFormat = stringFormat;
            ValueForMissing = Convert.ToDecimal(valueForMissing);
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
            Name = name;
            Order = order;
            MinValue = Convert.ToDecimal(minValue);
            MaxValue = Convert.ToDecimal(maxValue);
            Increment = Convert.ToDecimal(increment);
            DefaultMin = Convert.ToDecimal(defaultMin);
            DefaultMax = Convert.ToDecimal(defaultMax);
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
            return Name;
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
            Name = name;
            MinValue = minValue;
            MaxValue = maxValue;
            Factor = Convert.ToDecimal(factor);
        }

        public string Name { get; private set; }

        public int MinValue { get; private set; }

        public int MaxValue { get; private set; }

        public decimal Factor { get; private set; }

        public override string ToString()
        {
            return Name;
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
            Name = name;
            MinValue = minValue;
            MaxValue = maxValue;
            Factor = Convert.ToDecimal(factor);
        }

        public string Name { get; private set; }

        public int MinValue { get; private set; }

        public int MaxValue { get; private set; }

        public decimal Factor { get; private set; }

        public override string ToString()
        {
            return Name;
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
            Name = name;
            MinValue = minValue;
            MaxValue = maxValue;
            Factor = Convert.ToDecimal(factor);
        }

        public string Name { get; private set; }

        public int MinValue { get; private set; }

        public int MaxValue { get; private set; }

        public decimal Factor { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
